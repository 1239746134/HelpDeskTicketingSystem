using HelpDesk.BLL.DTOs;
using HelpDesk.BLL.Interfaces;
using HelpDesk.Web.Common;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Web.Controllers
{

    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            this._userService = userService;
        }

        // POST /api/auth/register
        [HttpPost("register")]
        public async Task<ApiResult<UserDto>> Register([FromBody] RegisterRequest request)
        {
            UserDto user = await _userService.RegisterAsync(request);
            return ApiResult<UserDto>.OK(user);
        }

        // POST /api/auth/login
        [HttpPost("login")]
        public async Task<ApiResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            LoginResponse response = await _userService.LoginAsync(request);
            return ApiResult<LoginResponse>.OK(response);
        }
    }
}
