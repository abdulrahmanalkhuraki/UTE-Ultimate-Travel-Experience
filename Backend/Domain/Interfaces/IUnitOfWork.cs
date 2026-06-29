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
        IGenericRepository<Booking> Bookings { get; }
        IGenericRepository<Companion> Companions { get; }
        IGenericRepository<Payment> Payments { get; }
        IGenericRepository<Person> Persons { get; }
        IGenericRepository<User> Users { get; }
        IGenericRepository<Role> Roles { get; }
        IGenericRepository<TourCompany> TourCompanies { get; }
        IGenericRepository<TourPackage> TourPackages { get; }
        IGenericRepository<Itinerary> Itineraries { get; }
        IGenericRepository<Activity> Activities { get; }
        IGenericRepository<TourPackage_Attraction> TourPackage_Attraction { get; }
        IGenericRepository<TouristGuide> TouristGuides { get; }
        IGenericRepository<Company_TouristGuide> Company_TouristGuide { get; }
        IGenericRepository<TourPackage_TouristGuide> TourPackage_TouristGuide { get; }
        IGenericRepository<TourPackageCabinClass> TourPackageCabinClasses { get; }
        IGenericRepository<Country> Countries { get; }
        IGenericRepository<City> Cities { get; }
        IGenericRepository<Notification> Notifications { get; }
        IGenericRepository<Wishlist> Wishlists { get; }
        IGenericRepository<DeviceToken> DeviceTokens { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
        
    }
}
