using Application.DTOs.Hotel.Request;
using Application.DTOs.Hotel.Response;
using Application.Interfaces.Hotel;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;

namespace Application.Services
{
    public class HotelService : IHotelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<HotelService> _logger;

        public HotelService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<HotelService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<HotelResponse> CreateAsync(HotelCreateRequest request, CancellationToken cancellationToken)
        {
            // Basic validation
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.HotelName))
                throw new ArgumentException("Hotel name is required");

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                // Check for duplicate
                bool existingHotel = await _unitOfWork.Hotels
                    .AnyAsync(h => h.HotelName == request.HotelName
                                 && h.Latitude.Equals(request.Latitude)
                                 && h.Longitude.Equals(request.Longitude), cancellationToken);

                if (existingHotel)
                    throw new InvalidOperationException("A hotel with the same name and location already exists");

                // Map request to entity
                var hotel = _mapper.Map<Hotel>(request);
                hotel.CreatedAtUtc = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.Hotels.AddAsync(hotel, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Created hotel: {HotelName} (ID: {HotelId})", hotel.HotelName, hotel.Id);

                // Map to response
                return _mapper.Map<HotelResponse>(hotel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating hotel: {HotelName}", request.HotelName);
                throw;
            }
        }

        public async Task<HotelResponse> GetAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid hotel ID");

            cancellationToken.ThrowIfCancellationRequested();

            var hotel = await _unitOfWork.Hotels.GetByIdAsync(id, cancellationToken);

            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {id} not found");

            return _mapper.Map<HotelResponse>(hotel);
        }

        public async Task<IReadOnlyList<HotelResponse>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var hotels = await _unitOfWork.Hotels.GetAllAsync(cancellationToken);

            return _mapper.Map<IReadOnlyList<HotelResponse>>(hotels);
        }

        public async Task<bool> UpdateAsync(int id, HotelUpdateRequest request, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid hotel ID");

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            cancellationToken.ThrowIfCancellationRequested();

            var hotel = await _unitOfWork.Hotels.GetByIdAsync(id, cancellationToken);

            if (hotel == null)
                return false;

            // Map update request to existing entity
            _mapper.Map(request, hotel);
            hotel.UpdatedAtUtc = DateTime.UtcNow;

            _unitOfWork.Hotels.Update(hotel);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated hotel: {HotelId}", id);

            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid hotel ID");

            cancellationToken.ThrowIfCancellationRequested();

            var hotel = await _unitOfWork.Hotels.GetByIdAsync(id, cancellationToken);

            if (hotel == null)
                return false;

            _unitOfWork.Hotels.Remove(hotel);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted hotel: {HotelId}", id);

            return true;
        }
    }
}