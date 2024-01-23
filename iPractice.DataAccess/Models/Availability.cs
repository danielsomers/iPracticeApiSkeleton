using System;
using System.Collections.Generic;

namespace iPractice.DataAccess.Models;
using System;
 using System.Collections.Generic;
 using System.ComponentModel.DataAnnotations;
 
 public class Availability
 {
     public long Id { get; set; }
 
     [Required]
     public long PsychologistId { get; set; }
 
     [Required]
     public DateTime StartDate { get; set; }
 
     [Required]
     public DateTime EndDate { get; set; }
 
     public List<TimeSlot> TimeSlots { get; set; } 
 }