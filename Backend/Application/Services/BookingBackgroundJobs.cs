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

        public async Task ProcessCompletedPackagesAsync(CancellationToken cancellationToken = default)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var packages = await _unitOfWork.TourPackages
                .Query()
                .Where(p => p.Status == TourPackageStatus.Active && p.EndDate < today)
                .ToListAsync(cancellationToken);

            if (packages.Count == 0)
            {
                _logger.LogDebug("No packages to mark as Completed today");
                return;
            }

            foreach (var pkg in packages)
            {
                pkg.Status = TourPackageStatus.Completed;
                pkg.UpdatedAtUtc = DateTime.UtcNow;
                _unitOfWork.TourPackages.Update(pkg);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Marked {Count} packages as Completed", packages.Count);
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

        public async Task SendRegistrationDeadlineRemindersAsync(CancellationToken cancellationToken = default)
        {
            var targetDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));

            var packages = await _unitOfWork.TourPackages
                .Query()
                .Where(p => p.RegistrationDeadline == targetDate && !p.IsDeleted)
                .ToListAsync(cancellationToken);

            if (packages.Count == 0)
            {
                _logger.LogDebug("No packages with registration deadline in 3 days");
                return;
            }

            foreach (var package in packages)
            {
                var wishlistUserIds = await _unitOfWork.Wishlists
                    .Query()
                    .AsNoTracking()
                    .Where(w => w.TourPackageId == package.Id)
                    .Select(w => w.UserId)
                    .ToListAsync(cancellationToken);

                if (wishlistUserIds.Count == 0)
                    continue;

                var message = $"Registration for '{package.PackageName}' closes in 3 days! Don't miss out — book your spot now.";

                foreach (var userId in wishlistUserIds)
                {
                    try
                    {
                        await _notificationService.NotifyAsync(
                            userId, message, NotificationType.RegistrationDeadlineReminder, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex,
                            "Failed to send registration deadline reminder to user {UserId} for package {PackageId}",
                            userId, package.Id);
                    }
                }

                _logger.LogInformation(
                    "Sent registration deadline reminders to {Count} users for package {PackageId}",
                    wishlistUserIds.Count, package.Id);
            }
        }
    }
}