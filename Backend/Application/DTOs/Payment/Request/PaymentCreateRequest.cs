using Domain.Enums;

namespace Application.DTOs.Payment.Request;

public sealed record PaymentCreateRequest
(
    PaymentMethod PaymentMethod
);
