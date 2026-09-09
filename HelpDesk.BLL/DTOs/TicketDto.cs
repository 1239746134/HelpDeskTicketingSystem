using HelpDesk.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.BLL.DTOs
{
    /// <summary>
    /// 列表项
    /// </summary>
    public class TicketDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public TicketCategoryEnum Category { get; set; }
        public TicketPriorityEnum Priority { get; set; }
        public TicketStatusEnum Status { get; set; }
        public int SubmitterId { get; set; }
        public string? SubmitterName { get; set; }   // 从 User 联查
        public int? AssigneeId { get; set; }
        public string? AssigneeName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
    }
}
