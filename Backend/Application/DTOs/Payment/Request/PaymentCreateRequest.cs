using Domain.Enums;

namespace Application.DTOs.Payment.Request;

public sealed record PaymentCreateRequest
(
    decimal Amount,
    PaymentMethod PaymentMethod
);
