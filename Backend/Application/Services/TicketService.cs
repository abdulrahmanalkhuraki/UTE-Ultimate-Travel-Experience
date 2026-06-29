using Application.DTOs.Ticket.Request;
using Application.DTOs.Ticket.Response;
using Application.Exceptions;
using Application.Interfaces.Ticket;
using Application.Interfaces.User;
using Application.Validators.Ticket;
using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly ILogger<TicketService> _logger;
        private readonly TicketCreateValidator _createValidator;
        private readonly ICurrentUserService _currentUser;
        private readonly IFileStorage _fileStorage;

        public TicketService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
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

            _logger.LogInformation("Attempting to create new Ticket");

            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Ticket creation validation failed: {Errors}",
                    string.Join(", ", validationResult.Errors));
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

                _logger.LogInformation("Successfully created ticket {TicketId}", ticket.Id);

                return _mapper.Map<TicketResponse>(ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating ticket");
                throw new ServiceException("Failed to create ticket.", ex);
            }
        }

        public async Task<IReadOnlyList<TicketResponse>> GetAsync(int? userId, CancellationToken cancellationToken)
        {
            if (userId.HasValue && userId <= 0)
                throw new ArgumentException($"Invalid User Id {userId}");

            _logger.LogDebug("Retrieving tickets");

            try
            {
                IQueryable<Ticket> query = _unitOfWork.Tickets
                    .Query()
                    .Include(t => t.User);

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
                _logger.LogError(ex, "Error retrieving tickets");

                throw new ServiceException("Failed to retrieve tickets.", ex);
            }
        }
    }
}
