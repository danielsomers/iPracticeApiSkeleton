using System;
using System.Linq;
using iPractice.DataAccess;
using System.Threading.Tasks;
using iPractice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace iPractice.Api.Services
{
    public class ClientService : IClientService
    {
        private readonly ApplicationDbContext _context;

        public ClientService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DoesClientExist(long clientId)
        {
            return await _context.Clients.AnyAsync(e => e.Id == clientId);
        }

        public async Task<bool> DoesClientBelongToPsychologist(long clientId, object psychologistId)
        {
            var psychologist = await _context.Psychologists
                                             .Include(p => p.Clients)
                                             .FirstOrDefaultAsync(p => p.Id.Equals(psychologistId));

            // Check if the psychologist exists and the client is associated with this psychologist.
            return psychologist?.Clients?.Any(c => c.Id == clientId) ?? false;
        }

        public async Task<bool> BookTimeSlot(TimeSlotCollection timeSlotCollection, long clientId)
        {
            try
            {
                foreach (var timeSlot in timeSlotCollection.Slots)
                {
                    iPractice.DataAccess.Models.TimeSlot matchingTimeSlot = await _context.TimeSlots
                            .FirstOrDefaultAsync(t => t.AvailabilityId == timeSlot.AvailabilityId && t.TimeSlotId == timeSlot.Id);

                    if (matchingTimeSlot != null)
                    {
                        matchingTimeSlot.IsBooked = true;
                        var client = await _context.Clients.FindAsync(clientId);
                        matchingTimeSlot.Client = client;
                    }
                    else
                    {
                        return false;
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }
    }
}