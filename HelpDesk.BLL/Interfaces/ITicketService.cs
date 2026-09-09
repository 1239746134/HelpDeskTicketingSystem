using HelpDesk.BLL.Common;
using HelpDesk.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.BLL.Interfaces
{
    /// <summary>
    ///
    /// </summary>
    public interface ITicketService
    {
        Task<TicketDto> CreateAsync(CreateTicketRequest request);          // 提交工单
        Task<PagedResult<TicketDto>> GetListAsync(TicketQuery query);      // 分页列表
        Task<TicketDetailDto> GetDetailAsync(int id);                      // 详情（含日志）
        Task<TicketDetailDto> UpdateStatusAsync(int id, UpdateStatusRequest request); // 状态流转
        Task<StatisticsDto> GetStatisticsAsync();                          // 统计
    }
}
