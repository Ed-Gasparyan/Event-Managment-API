using Application.DTOs.EventDTOs;
using FluentValidation;

namespace Application.Validators
{
    public class EventDtoValidator : AbstractValidator<EventDto>
    {
        public EventDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(100);

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("StartTime is required");

            RuleFor(x => x.EndTime)
                .NotEmpty().WithMessage("EndTime is required")
                .GreaterThan(x => x.StartTime).WithMessage("EndTime must be after StartTime");

            RuleFor(x => x.VenueId)
                .GreaterThan(0).WithMessage("VenueId must be greater than 0");
        }
    }
}
