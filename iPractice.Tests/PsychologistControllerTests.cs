using System.Net;
using iPractice.Api.Controllers;
using iPractice.Api.Models;
using iPractice.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace iPractice.Tests;

public class PsychologistControllerTests
{
    private readonly Mock<IAvailabilityService> _mockAvailabilityService;
    private readonly Mock<ILogger<PsychologistController>> _mockLogger;
    private readonly PsychologistController _controller;

    public PsychologistControllerTests()
    {
        _mockAvailabilityService = new Mock<IAvailabilityService>();
        _mockLogger = new Mock<ILogger<PsychologistController>>();
        _controller = new PsychologistController(_mockLogger.Object, _mockAvailabilityService.Object);
    }
    
    [Fact]
    public async Task CreateAvailability_ReturnsCreated_WhenPsychologistExists()
    {
        // Arrange
        long psychologistId = 1L;
        Availabilities availabilities = new Availabilities();

        _mockAvailabilityService.Setup(service => service.DoesPsychologistExist(psychologistId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.CreateAvailability(psychologistId, availabilities);

        // Assert
        Assert.IsType<CreatedResult>(result);
    }
    
    [Fact]
    public async Task CreateAvailability_ReturnsNotFound_WhenPsychologistNotExists()
    {
        // Arrange
        long psychologistId = 1L;
        Availabilities availabilities = new Availabilities();

        _mockAvailabilityService.Setup(service => service.DoesPsychologistExist(psychologistId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.CreateAvailability(psychologistId, availabilities);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task CreateAvailability_ReturnsBadRequest_WhenExceptionIsThrown()
    {
        // Arrange
        long psychologistId = 1L;
        Availabilities availabilities = new Availabilities(); 

        _mockAvailabilityService.Setup(service => service.DoesPsychologistExist(psychologistId))
            .ReturnsAsync(true);

        _mockAvailabilityService.Setup(service => service.SaveAvailability(availabilities))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.CreateAvailability(psychologistId, availabilities);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Test exception", badRequestResult.Value);
    }
    
    [Fact]
    public async Task UpdateAvailability_PsychologistNotFound_ReturnsNotFound()
    {
        _mockAvailabilityService.Setup(service => service.DoesPsychologistExist(It.IsAny<long>())).ReturnsAsync(false);

        var result = await _controller.UpdateAvailability(1, 1, new UpdateAvailability());

        Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("Psychologist not found.", (result.Result as NotFoundObjectResult).Value);
    }
    
    [Fact]
    public async Task UpdateAvailability_AvailabilityNotFound_ReturnsNotFound()
    {
        _mockAvailabilityService.Setup(service => service.DoesPsychologistExist(It.IsAny<long>())).ReturnsAsync(true);
        _mockAvailabilityService.Setup(service => service.GetAvailability(It.IsAny<long>())).ReturnsAsync((Availabilities)null);

        var result = await _controller.UpdateAvailability(1, 1, new UpdateAvailability());

        Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("AvailabilitySlots not found.", (result.Result as NotFoundObjectResult).Value);
    }
    
    [Fact]
    public async Task UpdateAvailability_ValidRequest_ReturnsOk()
    {
        _mockAvailabilityService.Setup(service => service.DoesPsychologistExist(It.IsAny<long>())).ReturnsAsync(true);
        _mockAvailabilityService.Setup(service => service.GetAvailability(It.IsAny<long>())).ReturnsAsync(new Availabilities());

        var result = await _controller.UpdateAvailability(1, 1, new UpdateAvailability());

        Assert.IsType<OkResult>(result.Result);
    }
    
    [Fact]
    public async Task UpdateAvailability_ErrorUpdating_ReturnsBadRequest()
    {
        _mockAvailabilityService.Setup(service => service.DoesPsychologistExist(It.IsAny<long>())).ReturnsAsync(true);
        _mockAvailabilityService.Setup(service => service.GetAvailability(It.IsAny<long>())).ReturnsAsync(new Availabilities());
        _mockAvailabilityService.Setup(service => service.UpdateAvailability(It.IsAny<Availabilities>(), It.IsAny<UpdateAvailability>())).Throws(new Exception("Error updating availability"));

        var result = await _controller.UpdateAvailability(1, 1, new UpdateAvailability());

        Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Error updating availability", (result.Result as BadRequestObjectResult).Value);
    }
}