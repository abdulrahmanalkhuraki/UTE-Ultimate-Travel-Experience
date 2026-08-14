using Application.Common.Constants;
using Application.Common.Logging;
using Application.DTOs.Booking.Response;
using Application.DTOs.Review.Request;
using Application.DTOs.Review.Response;
using Application.Exceptions;
using Application.Interfaces.Localization;
using Application.Interfaces.Review;
using Application.Interfaces.User;
using Application.Validators.Review;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizedMapper _mapper;
        private readonly ILogger<ReviewService> _logger;
        private readonly ReviewCreateValidator _createValidator;
        private readonly ICurrentUserService _currentUser;
        private const string ObjectName = "Review";

        public ReviewService(
            IUnitOfWork unitOfWork,
            ILocalizedMapper mapper,
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

            _logger.StartOperation("Create", ObjectName, 0);

            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.ValidationFailed("Create", ObjectName, string.Join(", ", validationResult.Errors));
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }

            try
            {
                var package = await _unitOfWork.TourPackages.FirstOrDefaultAsync(tp => tp.Id == request.PackageId);

                if (package is null)
                {
                    _logger.EntityNotFound("Tour Package", request.PackageId);
                    throw new NotFoundException(ExceptionMessages.NotFound("Tour Package", request.PackageId));
                }

                await EnsureUserBookedPackage(request.PackageId);

                var review = _mapper.Map<Review>(request);
                review.UserId = _currentUser.UserId ?? 0;

                await _unitOfWork.Reviews.AddAsync(review, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.SuccessfulOperation("Create", ObjectName);

                return _mapper.Map<ReviewResponse>(review);
            }
            catch (Exception ex) when (ex is NotFoundException)
            {
                _logger.ServerError("Create", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("create", ObjectName, ex.Message), ex);
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

            _logger.StartOperation("Retrieve", ObjectName, 0);

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
                _logger.ServerError("Retrieve", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("retrieve", ObjectName, ex.Message), ex);
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
                _logger.BusinessRuleViolated("Tour Package", "A completed booking is required before reviewing");
                throw new BusinessRuleException(ExceptionMessages.BusinessRule(
                    "Unable to submit review. A completed booking is required before reviewing a tour package."));
            }
        }
        #endregion

    }
}
