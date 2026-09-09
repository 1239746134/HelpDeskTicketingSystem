using HelpDesk.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.BLL.DTOs
{
    /// <summary>
    /// 列表查询参数（分页 + 筛选 + 排序）
    /// </summary>
    public class TicketQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public TicketStatusEnum? Status { get; set; }
        public TicketCategoryEnum? Category { get; set; }
        public TicketPriorityEnum? Priority { get; set; }
        public string? SortBy { get; set; }     // "CreatedAt" 或 "Priority"
        public string? SortOrder { get; set; }  // "asc" 或 "desc"

    }
}
