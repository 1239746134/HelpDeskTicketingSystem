using HelpDesk.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.BLL.Interfaces
{
    /// <summary>
    /// 注册/登录
    /// </summary>
    public interface IUserService
    {
        //注册    POST /api/auth/register
        Task<UserDto> RegisterAsync(RegisterRequest request);
        //登录    POST /api/auth/login
        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
}
