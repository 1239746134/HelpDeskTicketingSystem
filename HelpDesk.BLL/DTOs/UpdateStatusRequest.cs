using HelpDesk.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.BLL.DTOs
{
    /// <summary>
    /// 状态变更请求（IT 专用）
    /// </summary>
    public class UpdateStatusRequest
    {
        public TicketStatusEnum NewStatus { get; set; }
        public string? Note { get; set; }
    }
}
