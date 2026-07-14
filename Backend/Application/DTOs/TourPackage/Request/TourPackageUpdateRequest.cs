using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Application.DTOs.TourPackage.Request;

public sealed record TourPackageUpdateRequest
(
    string? PackageName,
    string? Description,
    string? MeetingPoint,
    int? CountryId,
    decimal? PricePerPerson,
    string? Currency,
    int? DurationInDays,
    DateOnly? StartDate,
    DateOnly? EndDate,
    DateOnly? RegistrationDeadline,
    int? TotalCapacity,
    List<int>? TouristGuideIds,
    ServiceLevel? ServiceLevel,
    List<CabinClassRequest>? CabinClasses,
    List<MediaCreateRequest>? Media,
    List<MediaUpdateRequest>? ExistingMedia,
    List<TourPackageDayRequest>? Days
);
