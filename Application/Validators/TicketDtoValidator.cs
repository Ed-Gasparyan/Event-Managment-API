using Application.DTOs.TicketDTO;
using FluentValidation;

namespace Application.Validators
{
    public class TicketDtoValidator : AbstractValidator<TicketDto>
    {
        public TicketDtoValidator()
        {
            RuleFor(x => x.EventId)
                .GreaterThan(0).WithMessage("EventId is required");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId is required");

            RuleFor(x => x.SeatNumber)
                .NotEmpty().WithMessage("Seat number is required")
                .MaximumLength(10);

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be >= 0");
        }
    }
}
