using CloudinaryDotNet.Actions;
using DateApp.DTOs;
using DateApp.Entities;
using DateApp.Extensions;
using DateApp.Helpers;
using DateApp.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DateApp.Controllers
{
    [Authorize]
    public class MessagesController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpPost]
        public async Task<ActionResult<MessageDto>>CreateMessage(CreateMessageDto createMessageDto)
        {
            var sender=await unitOfWork.MemberRepository.GetMemberByIdAsync(User.GetUserId());
            var recipient=await unitOfWork.MemberRepository.GetMemberByIdAsync(createMessageDto.RecipientId);
            if(sender==null || recipient == null||sender.Id== createMessageDto.RecipientId)
            {
                return BadRequest("cannot send this message !");
            }
            var message=new Message
            {
                SenderId=sender.Id,
                RecipientId=recipient.Id,
                Content=createMessageDto.Content
            };
            unitOfWork.MessageRepository.AddMessage(message);
            if(await unitOfWork.Complete())
            {
                return Ok(message.ToDto());
            }
            return BadRequest("Failed to send message !");
        }
        [HttpGet]
        public async Task<ActionResult<PaginateResult<MessageDto>>> GetMessagesByContainer(
            [FromQuery] MessageParams messageParams)
        {
            messageParams.MemberId=User.GetUserId();

            return await unitOfWork.MessageRepository.GetMessagesForMember(messageParams);
        }
        [HttpGet("thread/{recipientId}")]
        public async Task<ActionResult<IReadOnlyList<MessageDto>>>GetMessageThread(string recipientId)
        {
            var currentUserId=User.GetUserId();
            return Ok(await unitOfWork.MessageRepository.GetMessagesThread(currentUserId,recipientId));
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteMessage(string id)
        {
            var currentUserId = User.GetUserId();
            var message = await unitOfWork.MessageRepository.GetMessage(id);
            if (message == null) return NotFound("message not found");
            if(message.SenderId != currentUserId && message.RecipientId != currentUserId)
            {
                return Unauthorized("You are not authorized to delete this message");
            }
            if(message.SenderId == currentUserId)
            {
                message.SenderDeleted = true;
            }
            if(message.RecipientId == currentUserId)
            {
                message.RecipientDeleted = true;
            }
            if (message.SenderDeleted && message.RecipientDeleted)
            {
                unitOfWork.MessageRepository.DeleteMessage(message);
            }
            if(await unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("Failed to delete message !");
        }
    }
}
