using HelpDesk.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.BLL.Interfaces
{
    /// <summary>
    /// 读当前登录用户
    /// </summary>
    public interface ICurrentUserService
    {
        int? UserId { get; }
        string? UserName { get; }
        UserRoleEnum? Role { get; }
        bool IsAuthenticated { get; } // 是否已登录
        bool IsITSupport { get; }     // 是否 IT 角色（权限判断的快捷方式）
    }
}
