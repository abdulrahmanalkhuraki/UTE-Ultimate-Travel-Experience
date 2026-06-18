using Application.DTOs.Booking.Request;
using Application.DTOs.Booking.Response;
using Application.DTOs.Person.Request;
using AutoMapper.Configuration.Annotations;



namespace Application.Interfaces.Person
{
    public interface IPersonService
    {
        Task<Domain.Entities.Person> CreateAsync(PersonCreateRequest request, CancellationToken cancellationToken);
        Task<Domain.Entities.Person> UpdateAsync(int id, PersonUpdateRequest request, CancellationToken cancellationToken);
    }
}
