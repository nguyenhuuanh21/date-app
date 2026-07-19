using DateApp.DTOs;
using DateApp.Entities;
using DateApp.Extensions;
using DateApp.Helpers;
using DateApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DateApp.Data
{
    public class MessageRepository(AppDbContext dbContext) : IMessageRepository
    {
        public void AddMessage(Message message)
        {
            dbContext.Messages.Add(message);
        }

        public async Task<Message?> GetMessage(string id)
        {
            return await dbContext.Messages.FindAsync(id);
        }

        public async Task<PaginateResult<MessageDto>> GetMessagesForMember(MessageParams messageParams)
        {
            var query = dbContext.Messages
                .OrderByDescending(m => m.MessageSent)
                .AsQueryable();

            query = messageParams.Container switch
            {
                "Outbox" => query.Where(m => m.SenderId == messageParams.MemberId&&m.SenderDeleted==false),
                _ => query.Where(m => m.RecipientId == messageParams.MemberId && m.RecipientDeleted==false)
            };
            var messageQuery = query.Select(MessageExtension.ToDtoProjection());
            return await PaginateHelper.CreateAsync(messageQuery, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessagesThread(string currentUserId, string recipientId)
        {
            await dbContext.Messages
                .Where(x => x.RecipientId == currentUserId && x.SenderId == recipientId&&x.DateRead==null)
                .ExecuteUpdateAsync(setters=>setters.SetProperty(m=>m.DateRead,DateTime.UtcNow));
            return await dbContext.Messages
                .Where(x=>(x.RecipientId == currentUserId&&x.RecipientDeleted==false && x.SenderId == recipientId) 
                || (x.RecipientId == recipientId && x.SenderDeleted == false && x.SenderId == currentUserId))
                .OrderBy(m => m.MessageSent)
                .Select(MessageExtension.ToDtoProjection())
                .ToListAsync();

        }

        public async Task<bool> SaveAllAsync()
        {
            return await dbContext.SaveChangesAsync() > 0;
        }

        public void DeleteMessage(Message message)
        {
            dbContext.Messages.Remove(message);
        }
    }
}
