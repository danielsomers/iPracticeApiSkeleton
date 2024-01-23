using System;
using System.Collections.Generic;

namespace iPractice.Api.Models
{
    public class Availabilities
    {
        public class AvailabilitySlot
        {
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
        }
        
        public List<AvailabilitySlot> AvailabilitySlots { get; set; } = new List<AvailabilitySlot>();
        
        public long PsychologistId { get; set; }

    }
}