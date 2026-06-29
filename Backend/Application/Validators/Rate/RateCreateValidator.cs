using Application.DTOs.Rate.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Rate
{
    public sealed class RateCreateValidator : AbstractValidator<RateCreateRequest>
    {
        public RateCreateValidator()
        {
            RuleFor(p => p.rateValue)
                .InclusiveBetween(1, 5)
                .WithMessage("Rate must be between 1 and 5");
        }
    }
}
