using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.BLL.DTOs
{
    /// <summary>
    ///  统计看板
    /// </summary>
    public class StatisticsDto
    {
        public Dictionary<string, int> StatusCounts { get; set; }  // {"Pending":5, "InProgress":2, ...}
        public int NewThisMonth { get; set; }                      // 本月新增
    }
}
