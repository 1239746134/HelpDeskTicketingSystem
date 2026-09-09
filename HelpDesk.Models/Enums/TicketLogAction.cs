using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Models.Enums
{
    /// <summary>
    /// 审计日志动作类型
    /// </summary>
    public enum TicketLogActionEnum
    {
        Created,        //创建工单
        StatusChanged   //状态变更
    }
}
