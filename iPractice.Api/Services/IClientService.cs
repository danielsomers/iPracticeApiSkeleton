using System.Threading.Tasks;
using iPractice.Api.Models;

namespace iPractice.Api.Services;

public interface IClientService
{
    Task<bool> DoesClientExist(long clientId);
    Task<bool> DoesClientBelongToPsychologist(long clientId, object psychologistId);
    Task<bool> BookTimeSlot(TimeSlotCollection timeSlotCollection, long clientId);
}