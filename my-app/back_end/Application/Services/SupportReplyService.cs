using Application.Common.Constants;
using Application.Common.Logging;
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
        private const string ObjectName = "Support Reply";

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

            _logger.StartOperation("Create", ObjectName, request.TicketId, 0);

            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.ValidationFailed("Create", ObjectName, string.Join(", ", validationResult.Errors));
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
                    _logger.EntityNotFound("Ticket", request.TicketId);
                    throw new NotFoundException(ExceptionMessages.NotFound("Ticket", request.TicketId));
                }

                if (ticket.Status == TicketStatus.Closed)
                {
                    _logger.BusinessRuleViolated("Ticket", "Cannot reply to a closed ticket");
                    throw new BusinessRuleException(ExceptionMessages.BusinessRule(
                        $"Cannot reply to ticket '{request.TicketId}' because it is already closed."));
                }

                var reply = _mapper.Map<SupportReply>(request);
                reply.AdminId = _currentUser.UserId ?? 0;
                reply.CreatedAt = DateTime.UtcNow;

                await _unitOfWork.SupportReplies.AddAsync(reply, cancellationToken);

                ticket.Status = TicketStatus.Closed;
                _unitOfWork.Tickets.Update(ticket);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.SuccessfulOperation("Create", ObjectName);

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
                _logger.ServerError("Create", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("create", ObjectName, ex.Message), ex);
            }
        }

        public async Task<IReadOnlyList<SupportReplyResponse>> GetAsync(int ticketId, CancellationToken cancellationToken)
        {
            if (ticketId <= 0)
                throw new ArgumentException(ExceptionMessages.InvalidId(ObjectName));

            _logger.StartOperation("Retrieve", ObjectName, ticketId, 0);

            try
            {
                var exists = await _unitOfWork.Tickets.AnyAsync(t => t.Id == ticketId, cancellationToken);
                if (!exists)
                {
                    _logger.EntityNotFound("Ticket", ticketId);
                    throw new NotFoundException(ExceptionMessages.NotFound("Ticket", ticketId));
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
                _logger.ServerError("Retrieve", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("retrieve", ObjectName, ex.Message), ex);
            }
        }
    }
}
