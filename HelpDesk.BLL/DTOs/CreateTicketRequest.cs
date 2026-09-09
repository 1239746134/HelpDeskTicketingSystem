using HelpDesk.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.BLL.DTOs
{
    /// <summary>
    /// 提交工单请求
    /// </summary>
    public class CreateTicketRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public TicketCategoryEnum Category { get; set; }
        public TicketPriorityEnum Priority { get; set; }
    }
}
