企业 IT 服务工单系统（HelpDesk）

一个面向企业内部 IT 支持的工单管理系统。员工提交工单，IT 支持人员接单、处理、解决、关闭，全程记录审计日志。

后端基于 .NET 8 + ASP.NET Core Web API 独立设计与实现，采用经典四层分层架构。

功能特性

- 用户认证：注册 / 登录，JWT 令牌认证
- 角色权限：员工（Employee）与 IT 支持（ITSupport）两种角色，基于 RBAC 的权限控制
- 工单管理：提交、分页查询（多条件筛选 + 排序）、详情查看
- 状态流转：工单状态机，非法流转自动拦截
- 审计日志：全链路记录每次操作（操作人、前后状态、备注），可追溯
- 数据权限隔离：普通员工仅能查看本人提交的工单，IT 可查看全部
- 统计看板：工单状态分布 + 本月新增（IT 专属）
- 工程化：统一响应格式、全局异常处理、Serilog 结构化日志、Swagger 文档

技术栈

  分类  	技术                                      
  框架  	.NET 8 / ASP.NET Core Web API           
  ORM 	EF Core 8 + Pomelo.EntityFrameworkCore.MySql
  数据库 	MySQL                                   
  认证  	JWT（Microsoft.AspNetCore.Authentication.JwtBearer）
  密码  	PBKDF2（10 万次迭代 + 随机盐 + 恒定时间比较）          
  日志  	Serilog（控制台 + 按日滚动文件）                   
  文档  	Swagger / OpenAPI                       

项目结构（四层架构）

    HelpDeskTicketingSystem/
    ├── HelpDesk.Models    # 领域模型（实体 + 枚举），零依赖
    ├── HelpDesk.DAL       # 数据访问层（EF Core + 泛型仓储 + 工作单元）
    ├── HelpDesk.BLL       # 业务逻辑层（服务 + DTO + 安全 + 接口）
    └── HelpDesk.Web       # 表现层（控制器 + 中间件 + 配置）

依赖方向：Web → BLL → DAL → Models。

数据模型

- User：用户（用户名唯一、密码哈希、邮箱、角色）
- Ticket：工单（标题、描述、分类、优先级、状态、提交人、处理人）
- TicketLog：审计日志（工单、操作人、动作、状态流转、备注）

状态机

    Pending ──→ InProgress ──→ Resolved ──→ Closed
       │                          │  ▲
       │                          └──┘  (重新打开)
       └────────────────────────→ Closed

  流转                   	说明               
  Pending → InProgress 	接单（自动指派当前 IT）    
  Pending → Closed     	关闭               
  InProgress → Resolved	解决（记录 ResolvedAt）
  InProgress → Closed  	关闭               
  Resolved → Closed    	确认关闭             
  Resolved → InProgress	重新打开             

快速开始

环境要求

- .NET 8 SDK
- MySQL 8.0（本地运行）

配置数据库

1. 创建数据库：

    CREATE DATABASE HelpDesk CHARACTER SET utf8mb4;

1. 修改 appsettings.json 中的连接字符串：

    "ConnectionStrings": {
      "Default": "Server=localhost;Port=3306;Database=HelpDesk;User=root;Password=你的密码;"
    }

1. 执行数据库迁移：

    dotnet ef database update --project HelpDesk.DAL --startup-project HelpDesk.Web

运行

    cd HelpDesk.Web
    dotnet run

启动后访问 Swagger 文档：https://localhost:30678/swagger（或 http://localhost:30679/swagger）

 API 接口

  方法  	路径                      	说明            	权限       
  POST	/api/auth/register      	注册            	公开       
  POST	/api/auth/login         	登录（返回 JWT）    	公开       
  POST	/api/tickets            	提交工单          	登录       
  GET 	/api/tickets            	工单列表（分页/筛选/排序）	登录       
  GET 	/api/tickets/{id}       	工单详情（含日志）     	登录       
  PUT 	/api/tickets/{id}/status	变更状态          	ITSupport
  GET 	/api/tickets/statistics 	统计看板          	ITSupport

使用示例

1. 注册并登录获取 token
2. 在 Swagger 右上角点击 Authorize，粘贴 token（不含 Bearer 前缀）
3. 调用需要授权的接口

统一响应格式

所有接口返回统一包裹：

    {
      "code": 200,
      "message": "success",
      "data": { ... }
    }

- 业务异常：400 + 中文错误信息
- 未知异常：500 + 通用提示（详情仅记录日志）

 许可证

个人学习 / 演示项目。
