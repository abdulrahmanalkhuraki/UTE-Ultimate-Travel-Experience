using Application.Interfaces.Notifications;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class BookingBackgroundJobs
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly ILogger<BookingBackgroundJobs> _logger;

        public BookingBackgroundJobs(
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            ILogger<BookingBackgroundJobs> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ProcessStartedBookingsAsync(CancellationToken cancellationToken = default)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var bookings = await _unitOfWork.Bookings
                .Query()
                .Include(b => b.TourPackage)
                .Where(b => b.Status == BookingStatus.Confirmed
                    && b.TourPackage.StartDate == today)
                .ToListAsync(cancellationToken);

            if (bookings.Count == 0)
            {
                _logger.LogDebug("No bookings to mark as In_Progress today");
                return;
            }

            foreach (var booking in bookings)
            {
                booking.Status = BookingStatus.In_Progress;
                booking.UpdatedAtUtc = DateTime.UtcNow;
                _unitOfWork.Bookings.Update(booking);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Marked {Count} bookings as In_Progress", bookings.Count);
        }

        public async Task ProcessCompletedBookingsAsync(CancellationToken cancellationToken = default)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var bookings = await _unitOfWork.Bookings
                .Query()
                .Include(b => b.TourPackage)
                .Where(b => b.Status == BookingStatus.In_Progress
                    && b.TourPackage.EndDate == today)
                .ToListAsync(cancellationToken);

            if (bookings.Count == 0)
            {
                _logger.LogDebug("No bookings to mark as Completed today");
                return;
            }

            foreach (var booking in bookings)
            {
                booking.Status = BookingStatus.Completed;
                booking.UpdatedAtUtc = DateTime.UtcNow;
                _unitOfWork.Bookings.Update(booking);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Marked {Count} bookings as Completed", bookings.Count);
        }

        public async Task SendUpcomingBookingRemindersAsync(CancellationToken cancellationToken = default)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var targets = new (DateOnly Date, string When)[]
            {
                (today.AddDays(7), "in 1 week"),
                (today.AddDays(3), "in 3 days"),
                (today.AddDays(1), "tomorrow"),
            };

            foreach (var (targetDate, when) in targets)
            {
                var bookings = await _unitOfWork.Bookings
                    .Query()
                    .Include(b => b.TourPackage)
                    .Where(b => b.Status == BookingStatus.Confirmed
                        && b.TourPackage.StartDate == targetDate)
                    .ToListAsync(cancellationToken);

                foreach (var booking in bookings)
                {
                    var message = $"Your booking #{booking.Id} for {booking.TourPackage.PackageName} starts {when}. Get ready!";
                    await _notificationService.NotifyAsync(
                        booking.UserId,
                        message,
                        NotificationType.BookingStartingSoon,
                        cancellationToken);
                }

                _logger.LogInformation("Sent {Count} reminders for bookings starting {When}", bookings.Count, when);
            }
        }
    }
}