using System;
using System.Collections.Generic;

namespace iPractice.Api.Models
{
    public class TimeSlot
    {
        public long Id { get; set; }
        public long AvailabilityId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public long PsychologistId { get; set; }
        public long ClientId { get; set; }
    }

    public class TimeSlotCollection
    {
        public IEnumerable<TimeSlot> Slots { get; set; }
    }
}

