using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction _transaction;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Bookings = new GenericRepository<Booking>(_context);
            Companions = new GenericRepository<Companion>(_context);
            Payments = new GenericRepository<Payment>(_context);
            Persons = new GenericRepository<Person>(_context);
            Users = new GenericRepository<User>(_context);
            Roles = new GenericRepository<Role>(_context);
            TourCompanies = new GenericRepository<TourCompany>(_context);
            TourPackages = new GenericRepository<TourPackage>(_context);
            Itineraries = new GenericRepository<Itinerary>(_context);
            Activities = new GenericRepository<Activity>(_context);
            TourPackage_Attraction = new GenericRepository<TourPackage_Attraction>(_context);
            TouristGuides = new GenericRepository<TouristGuide>(_context);
            Company_TouristGuide = new GenericRepository<Company_TouristGuide>(_context);
            TourPackage_TouristGuide = new GenericRepository<TourPackage_TouristGuide>(_context);
            TourPackageCabinClasses = new GenericRepository<TourPackageCabinClass>(_context);
            Countries = new GenericRepository<Country>(_context);
            Cities = new GenericRepository<City>(_context);
            Notifications = new GenericRepository<Notification>(_context);
            Wishlists = new GenericRepository<Wishlist>(_context);
            Reviews = new GenericRepository<Review>(_context);
            Rates = new GenericRepository<Rate>(_context);
            DeviceTokens = new GenericRepository<DeviceToken>(_context);
            Tickets = new GenericRepository<Ticket>(_context);
            SupportReplies = new GenericRepository<SupportReply>(_context);
            Favorites = new GenericRepository<Favorite>(_context);
        }

        public IGenericRepository<Booking> Bookings { get; }

        public IGenericRepository<Companion> Companions { get; }

        public IGenericRepository<Payment> Payments { get; }

        public IGenericRepository<Person> Persons { get; }

        public IGenericRepository<Wishlist> Wishlists { get; }

        public IGenericRepository<User> Users { get; }

        public IGenericRepository<Role> Roles { get; }

        public IGenericRepository<TourCompany> TourCompanies { get; }

        public IGenericRepository<TourPackage> TourPackages { get; }

        public IGenericRepository<Itinerary> Itineraries { get; }

        public IGenericRepository<Activity> Activities { get; }

        public IGenericRepository<TourPackage_Attraction> TourPackage_Attraction { get; }

        public IGenericRepository<TouristGuide> TouristGuides { get; }

        public IGenericRepository<Company_TouristGuide> Company_TouristGuide { get; }

        public IGenericRepository<TourPackage_TouristGuide> TourPackage_TouristGuide { get; }

        public IGenericRepository<TourPackageCabinClass> TourPackageCabinClasses { get; }

        public IGenericRepository<Country> Countries { get; }

        public IGenericRepository<City> Cities { get; }

        public IGenericRepository<Review> Reviews { get; }

        public IGenericRepository<Rate> Rates { get; }

        public IGenericRepository<Notification> Notifications { get; }

        public IGenericRepository<DeviceToken> DeviceTokens { get; }

        public IGenericRepository<Ticket> Tickets { get; }

        public IGenericRepository<SupportReply> SupportReplies { get; }

        public IGenericRepository<Favorite> Favorites { get; }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _transaction?.CommitAsync(cancellationToken);
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _transaction?.RollbackAsync(cancellationToken);
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
