using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Models.Enums
{
    /// <summary>
    /// 工单状态
    /// </summary>
    public enum TicketStatusEnum
    {
        Pending,    //待定
        InProgress, //进行中
        Resolved,   //已解决
        Closed      //关闭
    }
}
