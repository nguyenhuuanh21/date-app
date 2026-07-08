using DateApp.Data;
using DateApp.DTOs;
using DateApp.Entities;
using DateApp.Extensions;
using DateApp.Helpers;
using DateApp.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DateApp.Controllers
{
    [Authorize]
    public class MembersController(IMemberRepository memberRepository,IPhotoService photoService) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers([FromQuery] MemberParams memberParams)
        {
            memberParams.CurrentMemberId=User.GetUserId();
            return Ok(await memberRepository.GetMembersAsync(memberParams));
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMember([FromRoute]string id)
        {
            var member =await memberRepository.GetMemberByIdAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            return Ok(member);
        }
        [HttpGet("{id}/photos")]
        public async Task<ActionResult<IReadOnlyList<Photo>>> GetMemberPhotos([FromRoute] string id)
        {
            return Ok(await memberRepository.GetPhotosForMemberAsync(id));
        }
        [HttpPut]
        public async Task<ActionResult> UpdateMember(MemberUpdateDto memberUpdateDto)
        {
            var memberId=User.GetUserId();
            var member = await memberRepository.GetMemberForUpdate(memberId);

            if (member == null)
            {
                return NotFound("cannot found member");
            }
            member.DisplayName=memberUpdateDto.DisplayName ?? member.DisplayName;
            member.Description=memberUpdateDto.Description ?? member.Description;
            member.City=memberUpdateDto.City ?? member.City;
            member.Country=memberUpdateDto.Country ?? member.Country;
            member.AppUser.DisplayName=memberUpdateDto.DisplayName ?? member.AppUser.DisplayName;
            memberRepository.Update(member);
            if(await memberRepository.SaveAllAsync())
            {
                return NoContent();
            }
            return BadRequest("Failed to update member !!!");
        }
        [HttpPost("add-photo")]
        public async Task<ActionResult<Photo>> AddPhoto([FromForm]  IFormFile file)
        {
            var member = await memberRepository.GetMemberForUpdate(User.GetUserId());
            if (member == null)
            {
                return NotFound("cannot found member");
            }
            var result=await photoService.UploadPhotoAsync(file);

            if(result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                MemberId=member.Id
            };
            if (member.ImageUrl == null)
            {
                member.ImageUrl = photo.Url;
                member.AppUser.ImageUrl= photo.Url;
            }
            member.Photos.Add(photo);

            if(await memberRepository.SaveAllAsync())
            {
                return photo;
            }
            return BadRequest("problem adding photo");

        }
        [HttpPut("set-main-photo/{photoId:int}")]
        public async Task<ActionResult> setMainPhoto([FromRoute] int photoId)
        {
            var member = await memberRepository.GetMemberForUpdate(User.GetUserId());
            if(member == null)
            {
                return NotFound("cannot found member");
            }
            var photo= member.Photos.SingleOrDefault(p=>p.Id==photoId);
            if (member.ImageUrl == photo?.Url || photo==null)
            {
                return BadRequest("cannot set this photo as main");
            }
            member.ImageUrl=photo.Url;
            member.AppUser.ImageUrl=photo.Url;
            if(await memberRepository.SaveAllAsync())
            {
                return NoContent();
            }
            return BadRequest("problem setting main photo");    
        }
        [HttpDelete("delete-photo/{photoId:int}")]
        public async Task<ActionResult> deletePhoto([FromRoute] int photoId)
        {
            var member = await memberRepository.GetMemberForUpdate(User.GetUserId());
            if (member == null)
            {
                return NotFound("cannot found member");
            }
            var photo = member.Photos.SingleOrDefault(p => p.Id == photoId);
            if (photo == null || photo.Url==member.ImageUrl)
            {
                return NotFound("you cannot delete this photo");
            }
            if(photo.PublicId != null)
            {
                var result=await photoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error != null)
                {
                    return BadRequest(result.Error.Message);
                }
            }
            member.Photos.Remove(photo);
            if(await memberRepository.SaveAllAsync())
            {
                return Ok();
            }
            return BadRequest("problem deleting photo");

        }

    }
}
