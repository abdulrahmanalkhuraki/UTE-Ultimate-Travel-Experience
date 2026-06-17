using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Hotel> Hotels { get; }
        IGenericRepository<Flight> Flights { get; }
        IGenericRepository<Booking> Bookings { get; }
        IGenericRepository<Companion> Companions { get; }
        IGenericRepository<Payment> Payments { get; }
        IGenericRepository<User> Users { get; }
        IGenericRepository<Role> Roles { get; }
        IGenericRepository<TourCompany> TourCompanies { get; }
        IGenericRepository<TourPackage> TourPackages { get; }
        IGenericRepository<Itinerary> PackageItineraries { get; }
        IGenericRepository<Activity> PackageItineraryAttractions { get; }
        IGenericRepository<TourPackage_City> PackageCities { get; }
        IGenericRepository<TouristGuide> TouristGuides { get; }
        IGenericRepository<Company_TouristGuide> CompanyGuides { get; }
        IGenericRepository<TourPackage_TouristGuide> TourPackageGuides { get; }
        IGenericRepository<TourPackageCabinClass> TourPackageCabinClasses { get; }
        IGenericRepository<Country> Countries { get; }
        IGenericRepository<City> Cities { get; }
        IGenericRepository<Notification> Notifications { get; }
        IGenericRepository<DeviceToken> DeviceTokens { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
        
    }
}
