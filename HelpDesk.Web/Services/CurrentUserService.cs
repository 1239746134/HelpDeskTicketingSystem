using HelpDesk.BLL.Interfaces;
using HelpDesk.Models.Enums;
using System.Security.Claims;

namespace HelpDesk.Web.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;


        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

        public int? UserId
        {
            get
            {
                var sub = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return int.TryParse(sub, out var id) ? id : null;
            }
        }


        public string? UserName => User?.FindFirst(ClaimTypes.Name)?.Value;

        public UserRoleEnum? Role
        {
            get
            {
                var role = User?.FindFirst(ClaimTypes.Role)?.Value;
                return Enum.TryParse<UserRoleEnum>(role, out var r) ? r : null;
            }
        }

        public bool IsITSupport => Role == UserRoleEnum.ITSupport;
    }
}
