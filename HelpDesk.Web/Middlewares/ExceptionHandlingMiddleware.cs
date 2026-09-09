using HelpDesk.BLL.Exceptions;
using HelpDesk.Web.Common;
using System.Text.Json;

namespace HelpDesk.Web.Middlewares
{
    /// <summary>
    /// 全局异常兜底
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            this._next = next;
            this._logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(BusinessException ex)
            {
                // 业务异常 → 400 + 中文消息
                await WriteAsync(context, StatusCodes.Status400BadRequest, ApiResult.Fail(400, ex.Message));
            }
            catch(Exception ex)
            {
                // 未知异常 → 500 + 通用消息（详情只进日志，不暴露给前端）
                _logger.LogError(ex, "未处理异常：{Path}", context.Request.Path);
                await WriteAsync(context, StatusCodes.Status500InternalServerError, ApiResult.Fail(500, "服务器内部错误"));
            }
        }

        private static async Task WriteAsync(HttpContext context, int statusCode, ApiResult result)
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json; charset=utf-8";
            await context.Response.WriteAsync(JsonSerializer.Serialize(result, options));
        }
    }
}
