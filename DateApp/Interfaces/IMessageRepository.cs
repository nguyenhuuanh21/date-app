using DateApp.DTOs;
using DateApp.Entities;
using DateApp.Helpers;

namespace DateApp.Interfaces
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message?> GetMessage(string id);
        Task<PaginateResult<MessageDto>> GetMessagesForMember(MessageParams messageParams);
        Task<IEnumerable<MessageDto>>GetMessagesThread(string currentUserId, string recipientId);
        Task<bool> SaveAllAsync();
    }
}
