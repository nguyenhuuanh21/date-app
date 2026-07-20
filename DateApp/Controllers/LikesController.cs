using DateApp.Entities;
using DateApp.Extensions;
using DateApp.Helpers;
using DateApp.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DateApp.Controllers
{
    public class LikesController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpPost("{targetMemberId}")]
        public async Task<ActionResult>ToggleLike([FromRoute] string targetMemberId)
        {
            var sourceMemberId=User.GetUserId();
            if(sourceMemberId==targetMemberId)
                return BadRequest("You cannot like yourself.");
            var existingLike=await unitOfWork.LikeRepository.GetMemberLike(sourceMemberId, targetMemberId);
            if (existingLike==null)
            {
                var like = new MemberLike
                {
                    SourceMemberId = sourceMemberId,
                    TargetMemberId = targetMemberId 
                };
                unitOfWork.LikeRepository.AddLike(like);
            }
            else
            {
                unitOfWork.LikeRepository.DeleteLike(existingLike);
            }
            if(await unitOfWork.Complete())
                return Ok();
            return BadRequest("Failed to update like.");
        }
        [HttpGet("list")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetCurrentMemberLikeIds()
        {
            var memberId=User.GetUserId();
            return Ok(await unitOfWork.LikeRepository.GetCurrentMemberLikeIds(memberId));
        }
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMemberLikes([FromQuery] LikeParams likeParams)
        {
            var memberId = User.GetUserId();
            likeParams.MemberId = memberId;
            return Ok(await unitOfWork.LikeRepository.GetMemberLikes(likeParams));

        }

    }
}
