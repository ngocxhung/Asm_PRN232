using Microsoft.AspNetCore.Mvc;
using BussinessObjects.Models;
using Repositories;
using LibraryManagement_WebAPI.Services;
using System.Threading.Tasks;

namespace LibraryManagement_WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        public AuthController(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userRepository.AuthenticateAsync(request.Username, request.Password);
            if (user == null)
                return Unauthorized("Invalid username or password");
            var token = _jwtService.GenerateJwtToken(user);
            return Ok(new { token, user = new { user.UserId, user.Username, user.FullName, user.Role } });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
} 