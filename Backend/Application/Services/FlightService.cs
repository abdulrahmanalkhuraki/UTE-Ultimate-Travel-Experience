using Application.DTOs.Flight.Request;
using Application.DTOs.Flight.Response;
using Application.DTOs.Hotel.Request;
using Application.DTOs.Hotel.Response;
using Application.Interfaces.Flight;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class FlightService : IFlightService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<FlightService> _logger;

        public FlightService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<FlightService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<FlightResponse> CreateAsync(FlightCreateRequest request, CancellationToken cancellationToken)
        {
            if (await _unitOfWork.Flights.AnyAsync(f => f.FlightNumber == request.FlightNumber && f.Airline == request.Airline))
                throw new InvalidOperationException($"An Flight with the Number '{request.FlightNumber}' already exists in {request.Airline} Airline.");

            var entity = _mapper.Map<Flight>(request);
            await _unitOfWork.Flights.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<FlightResponse>(entity);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.Flights.GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                return false;
            }
            _unitOfWork.Flights.Remove(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<IReadOnlyList<FlightResponse>> GetAllAsync(CancellationToken cancellationToken)
        {
            var list = await _unitOfWork.Flights.GetAllAsync(cancellationToken);
            return _mapper.Map<IReadOnlyList<FlightResponse>>(list);
        }

        public async Task<FlightResponse> GetAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.Flights.GetByIdAsync(id, cancellationToken);
            return entity == null ? null : _mapper.Map<FlightResponse>(entity);
        }

        public async Task<bool> UpdateAsync(int id, FlightUpdateRequest request, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.Flights.GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                return false;
            }

            _mapper.Map(request, entity);
            _unitOfWork.Flights.Update(entity);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
