using Application.Common.Constants;
using Application.Common.Logging;
using Application.DTOs.Rate.Request;
using Application.DTOs.Rate.Response;
using Application.DTOs.Review.Response;
using Application.Exceptions;
using Application.Interfaces.Localization;
using Application.Interfaces.Rate;
using Application.Interfaces.User;
using Application.Validators.Rate;
using Application.Validators.Review;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class RateService : IRateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizedMapper _mapper;
        private readonly ILogger<RateService> _logger;
        private readonly RateCreateValidator _createValidator;
        private readonly ICurrentUserService _currentUser;
        private const string ObjectName = "Rate";

        public RateService(
            IUnitOfWork unitOfWork,
            ILocalizedMapper mapper,
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

            _logger.StartOperation("Create", ObjectName, 0);

            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.ValidationFailed("Create", ObjectName, string.Join(", ", validationResult.Errors));
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }

            try
            {
                var package = await _unitOfWork.TourPackages
                    .FirstOrDefaultAsync(tp => tp.Id == request.PackageId);

                if (package is null)
                {
                    _logger.EntityNotFound("Tour Package", request.PackageId);
                    throw new NotFoundException(ExceptionMessages.NotFound("Tour Package", request.PackageId));
                }

                await EnsureUserBookedPackage(request.PackageId);

                var userId = _currentUser.UserId ?? throw new AuthException(ExceptionMessages.Auth());

                await EnsureNotRatedBeforeAsync(request.PackageId, userId, cancellationToken);

                var rate = _mapper.Map<Rate>(request);
                rate.UserId = userId;

                await _unitOfWork.Rates.AddAsync(rate, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.SuccessfulOperation("Create", ObjectName);

                var created = await BuildDetailedQuery()
                    .FirstOrDefaultAsync(r => r.Id == rate.Id, cancellationToken);

                return _mapper.Map<RateResponse>(created!);
            }
            catch (Exception ex) when (ex is NotFoundException)
            {
                _logger.ServerError("Create", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("create", ObjectName, ex.Message), ex);
            }
        }

        public async Task<IReadOnlyList<RateResponse>> GetAsync(int? userId, int? tourPackageId, CancellationToken cancellationToken)
        {
            if (userId.HasValue && userId <= 0)
                throw new ArgumentException($"Invalid User Id {userId}");

            if (tourPackageId.HasValue && tourPackageId <= 0)
                throw new ArgumentException($"Invalid Tour Package Id {tourPackageId}");

            _logger.StartOperation("Retrieve", ObjectName, 0);

            try
            {
                IQueryable<Rate> query = BuildDetailedQuery();

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
                _logger.ServerError("Retrieve", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("retrieve", ObjectName, ex.Message), ex);
            }
        }

        #region Helpers
        private IQueryable<Rate> BuildDetailedQuery()
        {
            return _unitOfWork.Rates.Query()
                .Include(r => r.User).ThenInclude(u => u.Person)
                    .ThenInclude(p => p.NationalityCountry).ThenInclude(n => n.Translations)
                .Include(r => r.User).ThenInclude(u => u.Person)
                    .ThenInclude(p => p.ResidentialCity).ThenInclude(c => c.Translations)
                .Include(r => r.User).ThenInclude(u => u.Role)
                .Include(r => r.Package).ThenInclude(p => p.Translations);
        }

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
                _logger.BusinessRuleViolated("Tour Package", "A completed booking is required before rating");
                throw new BusinessRuleException(ExceptionMessages.BusinessRule(
                    "Unable to submit rating. A completed booking is required before rating a tour package."));
            }
        }

        private async Task EnsureNotRatedBeforeAsync(int packageId, int userId, CancellationToken ct)
        {
            var isRatedBefore = await _unitOfWork.Rates
                .AnyAsync(r => r.PackageId == packageId && r.UserId == userId, ct);

            if (isRatedBefore)
            {
                _logger.BusinessRuleViolated(ObjectName, "The user has already rated this tour package");
                throw new BusinessRuleException(ExceptionMessages.BusinessRule("You have already rated this tour package."));
            }
        }
        #endregion
    }
}
