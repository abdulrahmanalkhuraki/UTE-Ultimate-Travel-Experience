using Application.DTOs.Booking.Response;
using Application.DTOs.Review.Request;
using Application.DTOs.Review.Response;
using Application.Exceptions;
using Application.Interfaces.Review;
using Application.Interfaces.User;
using Application.Validators.Review;
using AutoMapper;
using Azure;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ReviewService> _logger;
        private readonly ReviewCreateValidator _createValidator;
        private readonly ICurrentUserService _currentUser;

        public ReviewService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ReviewService> logger,
            ReviewCreateValidator createValidator,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        }

        public async Task<ReviewResponse> CreateAsync(ReviewCreateRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            _logger.LogInformation("Attempting to create new Review");

            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Review creation validation failed: {Errors}",
                    string.Join(", ", validationResult.Errors));
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }

            try
            {
                // check if package Exists
                var package = _unitOfWork.TourPackages.FirstOrDefaultAsync(tp => tp.Id == request.PackageId);

                if (package is null)
                {
                    _logger.LogWarning("Package with ID {PackageId} not found", request.PackageId);
                    throw new NotFoundException($"Package with ID {request.PackageId} not found");
                }

                await EnsureUserBookedPackage(request.PackageId);

                var review = _mapper.Map<Review>(request);
                review.UserId = _currentUser.UserId ?? 0;

                await _unitOfWork.Reviews.AddAsync(review, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully created review {ReviewId}", review.Id);

                return _mapper.Map<ReviewResponse>(review);
            }
            catch (Exception ex) when (ex is NotFoundException)
            {
                _logger.LogError(ex, "Unexpected error while creating review");
                throw new ServiceException($"Failed to create review: {ex.Message}", ex);
            }
        }

        public async Task<IReadOnlyList<ReviewResponse>> GetAsync(
            int? userId,
            int? tourPackageId,
            CancellationToken cancellationToken)
        {
            if (userId.HasValue && userId <= 0)
                throw new ArgumentException($"Invalid User Id {userId}");

            if (tourPackageId.HasValue && tourPackageId <= 0)
                throw new ArgumentException($"Invalid Tour Package Id {tourPackageId}");

            _logger.LogDebug("Retrieving reviews");

            try
            {
                IQueryable<Review> query = _unitOfWork.Reviews
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

                var reviews = await query
                    .OrderByDescending(r => r.CreatedAtUtc)
                    .ToListAsync(cancellationToken);

                var response = _mapper.Map<IReadOnlyList<ReviewResponse>>(reviews);

                _logger.LogDebug("Successfully retrieved {Count} reviews", response.Count);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reviews");

                throw new ServiceException("Failed to retrieve reviews.", ex);
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
                _logger.LogWarning($"User {_currentUser.UserId} attempted to review TourPackage {packageId} without a completed booking. review rejected.");
                throw new BusinessRuleException("Unable to submit review. A completed booking is required before review a tour package.");
            }
        }
        #endregion

    }
}
