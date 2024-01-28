using iPractice.Api.Models;
using iPractice.Api.Services;
using iPractice.DataAccess;
using iPractice.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using TimeSlot = iPractice.Api.Models.TimeSlot;

namespace iPractice.Tests
{
    public class AvailabilityServiceTests
    {
        [Fact]
        public async Task SaveAvailability_ShouldSaveAvailability()
        {
            // Arrange
    
            var availableSlots = new List<Availabilities.AvailabilitySlot>
            {
                new Availabilities.AvailabilitySlot
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1)
                }
            };
    
            var availabilityModel = new Availabilities
            {
                PsychologistId = 1,
                AvailabilitySlots = availableSlots
            };
    
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                var availabilityService = new AvailabilityService(context);

                // Act
                await availabilityService.SaveAvailability(availabilityModel);

                // Assert
                Assert.Equal(1, context.Availabilities.Count());
                Assert.Equal(availableSlots.First().StartTime,
                    context.Availabilities.First().TimeSlots.First().StartTime);
            }
        }

        [Fact]
        public async Task GetAvailableTimeSlots_ShouldReturnAvailableSlots()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using (var context = new ApplicationDbContext(options))
            {
                var clientId = 1;
                var client = new Client
                {
                    Id = clientId,
                    Psychologists = new List<Psychologist> 
                    {
                        new Psychologist 
                        {
                            Id = 1
                        }
                    }
                };
                context.Clients.Add(client);
                await context.SaveChangesAsync();
    
                var availabilityService = new AvailabilityService(context);
                // Act
                var result = await availabilityService.GetAvailableTimeSlots(clientId);
                // Assert
                Assert.NotNull(result);
            }
        }
        
        [Fact]
        public async Task AreTimeSlotsBooked_ShouldReturnTrueIfAnyTimeSlotsAreBooked()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;
            using (var context = new ApplicationDbContext(options))
            {
                var timeSlots = new List<TimeSlot> { new TimeSlot { Start = DateTime.Now,
                    End = DateTime.Now.AddMinutes(15), PsychologistId = 1, AvailabilityId = 1, Id = 1, ClientId = 1 } };

                var timeslotsSetup = timeSlots.Select(ts => new iPractice.DataAccess.Models.TimeSlot 
                {
                    StartTime = ts.Start, 
                    EndTime = ts.End, 
                    AvailabilityId = ts.AvailabilityId,
                    TimeSlotId = ts.Id,
                    IsBooked = true
                }).ToList();

                context.TimeSlots.AddRange(timeslotsSetup);
                await context.SaveChangesAsync();
                var availabilityService = new AvailabilityService(context);

                // Act
                var result = await availabilityService.AreTimeSlotsBooked(timeSlots);

                // Assert
                Assert.True(result);
            }
        }
        
        [Fact]
        public async Task GetAvailability_ValidId_ReturnsAvailability()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            using (var context = new ApplicationDbContext(options))
            {
                context.Availabilities.Add(new Availability
                {
                    Id = 1,
                    PsychologistId = 1,
                    TimeSlots = new List<iPractice.DataAccess.Models.TimeSlot>()
                    {
                        new iPractice.DataAccess.Models.TimeSlot{ StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) }
                    }
                });

                context.SaveChanges();

                // Arrange
                var service = new AvailabilityService(context);

                // Act
                var availability = await service.GetAvailability(1);

                // Assert
                Assert.NotNull(availability);
                Assert.Single(availability.AvailabilitySlots);
            }
        }
        
        [Fact]
        public async Task UpdateAvailability_ShouldUpdateAvailabilityCorrectlyOrThrowException()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            using (var context = new ApplicationDbContext(options))
            {
                var availability = new iPractice.DataAccess.Models.Availability() 
                {
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMinutes(15),
                    PsychologistId = 1
                };

                context.Availabilities.Add(availability);
                await context.SaveChangesAsync();
   
                // Arrange
                var updateAvailability = new UpdateAvailability();
                var existingAvailabilities = new Availabilities
                {
                    PsychologistId = 1,
                    AvailabilitySlots = new List<Availabilities.AvailabilitySlot>()
                    {
                        new Availabilities.AvailabilitySlot()
                        {
                            StartTime = DateTime.Now,
                            EndTime = DateTime.Now.AddMinutes(15)
                        }
                    }
                };

                var service = new AvailabilityService(context);

                // Act
                await service.UpdateAvailability(existingAvailabilities, updateAvailability);

                // Assert
                Assert.NotEmpty(existingAvailabilities.AvailabilitySlots);

                // Assert for Exception
                // Assuming non-existing psychologistId is set here 
                existingAvailabilities.PsychologistId = 0;

                await Assert.ThrowsAsync<Exception>(() => service.UpdateAvailability(existingAvailabilities, updateAvailability));
            }
        }
        
        [Fact]
        public async Task DoesPsychologistExist_ExistingId_ReturnsTrue()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;

            // Insert seed data into the database using one instance of the context
            using (var context = new ApplicationDbContext(options))
            {
                context.Psychologists.Add(new Psychologist { Id = 1 });
                context.SaveChanges();
            }

            // Act
            bool psychologistExists;
            using (var context = new ApplicationDbContext(options))
            {
                var service = new AvailabilityService(context);
                psychologistExists = await service.DoesPsychologistExist(1);
            }

            // Assert
            Assert.True(psychologistExists);
        }

        [Fact]
        public async Task DoesPsychologistExist_NonExistingId_ReturnsFalse()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;

            // Act
            bool psychologistExists;
            using (var context = new ApplicationDbContext(options))
            {
                var service = new AvailabilityService(context);
                psychologistExists = await service.DoesPsychologistExist(1);
            }

            // Assert
            Assert.False(psychologistExists);
        }
    }
}