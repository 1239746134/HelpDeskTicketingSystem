using HelpDesk.BLL.Common;
using HelpDesk.BLL.DTOs;
using HelpDesk.BLL.Interfaces;
using HelpDesk.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Web.Controllers
{

    [ApiController]
    [Route("api/tickets")]
    [Authorize]                     // 整个 Controller 都要登录
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // POST /api/tickets
        [HttpPost]
        public async Task<ApiResult<TicketDto>> Create([FromBody] CreateTicketRequest request)
        {
            return ApiResult<TicketDto>.OK(await _ticketService.CreateAsync(request));
        }

        // GET /api/tickets?page=1&pageSize=10&status=Pending&sortBy=CreatedAt&sortOrder=desc
        [HttpGet]
        public async Task<ApiResult<PagedResult<TicketDto>>> GetList([FromQuery] TicketQuery query)
        {
            return ApiResult<PagedResult<TicketDto>>.OK(await _ticketService.GetListAsync(query));
        }

        // GET /api/tickets/{id}
        [HttpGet("{id:int}")]
        public async Task<ApiResult<TicketDetailDto>> GetDetail(int id)
        {
            return ApiResult<TicketDetailDto>.OK(await _ticketService.GetDetailAsync(id));
        }

        // PUT /api/tickets/{id}/status
        [HttpPut("{id:int}/status")]
        [Authorize(Roles = "ITSupport")]              // 只有 IT 能改状态
        public async Task<ApiResult<TicketDetailDto>> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            return ApiResult<TicketDetailDto>.OK(await _ticketService.UpdateStatusAsync(id, request));
        }

        // GET /api/tickets/statistics
        [HttpGet("statistics")]
        [Authorize(Roles = "ITSupport")]              // 只有 IT 能看统计
        public async Task<ApiResult<StatisticsDto>> GetStatistics()
        {
            return ApiResult<StatisticsDto>.OK(await _ticketService.GetStatisticsAsync());
        }
    }
}
