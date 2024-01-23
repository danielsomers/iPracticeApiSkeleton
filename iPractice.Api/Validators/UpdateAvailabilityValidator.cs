using FluentValidation;
using iPractice.Api.Models;

namespace iPractice.Api.Validators;

public class UpdateAvailabilityValidator : AbstractValidator<UpdateAvailability>
{
    public UpdateAvailabilityValidator()
    {
        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required.")
            .LessThan(x => x.EndTime).WithMessage("Start time must be less than the end time.");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("End time is required.")
            .GreaterThan(x => x.StartTime).WithMessage("End time must be greater than the start time.");
    }
}