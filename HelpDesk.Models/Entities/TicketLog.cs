using HelpDesk.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Models.Entities
{
    /// <summary>
    /// 工单处理日志
    /// </summary>
    public class TicketLog
    {
        public int Id { get; set; }

        public int TicketId { get; set; }      //外键：属于哪张工单
        public Ticket Ticket { get; set; }

        public int OperatorId { get; set; }    //外键：谁操作的
        public User Operator { get; set; }

        public TicketLogActionEnum Action { get; set; }

        public TicketStatusEnum? FromStatus { get; set; }
        public TicketStatusEnum ToStatus { get; set; }

        public string Note { get; set; }        //备注
        public DateTime CreatedAt { get; set; }
    }
}
