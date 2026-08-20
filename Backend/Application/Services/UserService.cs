using Application.Common.Constants;
using Application.Common.Logging;
using Application.DTOs.Pagination;
using Application.DTOs.User.Request;
using Application.DTOs.User.Response;
using Application.Exceptions;
using Application.Interfaces.Auth;
using Application.Interfaces.Localization;
using Application.Interfaces.User;
using Application.Validators.User;
using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using ValidationException = Application.Exceptions.ValidationException;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizedMapper _mapper;
        private readonly ILanguageContext _language;
        private readonly ILogger<UserService> _logger;
        private readonly IMemoryCache _cache;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IFileStorage _fileStorage;
        private readonly IJwtTokenGenerator _tokens;
        private readonly UserUpdateValidator _userUpdateValidator;
        private readonly CompleteProfileValidator _completeProfileValidator;
        private readonly UpdateLocationValidator _updateLocationValidator;
        private readonly ChangePasswordValidator _changePasswordValidator;

        // Cache constants
        private const string UserCacheKeyPrefix = "user_";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan SlidingCacheDuration = TimeSpan.FromMinutes(2);
        private const string ObjectName = "User";

        private string UsersListCacheKey => $"all_users_{_language.LanguageCode}";

        public UserService(
            IUnitOfWork unitOfWork,
            ILocalizedMapper mapper,
            ILanguageContext language,
            ILogger<UserService> logger,
            IMemoryCache cache,
            IPasswordHasher passwordHasher,
            IFileStorage fileStorage,
            IJwtTokenGenerator tokens,
            UserUpdateValidator updateMeValidator,
            CompleteProfileValidator completeProfileValidator,
            UpdateLocationValidator updateLocationValidator,
            ChangePasswordValidator changePasswordValidator)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _language = language ?? throw new ArgumentNullException(nameof(language));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
            _tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
            _userUpdateValidator = updateMeValidator ?? throw new ArgumentNullException(nameof(updateMeValidator));
            _completeProfileValidator = completeProfileValidator ?? throw new ArgumentNullException(nameof(completeProfileValidator));
            _updateLocationValidator = updateLocationValidator ?? throw new ArgumentNullException(nameof(updateLocationValidator));
            _changePasswordValidator = changePasswordValidator ?? throw new ArgumentNullException(nameof(changePasswordValidator));
        }

        public async Task<CompleteProfileResponse> CompleteProfileAsync(int userId, CompleteProfileRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            if (userId <= 0)
                throw new ArgumentException("Invalid user ID", nameof(userId));

            _logger.LogInformation("User {UserId} attempting to complete profile", userId);

            var validationResult = await _completeProfileValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Profile completion validation failed for user {UserId}: {Errors}",
                    userId, string.Join(", ", validationResult.Errors));
                throw new ValidationException(validationResult.Errors);
            }


            try
            {
                var user = await _unitOfWork.Users
                    .Query()
                    .Include(u => u.Role)
                    .Include(u => u.Person).ThenInclude(p => p.NationalityCountry)
                        .ThenInclude(n => n.Translations)
                    .Include(u => u.Person).ThenInclude(p => p.ResidentialCity)
                        .ThenInclude(c => c.Translations)
                    .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found for profile completion", userId);
                    throw new NotFoundException($"User with ID '{userId}' not found");
                }

                if (!user.IsEmailVerified)
                    throw new ForbiddenException("Email must be verified before completing the profile.");

                if (user.PersonId.HasValue)
                    throw new ConflictException("Profile has already been completed. Use the update endpoint to change it.");

                if(user.Role.RoleName == "Tourist" && request.PassportImage == null)
                    throw new ValidationException("Passport Image Is Required");


                // Ensure national/passport numbers are unique across other users
                var nationalNumber = request.NationalNumber.Trim();
                var passportNumber = request.PassportNumber.Trim();

                var nationalTaken = await _unitOfWork.Users
                    .Query()
                    .Include(u => u.Person)
                    .AnyAsync(u => u.Person != null &&
                    u.Person.NationalNumber == nationalNumber &&
                    u.Id != userId, cancellationToken);

                if (nationalTaken)
                    throw new ConflictException("This national number is already registered.");

                var passportTaken = await _unitOfWork.Users
                    .Query()
                    .Include(u => u.Person)
                    .AnyAsync(u => u.Person != null &&
                    u.Person.PassportNumber == passportNumber &&
                    u.Id != userId, cancellationToken);

                if (passportTaken)
                    throw new ConflictException("This passport number is already registered.");

                // Save uploaded image
                string? profileImageUrl = null;
                if (request.Image is { Length: > 0 })
                    profileImageUrl = await _fileStorage.SaveAsync(request.Image, "profiles", cancellationToken);

                // Save uploaded Passport scan
                string? passportImageUrl = null;
                if (request.PassportImage is { Length: > 0 })
                    passportImageUrl = await _fileStorage.SaveAsync(request.PassportImage, "passports", cancellationToken);

                var nationalIdImageUrl = await _fileStorage.SaveAsync(request.NationalIdImage, "national-ids", cancellationToken);


                var person = _mapper.Map<Person>(request);
                person.NationalNumber = nationalNumber;
                person.PassportNumber = passportNumber;
                person.NationalIdCard = nationalIdImageUrl;
                person.ProfileImage = profileImageUrl;
                person.PassportScan = passportImageUrl;

                user.Person = person;
                user.BankAccount = request.BankAccount.Trim();
                user.UpdatedAtUtc = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                person.NationalityCountry = await _unitOfWork.Countries.Query()
                    .Include(c => c.Translations)
                    .FirstAsync(c => c.Id == person.NationalityCountryId, cancellationToken);
                person.ResidentialCity = await _unitOfWork.Cities.Query()
                    .Include(c => c.Translations)
                    .FirstAsync(c => c.Id == person.ResidentialCityId, cancellationToken);

                InvalidateUserCache(userId);


                var userResponse = _mapper.Map<UserResponse>(user);

                _logger.LogInformation("User {UserId} successfully completed their profile", userId);


                var (token, expiresAt) = _tokens.GenerateToken(user);

                var response = new CompleteProfileResponse()
                {
                    User = userResponse,
                    Token = token
                };

                return response;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (ConflictException)
            {
                throw;
            }
            catch (ForbiddenException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.ConcurrencyConflict(ObjectName);
                throw new ConcurrencyException(ExceptionMessages.Concurrency(ObjectName), ex);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
            {
                _logger.ConflictDetected(ObjectName, "unique field value already taken");
                throw new ConflictException(ExceptionMessages.Conflict(ObjectName, "unique field value already taken"));
            }
            catch (Exception ex)
            {
                _logger.ServerError("Complete Profile", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("complete profile", ObjectName, ex.Message), ex);
            }
        }

        public async Task<UserResponse> UpdateAsync(int userId, UserUpdateRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            if (userId <= 0)
                throw new ArgumentException("Invalid user ID", nameof(userId));

            _logger.LogInformation("User {UserId} attempting to update their profile", userId);

            // Validate request
            var validationResult = await _userUpdateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Profile update validation failed for user {UserId}: {Errors}",
                    userId, string.Join(", ", validationResult.Errors));
                throw new ValidationException(validationResult.Errors);
            }

            try
            {
                var user = await _unitOfWork.Users
                    .Query()
                    .Include(u => u.Role)
                    .Include(u => u.Person).ThenInclude(p => p.NationalityCountry)
                        .ThenInclude(n => n.Translations)
                    .Include(u => u.Person).ThenInclude(p => p.ResidentialCity)
                        .ThenInclude(c => c.Translations)
                    .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found for self-update", userId);
                    throw new NotFoundException($"User with ID '{userId}' not found");
                }

                if (user.Person == null)
                {
                    _logger.LogWarning("User with ID {UserId} did not complete his profile", userId);
                    throw new ForbiddenException($"User with ID '{userId}' did not complete his profile");
                }


                var person = user.Person;

                // check if nationalNumber exists in the system
                if (!string.IsNullOrWhiteSpace(request.NationalNumber))
                {
                    var newNationalNumber = request.NationalNumber.Trim();

                    if (!string.Equals(user.Person.NationalNumber, newNationalNumber, StringComparison.OrdinalIgnoreCase))
                    {
                        var nationalTaken = await _unitOfWork.Users
                            .Query()
                            .Include(u => u.Person)
                            .AnyAsync(u => u.Person != null &&
                            u.Person.NationalNumber == newNationalNumber &&
                            u.Id != userId, cancellationToken);

                        if (nationalTaken)
                        {
                            _logger.LogWarning("Duplicate national number attempt for user {UserId}", userId);
                            throw new ConflictException("This national number is already registered.");
                        }

                        person.NationalNumber = newNationalNumber;
                    }
                }
                // check if PassportNumber exists in the system
                if (!string.IsNullOrWhiteSpace(request.PassportNumber))
                {
                    var newPassportNumber = request.PassportNumber.Trim();

                    if (!string.Equals(user.Person.PassportNumber, newPassportNumber, StringComparison.OrdinalIgnoreCase))
                    {
                        var passportTaken = await _unitOfWork.Users
                            .Query()
                            .Include(u => u.Person)
                            .AnyAsync(u => u.Person != null &&
                            u.Person.PassportNumber == newPassportNumber &&
                            u.Id != userId, cancellationToken);

                        if (passportTaken)
                        {
                            _logger.LogWarning("Duplicate passport number attempt for user {UserId}", userId);
                            throw new ConflictException("This passport number is already registered.");
                        }

                        person.PassportNumber = newPassportNumber;
                    }
                }

                if (request.ResidentialCityId.HasValue)
                {
                    if (request.ResidentialCityId.Value <= 0)
                        throw new ValidationException("ResidentialCityId must be greater than 0.");

                    var cityExists = await _unitOfWork.Cities.AnyAsync(c => c.Id == request.ResidentialCityId.Value, cancellationToken);
                    if (!cityExists)
                        throw new ValidationException("ResidentialCityId is invalid");

                    person.ResidentialCityId = request.ResidentialCityId.Value;
                }

                _mapper.Map(request, person);


                // Upload new images if provided
                if (request.Image is { Length: > 0 })
                    person.ProfileImage = await _fileStorage.SaveAsync(request.Image, "profiles", cancellationToken);

                if (request.NationalIdImage is { Length: > 0 })
                    person.NationalIdCard = await _fileStorage.SaveAsync(request.NationalIdImage, "national-ids", cancellationToken);

                if (request.PassportImage is { Length: > 0 })
                    person.PassportScan = await _fileStorage.SaveAsync(request.PassportImage, "passports", cancellationToken);

                person.UpdatedAtUtc = DateTime.UtcNow;
                user.Person = person;
                user.UpdatedAtUtc = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Invalidate cache
                InvalidateUserCache(userId);

                var response = _mapper.Map<UserResponse>(user);

                _logger.LogInformation("User {UserId} successfully updated their profile", userId);

                return response;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (ConflictException)
            {
                throw;
            }
            catch (AuthException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.ConcurrencyConflict(ObjectName);
                throw new ConcurrencyException(ExceptionMessages.Concurrency(ObjectName), ex);
            }
            catch (Exception ex)
            {
                _logger.ServerError("Update", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("update", ObjectName, ex.Message), ex);
            }
        }

        public async Task<bool> DeleteMyAccountAsync(int userId, DeleteAccountRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            if (userId <= 0)
                throw new ArgumentException("Invalid user ID", nameof(userId));

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ValidationException("Password is required to delete the account");

            _logger.LogInformation("User {UserId} attempting to delete their account", userId);

            try
            {
                var entity = await _unitOfWork.Users
                    .Query()
                    .Include(u => u.Person)
                    .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);
                if (entity == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found for self-deletion", userId);
                    return false;
                }

                if (!_passwordHasher.Verify(request.Password, entity.Password))
                {
                    _logger.LogWarning("Incorrect password for user {UserId} during account deletion", userId);
                    throw new AuthException("Incorrect password");
                }

                entity.IsDeleted = true;
                _unitOfWork.Users.Update(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Invalidate cache
                InvalidateUserCache(userId);

                _logger.LogInformation("User {UserId} successfully deleted their account", userId);
                return true;
            }
            catch (AuthException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.ServerError("Delete Account", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("delete", ObjectName, ex.Message), ex);
            }
        }

        public async Task<bool> AdminDeleteUserAsync(int userId, CancellationToken cancellationToken = default, bool IsHardDelete = false)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID", nameof(userId));

            _logger.LogInformation("Admin attempting to delete user {UserId}", userId);

            try
            {
                var entity = await _unitOfWork.Users
                    .Query()
                    .Include(u => u.Role)
                    .Include(u => u.Person)
                    .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

                if (entity == null)
                {
                    _logger.LogWarning("Admin delete: user with ID {UserId} not found", userId);
                    return false;
                }

                if (entity.IsDeleted && !IsHardDelete)
                {
                    _logger.LogWarning("Admin delete: user with ID {UserId} Has been Deleted A soft delete Before", userId);
                    throw new ConflictException($"user with ID {userId} Has been Deleted A soft delete Before");
                }

                // Prevent deleting Admin accounts via this endpoint (safety net)
                if (entity.Role != null && entity.Role.RoleName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Admin delete blocked for user {UserId}: target is also an Admin", userId);
                    throw new ForbiddenException("Admin accounts cannot be deleted from this endpoint.");
                }

                if (IsHardDelete)
                {
                    if (entity.Person != null)
                        _unitOfWork.Persons.Remove(entity.Person);
                    _unitOfWork.Users.Remove(entity);
                }
                else
                {
                    entity.IsDeleted = true;
                    _unitOfWork.Users.Update(entity);
                }
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                InvalidateUserCache(userId);

                _logger.LogInformation("Admin successfully deleted user {UserId}", userId);
                return true;
            }
            catch (ForbiddenException)
            {
                throw;
            }
            catch (ConflictException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.ServerError("Admin Delete", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("delete", ObjectName, ex.Message), ex);
            }
        }

        public async Task<UserResponse> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid user ID", nameof(id));

            _logger.LogDebug("Retrieving user with ID {UserId}", id);

            // Try cache first
            var cacheKey = $"{UserCacheKeyPrefix}{id}_{_language.LanguageCode}";
            if (_cache.TryGetValue(cacheKey, out UserResponse? cachedUser) && cachedUser != null)
            {
                _logger.LogDebug("Cache hit for user {UserId}", id);
                return cachedUser;
            }

            try
            {
                var entity = await _unitOfWork.Users
                    .Query()
                    .IgnoreQueryFilters()
                    .Include(u => u.Role)
                    .Include(u => u.Person).ThenInclude(p => p.NationalityCountry)
                        .ThenInclude(n => n.Translations)
                    .Include(u => u.Person).ThenInclude(p => p.ResidentialCity)
                        .ThenInclude(c => c.Translations)
                    .Where(u => u.Id == id && !u.IsDeleted)
                    .FirstOrDefaultAsync(cancellationToken);

                if (entity == null)
                {
                    _logger.LogDebug("User with ID {UserId} not found", id);
                    throw new NotFoundException($"User with ID {id} not found");
                }

                var response = _mapper.Map<UserResponse>(entity);

                // Cache the result with sliding expiration
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration,
                    SlidingExpiration = SlidingCacheDuration,
                    Priority = CacheItemPriority.Normal
                };

                _cache.Set(cacheKey, response, cacheOptions);

                _logger.LogDebug("Successfully retrieved user {UserId}", id);

                return response;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.ServerError("Retrieve", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("retrieve", ObjectName, ex.Message), ex);
            }
        }

        public async Task<PaginatedResponse<UserResponse>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                throw new ValidationException(ExceptionMessages.InvalidPagination());

            _logger.LogDebug("Retrieving all users");


            var chachKey = $"{UsersListCacheKey}_page{page}_size{pageSize}";
            // Try cache
            if (_cache.TryGetValue(chachKey, out PaginatedResponse<UserResponse>? cachedUsers) && cachedUsers != null)
            {
                _logger.LogDebug("Cache hit for all users");
                return cachedUsers;
            }
            try
            {
                var query = _unitOfWork.Users
                    .Query()
                    .Where(u => !u.IsDeleted);

                var entities = await query
                     .Include(u => u.Role)
                    .Include(u => u.Person).ThenInclude(p => p.NationalityCountry)
                        .ThenInclude(n => n.Translations)
                    .Include(u => u.Person).ThenInclude(p => p.ResidentialCity)
                        .ThenInclude(c => c.Translations)
                     .OrderBy(u => u.Person != null ? u.Person.FirstName : string.Empty)
                    .ThenBy(u => u.Person != null ? u.Person.LastName : string.Empty)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                var totalItemsCount = await query.CountAsync(cancellationToken);

                var items = _mapper.Map<IReadOnlyList<UserResponse>>(entities);

                var pagenationMetadata = new PaginationMetadata
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = totalItemsCount
                };

                var response = new PaginatedResponse<UserResponse> { Items = items, Pagination = pagenationMetadata };

                // Cache the result with lower priority
                _cache.Set(chachKey, response, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration,
                    Priority = CacheItemPriority.Low
                });

                _logger.LogDebug("Successfully retrieved {Count} users", totalItemsCount);

                return response;
            }
            catch (Exception ex)
            {
                _logger.ServerError("Retrieve All", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("retrieve", ObjectName, ex.Message), ex);
            }
        }

        public async Task<PaginatedResponse<UserResponse>> FilterAsync(
            string? firstName,
            string? lastName,
            string? email,
            string? roleName,
            bool? isEmailVerified,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                throw new ValidationException(ExceptionMessages.InvalidPagination());

            _logger.LogDebug("Filtering users with parameters - FirstName: {FirstName}, LastName: {LastName}, " +
                             "Email: {Email}, RoleId: {RoleId}, IsEmailVerified: {IsEmailVerified}",
                firstName ?? "Any", lastName ?? "Any", email ?? "Any",
                roleName ?? "Any", isEmailVerified?.ToString() ?? "Any");

            try
            {
                var query = _unitOfWork.Users.Query()
                    .Where(u => !u.IsDeleted)
                    .Include(u => u.Role)
                    .Include(u => u.Person).ThenInclude(p => p.NationalityCountry)
                        .ThenInclude(n => n.Translations)
                    .Include(u => u.Person).ThenInclude(p => p.ResidentialCity)
                        .ThenInclude(c => c.Translations)
                    .AsQueryable();

                Role? role;
                if (!string.IsNullOrWhiteSpace(roleName))
                {
                    role = await _unitOfWork.Roles.FirstOrDefaultAsync(r => r.RoleName.ToLower() == roleName.ToLower());
                    if (role == null)
                    {
                        _logger.LogWarning($"Role '{roleName}' Not Found");
                        throw new NotFoundException($"Role '{roleName}' Not Found, Role name Should be in ['Tourist','TourCompany','Admin']");
                    }
                    query = query.Where(u => u.RoleId == role.RoleId);
                    _logger.LogDebug("Applied role filter: {RoleName}", roleName);
                }

                if (!string.IsNullOrWhiteSpace(firstName))
                {
                    var search = firstName.ToLower();
                    query = query.Where(u => u.Person != null && u.Person.FirstName.ToLower().Contains(search));
                    _logger.LogDebug("Applied first name filter: {FirstName}", firstName);
                }

                if (!string.IsNullOrWhiteSpace(lastName))
                {
                    var search = lastName.ToLower();
                    query = query.Where(u => u.Person != null && u.Person.LastName.ToLower().Contains(search));
                    _logger.LogDebug("Applied last name filter: {LastName}", lastName);
                }

                if (!string.IsNullOrWhiteSpace(email))
                {
                    var search = email.ToLower();
                    query = query.Where(u => u.Email.ToLower().Contains(search));
                    _logger.LogDebug("Applied email filter: {Email}", email);
                }

                if (isEmailVerified.HasValue)
                {
                    query = query.Where(u => u.IsEmailVerified == isEmailVerified.Value);
                    _logger.LogDebug("Applied email verification filter: {IsEmailVerified}", isEmailVerified.Value);
                }

                var entities = await query
                    .AsNoTracking()
                    .OrderBy(u => u.Person != null ? u.Person.FirstName : string.Empty)
                    .ThenBy(u => u.Person != null ? u.Person.LastName : string.Empty)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                var totalItemsCount = await query.CountAsync(cancellationToken);

                var items = _mapper.Map<IReadOnlyList<UserResponse>>(entities);

                var pagenationMetadata = new PaginationMetadata
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = totalItemsCount
                };

                var response = new PaginatedResponse<UserResponse>
                {
                    Items = items,
                    Pagination = pagenationMetadata
                };

                _logger.LogInformation("User filter completed. Found {Count} users matching criteria", totalItemsCount);

                return response;
            }
            catch (Exception ex)
            {
                _logger.ServerError("Filter", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("filter", ObjectName, ex.Message), ex);
            }
        }

        public async Task<UserResponse> UpdateLocationAsync(int userId, UpdateLocationRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            if (userId <= 0)
                throw new ArgumentException("Invalid user ID", nameof(userId));

            _logger.LogInformation("User {UserId} attempting to update location", userId);

            var validationResult = await _updateLocationValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Location update validation failed for user {UserId}: {Errors}",
                    userId, string.Join(", ", validationResult.Errors));
                throw new ValidationException(validationResult.Errors);
            }

            try
            {
                var user = await _unitOfWork.Users
                    .Query()
                    .Include(u => u.Role)
                    .Include(u => u.Person).ThenInclude(p => p.NationalityCountry)
                        .ThenInclude(n => n.Translations)
                    .Include(u => u.Person).ThenInclude(p => p.ResidentialCity)
                        .ThenInclude(c => c.Translations)
                    .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found for location update", userId);
                    throw new NotFoundException($"User with ID '{userId}' not found");
                }

                user.Longitude = request.Longitude;
                user.Latitude = request.Latitude;
                user.UpdatedAtUtc = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                InvalidateUserCache(userId);

                var response = _mapper.Map<UserResponse>(user);

                _logger.LogInformation("User {UserId} successfully updated their location", userId);

                return response;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.ConcurrencyConflict(ObjectName);
                throw new ConcurrencyException(ExceptionMessages.Concurrency(ObjectName), ex);
            }
            catch (Exception ex)
            {
                _logger.ServerError("Update Location", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("update location", ObjectName, ex.Message), ex);
            }
        }

        public async Task<UserResponse> ChangePasswordAsync(int userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            if (userId <= 0)
                throw new ArgumentException("Invalid user ID", nameof(userId));

            _logger.LogInformation("User {UserId} attempting to change password", userId);

            var validationResult = await _changePasswordValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Password change validation failed for user {UserId}: {Errors}",
                    userId, string.Join(", ", validationResult.Errors));
                throw new ValidationException(validationResult.Errors);
            }

            try
            {
                var user = await _unitOfWork.Users
                    .Query()
                    .Include(u => u.Role)
                    .Include(u => u.Person).ThenInclude(p => p.NationalityCountry)
                        .ThenInclude(n => n.Translations)
                    .Include(u => u.Person).ThenInclude(p => p.ResidentialCity)
                        .ThenInclude(c => c.Translations)
                    .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found for password change", userId);
                    throw new NotFoundException($"User with ID '{userId}' not found");
                }

                if (!_passwordHasher.Verify(request.CurrentPassword, user.Password))
                {
                    _logger.LogWarning("Incorrect current password for user {UserId} during password change", userId);
                    throw new AuthException("Current password is incorrect");
                }

                if (_passwordHasher.Verify(request.NewPassword, user.Password))
                {
                    _logger.LogWarning("User {UserId} attempted to reuse the same password", userId);
                    throw new ValidationException("New password must be different from your current password");
                }

                user.Password = _passwordHasher.Hash(request.NewPassword);
                user.UpdatedAtUtc = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                InvalidateUserCache(userId);

                var response = _mapper.Map<UserResponse>(user);

                _logger.LogInformation("User {UserId} successfully changed their password", userId);

                return response;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (AuthException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.ConcurrencyConflict(ObjectName);
                throw new ConcurrencyException(ExceptionMessages.Concurrency(ObjectName), ex);
            }
            catch (Exception ex)
            {
                _logger.ServerError("Change Password", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("change password", ObjectName, ex.Message), ex);
            }
        }

        public async Task<DeletedUsersResponse> GetDeletedUsersAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Retrieving all deleted users");

            try
            {
                var query = _unitOfWork.Users
                    .Query()
                    .IgnoreQueryFilters()
                    .Include(u => u.Role)
                    .Include(u => u.Person).ThenInclude(p => p.NationalityCountry)
                        .ThenInclude(n => n.Translations)
                    .Include(u => u.Person).ThenInclude(p => p.ResidentialCity)
                        .ThenInclude(c => c.Translations)
                    .Where(u => u.IsDeleted);

                var totalCount = await query.CountAsync(cancellationToken);

                var entities = await query
                    .OrderBy(u => u.UpdatedAtUtc)
                    .ToListAsync(cancellationToken);

                var users = _mapper.Map<IReadOnlyList<UserResponse>>(entities);

                _logger.LogDebug("Successfully retrieved {Count} deleted users", totalCount);

                return new DeletedUsersResponse
                {
                    Users = users,
                    TotalCount = totalCount
                };
            }
            catch (Exception ex)
            {
                _logger.ServerError("Retrieve Deleted", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("retrieve deleted", ObjectName, ex.Message), ex);
            }
        }


        #region Private Helper Methods

        private void InvalidateUserCache(int? specificUserId = null)
        {
            if (specificUserId.HasValue)
            {
                foreach (var lang in LanguageCodes.Supported)
                {
                    var cacheKey = $"{UserCacheKeyPrefix}{specificUserId.Value}_{lang}";
                    _cache.Remove(cacheKey);
                    _logger.LogDebug("Invalidated cache for user {UserId}", specificUserId.Value);
                }
            }

            // Always invalidate the list cache when any user changes
            foreach (var lang in LanguageCodes.Supported)
                _cache.Remove($"all_users_{lang}");
            _logger.LogDebug("Invalidated all users list cache");
        }

        #endregion
    }
}
