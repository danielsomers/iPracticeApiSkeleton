using System;
using System.Collections.Generic;
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
    public class PsychologistController : ControllerBase
    {
        private readonly ILogger<PsychologistController> _logger;
        private readonly IAvailabilityService _availabilityService;

        public PsychologistController(ILogger<PsychologistController> logger,
            IAvailabilityService availabilityService)
        {
            _logger = logger;
            _availabilityService = availabilityService;
        }

        /// <summary>
        /// Add a block of time during which the psychologist is available during normal business hours
        /// </summary>
        /// <param name="psychologistId"></param>
        /// <param name="availabilities"></param>
        /// <returns>Ok if the availabilities was created</returns>
        [HttpPost("{psychologistId}/availability")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CreateAvailability([FromRoute] long psychologistId, [FromBody] Availabilities availabilities)
        {
            try
            {
                if (!(await _availabilityService.DoesPsychologistExist(psychologistId)))
                {
                    return NotFound("Psychologist not found.");
                }

                availabilities.PsychologistId = psychologistId;
                await _availabilityService.SaveAvailability(availabilities);
                return Created();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error creating availabilities: {e.Message}");
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Update availabilities of a psychologist
        /// </summary>
        /// <param name="psychologistId">The psychologist's ID</param>
        /// <param name="availabilityId">The ID of the availabilities block</param>
        /// <param name="updateAvailability"></param>
        /// <returns>List of availabilities slots</returns>
        [HttpPut("{psychologistId}/availability/{availabilityId}")]
        [ProducesResponseType(typeof(Availabilities), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Availabilities>> UpdateAvailability([FromRoute] long psychologistId, [FromRoute] long availabilityId, [FromBody] UpdateAvailability updateAvailability)
        {
            try
            {
                if (!(await _availabilityService.DoesPsychologistExist(psychologistId)))
                {
                    return NotFound("Psychologist not found.");
                }

                var existingAvailability = await _availabilityService.GetAvailability(availabilityId);
                if (existingAvailability == null)
                {
                    return NotFound("AvailabilitySlots not found.");
                }

                await _availabilityService.UpdateAvailability(existingAvailability, updateAvailability);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error updating availabilities: {e.Message}");
                return BadRequest(e.Message);
            }
        }
    }
}
