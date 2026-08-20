using Application;
using Application.DTOs.Rate.Request;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Rate
{
    public sealed class RateCreateValidator : AbstractValidator<RateCreateRequest>
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        public RateCreateValidator(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;

            RuleFor(p => p.rateValue)
                .InclusiveBetween(1, 5)
                .WithMessage(_localizer["Rate must be between 1 and 5"]);
        }
    }
}
