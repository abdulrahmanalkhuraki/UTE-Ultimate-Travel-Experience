using Application.Common.Constants;
using Application.Common.Logging;
using Application.DTOs.Ticket.Request;
using Application.DTOs.Ticket.Response;
using Application.Exceptions;
using Application.Interfaces.Localization;
using Application.Interfaces.Ticket;
using Application.Interfaces.User;
using Application.Validators.Ticket;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizedMapper _mapper;
        private readonly ILogger<TicketService> _logger;
        private readonly TicketCreateValidator _createValidator;
        private readonly ICurrentUserService _currentUser;
        private readonly IFileStorage _fileStorage;
        private const string ObjectName = "Ticket";

        public TicketService(
            IUnitOfWork unitOfWork,
            ILocalizedMapper mapper,
            ILogger<TicketService> logger,
            TicketCreateValidator createValidator,
            ICurrentUserService currentUser,
            IFileStorage fileStorage)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
        }

        public async Task<TicketResponse> CreateAsync(TicketCreateRequest request, CancellationToken cancellationToken)
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
                var ticket = _mapper.Map<Ticket>(request);
                ticket.UserId = _currentUser.UserId ?? 0;
                ticket.Status = TicketStatus.Open;
                ticket.CreatedAt = DateTime.UtcNow;

                if (request.Image is { Length: > 0 })
                {
                    ticket.ImageUrl = await _fileStorage.SaveAsync(request.Image, "tickets", cancellationToken);
                }

                await _unitOfWork.Tickets.AddAsync(ticket, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.SuccessfulOperation(userId: _currentUser.UserId ?? 0, "Create", ObjectName, ticket.Id);

                var created = await BuildDetailedQuery()
                    .FirstOrDefaultAsync(t => t.Id == ticket.Id, cancellationToken);

                return _mapper.Map<TicketResponse>(created!);
            }
            catch (Exception ex)
            {
                _logger.ServerError("Create", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("create", ObjectName, ex.Message), ex);
            }
        }

        public async Task<IReadOnlyList<TicketResponse>> GetAsync(int? userId, CancellationToken cancellationToken)
        {
            if (userId.HasValue && userId <= 0)
                throw new ArgumentException($"Invalid User Id {userId}");

            _logger.StartOperation("Retrieve", ObjectName, 0);

            try
            {
                IQueryable<Ticket> query = BuildDetailedQuery();

                if (userId.HasValue)
                {
                    query = query.Where(t => t.UserId == userId.Value);
                }

                var tickets = await query
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync(cancellationToken);

                var response = _mapper.Map<IReadOnlyList<TicketResponse>>(tickets);

                _logger.LogDebug("Successfully retrieved {Count} tickets", response.Count);

                return response;
            }
            catch (Exception ex)
            {
                _logger.ServerError("Retrieve", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("retrieve", ObjectName, ex.Message), ex);
            }
        }

        private IQueryable<Ticket> BuildDetailedQuery()
        {
            return _unitOfWork.Tickets.Query()
                .Include(t => t.User).ThenInclude(u => u.Person)
                    .ThenInclude(p => p.NationalityCountry).ThenInclude(n => n.Translations)
                .Include(t => t.User).ThenInclude(u => u.Person)
                    .ThenInclude(p => p.ResidentialCity).ThenInclude(c => c.Translations)
                .Include(t => t.User).ThenInclude(u => u.Role);
        }
    }
}
