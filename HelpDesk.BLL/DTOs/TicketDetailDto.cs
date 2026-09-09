using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.BLL.DTOs
{
    /// <summary>
    /// 详情 = 列表项 + 正文 + 日志时间线
    /// </summary>
    public class TicketDetailDto : TicketDto
    {
        public string Description { get; set; }
        public List<TicketLogDto> Logs { get; set; } = new();
    }
}
