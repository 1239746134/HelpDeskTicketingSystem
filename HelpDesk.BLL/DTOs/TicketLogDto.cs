using HelpDesk.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.BLL.DTOs
{
    /// <summary>
    /// 审计日志项
    /// </summary>
    public class TicketLogDto
    {
        public int Id { get; set; }
        public int OperatorId { get; set; }
        public string? OperatorName { get; set; }
        public TicketLogActionEnum Action { get; set; }
        public TicketStatusEnum? FromStatus { get; set; }
        public TicketStatusEnum ToStatus { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
