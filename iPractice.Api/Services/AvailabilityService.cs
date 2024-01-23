using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iPractice.Api.Models;
using iPractice.DataAccess;
using iPractice.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using TimeSlot = iPractice.Api.Models.TimeSlot;

namespace iPractice.Api.Services
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly ApplicationDbContext _context;

        public AvailabilityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveAvailability(Availabilities model)
        {
            var availabilities = new List<DataAccess.Models.Availability>();
            model.AvailabilitySlots.ForEach(x =>
            {
                var availability = new DataAccess.Models.Availability
                {
                    StartDate = x.StartTime,
                    EndDate = x.EndTime,
                    PsychologistId = model.PsychologistId,
                    TimeSlots = new List<DataAccess.Models.TimeSlot>()
                };

                DateTime slotStart = x.StartTime;
                while (slotStart < x.EndTime)
                {
                    availability.TimeSlots.Add(new DataAccess.Models.TimeSlot
                    {
                        StartTime = slotStart,
                        EndTime = slotStart.AddMinutes(15),
                        Availability = availability,
                        IsBooked = false,
                        Client = null
                    });
                    slotStart = slotStart.AddMinutes(15);
                }

                availabilities.Add(availability);
            });

            _context.Availabilities.AddRange(availabilities);
            await _context.SaveChangesAsync();
        }

        public async Task<TimeSlotCollection> GetAvailableTimeSlots(long clientId)
        {
            // Get the client
            var client = await _context.Clients.Include(c => c.Psychologists).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client == null) throw new ArgumentException("Client not found");

            // Get all availabilities for all psychologists that works with the client
            var psychologistIds = client.Psychologists.Select(p => p.Id);
            var availabilities = await _context.Availabilities
                .Include(a => a.TimeSlots)
                .Where(a => psychologistIds.Contains(a.PsychologistId))
                .ToListAsync();

            // Filter the timeslots which are not booked
            var availableTimeSlots = availabilities
                .SelectMany(a => a.TimeSlots)
                .Where(t => !t.IsBooked).ToList();

            // Map availabilities to TimeSlotCollection. 
            var timeSlotViewModels = new TimeSlotCollection
            {
                Slots = availableTimeSlots.Select(t => new iPractice.Api.Models.TimeSlot()
                {
                    Id = t.TimeSlotId,
                    Start = t.StartTime,
                    End = t.EndTime,
                    PsychologistId = t.Availability.PsychologistId,
                    AvailabilityId = t.AvailabilityId
                })
                
            };

            return timeSlotViewModels;
        }

        public async Task<bool> AreTimeSlotsBooked(IEnumerable<TimeSlot> timeSlots)
        {
            // Fetch the IDs of the timeslots to check
            var timeSlotIds = timeSlots.Select(t => t.Id);

            // Fetch timeslots from the database
            var bookedTimeSlots = await _context.TimeSlots
                                                .Where(t => timeSlotIds.Contains(t.TimeSlotId) && t.IsBooked)
                                                .ToListAsync();

            // Return true if any of the timeslots are booked    
            return bookedTimeSlots.Any();
        }

        public async Task<Availabilities> GetAvailability(long availabilityId)
        {
            // Fetch availabilities from the database
            var availability = await _context.Availabilities
                                             .Include(a => a.TimeSlots)
                                             .FirstOrDefaultAsync(a => a.Id == availabilityId);
        
            // If availabilities not found, return null
            if (availability == null) return null;
            
            // Convert to AvailabilitySlots
            var availabilityViewModel = new Availabilities
            {
                PsychologistId = availability.PsychologistId,
                AvailabilitySlots = availability.TimeSlots.Select(t => new Availabilities.AvailabilitySlot()
                {
                    StartTime = t.StartTime,
                    EndTime = t.EndTime
                }).ToList()
            };
        
            return availabilityViewModel;
        }

        public async Task UpdateAvailability(Availabilities existingAvailabilities,
            UpdateAvailability updateAvailability)
        {
            // Get the availabilities to be updated from the database
            var availability = await _context.Availabilities
                .Include(a => a.TimeSlots)
                .FirstOrDefaultAsync(a => a.PsychologistId == existingAvailabilities.PsychologistId);

            if (availability == null) 
            {
                throw new Exception("AvailabilitySlots not found");
            }
            

            // Create a list to hold new time slots
            List<DataAccess.Models.TimeSlot> newTimeSlots = new List<DataAccess.Models.TimeSlot>();

            // Create timeslots in 15 minutes increment from minTime to maxTime
            for (var slotTime = updateAvailability.StartTime; slotTime < updateAvailability.EndTime; slotTime = slotTime.AddMinutes(15))
            {
                // Do not overwrite old timeslots
                if (!availability.TimeSlots.Any(t => t.StartTime == slotTime && t.EndTime == slotTime.AddMinutes(15)))
                {
                    newTimeSlots.Add(new DataAccess.Models.TimeSlot
                    {
                        StartTime = slotTime,
                        EndTime = slotTime.AddMinutes(15),
                        IsBooked = false
                    });
                }
            }

            // Remove timeslots falling outside the new range
            availability.TimeSlots.RemoveAll(slot => slot.StartTime < updateAvailability.StartTime || slot.EndTime > updateAvailability.EndTime);

            // Add new timeslots
            availability.TimeSlots.AddRange(newTimeSlots);

            // Save changes
            await _context.SaveChangesAsync();
        }
        public async Task<bool> DoesPsychologistExist(long id)
        {
            bool psychologistExist = await _context.Psychologists.AnyAsync(p => p.Id == id);
            return psychologistExist;
        }
    }
}