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
                throw new ArgumentException("Invalid tour package ID", nameof(tourPackageId));

            var userId = _currentUser.UserId ?? throw new AuthException("User must be authenticated");

            _logger.LogInformation("Attempting to add tour package {PackageId} to user {UserId} wishlist",
                tourPackageId, userId);

            var exists = await _unitOfWork.TourPackages.AnyAsync(tp => tp.Id == tourPackageId && !tp.IsDeleted);

            if (!exists)
            {
                _logger.LogWarning("Tour package {PackageId} not found", tourPackageId);
                throw new NotFoundException($"Tour package with ID {tourPackageId} not found");
            }

            var isAdded = await _unitOfWork.Wishlists
                .Query()
                .AnyAsync(w => w.TourPackageId == tourPackageId && w.UserId == userId);

            if (isAdded)
            {
                _logger.LogWarning("Tour package {PackageId} already in user wishlist", tourPackageId);
                throw new ConflictException($"Tour package with ID {tourPackageId} is already in your wishlist");
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

                _logger.LogInformation("Tour package {PackageId} added to user wishlist", tourPackageId);
                return true;
            }
            catch (Exception ex) when (ex is not NotFoundException and not ConflictException and not ArgumentException)
            {
                _logger.LogError(ex, "Unexpected error while adding tour package {PackageId} to user wishlist", tourPackageId);
                throw new ServiceException($"Failed to add tour package to wishlist: {ex.Message}", ex);
            }
        }

        public async Task<bool> RemoveFromWishlistAsync(int tourPackageId, CancellationToken cancellationToken = default)
        {
            if (tourPackageId <= 0)
                throw new ArgumentException("Invalid tour package ID", nameof(tourPackageId));

            var userId = _currentUser.UserId ?? throw new AuthException("User must be authenticated");

            _logger.LogInformation("Attempting to remove tour package {PackageId} from user {UserId} wishlist",
                tourPackageId, userId);

            var exists = await _unitOfWork.TourPackages.AnyAsync(tp => tp.Id == tourPackageId && !tp.IsDeleted);

            if (!exists)
            {
                _logger.LogWarning("Tour package {PackageId} not found", tourPackageId);
                throw new NotFoundException($"Tour package with ID {tourPackageId} not found");
            }

            var entity = await _unitOfWork.Wishlists
                .Query()
                .FirstOrDefaultAsync(w => w.TourPackageId == tourPackageId && w.UserId == userId);

            if (entity is null)
            {
                _logger.LogWarning("Tour package {PackageId} is not in user wishlist", tourPackageId);
                throw new NotFoundException($"Tour package with ID {tourPackageId} is not in your wishlist");
            }

            try
            {
                _unitOfWork.Wishlists.Remove(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Tour package {PackageId} removed from user wishlist", tourPackageId);
                return true;
            }
            catch (Exception ex) when (ex is not NotFoundException and not ArgumentException)
            {
                _logger.LogError(ex, "Unexpected error while removing tour package {PackageId} from user wishlist", tourPackageId);
                throw new ServiceException($"Failed to remove tour package from wishlist: {ex.Message}", ex);
            }
        }

        public async Task<IReadOnlyList<TourPackageResponse>> GetWishlistAsync(CancellationToken cancellationToken = default)
        {
            var userId = _currentUser.UserId ?? throw new AuthException("User must be authenticated");

            var entities = await _unitOfWork.Wishlists.Query()
                .Where(w => w.UserId == userId)
                .Include(w => w.TourPackage)
                .Select(w => w.TourPackage)
                .ToListAsync(cancellationToken);

            return _mapper.Map<IReadOnlyList<TourPackageResponse>>(entities);
        }
    }
}
