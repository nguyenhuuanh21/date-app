using DateApp.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DateApp.Controllers
{

    public class AdminController(UserManager<AppUser>userManager) : BaseApiController
    {
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users=await userManager.Users.ToListAsync();
            var userList=new List<object>();
            foreach (var user in users)
            {
                var roles=await userManager.GetRolesAsync(user);
                userList.Add(new
                {
                    user.Id,
                    user.Email,
                    Roles = roles.ToList()
                });

            }
            return Ok(userList);
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("edit-role/{userId}")]
        public async Task<ActionResult> EditRole([FromRoute]string userId, [FromQuery]string roles)
        {
            if(string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roles))
            {
                return BadRequest("UserId and roles are required.");
            }
            var selectedRoles=roles.Split(",").ToArray();
            var user=await userManager.FindByIdAsync(userId);
            if(user==null)
            {
                return NotFound("User not found.");
            }
            var userRoles=await userManager.GetRolesAsync(user);
            var result=await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if(!result.Succeeded)
            {
                return BadRequest("Failed to add roles.");
            }
            result=await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if(!result.Succeeded)
            {
                return BadRequest("Failed to remove roles.");
            }
            return Ok(await userManager.GetRolesAsync(user));
        }
        [Authorize(Policy = "RequireModeratorRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosToModerate()
        {
            return Ok("This is a moderator endpoint that returns photos to moderate.");
        }
    }
}
