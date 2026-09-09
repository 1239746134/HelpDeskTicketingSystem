using HelpDesk.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Models.Entities
{
    /// <summary>
    /// 用户实体
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public UserRoleEnum Role { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<Ticket> SubmittedTickets { get; set; } = new List<Ticket>();      //我提交的工单
        public ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();       //分派给我的工单
        public ICollection<TicketLog> Logs { get; set; } = new List<TicketLog>();               //我的操作记录
    }
}
