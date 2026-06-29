using Application.DTOs.Favorite.Response;
using Application.Exceptions;
using Application.Interfaces.Favorite;
using Application.Interfaces.User;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<FavoriteService> _logger;
        private readonly ICurrentUserService _currentUser;

        public FavoriteService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<FavoriteService> logger,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        }

        public async Task<FavoriteResponse> AddAsync(int companyId, CancellationToken cancellationToken)
        {
            if (companyId <= 0)
                throw new ArgumentException($"Invalid Company Id {companyId}");

            var userId = _currentUser.UserId ?? 0;

            _logger.LogInformation("User {UserId} attempting to add company {CompanyId} to favorites", userId, companyId);

            try
            {
                var company = await _unitOfWork.TourCompanies
                    .FirstOrDefaultAsync(tc => tc.Id == companyId, cancellationToken);

                if (company is null)
                {
                    _logger.LogWarning("Company with ID {CompanyId} not found", companyId);
                    throw new NotFoundException($"Company with ID {companyId} not found");
                }

                if (company.UserId == userId)
                {
                    _logger.LogWarning("User {UserId} attempted to favorite their own company {CompanyId}", userId, companyId);
                    throw new BusinessRuleException("You cannot add your own company to favorites");
                }

                var existing = await _unitOfWork.Favorites
                    .FirstOrDefaultAsync(f => f.CompanyId == companyId && f.UserId == userId, cancellationToken);

                if (existing is not null)
                {
                    _logger.LogWarning("Company {CompanyId} is already in user {UserId} favorites", companyId, userId);
                    throw new BusinessRuleException("This company is already in your favorites");
                }

                var favorite = new Favorite
                {
                    CompanyId = companyId,
                    UserId = userId
                };

                await _unitOfWork.Favorites.AddAsync(favorite, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully added company {CompanyId} to user {UserId} favorites", companyId, userId);

                return new FavoriteResponse
                {
                    Id = favorite.Id,
                    CompanyId = company.Id,
                    CompanyName = company.Name,
                    CompanyLogo = company.Logo,
                    CreatedAtUtc = favorite.CreatedAtUtc
                };
            }
            catch (Exception ex) when (ex is NotFoundException or BusinessRuleException)
            {
                _logger.LogError(ex, "Error adding company {CompanyId} to favorites for user {UserId}", companyId, userId);
                throw new ServiceException($"Failed to add favorite: {ex.Message}", ex);
            }
        }

        public async Task<IReadOnlyList<FavoriteResponse>> GetUserFavoritesAsync(CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId ?? 0;

            _logger.LogDebug("Retrieving favorites for user {UserId}", userId);

            try
            {
                var favorites = await _unitOfWork.Favorites
                    .Query()
                    .Include(f => f.Company)
                    .Where(f => f.UserId == userId)
                    .OrderByDescending(f => f.CreatedAtUtc)
                    .ToListAsync(cancellationToken);

                var response = favorites.Select(f => new FavoriteResponse
                {
                    Id = f.Id,
                    CompanyId = f.Company.Id,
                    CompanyName = f.Company.Name,
                    CompanyLogo = f.Company.Logo,
                    CreatedAtUtc = f.CreatedAtUtc
                }).ToList();

                _logger.LogDebug("Successfully retrieved {Count} favorites for user {UserId}", response.Count, userId);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving favorites for user {UserId}", userId);
                throw new ServiceException("Failed to retrieve favorites.", ex);
            }
        }
    }
}
