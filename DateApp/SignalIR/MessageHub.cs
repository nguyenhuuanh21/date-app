using DateApp.DTOs;
using DateApp.Entities;
using DateApp.Extensions;
using DateApp.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace DateApp.SignalIR
{
    public class MessageHub(IMessageRepository messageRepository,
        IMemberRepository memberRepository,
        IHubContext<PresenceHub> hubContext  ):Hub
    {
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext?.Request.Query["userId"].ToString()
                ??throw new HubException("Other user not found");
            var groupName = GetGroupName(Context.User?.GetUserId() ?? "", otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await AddToGroup(groupName);
            var messages = await messageRepository.GetMessagesThread(Context.User?.GetUserId() ?? "", otherUser);
            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await messageRepository.RemoveConnection(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var sender = await memberRepository.GetMemberByIdAsync(Context.User.GetUserId());
            var recipient=await memberRepository.GetMemberByIdAsync(createMessageDto.RecipientId);
            if(recipient==null|| sender==null||sender== recipient) throw new HubException("cannot send message");
            var message = new Message
            {
                SenderId = sender.Id,
                RecipientId = recipient.Id,
                Sender=sender,
                Recipient=recipient,
                Content=createMessageDto.Content
            };
            var groupName = GetGroupName(sender.Id, recipient.Id);
            var group = await messageRepository.GetMessageGroup(groupName);
            if (group != null&&group.Connections.Any(x=>x.UserId==recipient.Id))
            {
                message.DateRead = DateTime.UtcNow;
            }
            messageRepository.AddMessage(message);
            if(await messageRepository.SaveAllAsync())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", message.ToDto());
                var connections = await PrecenseTracker.GetConnectionsForUser(recipient.Id);
                if(connections != null&& connections.Count>0&& group != null && group.Connections.Any(x => x.UserId == recipient.Id))
                {
                    await hubContext.Clients.Clients(connections).SendAsync("NewMessageReceived",message.ToDto());
                }
                return;
            }
            throw new HubException ("Failed to send message");
        }
        private async Task<bool> AddToGroup(string groupName)
        {
            var group = await messageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUserId());
            if (group == null)
            {
                group = new Group(groupName) { Name = groupName };
                messageRepository.addGroup(group);
            }
            group.Connections.Add(connection);
            return await messageRepository.SaveAllAsync();
        }
        private static string GetGroupName(string? caller, string? other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}
