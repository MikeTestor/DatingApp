using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{   
    [Authorize] 
    public class MembersController : ControllerApiBase
    {
        [HttpGet]  // localhost:5000/api/members
        public async Task<ActionResult<List<AppUser>>> GetMembers(AppDbContext context)
        {
            var members = await context.Users.ToListAsync();
            return Ok(members);
        }

        [HttpGet("{id}")]  // localhost:5000/api/members/Mike-id
        public async Task<ActionResult<AppUser>> GetMember(string id, AppDbContext context)
        {
            var member = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (member == null)
            {
                return NotFound();
            }
            return Ok(member);
        }
    }
}
