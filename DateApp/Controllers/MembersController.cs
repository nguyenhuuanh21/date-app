using DateApp.Data;
using DateApp.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DateApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController(AppDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<AppUser>>> GetMembers()
        {
            var members= await context.AppUsers.ToListAsync();
            return Ok(members);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetMember([FromRoute]string id)
        {
            var member =await context.AppUsers.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            return Ok(member);
        }
    }
}
