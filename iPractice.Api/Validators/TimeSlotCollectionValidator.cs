namespace iPractice.Api.Validators;

using FluentValidation;
using iPractice.Api.Models;

public class TimeSlotValidator : AbstractValidator<TimeSlot>
{
    public TimeSlotValidator()
    {
        RuleFor(t => t.AvailabilityId).NotEmpty();
        RuleFor(t => t.PsychologistId).NotEmpty();
    }
}

public class TimeSlotCollectionValidator : AbstractValidator<TimeSlotCollection>
{
    public TimeSlotCollectionValidator()
    {
        RuleFor(c => c.Slots).NotEmpty();
        RuleForEach(c => c.Slots).SetValidator(new TimeSlotValidator());
    }
}