using Application.Common.Constants;
using Application.Common.Logging;
using Application.DTOs.Pagination;
using Application.DTOs.TourPackage.Response;
using Application.Exceptions;
using Application.Interfaces.Localization;
using Application.Interfaces.TourPackage;
using Application.Interfaces.User;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizedMapper _mapper;
        private readonly ILanguageContext _language;
        private readonly ILogger<WishlistService> _logger;
        private readonly ICurrentUserService _currentUser;
        private const string ObjectName = "Tour Package";

        public WishlistService(
            IUnitOfWork unitOfWork,
            ILocalizedMapper mapper,
            ILanguageContext language,
            ILogger<WishlistService> logger,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _language = language ?? throw new ArgumentNullException(nameof(language));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        }

        public async Task<bool> AddToWishlistAsync(int tourPackageId, CancellationToken cancellationToken = default)
        {
            if (tourPackageId <= 0)
                throw new ArgumentException(ExceptionMessages.InvalidId(ObjectName), nameof(tourPackageId));

            var userId = _currentUser.UserId ?? throw new AuthException(ExceptionMessages.Auth());

            _logger.StartOperation("Add to Wishlist", ObjectName, tourPackageId, userId);

            var exists = await _unitOfWork.TourPackages.AnyAsync(tp => tp.Id == tourPackageId && !tp.IsDeleted);

            if (!exists)
            {
                _logger.EntityNotFound(ObjectName, tourPackageId);
                throw new NotFoundException(ExceptionMessages.NotFound(ObjectName, tourPackageId));
            }

            var isAdded = await _unitOfWork.Wishlists
                .Query()
                .AnyAsync(w => w.TourPackageId == tourPackageId && w.UserId == userId);

            if (isAdded)
            {
                _logger.ConflictDetected(ObjectName, "This tour package is already in your wishlist");
                throw new ConflictException(ExceptionMessages.Conflict(ObjectName, "already in your wishlist"));
            }

            try
            {
                var wishlist = new Wishlist
                {
                    UserId = userId,
                    TourPackageId = tourPackageId,
                    CreatedAtUtc = DateTime.UtcNow,
                    UpdatedAtUtc = DateTime.UtcNow,
                };

                await _unitOfWork.Wishlists.AddAsync(wishlist, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.SuccessfulOperation(userId, "Add to Wishlist", ObjectName, tourPackageId);
                return true;
            }
            catch (Exception ex) when (ex is not NotFoundException and not ConflictException and not ArgumentException)
            {
                _logger.ServerError("Add to Wishlist", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("add to wishlist", ObjectName, ex.Message), ex);
            }
        }

        public async Task<bool> RemoveFromWishlistAsync(int tourPackageId, CancellationToken cancellationToken = default)
        {
            if (tourPackageId <= 0)
                throw new ArgumentException(ExceptionMessages.InvalidId(ObjectName), nameof(tourPackageId));

            var userId = _currentUser.UserId ?? throw new AuthException(ExceptionMessages.Auth());

            _logger.StartOperation("Remove from Wishlist", ObjectName, tourPackageId, userId);

            var exists = await _unitOfWork.TourPackages.AnyAsync(tp => tp.Id == tourPackageId && !tp.IsDeleted);

            if (!exists)
            {
                _logger.EntityNotFound(ObjectName, tourPackageId);
                throw new NotFoundException(ExceptionMessages.NotFound(ObjectName, tourPackageId));
            }

            var entity = await _unitOfWork.Wishlists
                .Query()
                .FirstOrDefaultAsync(w => w.TourPackageId == tourPackageId && w.UserId == userId);

            if (entity is null)
            {
                _logger.EntityNotFound(ObjectName, tourPackageId);
                throw new NotFoundException(ExceptionMessages.NotFound(ObjectName, tourPackageId));
            }

            try
            {
                _unitOfWork.Wishlists.Remove(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.SuccessfulOperation(userId, "Remove from Wishlist", ObjectName, tourPackageId);
                return true;
            }
            catch (Exception ex) when (ex is not NotFoundException and not ArgumentException)
            {
                _logger.ServerError("Remove from Wishlist", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("remove from wishlist", ObjectName, ex.Message), ex);
            }
        }

        public async Task<PaginatedResponse<TourPackageResponse>> GetWishlistAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
            {
                throw new ValidationException(ExceptionMessages.InvalidPagination());
            }

            var userId = _currentUser.UserId ?? throw new AuthException(ExceptionMessages.Auth());

            try
            {
                var entities = await _unitOfWork.Wishlists.Query()
                .Where(w => w.UserId == userId)
                .Include(w => w.TourPackage)
                    .ThenInclude(tp => tp.Translations)
                .Select(w => w.TourPackage)
                .ToListAsync(cancellationToken);

                var items = _mapper.Map<IReadOnlyList<TourPackageResponse>>(entities);

                var paginationMetadata = new PaginationMetadata
                {
                    PageSize = pageSize,
                    Page = page,
                    TotalItems = items.Count
                };

                return new PaginatedResponse<TourPackageResponse>() { Items = items,Pagination =  paginationMetadata};
            }
            catch(Exception ex)
            {
                _logger.ServerError("retrieve", "Wishlist", ex);
                throw new ServiceException(ExceptionMessages.ServiceException("retrieve", "Wishlist", ex.Message), ex);
            }

        }
    }
}
