using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iPractice.Tests;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using iPractice.Api.Controllers;
using iPractice.Api.Models;
using iPractice.Api.Services;
using System.Collections.Generic;

public class ClientControllerTests
{
    private readonly ClientController _clientController;
    private readonly Mock<ILogger<ClientController>> _loggerMock = new Mock<ILogger<ClientController>>();
    private readonly Mock<IClientService> _clientServiceMock = new Mock<IClientService>();
    private readonly Mock<IAvailabilityService> _availabilityServiceMock = new Mock<IAvailabilityService>();

    public ClientControllerTests()
    {
        _clientController = new ClientController(_loggerMock.Object, _clientServiceMock.Object, _availabilityServiceMock.Object);
    }

    [Fact]
    public async Task GetAvailableTimeSlots_ReturnsNotFound_WhenClientDoesNotExist()
    {
        _clientServiceMock.Setup(s => s.DoesClientExist(It.IsAny<long>())).ReturnsAsync(false);
        var result = await _clientController.GetAvailableTimeSlots(1);
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }
    
    [Fact]
    public async Task GetAvailableTimeSlots_NoTimeSlotsAvailable_ReturnsNotFound()
    {
        // Arrange
        const long clientId = 1;
        _clientServiceMock.Setup(x => x.DoesClientExist(clientId))
            .Returns(Task.FromResult(true));
        _availabilityServiceMock.Setup(x => x.GetAvailableTimeSlots(clientId))
            .Returns(Task.FromResult(new TimeSlotCollection 
            { 
                Slots = Enumerable.Empty<TimeSlot>().ToList() 
            }));

        // Act
        var result = await _clientController.GetAvailableTimeSlots(clientId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAvailableTimeSlots_TimeSlotsAvailable_ReturnsOK()
    {
        // Arrange
        const long clientId = 1;
        _clientServiceMock.Setup(x => x.DoesClientExist(clientId))
            .Returns(Task.FromResult(true));
        _availabilityServiceMock.Setup(x => x.GetAvailableTimeSlots(clientId))
            .Returns(Task.FromResult(new TimeSlotCollection 
            {
                Slots = new List<TimeSlot> { new TimeSlot() }.ToList()
            }));

        // Act
        var result = await _clientController.GetAvailableTimeSlots(clientId);

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
    }


    [Fact]
    public async Task CreateAppointment_ReturnsNotFound_WhenClientDoesNotExist()
    {
        _clientServiceMock.Setup(s => s.DoesClientExist(It.IsAny<long>())).ReturnsAsync(false);
        var result = await _clientController.CreateAppointment(1, new TimeSlotCollection());
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    // Test for BadRequest when client does not belong to specified psychologist
    [Fact]
    public async Task CreateAppointment_ShouldReturnBadRequest_WhenClientDoesNotBelongToSpecifiedPsychologist()
    {
        // Arrange
        _clientServiceMock.Setup(x => x.DoesClientExist(It.IsAny<long>()))
            .ReturnsAsync(true);
        _clientServiceMock.Setup(x => x.DoesClientBelongToPsychologist(It.IsAny<long>(), It.IsAny<long>()))
            .ReturnsAsync(false);

        // Act
        var result = await _clientController.CreateAppointment(1, new TimeSlotCollection()
        {
            Slots = new List<TimeSlot> { new TimeSlot() }   // adding one timeslot to the collection
        });


        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    // Test for BadRequest when one or more timeslots have already been booked
    [Fact]
    public async Task CreateAppointment_ShouldReturnBadRequest_WhenTimeSlotsAlreadyBooked()
    {
        // Arrange
        _clientServiceMock.Setup(x => x.DoesClientExist(It.IsAny<long>()))
            .ReturnsAsync(true);
        _clientServiceMock.Setup(x => x.DoesClientBelongToPsychologist(It.IsAny<long>(), It.IsAny<long>()))
            .ReturnsAsync(true);
        _availabilityServiceMock.Setup(x => x.AreTimeSlotsBooked(It.IsAny<List<TimeSlot>>()))
            .ReturnsAsync(true);

        // Act
        var result = await _clientController.CreateAppointment(1, new TimeSlotCollection()
        {
            Slots = new List<TimeSlot> { new TimeSlot() }   // adding one timeslot to the collection
        });

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    // Test for InternalServerError when unable to save timeslot
    [Fact]
    public async Task CreateAppointment_ShouldReturnInternalServerError_WhenUnableToSaveTimeSlot()
    {
        // Arrange
        _clientServiceMock.Setup(x => x.DoesClientExist(It.IsAny<long>()))
            .ReturnsAsync(true);
        _clientServiceMock.Setup(x => x.DoesClientBelongToPsychologist(It.IsAny<long>(), It.IsAny<long>()))
            .ReturnsAsync(true);
        _availabilityServiceMock.Setup(x => x.AreTimeSlotsBooked(It.IsAny<List<TimeSlot>>()))
            .ReturnsAsync(false);
        _clientServiceMock.Setup(x => x.BookTimeSlot(It.IsAny<TimeSlotCollection>(), It.IsAny<long>()))
            .ReturnsAsync(false);

        // Act
        var result = await _clientController.CreateAppointment(1, new TimeSlotCollection()
        {
            Slots = new List<TimeSlot> { new TimeSlot() }   // adding one timeslot to the collection
        });

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, ((ObjectResult)result).StatusCode);
    }

    // Test for Created status code on successful booking
    [Fact]
    public async Task CreateAppointment_ShouldReturnCreatedOnSuccessfulRequest()
    {
        // Arrange
        _clientServiceMock.Setup(x => x.DoesClientExist(It.IsAny<long>()))
            .ReturnsAsync(true);
        _clientServiceMock.Setup(x => x.DoesClientBelongToPsychologist(It.IsAny<long>(), It.IsAny<long>()))
            .ReturnsAsync(true);
        _availabilityServiceMock.Setup(x => x.AreTimeSlotsBooked(It.IsAny<List<TimeSlot>>()))
            .ReturnsAsync(false);
        _clientServiceMock.Setup(x => x.BookTimeSlot(It.IsAny<TimeSlotCollection>(), It.IsAny<long>()))
            .ReturnsAsync(true);

        // Act
        var result = await _clientController.CreateAppointment(1, new TimeSlotCollection()
        {
            Slots = new List<TimeSlot> { new TimeSlot() }   // adding one timeslot to the collection
        });

        // Assert
        Assert.IsType<CreatedResult>(result);
    }
}