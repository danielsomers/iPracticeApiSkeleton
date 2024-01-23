using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using iPractice.Api.Models;

public class AvailabilityValidator : AbstractValidator<Availabilities>
{
    public AvailabilityValidator()
    {

        RuleFor(x => x.AvailabilitySlots).NotNull().WithMessage("AvailabilitySlots is required.")
            .Must(x => x.Count > 0).WithMessage("At least one availability record is required.")
            .ForEach(ruleBuilder =>
            {
                ruleBuilder.Must(a => a.EndTime > a.StartTime)
                    .WithMessage("EndTime must be greater than StartTime.");
                ruleBuilder.Must(a => a.StartTime > DateTime.Now)
                    .WithMessage("StartTime must not be in the past.");
                ruleBuilder.Must(a => BeValidTime(a.StartTime))
                    .WithMessage("StartTime must be on the hour, quarter past, half past, or quarter to.");
                ruleBuilder.Must(a => BeValidTime(a.EndTime))
                    .WithMessage("EndTime must be on the hour, quarter past, half past, or quarter to.");
            });
    }

    // Check if the time is on the hour, quarter past, half past, or quarter to
    private bool BeValidTime(DateTime time)
    {
        return time.Minute % 15 == 0;
    }
}