using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction _transaction;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Hotels = new GenericRepository<Hotel>(_context);
            Flights = new GenericRepository<Flight>(_context);
            Users = new GenericRepository<User>(_context);
            Roles = new GenericRepository<Role>(_context);
            TourCompanies = new GenericRepository<TourCompany>(_context);
            TourPackages = new GenericRepository<TourPackage>(_context);
            PackageItineraries = new GenericRepository<PackageItinerary>(_context);
            PackageItineraryAttractions = new GenericRepository<PackageItineraryAttraction>(_context);
            PackageCities = new GenericRepository<PackageCity>(_context);
            TouristGuides = new GenericRepository<TouristGuide>(_context);
            CompanyGuides = new GenericRepository<CompanyGuide>(_context);
            TourPackageGuides = new GenericRepository<TourPackageGuide>(_context);
            TourPackageCabinClasses = new GenericRepository<TourPackageCabinClass>(_context);
            Countries = new GenericRepository<Country>(_context);
            Cities = new GenericRepository<City>(_context);
            Notifications = new GenericRepository<Notification>(_context);
            DeviceTokens = new GenericRepository<DeviceToken>(_context);
        }

        public IGenericRepository<Hotel> Hotels { get; }

        public IGenericRepository<Flight> Flights { get; }

        public IGenericRepository<User> Users { get; }

        public IGenericRepository<Role> Roles { get; }

        public IGenericRepository<TourCompany> TourCompanies { get; }

        public IGenericRepository<TourPackage> TourPackages { get; }

        public IGenericRepository<PackageItinerary> PackageItineraries { get; }

        public IGenericRepository<PackageItineraryAttraction> PackageItineraryAttractions { get; }

        public IGenericRepository<PackageCity> PackageCities { get; }

        public IGenericRepository<TouristGuide> TouristGuides { get; }

        public IGenericRepository<CompanyGuide> CompanyGuides { get; }

        public IGenericRepository<TourPackageGuide> TourPackageGuides { get; }

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
