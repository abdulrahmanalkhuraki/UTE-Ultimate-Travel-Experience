using Application.DTOs.Rate.Request;
using Application.DTOs.Rate.Response;
using Application.DTOs.Review.Response;
using Application.Exceptions;
using Application.Interfaces.Rate;
using Application.Interfaces.User;
using Application.Validators.Rate;
using Application.Validators.Review;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace Application.Services
{
    public class RateService : IRateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RateService> _logger;
        private readonly RateCreateValidator _createValidator;
        private readonly ICurrentUserService _currentUser;

        public RateService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<RateService> logger,
            RateCreateValidator createValidator,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        }

        public async Task<RateResponse> CreateAsync(RateCreateRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            _logger.LogInformation("Attempting to create new Rate");

            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Rate creation validation failed: {Errors}",
                    string.Join(", ", validationResult.Errors));
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }

            try
            {
                // check if package Exists
                var package = _unitOfWork.TourPackages
                    .FirstOrDefaultAsync(tp => tp.Id == request.PackageId);

                if (package is null)
                {
                    _logger.LogWarning("Package with ID {PackageId} not found", request.PackageId);
                    throw new NotFoundException($"Package with ID {request.PackageId} not found");
                }
                // check if user has one completed booking in this tourpackage
                await EnsureUserBookedPackage(request.PackageId);

                var rate = _mapper.Map<Rate>(request);
                rate.UserId = _currentUser.UserId ?? 0;

                await _unitOfWork.Rates.AddAsync(rate, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully created rate {ReviewId}", rate.Id);

                return _mapper.Map<RateResponse>(rate);
            }
            catch (Exception ex) when (ex is NotFoundException)
            {
                _logger.LogError(ex, "Unexpected error while creating rate");
                throw new ServiceException($"Failed to create rate: {ex.Message}", ex);
            }
        }

        public async Task<IReadOnlyList<RateResponse>> GetAsync(int? userId, int? tourPackageId, CancellationToken cancellationToken)
        {
            if (userId.HasValue && userId <= 0)
                throw new ArgumentException($"Invalid User Id {userId}");

            if (tourPackageId.HasValue && tourPackageId <= 0)
                throw new ArgumentException($"Invalid Tour Package Id {tourPackageId}");

            _logger.LogDebug("Retrieving rates");

            try
            {
                IQueryable<Rate> query = _unitOfWork.Rates
                    .Query()
                    .Include(r => r.User)
                    .Include(r => r.Package);

                if (userId.HasValue)
                {
                    query = query.Where(r => r.UserId == userId.Value);
                }

                if (tourPackageId.HasValue)
                {
                    query = query.Where(r => r.PackageId == tourPackageId.Value);
                }

                var rates = await query
                    .OrderByDescending(r => r.CreatedAtUtc)
                    .ToListAsync(cancellationToken);

                var response = _mapper.Map<IReadOnlyList<RateResponse>>(rates);

                _logger.LogDebug("Successfully retrieved {Count} rates", response.Count);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving rates");

                throw new ServiceException("Failed to retrieve rates.", ex);
            }
        }

        #region Helpers
        private async Task EnsureUserBookedPackage(int packageId)
        {
            var isBooked = await _unitOfWork.Bookings
                .Query()
                .Where(b => b.TourPackageId == packageId)
                .Where(b => b.UserId == _currentUser.UserId)
                .Where(b => b.Status == Domain.Enums.BookingStatus.Completed)
                .AnyAsync();

            if (!isBooked)
            {
                _logger.LogWarning($"User {_currentUser.UserId} attempted to rate TourPackage {packageId} without a completed booking. Rating rejected.");
                throw new BusinessRuleException("Unable to submit rating. A completed booking is required before rating a tour package.");
            }
        }
        #endregion
    }
}
