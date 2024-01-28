using iPractice.Api.Models;
using iPractice.Api.Services;
using iPractice.DataAccess;
using iPractice.DataAccess.Models;
using TimeSlot = iPractice.DataAccess.Models.TimeSlot;

namespace iPractice.Tests;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class ClientServiceTests
{
    [Fact]
     public async Task DoesClientExist_WithExistingClient_ReturnsTrue()
     {
         var options = new DbContextOptionsBuilder<ApplicationDbContext>()
             .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
             .Options;
         using (var context = new ApplicationDbContext(options))
         {
             // Here we're clearing any existing data in the Clients DbSet
             context.Clients.RemoveRange(context.Clients);
             context.SaveChanges();
     
             context.Clients.Add(new Client { Id = 123 }); 
             context.SaveChanges();
         }
     
         using (var context = new ApplicationDbContext(options))
         {
             var service = new ClientService(context);
             var result = await service.DoesClientExist(123);
             Assert.True(result);
         }
     }
    
    [Fact]
    public async Task DoesClientBelongToPsychologist_WithExistingClientAndPsychologist_ReturnsTrue()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
                        
        using (var context = new ApplicationDbContext(options))
        {
            var psychologist = new Psychologist { Id = 1L };
            var client = new Client { Id = 123L }; 
            psychologist.Clients = new List<Client> { client };
            context.Psychologists.Add(psychologist);
            await context.SaveChangesAsync(); 
        }

        using (var context = new ApplicationDbContext(options))
        {
            var service = new ClientService(context);
            var result = await service.DoesClientBelongToPsychologist(123L, 1L);
            Assert.True(result);
            var psychologist = await context.Psychologists.FindAsync(1L);
            var client = await context.Clients.FindAsync(123L);
            Assert.NotNull(psychologist);
            Assert.NotNull(client);
            Assert.Contains(client, psychologist.Clients);
        }
    }

    [Fact]
    public async Task BookTimeSlot_WithValidTimeSlotAndClient_ReturnsTrue()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using (var context = new ApplicationDbContext(options))
        {
            var client = new Client { Id = 123 }; 
            var timeSlot = new TimeSlotCollection { Slots = new List<iPractice.Api.Models.TimeSlot>() 
                { new iPractice.Api.Models.TimeSlot { AvailabilityId = 456, Id = 789 } } };
            context.Clients.Add(client);
            context.TimeSlots.Add(new TimeSlot { AvailabilityId = 456, TimeSlotId = 789, IsBooked = false });
            context.SaveChanges(); 

            var service = new ClientService(context);
            var result = await service.BookTimeSlot(timeSlot, 123); 
            Assert.True(result);
        }
    }
}