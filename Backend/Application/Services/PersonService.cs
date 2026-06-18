using Application.DTOs.Person.Request;
using Application.Exceptions;
using Application.Interfaces.Person;
using Application.Interfaces.User;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;
using ValidationException = Application.Exceptions.ValidationException;

namespace Application.Services
{
    public class PersonService : IPersonService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PersonService> _logger;
        private readonly IValidator<PersonCreateRequest> _createValidator;
        private readonly IValidator<PersonUpdateRequest> _updateValidator;
        private readonly IMapper _mapper;
        private readonly IFileStorage _fileStorage;

        public PersonService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IFileStorage fileStorage,
            ILogger<PersonService> logger,
            IValidator<PersonCreateRequest> createValidator,
            IValidator<PersonUpdateRequest> updateValidator)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
        }

        public async Task<Person> CreateAsync(PersonCreateRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }

            try
            {
                
                var residentialCityExists = await _unitOfWork.Cities.AnyAsync(c => c.Id == request.ResidentialCityId);
                if (!residentialCityExists)
                {
                    throw new NotFoundException($"Residential City with Id = {request.ResidentialCityId} Not Found");
                }

                var person = _mapper.Map<Person>(request);

                if (request.ProfileImage != null && request.ProfileImage.Length > 0)
                {
                    person.ProfileImage = await _fileStorage.SaveAsync(request.ProfileImage, "profiles", cancellationToken);
                }

                if (request.NationalIdCard != null && request.NationalIdCard.Length > 0)
                {
                    person.NationalIdCard = await _fileStorage.SaveAsync(request.NationalIdCard, "national-ids", cancellationToken);
                }

                if (request.PassportScan != null && request.PassportScan.Length > 0)
                {
                    person.PassportScan = await _fileStorage.SaveAsync(request.PassportScan, "passports", cancellationToken);
                }

                await _unitOfWork.Persons.AddAsync(person);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return person;
            }
            catch(NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating a person.");
                throw;
            }
        }

        public async Task<Person> UpdateAsync(int id, PersonUpdateRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }

            try
            {
                var person = await _unitOfWork.Persons.GetByIdAsync(id);
                if (person == null)
                {
                    throw new NotFoundException($"Person with Id = {id} Not Found");
                }

                
                if (request.ResidentialCityId.HasValue && request.ResidentialCityId.Value != person.ResidentialCityId)
                {
                    var residentialCityExists = await _unitOfWork.Cities.AnyAsync(c => c.Id == request.ResidentialCityId.Value);
                    if (!residentialCityExists)
                    {
                        throw new NotFoundException($"Residential City with Id = {request.ResidentialCityId.Value} Not Found");
                    }
                }

                _mapper.Map(request, person);


                if (request.ProfileImage != null && request.ProfileImage.Length > 0)
                {
                    person.ProfileImage = await _fileStorage.SaveAsync(request.ProfileImage, "profiles", cancellationToken);
                }

                if (request.NationalIdCard != null && request.NationalIdCard.Length > 0)
                {
                    person.NationalIdCard = await _fileStorage.SaveAsync(request.NationalIdCard, "national-ids", cancellationToken);
                }

                if (request.PassportScan != null && request.PassportScan.Length > 0)
                {
                    person.PassportScan = await _fileStorage.SaveAsync(request.PassportScan, "passports", cancellationToken);
                }

                _unitOfWork.Persons.Update(person);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return person;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating person with Id = {PersonId}.", id);
                throw;
            }
        }
    }
}