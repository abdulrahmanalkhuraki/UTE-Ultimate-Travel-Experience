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
            Users = new GenericRepository<User>(_context);
            Roles = new GenericRepository<Role>(_context);
            TourCompanies = new GenericRepository<TourCompany>(_context);
            TourPackages = new GenericRepository<TourPackage>(_context);
            Itineraries = new GenericRepository<Itinerary>(_context);
            Activities = new GenericRepository<Activity>(_context);
            PackageCities = new GenericRepository<TourPackage_City>(_context);
            TouristGuides = new GenericRepository<TouristGuide>(_context);
            CompanyGuides = new GenericRepository<Company_TouristGuide>(_context);
            TourPackageGuides = new GenericRepository<TourPackage_TouristGuide>(_context);
            TourPackageCabinClasses = new GenericRepository<TourPackageCabinClass>(_context);
            Countries = new GenericRepository<Country>(_context);
            Cities = new GenericRepository<City>(_context);
            Notifications = new GenericRepository<Notification>(_context);
            DeviceTokens = new GenericRepository<DeviceToken>(_context);
        }

        public IGenericRepository<Booking> Bookings { get; }

        public IGenericRepository<Companion> Companions { get; }

        public IGenericRepository<Payment> Payments { get; }

        public IGenericRepository<User> Users { get; }

        public IGenericRepository<Role> Roles { get; }

        public IGenericRepository<TourCompany> TourCompanies { get; }

        public IGenericRepository<TourPackage> TourPackages { get; }

        public IGenericRepository<Itinerary> Itineraries { get; }

        public IGenericRepository<Activity> Activities { get; }

        public IGenericRepository<TourPackage_City> PackageCities { get; }

        public IGenericRepository<TouristGuide> TouristGuides { get; }

        public IGenericRepository<Company_TouristGuide> CompanyGuides { get; }

        public IGenericRepository<TourPackage_TouristGuide> TourPackageGuides { get; }

        public IGenericRepository<TourPackageCabinClass> TourPackageCabinClasses { get; }

        public IGenericRepository<Country> Countries { get; }

        public IGenericRepository<City> Cities { get; }

        public IGenericRepository<Notification> Notifications { get; }

        public IGenericRepository<DeviceToken> DeviceTokens { get; }

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
