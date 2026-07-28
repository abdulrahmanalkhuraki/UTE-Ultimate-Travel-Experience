using Application.Common.Constants;
using Application.Common.Logging;
using Application.DTOs.TourPackage.Response;
using Application.Exceptions;
using Application.Interfaces.TourPackage;
using Application.Interfaces.User;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<WishlistService> _logger;
        private readonly ICurrentUserService _currentUser;
        private const string ObjectName = "Tour Package";

        public WishlistService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<WishlistService> logger,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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

        public async Task<IReadOnlyList<TourPackageResponse>> GetWishlistAsync(CancellationToken cancellationToken = default)
        {
            var userId = _currentUser.UserId ?? throw new AuthException(ExceptionMessages.Auth());

            var entities = await _unitOfWork.Wishlists.Query()
                .Where(w => w.UserId == userId)
                .Include(w => w.TourPackage)
                .Select(w => w.TourPackage)
                .ToListAsync(cancellationToken);

            return _mapper.Map<IReadOnlyList<TourPackageResponse>>(entities);
        }
    }
}
