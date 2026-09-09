using HelpDesk.Models.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Models.Entities
{
    /// <summary>
    /// 工单实体
    /// </summary>
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }  //描述
        public TicketCategoryEnum Category { get; set; }
        public TicketPriorityEnum Priority { get; set; }
        public TicketStatusEnum Status { get; set; }

        public int SubmitterId { get; set; }     //外键：提交人（必填）
        public User Submitter { get; set; }

        public int? AssigneeId { get; set; }     //外键：处理人（可空，未接单时为空）
        public User? Assignee { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }  //可空：还没解决时为空
        public DateTime? ClosedAt { get; set; }    //可空：还没关闭时为空

        public ICollection<TicketLog> Logs { get; set; } = new List<TicketLog>();

    }
}
