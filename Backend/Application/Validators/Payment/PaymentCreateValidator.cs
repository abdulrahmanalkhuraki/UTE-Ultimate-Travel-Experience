using Application.DTOs.Payment.Request;
using Domain.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Payment
{
    public sealed class PaymentCreateValidator : AbstractValidator<PaymentCreateRequest>
    {
        public PaymentCreateValidator()
        {
            RuleFor(x => x.PaymentMethod).IsInEnum()
                .WithMessage("Invalid Payment method");
        }
    }
}
