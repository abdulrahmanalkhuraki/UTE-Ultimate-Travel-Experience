using Application.DTOs.SupportReply.Request;
using Application.DTOs.SupportReply.Response;
using Application.Exceptions;
using Application.Interfaces.Notifications;
using Application.Interfaces.SupportReply;
using Application.Interfaces.User;
using Application.Validators.SupportReply;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class SupportReplyService : ISupportReplyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SupportReplyService> _logger;
        private readonly SupportReplyCreateValidator _createValidator;
        private readonly ICurrentUserService _currentUser;
        private readonly INotificationService _notificationService;

        public SupportReplyService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<SupportReplyService> logger,
            SupportReplyCreateValidator createValidator,
            ICurrentUserService currentUser,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public async Task<SupportReplyResponse> CreateAsync(SupportReplyCreateRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            _logger.LogInformation("Attempting to create new SupportReply for ticket {TicketId}", request.TicketId);

            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("SupportReply creation validation failed: {Errors}",
                    string.Join(", ", validationResult.Errors));
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }

            try
            {
                var ticket = await _unitOfWork.Tickets
                    .Query()
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken);

                if (ticket is null)
                {
                    _logger.LogWarning("Ticket with ID {TicketId} not found", request.TicketId);
                    throw new NotFoundException($"Ticket with ID {request.TicketId} not found");
                }

                if (ticket.Status == TicketStatus.Closed)
                {
                    _logger.LogWarning("Attempted to reply to closed ticket {TicketId}", request.TicketId);
                    throw new BusinessRuleException($"Cannot reply to ticket '{request.TicketId}' because it is already closed.");
                }

                var reply = _mapper.Map<SupportReply>(request);
                reply.AdminId = _currentUser.UserId ?? 0;
                reply.CreatedAt = DateTime.UtcNow;

                await _unitOfWork.SupportReplies.AddAsync(reply, cancellationToken);

                ticket.Status = TicketStatus.Closed;
                _unitOfWork.Tickets.Update(ticket);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully created SupportReply {ReplyId} for ticket {TicketId}", reply.Id, reply.TicketId);

                try
                {
                    var message = $"Your ticket #{ticket.Id} has received a reply from support and has been closed.";
                    await _notificationService.NotifyAsync(ticket.UserId, message, NotificationType.General, cancellationToken);
                    _logger.LogInformation("Notification sent to user {UserId} for ticket reply {TicketId}", ticket.UserId, ticket.Id);
                }
                catch (Exception notifEx)
                {
                    _logger.LogWarning(notifEx, "Failed to send notification for ticket reply {TicketId}, but reply was saved", ticket.Id);
                }

                return _mapper.Map<SupportReplyResponse>(reply);
            }
            catch (Exception ex) when (ex is NotFoundException or BusinessRuleException)
            {
                _logger.LogError(ex, "Error creating SupportReply for ticket {TicketId}", request.TicketId);
                throw new ServiceException($"Failed to create reply: {ex.Message}", ex);
            }
        }

        public async Task<IReadOnlyList<SupportReplyResponse>> GetAsync(int ticketId, CancellationToken cancellationToken)
        {
            if (ticketId <= 0)
                throw new ArgumentException($"Invalid Ticket Id {ticketId}");

            _logger.LogDebug("Retrieving SupportReplies for ticket {TicketId}", ticketId);

            try
            {
                var exists = await _unitOfWork.Tickets.AnyAsync(t => t.Id == ticketId, cancellationToken);
                if (!exists)
                {
                    _logger.LogWarning("Ticket with ID {TicketId} not found", ticketId);
                    throw new NotFoundException($"Ticket with ID {ticketId} not found");
                }

                var replies = await _unitOfWork.SupportReplies
                    .Query()
                    .Where(r => r.TicketId == ticketId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync(cancellationToken);

                var response = _mapper.Map<IReadOnlyList<SupportReplyResponse>>(replies);

                _logger.LogDebug("Successfully retrieved {Count} SupportReplies for ticket {TicketId}", response.Count, ticketId);

                return response;
            }
            catch (Exception ex) when (ex is NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving SupportReplies for ticket {TicketId}", ticketId);
                throw new ServiceException("Failed to retrieve replies.", ex);
            }
        }
    }
}
