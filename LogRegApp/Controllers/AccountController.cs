using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using LogRegApp.Models;
using System.Security.Cryptography;
using System.Text;
using LogRegApp.DTO;
using Microsoft.EntityFrameworkCore;

namespace LogRegApp.Controllers
{
    [Microsoft.AspNetCore.Components.Route("api/[controller]")]
    [ApiController]
    public class AccountController:ControllerBase
    {
        public ApplicationDbContext _context { get; set; }

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
        {
            if(await UserExists(registerDto.Username))
            {
                return BadRequest("Username already exists!");
            }

            var hmac = new HMACSHA256();

            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key,
            };

            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.AppUsers.AnyAsync(x => x.UserName == username.ToLower());
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUser>>Login(LoginDto loginDto)
        {
            var user = await _context.AppUsers
                .SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

            if (user == null)
            {
                return BadRequest("Invalid username");
            }

            var hmac=new HMACSHA256(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for(int i=0;i<computedHash.Length;i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    return BadRequest("Invalid password");
                }
            }

            return Ok(user);
        }
    }
}
