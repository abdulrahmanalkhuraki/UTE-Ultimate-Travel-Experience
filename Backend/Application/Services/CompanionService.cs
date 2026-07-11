using Application.DTOs.Companion.Request;
using Application.DTOs.Companion.Response;
using Application.DTOs.TourPackage.Response;
using Application.Exceptions;
using Application.Interfaces.Companion;
using Application.Interfaces.User;
using Application.Validators.Companion;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ValidationException = Application.Exceptions.ValidationException;

namespace Application.Services
{
    public class CompanionService : ICompanionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CompanionService> _logger;
        private readonly IFileStorage _fileStorage;
        private readonly ICurrentUserService _currentUser;
        private readonly CompanionCreateValidator _createValidator;
        private readonly CompanionUpdateValidator _updateValidator;

        private const string CompanionImageFolder = "companion-images";

        public CompanionService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CompanionService> logger,
            IFileStorage fileStorage,
            ICurrentUserService currentUser,
            CompanionCreateValidator createValidator,
            CompanionUpdateValidator updateValidator)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
        }

        public async Task<CompanionResponse> CreateAsync(int userId, CompanionCreateRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var userExists = await _unitOfWork.Users
                .Query()
                .AnyAsync(u => u.Id == userId, cancellationToken);
            if (!userExists)
                throw new NotFoundException($"User with ID {userId} not found");
            
            await EnsureCountryExistsAsync(request.NationalityCountryId, cancellationToken);
            await EnsureCityExistsAsync(request.ResidentialCityId, cancellationToken);

            try
            {
                var person = _mapper.Map<Person>(request);
                person.CreatedAtUtc = DateTime.UtcNow;
                person.UpdatedAtUtc = DateTime.UtcNow;

                if (request.NationalIdCard is not null)
                    person.NationalIdCard = await _fileStorage.SaveAsync(request.NationalIdCard, CompanionImageFolder, cancellationToken);
                if (request.PassportScan is not null)
                    person.PassportScan = await _fileStorage.SaveAsync(request.PassportScan, CompanionImageFolder, cancellationToken);
                if (request.ResidencyCard is not null)
                    person.ResidencyCard = await _fileStorage.SaveAsync(request.ResidencyCard, CompanionImageFolder, cancellationToken);

                var companion = new Companion
                {
                    Relationship = request.Relationship,
                    UserId = userId,
                    Person = person,
                };

                await _unitOfWork.Companions.AddAsync(companion, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Companion {CompanionId} created for user {UserId}", companion.Id, userId);

                return await BuildResponseAsync(companion.Id, cancellationToken);
            }
            catch (Exception ex) when (ex is not ValidationException and not NotFoundException)
            {
                _logger.LogError(ex, "Error creating companion for user {UserId}", userId);
                throw new ServiceException($"Failed to create companion: {ex.Message}", ex);
            }
        }

        public async Task<CompanionResponse> GetAsync(int id, int userId, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid companion ID", nameof(id));

            await EnsureCompanionBelongsToUserAsync(id, userId, cancellationToken);

            return await BuildResponseAsync(id, cancellationToken);
        }

        public async Task<IReadOnlyList<CompanionResponse>> GetAllAsync(int userId, CancellationToken cancellationToken)
        {
            var entities = await QueryWithGraph()
                .Where(c => c.UserId == userId)
                .ToListAsync(cancellationToken);

            var responses = new List<CompanionResponse>(entities.Count);
            foreach (var entity in entities)
            {
                responses.Add(await BuildResponseFromEntity(entity, cancellationToken));
            }

            return responses.AsReadOnly();
        }

        public async Task<CompanionResponse> UpdateAsync(int id, int userId, CompanionUpdateRequest request, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid companion ID", nameof(id));

            ArgumentNullException.ThrowIfNull(request, nameof(request));

            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            if (request.NationalityCountryId.HasValue)
                await EnsureCountryExistsAsync(request.NationalityCountryId.Value, cancellationToken);
            if (request.ResidentialCityId.HasValue)
                await EnsureCityExistsAsync(request.ResidentialCityId.Value, cancellationToken);

            try
            {
                var entity = await _unitOfWork.Companions
                    .Query()
                    .Include(c => c.Person)
                    .Include(c => c.CompanionBookings)
                        .ThenInclude(cb => cb.Booking)
                            .ThenInclude(b => b.TourPackage)
                    .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

                if (entity is null)
                    throw new NotFoundException($"Companion with ID {id} not found");

                await EnsureCompanionBelongsToUserAsync(id, userId, cancellationToken);

                await ApplyPartialUpdateAsync(entity, request, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Companion {CompanionId} updated", id);

                return await BuildResponseFromEntity(entity, cancellationToken);
            }
            catch (Exception ex) when (ex is not ValidationException and not NotFoundException and not ForbiddenException)
            {
                _logger.LogError(ex, "Error updating companion {CompanionId}", id);
                throw new ServiceException($"Failed to update companion: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id, int userId, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid companion ID", nameof(id));

            await EnsureCompanionBelongsToUserAsync(id, userId, cancellationToken);

            try
            {
                var entity = await _unitOfWork.Companions
                    .Query()
                    .Include(c => c.Person)
                    .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

                if (entity is null)
                    return false;

                if (entity.Person is null)
                    return false;


                _unitOfWork.Persons.Remove(entity.Person);
                _unitOfWork.Companions.Remove(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Companion {CompanionId} deleted", id);
                return true;
            }
            catch (Exception ex) when (ex is not NotFoundException and not ForbiddenException)
            {
                _logger.LogError(ex, "Error deleting companion {CompanionId}", id);
                throw new ServiceException($"Failed to delete companion: {ex.Message}", ex);
            }
        }

        #region Helpers

        private async Task ApplyPartialUpdateAsync(Companion entity, CompanionUpdateRequest request, CancellationToken cancellationToken)
        {
            _mapper.Map(request, entity.Person);

            if (request.NationalityCountryId.HasValue) 
                entity.Person.NationalityCountryId = request.NationalityCountryId.Value;
            if (request.Relationship.HasValue) 
                entity.Relationship = request.Relationship.Value;

            entity.Person.UpdatedAtUtc = DateTime.UtcNow;

            if (request.NationalIdCard is not null)
                entity.Person.NationalIdCard = await _fileStorage.SaveAsync(request.NationalIdCard, CompanionImageFolder, cancellationToken);
            if (request.PassportScan is not null)
                entity.Person.PassportScan = await _fileStorage.SaveAsync(request.PassportScan, CompanionImageFolder, cancellationToken);
            if (request.ResidencyCard is not null)
                entity.Person.ResidencyCard = await _fileStorage.SaveAsync(request.ResidencyCard, CompanionImageFolder, cancellationToken);
        }

        private IQueryable<Companion> QueryWithGraph() =>
            _unitOfWork.Companions
                .Query()
                .AsNoTracking()
                .Include(c => c.Person)
                    .ThenInclude(p => p.ResidentialCity)
                .Include(c => c.CompanionBookings)
                    .ThenInclude(cb => cb.Booking)
                        .ThenInclude(b => b.TourPackage);

        private async Task<CompanionResponse> BuildResponseAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await QueryWithGraph().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            if (entity is null)
                throw new NotFoundException($"Companion with ID {id} not found");
            return await BuildResponseFromEntity(entity, cancellationToken);
        }

        private async Task<CompanionResponse> BuildResponseFromEntity(Companion entity, CancellationToken cancellationToken)
        {
            var response = _mapper.Map<CompanionResponse>(entity);

            response.RegistrationDate = DateOnly.FromDateTime(entity.Person.CreatedAtUtc);

            var CompletedCompanionBookings = entity.CompanionBookings.Where(cb => cb.Booking.Status == BookingStatus.Completed).ToList();

            if (CompletedCompanionBookings != null &&
                CompletedCompanionBookings.Count != 0)
            {

                response.JoinedPackagesCount = entity.CompanionBookings
                    .Select(cb => cb.Booking.TourPackageId)
                    .Distinct()
                    .Count();

                response.TotalAmountSpent = entity.CompanionBookings
                    .Where(cb => cb.Booking.TourPackage != null)
                    .Sum(cb => cb.Booking.TourPackage.PricePerPerson);

                var lastBooking = entity.CompanionBookings
                    .Where(cb => cb.Booking.TourPackage != null)
                    .OrderByDescending(cb => cb.Booking.TourPackage.StartDate)
                    .FirstOrDefault();

                if (lastBooking?.Booking.TourPackage != null)
                    response.LastTourPackage = _mapper.Map<TourPackageResponse>(lastBooking.Booking.TourPackage);
            }

            return response;
        }

        private async Task EnsureCompanionBelongsToUserAsync(int companionId, int userId, CancellationToken cancellationToken)
        {
            var belongs = await _unitOfWork.Companions
                .Query().AsNoTracking()
                .AnyAsync(c => c.Id == companionId && c.UserId == userId, cancellationToken);

            if (!belongs)
                throw new ForbiddenException("This companion does not belong to you.");
        }

        private async Task EnsureCountryExistsAsync(int countryId, CancellationToken cancellationToken)
        {
            var exists = await _unitOfWork.Countries
                .Query().AsNoTracking()
                .AnyAsync(c => c.Id == countryId, cancellationToken);
            if (!exists)
                throw new NotFoundException($"Country with ID {countryId} not found");
        }

        private async Task EnsureCityExistsAsync(int cityId, CancellationToken cancellationToken)
        {
            var exists = await _unitOfWork.Cities
                .Query().AsNoTracking()
                .AnyAsync(c => c.Id == cityId, cancellationToken);
            if (!exists)
                throw new NotFoundException($"City with ID {cityId} not found");
        }

        #endregion
    }
}
