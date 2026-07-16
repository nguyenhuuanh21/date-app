using DateApp.Entities;
using DateApp.Extensions;
using DateApp.Helpers;
using DateApp.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DateApp.Controllers
{
    public class LikesController(ILikesRepository likesRepository) : BaseApiController
    {
        [HttpPost("{targetMemberId}")]
        public async Task<ActionResult>ToggleLike([FromRoute] string targetMemberId)
        {
            var sourceMemberId=User.GetUserId();
            if(sourceMemberId==targetMemberId)
                return BadRequest("You cannot like yourself.");
            var existingLike=await likesRepository.GetMemberLike(sourceMemberId, targetMemberId);
            if (existingLike==null)
            {
                var like = new MemberLike
                {
                    SourceMemberId = sourceMemberId,
                    TargetMemberId = targetMemberId 
                };
                likesRepository.AddLike(like);
            }
            else
            {
                likesRepository.DeleteLike(existingLike);
            }
            if(await likesRepository.SaveAllAsync())
                return Ok();
            return BadRequest("Failed to update like.");
        }
        [HttpGet("list")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetCurrentMemberLikeIds()
        {
            var memberId=User.GetUserId();
            return Ok(await likesRepository.GetCurrentMemberLikeIds(memberId));
        }
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMemberLikes([FromQuery] LikeParams likeParams)
        {
            var memberId = User.GetUserId();
            likeParams.MemberId = memberId;
            return Ok(await likesRepository.GetMemberLikes(likeParams));

        }

    }
}
