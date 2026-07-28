using Application.Common.Constants;
using Application.Common.Logging;
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
        private const string ObjectName = "Favorite";

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
                throw new ArgumentException(ExceptionMessages.InvalidId(ObjectName));

            var userId = _currentUser.UserId ?? 0;

            _logger.StartOperation("Add", ObjectName, userId);

            try
            {
                var company = await _unitOfWork.TourCompanies
                    .FirstOrDefaultAsync(tc => tc.Id == companyId, cancellationToken);

                if (company is null)
                {
                    _logger.EntityNotFound(ObjectName, companyId);
                    throw new NotFoundException(ExceptionMessages.NotFound(ObjectName, companyId));
                }

                if (company.UserId == userId)
                {
                    _logger.BusinessRuleViolated(ObjectName, "You cannot add your own company to favorites");
                    throw new BusinessRuleException(ExceptionMessages.BusinessRule("You cannot add your own company to favorites"));
                }

                var existing = await _unitOfWork.Favorites
                    .FirstOrDefaultAsync(f => f.CompanyId == companyId && f.UserId == userId, cancellationToken);

                if (existing is not null)
                {
                    _logger.BusinessRuleViolated(ObjectName, "This company is already in your favorites");
                    throw new BusinessRuleException(ExceptionMessages.BusinessRule("This company is already in your favorites"));
                }

                var favorite = new Favorite
                {
                    CompanyId = companyId,
                    UserId = userId
                };

                await _unitOfWork.Favorites.AddAsync(favorite, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.SuccessfulOperation(userId, "Add", ObjectName, companyId);

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
                _logger.ServerError("Add", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("add", ObjectName, ex.Message), ex);
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
                _logger.ServerError("Retrieve", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("retrieve", ObjectName, ex.Message), ex);
            }
        }
    }
}
