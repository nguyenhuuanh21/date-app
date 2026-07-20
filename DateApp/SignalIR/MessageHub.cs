using DateApp.DTOs;
using DateApp.Entities;
using DateApp.Extensions;
using DateApp.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace DateApp.SignalIR
{
    public class MessageHub(IUnitOfWork unitOfWork,
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
            var messages = await unitOfWork.MessageRepository.GetMessagesThread(Context.User?.GetUserId() ?? "", otherUser);
            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await unitOfWork.MessageRepository.RemoveConnection(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var sender = await unitOfWork.MemberRepository.GetMemberByIdAsync(Context.User.GetUserId());
            var recipient=await unitOfWork.MemberRepository.GetMemberByIdAsync(createMessageDto.RecipientId);
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
            var group = await unitOfWork.MessageRepository.GetMessageGroup(groupName);
            if (group != null&&group.Connections.Any(x=>x.UserId==recipient.Id))
            {
                message.DateRead = DateTime.UtcNow;
            }
            unitOfWork.MessageRepository.AddMessage(message);
            if(await unitOfWork.Complete())
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
            var group = await unitOfWork.MessageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUserId());
            if (group == null)
            {
                group = new Group(groupName) { Name = groupName };
                unitOfWork.MessageRepository.addGroup(group);
            }
            group.Connections.Add(connection);
            return await unitOfWork.Complete();
        }
        private static string GetGroupName(string? caller, string? other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}
