using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using iPractice.Api.Models;
using iPractice.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace iPractice.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly ILogger<ClientController> _logger;
        private readonly IClientService _clientService; // Client Service
        private readonly IAvailabilityService _availabilityService;

        public ClientController(ILogger<ClientController> logger, IClientService clientService, IAvailabilityService availabilityService)
        {
            _logger = logger;
            _clientService = clientService; // Client Service Assignment
            _availabilityService = availabilityService;
        }
        
        /// <summary>
        /// The client can see when his psychologists are available.
        /// Get available slots from his two psychologists.
        /// </summary>
        /// <param name="clientId">The client ID</param>
        /// <returns>All time slots for the selected client</returns>
        [HttpGet("{clientId}/timeslots")]
        [ProducesResponseType(typeof(IEnumerable<TimeSlotCollection>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<TimeSlotCollection>>> GetAvailableTimeSlots(long clientId)
        {
            try
            {
                if (!await _clientService.DoesClientExist(clientId)) // Check if Client Exists asynchronously
                    return NotFound("Client does not exist");

                var timeSlots = await _availabilityService.GetAvailableTimeSlots(clientId);
                if (timeSlots == null || !timeSlots.Slots.ToList().Any())
                    return NotFound("No available time slots found");
        
                return Ok(timeSlots);
            }
            catch (Exception ex)
            {
                // Log the exception here
                _logger.LogError(ex, $"Getting available time slots for client with ID: {clientId}.");
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
        
        [HttpPost("{clientId}/appointment")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CreateAppointment(long clientId, [FromBody] TimeSlotCollection timeSlotCollection)
        {
            try
            {
                bool clientExists = await _clientService.DoesClientExist(clientId);
                if (!clientExists)
                    return NotFound("Client does not exist");

                var firstTimeSlot = timeSlotCollection.Slots.First();

                bool clientBelongsToPsychologist = await _clientService.DoesClientBelongToPsychologist(clientId, firstTimeSlot.PsychologistId);
                if (!clientBelongsToPsychologist)
                    return BadRequest("Client does not belong to the specified psychologist");

                bool areTimeSlotsBooked = await _availabilityService.AreTimeSlotsBooked(timeSlotCollection.Slots);
                if (areTimeSlotsBooked)
                    return BadRequest("One or more timeslots have already been booked");

                bool isTimeSlotSaved = await _clientService.BookTimeSlot(timeSlotCollection,clientId);
                if (!isTimeSlotSaved)
                    return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while saving the timeslot.");

                return Created();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while creating an appointment for client with ID: {clientId}.");
                return StatusCode((int) HttpStatusCode.InternalServerError, "An error occurred while processing the request.");
            }
        }
    }
}