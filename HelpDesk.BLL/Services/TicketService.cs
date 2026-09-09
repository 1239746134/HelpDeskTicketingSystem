using HelpDesk.BLL.Common;
using HelpDesk.BLL.DTOs;
using HelpDesk.BLL.Exceptions;
using HelpDesk.BLL.Interfaces;
using HelpDesk.DAL.Repositories;
using HelpDesk.Models.Entities;
using HelpDesk.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.BLL.Services
{
    public class TicketService : ITicketService
    {
        private readonly IRepository<Ticket> _ticketRepo;
        private readonly IRepository<TicketLog> _logRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<TicketService> _logger;

        public TicketService(IRepository<Ticket> ticketRepo, IRepository<TicketLog> logRepo, IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<TicketService> logger)
        {
            this._ticketRepo = ticketRepo;
            this._logRepo = logRepo;
            this._unitOfWork = unitOfWork;
            this._currentUserService = currentUserService;
            this._logger = logger;
        }


        public async Task<TicketDto> CreateAsync(CreateTicketRequest request)
        {
            //权限校验
            if (_currentUserService.IsAuthenticated == false)
            {
                throw new BusinessException("请先登录");
            }

            //参数校验
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw new BusinessException("标题不能为空");
            }
            if (string.IsNullOrWhiteSpace(request.Description))
            {
                throw new BusinessException("描述不能为空");
            }

            //当前用户
            int userId = _currentUserService.UserId!.Value;

            //构造工单
            Ticket ticket = new Ticket()
            {
                Title = request.Title,
                Description = request.Description,
                Category = request.Category,
                Priority = request.Priority,
                Status = TicketStatusEnum.Pending,      // 新单固定 Pending
                SubmitterId = userId,
                CreatedAt = DateTime.Now
            };

            //构造第一条审计日志
            TicketLog log = new TicketLog()
            {
                Ticket = ticket,
                OperatorId = userId,
                Action = TicketLogActionEnum.Created,
                FromStatus = null,
                ToStatus = TicketStatusEnum.Pending,
                Note = "提交工单",
                CreatedAt = DateTime.Now
            };

            //写入数据库
            await _ticketRepo.AddAsync(ticket);
            await _logRepo.AddAsync(log);
            await _unitOfWork.SaveChangesAsync();

            //返回 DTO
            return new TicketDto()
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Category = ticket.Category,
                Priority = ticket.Priority,
                Status = ticket.Status,
                SubmitterId = ticket.SubmitterId,
                AssigneeId = null,         // 新单还没人接
                AssigneeName = null,
                CreatedAt = ticket.CreatedAt,
                ResolvedAt = null,
                ClosedAt = null
            };

        }

        public async Task<TicketDetailDto> GetDetailAsync(int id)
        {
            //带日志的查询
            Ticket? ticket = await _ticketRepo.Query()
                .Include(t => t.Submitter)
                .Include(t => t.Assignee)
                .Include(t => t.Logs)
                .ThenInclude(l => l.Operator)
                .FirstOrDefaultAsync(t => t.Id == id);

            //不存在
            if(ticket == null)
            {
                throw new BusinessException("工单不存在");
            }

            //权限校验
            if (_currentUserService.IsAuthenticated == false)
            {
                throw new BusinessException("请先登录");
            }
            if(_currentUserService.IsITSupport==false && ticket.Submitter.Id != _currentUserService.UserId!.Value)
            {
                throw new BusinessException("无权查看该工单");
            }


            return BuildDetailDto(ticket);
        }

        public async Task<PagedResult<TicketDto>> GetListAsync(TicketQuery query)
        {
            //基础查询
            IQueryable<Ticket> q = _ticketRepo.Query().Include(t => t.Submitter).Include(t => t.Assignee);

            //权限过滤
            if (_currentUserService.IsAuthenticated == false)
            {
                throw new BusinessException("请先登录");
            }
            if (_currentUserService.IsITSupport == false)
            {
                q = q.Where(t => t.SubmitterId == _currentUserService.UserId!.Value);
            }

            //条件筛选
            if (query.Status.HasValue)
            {
                q = q.Where(t => t.Status == query.Status.Value);
            }
            if (query.Category.HasValue)
            {
                q = q.Where(t => t.Category == query.Category.Value);
            }

            if (query.Priority.HasValue)
            {
                q = q.Where(t => t.Priority == query.Priority.Value);
            }

            //排序
            bool desc = string.Equals(query.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);
            q = query.SortBy?.ToLowerInvariant() switch
            {
                "priority" => desc
                    ? q.OrderByDescending(t => t.Priority)
                    : q.OrderBy(t => t.Priority),
                _ => desc
                    ? q.OrderByDescending(t => t.CreatedAt)
                    : q.OrderBy(t => t.CreatedAt)   // 默认 CreatedAt
            };


            //总数
            int total = await q.CountAsync();

            //分页 + 投影
            List<TicketDto> items = await q
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(t => new TicketDto()
                {
                    Id = t.Id,
                    Title = t.Title,
                    Category = t.Category,
                    Priority = t.Priority,
                    Status = t.Status,
                    SubmitterId = t.SubmitterId,
                    SubmitterName = t.Submitter.UserName,
                    AssigneeId = t.AssigneeId,
                    AssigneeName = t.Assignee != null ? t.Assignee.UserName : null,
                    CreatedAt = t.CreatedAt,
                    ResolvedAt = t.ResolvedAt,
                    ClosedAt = t.ClosedAt
                })
                .ToListAsync();

            return PagedResult<TicketDto>.Create(items, query.Page, query.PageSize, total);
        }

        public async Task<StatisticsDto> GetStatisticsAsync()
        {
            if (_currentUserService.IsITSupport == false)
            {
                throw new BusinessException("无权查看统计");
            }

            var grouped = await _ticketRepo.Query()
                .GroupBy(t => t.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            var statusCounts = new Dictionary<string, int>();
            foreach(TicketStatusEnum s in Enum.GetValues<TicketStatusEnum>())
            {
                statusCounts[s.ToString()] = 0;
            }
            foreach(var g in grouped)
            {
                statusCounts[g.Status.ToString()] = g.Count;
            }


            var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            int newThisMonth = await _ticketRepo.Query()
                .CountAsync(t => t.CreatedAt >= firstDayOfMonth);

            return new StatisticsDto
            {
                StatusCounts = statusCounts,
                NewThisMonth = newThisMonth
            };
        }

        public async Task<TicketDetailDto> UpdateStatusAsync(int id, UpdateStatusRequest request)
        {
            //查工单
            Ticket? ticket = await _ticketRepo.Query()
                .Include(t => t.Submitter)
                .Include(t => t.Assignee)
                .Include(t => t.Logs).ThenInclude(l => l.Operator)
                .FirstOrDefaultAsync(t => t.Id == id);

            if(ticket == null)
            {
                throw new BusinessException("工单不存在");
            }

            //权限
            if (_currentUserService.IsITSupport == false)
            {
                throw new BusinessException("只有 IT 人员可以变更工单状态");

            }

            TicketStatusEnum from = ticket.Status;
            TicketStatusEnum to = request.NewStatus;

            //状态机校验
            if (IsValidTransition(from, to) == false)
            {
                throw new BusinessException($"非法状态流转：{from} → {to}");
            }

            //副作用处理
            ticket.Status = to;
            switch (to)
            {
                case TicketStatusEnum.InProgress when from == TicketStatusEnum.Pending:
                    ticket.AssigneeId = _currentUserService.UserId!.Value;   // 接单，指派给当前 IT
                    break;
                case TicketStatusEnum.InProgress when from == TicketStatusEnum.Resolved:
                    ticket.ResolvedAt = null;                          // 重新打开，清掉解决时间
                    break;
                case TicketStatusEnum.Resolved:
                    ticket.ResolvedAt = DateTime.Now;                  // 解决，记时间
                    break;
                case TicketStatusEnum.Closed:
                    ticket.ClosedAt = DateTime.Now;                    // 关闭，记时间
                    break;
            }

            //写审计日志
            TicketLog? log = new TicketLog
            {
                Ticket = ticket,
                OperatorId = _currentUserService.UserId!.Value,
                Action = TicketLogActionEnum.StatusChanged,
                FromStatus = from,
                ToStatus = to,
                Note = request.Note ?? "",
                CreatedAt = DateTime.Now
            };

            await _logRepo.AddAsync(log);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"工单 {ticket.Id} 状态流转：{from} → {to}，操作人={_currentUserService.UserName}");

            //返回最新详情
            return BuildDetailDto(ticket);
        }

        //状态机规则
        private static bool IsValidTransition(TicketStatusEnum from, TicketStatusEnum to)
        {
            return (from, to) switch
            {
                (TicketStatusEnum.Pending, TicketStatusEnum.InProgress) => true,  // 接单
                (TicketStatusEnum.Pending, TicketStatusEnum.Closed) => true,  // 关闭（重复）
                (TicketStatusEnum.InProgress, TicketStatusEnum.Resolved) => true,  // 解决
                (TicketStatusEnum.InProgress, TicketStatusEnum.Closed) => true,  // 关闭
                (TicketStatusEnum.Resolved, TicketStatusEnum.Closed) => true,  // 确认关闭
                (TicketStatusEnum.Resolved, TicketStatusEnum.InProgress) => true,  // 重新打开
                _ => false                                                          // 其余全部非法
            };
        }


        private TicketDetailDto BuildDetailDto(Ticket ticket)
        {
            int? currentUserId = _currentUserService.UserId;
            string? currentUserName = _currentUserService.UserName;

            return new TicketDetailDto()
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Category = ticket.Category,
                Priority = ticket.Priority,
                Status = ticket.Status,
                SubmitterId = ticket.SubmitterId,
                SubmitterName = ticket.Submitter != null ? ticket.Submitter.UserName : null,
                AssigneeId = ticket.AssigneeId,
                AssigneeName = ticket.Assignee != null ? ticket.Assignee.UserName
                    : (ticket.AssigneeId == currentUserId ? currentUserName : null),
                CreatedAt = ticket.CreatedAt,
                ResolvedAt = ticket.ResolvedAt,
                ClosedAt = ticket.ClosedAt,
                Logs = ticket.Logs
                .OrderBy(l => l.CreatedAt)
                .Select(l => new TicketLogDto()
                {
                    Id = l.Id,
                    OperatorId = l.OperatorId,
                    OperatorName = l.Operator != null ? l.Operator.UserName
                        : (l.OperatorId == currentUserId ? currentUserName : null),
                    Action = l.Action,
                    FromStatus = l.FromStatus,
                    ToStatus = l.ToStatus,
                    Note = l.Note,
                    CreatedAt = l.CreatedAt
                })
                .ToList()
            };
        }
    }
}
