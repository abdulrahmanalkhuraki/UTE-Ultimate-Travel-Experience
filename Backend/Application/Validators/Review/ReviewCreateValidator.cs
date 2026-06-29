using Application.DTOs.Review.Request;
using FluentValidation;

namespace Application.Validators.Review
{
    public sealed class ReviewCreateValidator : AbstractValidator<ReviewCreateRequest>
    {
        public ReviewCreateValidator()
        {
            RuleFor(p => p.comment)
                .NotEmpty().WithMessage("Comment is Required.")
                .MaximumLength(500).WithMessage("Comment must not exceed 50 characters");
        }
    }
}
