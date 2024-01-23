using System.Collections.Generic;
using System.Threading.Tasks;
using iPractice.Api.Models;

namespace iPractice.Api.Services;

public interface IAvailabilityService
{
    Task SaveAvailability(Availabilities model);
    Task<TimeSlotCollection> GetAvailableTimeSlots(long clientId);
    Task<bool> AreTimeSlotsBooked(IEnumerable<TimeSlot> timeSlots);
    Task<Availabilities> GetAvailability(long availabilityId);
    Task UpdateAvailability(Availabilities existingAvailabilities, UpdateAvailability updateAvailability);
    Task<bool> DoesPsychologistExist(long id);
}