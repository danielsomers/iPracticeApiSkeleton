using System;

namespace iPractice.DataAccess.Models;

public class TimeSlot
{
    public long TimeSlotId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public long AvailabilityId { get; set; }
    public Availability Availability { get; set; }
    public bool IsBooked { get; set; }
    public Client Client { get; set; }
}