using Application.DTOs.VenueDTOs;
using FluentValidation;

namespace Application.Validators
{
    public class VenueDtoValidator : AbstractValidator<VenueDto>
    {
        public VenueDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Venue name is required")
                .MaximumLength(100);

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required")
                .MaximumLength(200);

            RuleFor(x => x.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be greater than 0");
        }
    }
}
