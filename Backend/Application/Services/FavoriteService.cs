using Application.Common.Constants;
using Application.Common.Logging;
using Application.DTOs.Favorite.Response;
using Application.DTOs.Pagination;
using Application.Exceptions;
using Application.Interfaces.Favorite;
using Application.Interfaces.User;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
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

        public async Task<bool> AddAsync(int companyId, CancellationToken cancellationToken)
        {
            if (companyId <= 0)
                throw new ArgumentException(ExceptionMessages.InvalidId(ObjectName));
            
            var userId = _currentUser.UserId ?? throw new AuthException(ExceptionMessages.Auth());

            _logger.LogInformation("Attempting To Add Company {companyId} to user {userId} Favorites",companyId,userId);

            try
            {
                var isCompanyExists = await _unitOfWork.TourCompanies
                    .AnyAsync(tc => tc.Id == companyId, cancellationToken);

                if (!isCompanyExists)
                {
                    _logger.EntityNotFound(ObjectName, companyId);
                    throw new NotFoundException(ExceptionMessages.NotFound(ObjectName, companyId));
                }

                var isFavoritedBefore = await _unitOfWork.Favorites
                    .AnyAsync(f => f.CompanyId == companyId && f.UserId == userId, cancellationToken);

                if (isFavoritedBefore)
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

                _logger.LogInformation("Company {companyId} has been add successfully to user {userId} favorites.",companyId,userId);

                return true;
            }
            catch (Exception ex) when (ex is NotFoundException or BusinessRuleException)
            {
                _logger.ServerError("Add", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("add", ObjectName, ex.Message), ex);
            }
        }

        public async Task<PaginatedResponse<FavoriteResponse>> GetUserFavoritesAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId ?? throw new AuthException(ExceptionMessages.Auth());

            _logger.LogDebug("Retrieving favorites for user {UserId}", userId);

            try
            {
                var favorites = await _unitOfWork.Favorites
                    .Query()
                    .Include(f => f.Company)
                    .Where(f => f.UserId == userId)
                    .OrderByDescending(f => f.CreatedAtUtc)
                    .Select(f => new FavoriteResponse
                    {
                        Id = f.Id,
                        CompanyId = f.CompanyId,
                        CompanyName = f.Company.Name,
                        CompanyDescription = f.Company.Description,
                        CompanyLogo = f.Company.Logo,
                        NumberOfPackages = f.Company.TourPackages.Count,
                        NumberOfTourists = f.Company.TourPackages
                        .Sum(tp => tp.Bookings
                        .Count(b => 
                        b.Status == BookingStatus.Completed || 
                        b.Status == BookingStatus.Confirmed || 
                        b.Status == BookingStatus.In_Progress)),
                        Rate = f.Company.TourPackages.Average(tp => tp.Rate)
                    })
                    .ToListAsync(cancellationToken);


                var paginationMetadata = new PaginationMetadata
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = favorites.Count
                };

                var response = new PaginatedResponse<FavoriteResponse> { Items = favorites, Pagination = paginationMetadata };

                _logger.LogDebug("Successfully retrieved {Count} favorites for user {UserId}", favorites.Count, userId);

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
