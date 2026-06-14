using Application.DTOs.User.Request;
using Application.DTOs.User.Response;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Auth;
using Application.Interfaces.User;
using Application.Validators.User;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ValidationException = Application.Exceptions.ValidationException;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly IMemoryCache _cache;
        private readonly Interfaces.Auth.IPasswordHasher _passwordHasher;
        private readonly IFileStorage _fileStorage;
        private readonly UpdateMeValidator _updateMeValidator;
        private readonly CompleteProfileValidator _completeProfileValidator;
        private readonly CompleteCompanyProfileValidator _completeCompanyProfileValidator;

        // Cache constants
        private const string UserCacheKeyPrefix = "user_";
        private const string UsersListCacheKey = "all_users";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan SlidingCacheDuration = TimeSpan.FromMinutes(2);

        public UserService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<UserService> logger,
            IMemoryCache cache,
            Interfaces.Auth.IPasswordHasher passwordHasher,
            IFileStorage fileStorage,
            UpdateMeValidator updateMeValidator,
            CompleteProfileValidator completeProfileValidator,
            CompleteCompanyProfileValidator completeCompanyProfileValidator)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
            _updateMeValidator = updateMeValidator ?? throw new ArgumentNullException(nameof(updateMeValidator));
            _completeProfileValidator = completeProfileValidator ?? throw new ArgumentNullException(nameof(completeProfileValidator));
            _completeCompanyProfileValidator = completeCompanyProfileValidator ?? throw new ArgumentNullException(nameof(completeCompanyProfileValidator));
        }

        public async Task<UserResponse> CompleteProfileAsync(int userId, CompleteProfileRequest request, CancellationToken cancellationToken = default)
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
                var entity = await _unitOfWork.Users
                    .Query()
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

                if (entity == null)
                {
                    _logger.LogWarning("User {UserId} not found for profile completion", userId);
                    throw new NotFoundException($"User with ID '{userId}' not found");
                }

                if (!entity.IsEmailVerified)
                    throw new ForbiddenException("Email must be verified before completing the profile.");

                if (entity.IsProfileCompleted)
                    throw new ConflictException("Profile has already been completed. Use the update endpoint to change it.");

                // Resolve role: when no RoleId is provided, default to the "User" role.
                Role role;
                if (request.RoleId is > 0)
                {
                    role = await _unitOfWork.Roles
                        .Query()
                        .FirstOrDefaultAsync(r => r.Id == request.RoleId.Value, cancellationToken)
                        ?? throw new NotFoundException($"Role with id {request.RoleId.Value} does not exist.");

                    if (role.RoleName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                        throw new ForbiddenException("Selecting Admin role is not allowed from this endpoint.");
                }
                else
                {
                    role = await _unitOfWork.Roles
                        .Query()
                        .FirstOrDefaultAsync(r => r.RoleName == "Tourist", cancellationToken)
                        ?? throw new NotFoundException("Default 'Tourist' role does not exist.");
                }

                // Ensure national/passport numbers are unique across other users
                var nationalNumber = request.NationalNumber.Trim();
                var passportNumber = request.PassportNumber.Trim();

                var nationalTaken = await _unitOfWork.Users
                    .Query()
                    .AnyAsync(u => u.NationalNumber == nationalNumber && u.Id != userId, cancellationToken);

                if (nationalTaken)
                    throw new ConflictException("This national number is already registered.");

                var passportTaken = await _unitOfWork.Users
                    .Query()
                    .AnyAsync(u => u.PassportNumber == passportNumber && u.Id != userId, cancellationToken);

                if (passportTaken)
                    throw new ConflictException("This passport number is already registered.");

                // Save uploaded images
                string? profileImageUrl = null;
                if (request.Image is { Length: > 0 })
                    profileImageUrl = await _fileStorage.SaveAsync(request.Image, "profiles", cancellationToken);

                var nationalIdImageUrl = await _fileStorage.SaveAsync(request.NationalIdImage, "national-ids", cancellationToken);
                var passportImageUrl = await _fileStorage.SaveAsync(request.PassportImage, "passports", cancellationToken);

                entity.FirstName          = request.FirstName.Trim();
                entity.LastName           = request.LastName.Trim();
                entity.PlaceOfResidence   = request.PlaceOfResidence.Trim();
                entity.CurrentLocation    = request.CurrentLocation.Trim();
                entity.Gender             = request.Gender;
                entity.DateOfBirth        = request.DateOfBirth!.Value;
                entity.NationalNumber     = nationalNumber;
                entity.PassportNumber     = passportNumber;
                entity.BankAccount        = request.BankAccount.Trim();
                entity.RoleId             = role.Id;
                entity.Phone              = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();
                if (profileImageUrl != null) entity.Image = profileImageUrl;
                entity.NationalIdImage    = nationalIdImageUrl;
                entity.PassportImage      = passportImageUrl;
                entity.IsProfileCompleted = true;
                entity.UpdatedAtUtc       = DateTime.UtcNow;

                _unitOfWork.Users.Update(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Reload with role for the response
                var refreshed = await _unitOfWork.Users
                    .Query()
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

                InvalidateUserCache(userId);

                var response = _mapper.Map<UserResponse>(refreshed ?? entity);

                _logger.LogInformation("User {UserId} successfully completed their profile", userId);

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
                _logger.LogError(ex, "Concurrency conflict while completing profile for user {UserId}", userId);
                throw new ConcurrencyException("The user was modified by another user. Please refresh and try again.", ex);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
            {
                _logger.LogError(ex, "Unique constraint violation while completing profile for user {UserId}", userId);
                throw new ConflictException("A unique field value is already taken.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while completing profile for user {UserId}", userId);
                throw new ServiceException($"Failed to complete profile: {ex.Message}", ex);
            }
        }

        public async Task<UserResponse> CompleteCompanyProfileAsync(int userId, CompleteCompanyProfileRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            if (userId <= 0)
                throw new ArgumentException("Invalid user ID", nameof(userId));

            _logger.LogInformation("User {UserId} attempting to complete company profile", userId);

            var validationResult = await _completeCompanyProfileValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Company profile completion validation failed for user {UserId}: {Errors}",
                    userId, string.Join(", ", validationResult.Errors));
                throw new ValidationException(validationResult.Errors);
            }

            try
            {
                var entity = await _unitOfWork.Users
                    .Query()
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

                if (entity == null)
                {
                    _logger.LogWarning("User {UserId} not found for company profile completion", userId);
                    throw new NotFoundException($"User with ID '{userId}' not found");
                }

                if (!entity.IsEmailVerified)
                    throw new ForbiddenException("Email must be verified before completing the profile.");

                if (entity.IsProfileCompleted)
                    throw new ConflictException("Profile has already been completed. Use the update endpoint to change it.");

                // Role is fixed to "TourCompany" for this flow; it is not chosen by the client.
                var role = await _unitOfWork.Roles
                    .Query()
                    .FirstOrDefaultAsync(r => r.RoleName == "TourCompany", cancellationToken)
                    ?? throw new NotFoundException("Default 'TourCompany' role does not exist.");

                // Ensure national number is unique across other users
                var nationalNumber = request.NationalNumber.Trim();

                var nationalTaken = await _unitOfWork.Users
                    .Query()
                    .AnyAsync(u => u.NationalNumber == nationalNumber && u.Id != userId, cancellationToken);

                if (nationalTaken)
                    throw new ConflictException("This national number is already registered.");

                // Save uploaded images
                string? profileImageUrl = null;
                if (request.Image is { Length: > 0 })
                    profileImageUrl = await _fileStorage.SaveAsync(request.Image, "profiles", cancellationToken);

                var nationalIdImageUrl = await _fileStorage.SaveAsync(request.NationalIdImage, "national-ids", cancellationToken);

                entity.FirstName          = request.FirstName.Trim();
                entity.LastName           = request.LastName.Trim();
                entity.Phone              = request.Phone.Trim();
                entity.PlaceOfResidence   = request.PlaceOfResidence.Trim();
                entity.Gender             = request.Gender;
                entity.DateOfBirth        = request.DateOfBirth!.Value;
                entity.NationalNumber     = nationalNumber;
                entity.BankAccount        = request.BankAccount.Trim();
                entity.RoleId             = role.Id;
                if (profileImageUrl != null) entity.Image = profileImageUrl;
                entity.NationalIdImage    = nationalIdImageUrl;
                entity.IsProfileCompleted = true;
                entity.UpdatedAtUtc       = DateTime.UtcNow;

                _unitOfWork.Users.Update(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Reload with role for the response
                var refreshed = await _unitOfWork.Users
                    .Query()
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

                InvalidateUserCache(userId);

                var response = _mapper.Map<UserResponse>(refreshed ?? entity);

                _logger.LogInformation("User {UserId} successfully completed their company profile", userId);

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
                _logger.LogError(ex, "Concurrency conflict while completing company profile for user {UserId}", userId);
                throw new ConcurrencyException("The user was modified by another user. Please refresh and try again.", ex);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
            {
                _logger.LogError(ex, "Unique constraint violation while completing company profile for user {UserId}", userId);
                throw new ConflictException("A unique field value is already taken.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while completing company profile for user {UserId}", userId);
                throw new ServiceException($"Failed to complete company profile: {ex.Message}", ex);
            }
        }

        public async Task<UserResponse> UpdateMeAsync(int userId, UpdateMeRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            if (userId <= 0)
                throw new ArgumentException("Invalid user ID", nameof(userId));

            _logger.LogInformation("User {UserId} attempting to update their profile", userId);

            // Validate request
            var validationResult = await _updateMeValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Profile update validation failed for user {UserId}: {Errors}",
                    userId, string.Join(", ", validationResult.Errors));
                throw new ValidationException(validationResult.Errors);
            }

            try
            {
                var entity = await _unitOfWork.Users
                    .Query()
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

                if (entity == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found for self-update", userId);
                    throw new NotFoundException($"User with ID '{userId}' not found");
                }

                // Handle password change: verify CurrentPassword before applying NewPassword
                if (!string.IsNullOrWhiteSpace(request.NewPassword))
                {
                    if (!_passwordHasher.Verify(request.CurrentPassword!, entity.Password))
                    {
                        _logger.LogWarning("Incorrect current password for user {UserId} during password change", userId);
                        throw new AuthException("Current password is incorrect");
                    }

                    entity.Password = _passwordHasher.Hash(request.NewPassword);
                }

                // Update only fields that were provided (partial update)
                if (!string.IsNullOrWhiteSpace(request.FirstName))
                    entity.FirstName = request.FirstName.Trim();

                if (!string.IsNullOrWhiteSpace(request.LastName))
                    entity.LastName = request.LastName.Trim();

                if (!string.IsNullOrWhiteSpace(request.Phone))
                    entity.Phone = request.Phone.Trim();

                if (request.DateOfBirth.HasValue)
                    entity.DateOfBirth = request.DateOfBirth.Value;

                if (!string.IsNullOrWhiteSpace(request.Gender))
                    entity.Gender = request.Gender;

                if (!string.IsNullOrWhiteSpace(request.PlaceOfResidence))
                    entity.PlaceOfResidence = request.PlaceOfResidence.Trim();

                if (!string.IsNullOrWhiteSpace(request.CurrentLocation))
                    entity.CurrentLocation = request.CurrentLocation.Trim();

                if (!string.IsNullOrWhiteSpace(request.BankAccount))
                    entity.BankAccount = request.BankAccount.Trim();

                if (!string.IsNullOrWhiteSpace(request.NationalNumber))
                {
                    var newNationalNumber = request.NationalNumber.Trim();

                    if (!string.Equals(entity.NationalNumber, newNationalNumber, StringComparison.OrdinalIgnoreCase))
                    {
                        var nationalTaken = await _unitOfWork.Users
                            .Query()
                            .AnyAsync(u => u.NationalNumber == newNationalNumber && u.Id != userId, cancellationToken);

                        if (nationalTaken)
                        {
                            _logger.LogWarning("Duplicate national number attempt for user {UserId}", userId);
                            throw new ConflictException("This national number is already registered.");
                        }

                        entity.NationalNumber = newNationalNumber;
                    }
                }

                if (!string.IsNullOrWhiteSpace(request.PassportNumber))
                {
                    var newPassportNumber = request.PassportNumber.Trim();

                    if (!string.Equals(entity.PassportNumber, newPassportNumber, StringComparison.OrdinalIgnoreCase))
                    {
                        var passportTaken = await _unitOfWork.Users
                            .Query()
                            .AnyAsync(u => u.PassportNumber == newPassportNumber && u.Id != userId, cancellationToken);

                        if (passportTaken)
                        {
                            _logger.LogWarning("Duplicate passport number attempt for user {UserId}", userId);
                            throw new ConflictException("This passport number is already registered.");
                        }

                        entity.PassportNumber = newPassportNumber;
                    }
                }

                // Upload new images if provided
                if (request.Image is { Length: > 0 })
                    entity.Image = await _fileStorage.SaveAsync(request.Image, "profiles", cancellationToken);

                if (request.NationalIdImage is { Length: > 0 })
                    entity.NationalIdImage = await _fileStorage.SaveAsync(request.NationalIdImage, "national-ids", cancellationToken);

                if (request.PassportImage is { Length: > 0 })
                    entity.PassportImage = await _fileStorage.SaveAsync(request.PassportImage, "passports", cancellationToken);

                entity.UpdatedAtUtc = DateTime.UtcNow;

                _unitOfWork.Users.Update(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Invalidate cache
                InvalidateUserCache(userId);

                var response = _mapper.Map<UserResponse>(entity);

                _logger.LogInformation("User {UserId} successfully updated their profile", userId);

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
            catch (AuthException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict while updating user {UserId}", userId);
                throw new ConcurrencyException("The user was modified by another user. Please refresh and try again.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating user {UserId}", userId);
                throw new ServiceException($"Failed to update user: {ex.Message}", ex);
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
                var entity = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
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

                _unitOfWork.Users.Remove(entity);
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
                _logger.LogError(ex, "Unexpected error while deleting user {UserId}", userId);
                throw new ServiceException($"Failed to delete user: {ex.Message}", ex);
            }
        }

        public async Task<bool> AdminDeleteUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID", nameof(userId));

            _logger.LogInformation("Admin attempting to delete user {UserId}", userId);

            try
            {
                var entity = await _unitOfWork.Users
                    .Query()
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

                if (entity == null)
                {
                    _logger.LogWarning("Admin delete: user with ID {UserId} not found", userId);
                    return false;
                }

                // Prevent deleting Admin accounts via this endpoint (safety net)
                if (entity.Role != null && entity.Role.RoleName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Admin delete blocked for user {UserId}: target is also an Admin", userId);
                    throw new ForbiddenException("Admin accounts cannot be deleted from this endpoint.");
                }

                _unitOfWork.Users.Remove(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                InvalidateUserCache(userId);

                _logger.LogInformation("Admin successfully deleted user {UserId}", userId);
                return true;
            }
            catch (ForbiddenException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while admin-deleting user {UserId}", userId);
                throw new ServiceException($"Failed to delete user: {ex.Message}", ex);
            }
        }

        public async Task<UserResponse> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid user ID", nameof(id));

            _logger.LogDebug("Retrieving user with ID {UserId}", id);

            // Try cache first
            var cacheKey = $"{UserCacheKeyPrefix}{id}";
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
                    .Where(u => u.Id == id)
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
                _logger.LogError(ex, "Error retrieving user {UserId}", id);
                throw new ServiceException($"Failed to retrieve user: {ex.Message}", ex);
            }
        }

        public async Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Retrieving all users");

            // Try cache
            if (_cache.TryGetValue(UsersListCacheKey, out IReadOnlyList<UserResponse>? cachedUsers) && cachedUsers != null)
            {
                _logger.LogDebug("Cache hit for all users");
                return cachedUsers;
            }

            try
            {
                var entities = await _unitOfWork.Users
                    .Query()
                    .Include(u => u.Role)
                    .OrderBy(u => u.FirstName)
                    .ThenBy(u => u.LastName)
                    .ToListAsync(cancellationToken);

                var response = _mapper.Map<IReadOnlyList<UserResponse>>(entities);

                // Cache the result with lower priority
                _cache.Set(UsersListCacheKey, response, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration,
                    Priority = CacheItemPriority.Low
                });

                _logger.LogDebug("Successfully retrieved {Count} users", response.Count);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                throw new ServiceException($"Failed to retrieve users: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                return false;

            try
            {
                return await _unitOfWork.Users
                    .Query()
                    .AnyAsync(u => u.Id == id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking existence of user {UserId}", id);
                throw new ServiceException($"Failed to check user existence: {ex.Message}", ex);
            }
        }

        public async Task<IReadOnlyList<UserResponse>> FilterAsync(
            string? firstName = null,
            string? lastName = null,
            string? email = null,
            int? roleId = null,
            bool? isEmailVerified = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Filtering users with parameters - FirstName: {FirstName}, LastName: {LastName}, " +
                             "Email: {Email}, RoleId: {RoleId}, IsEmailVerified: {IsEmailVerified}",
                firstName ?? "Any", lastName ?? "Any", email ?? "Any",
                roleId?.ToString() ?? "Any", isEmailVerified?.ToString() ?? "Any");

            try
            {
                var query = _unitOfWork.Users.Query().Include(u => u.Role).AsQueryable();

                if (!string.IsNullOrWhiteSpace(firstName))
                {
                    var search = firstName.ToLower();
                    query = query.Where(u => u.FirstName != null && u.FirstName.ToLower().Contains(search));
                    _logger.LogDebug("Applied first name filter: {FirstName}", firstName);
                }

                if (!string.IsNullOrWhiteSpace(lastName))
                {
                    var search = lastName.ToLower();
                    query = query.Where(u => u.LastName != null && u.LastName.ToLower().Contains(search));
                    _logger.LogDebug("Applied last name filter: {LastName}", lastName);
                }

                if (!string.IsNullOrWhiteSpace(email))
                {
                    var search = email.ToLower();
                    query = query.Where(u => u.Email.ToLower().Contains(search));
                    _logger.LogDebug("Applied email filter: {Email}", email);
                }

                if (roleId.HasValue && roleId.Value > 0)
                {
                    query = query.Where(u => u.RoleId == roleId.Value);
                    _logger.LogDebug("Applied role filter: {RoleId}", roleId.Value);
                }

                if (isEmailVerified.HasValue)
                {
                    query = query.Where(u => u.IsEmailVerified == isEmailVerified.Value);
                    _logger.LogDebug("Applied email verification filter: {IsEmailVerified}", isEmailVerified.Value);
                }

                var entities = await query
                    .AsNoTracking()
                    .OrderBy(u => u.FirstName)
                    .ThenBy(u => u.LastName)
                    .Take(100)
                    .ToListAsync(cancellationToken);

                var response = _mapper.Map<IReadOnlyList<UserResponse>>(entities);

                _logger.LogInformation("User filter completed. Found {Count} users matching criteria", response.Count);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering users with parameters - FirstName: {FirstName}, LastName: {LastName}, " +
                                     "Email: {Email}, RoleId: {RoleId}",
                    firstName ?? "Any", lastName ?? "Any", email ?? "Any", roleId?.ToString() ?? "Any");
                throw new ServiceException($"Failed to filter users: {ex.Message}", ex);
            }
        }

        #region Private Helper Methods

        private void InvalidateUserCache(int? specificUserId = null)
        {
            if (specificUserId.HasValue)
            {
                var cacheKey = $"{UserCacheKeyPrefix}{specificUserId.Value}";
                _cache.Remove(cacheKey);
                _logger.LogDebug("Invalidated cache for user {UserId}", specificUserId.Value);
            }

            // Always invalidate the list cache when any user changes
            _cache.Remove(UsersListCacheKey);
            _logger.LogDebug("Invalidated all users list cache");
        }

        #endregion
    }
}
