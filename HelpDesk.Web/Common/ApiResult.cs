namespace HelpDesk.Web.Common
{
    /// <summary>
    /// 统一 API 响应包裹
    /// </summary>
    public class ApiResult<T>
    {
        public int Code { get; set; }   //用 HTTP 状态码语义
        public string Message { get; set; }  //提示消息（成功为 "success"，失败为具体原因）
        public T? Data { get; set; }         //业务数据

        public ApiResult(int code, string message, T? data)
        {
            this.Code = code;
            this.Message = message;
            this.Data = data;
        }

        //成功
        public static ApiResult<T> OK(T data)
        {
            return new ApiResult<T>(200, "success", data);
        }

        //成功但无数据
        public static ApiResult<T> OK()
        {
            return new ApiResult<T>(200, "success", default);
        }

        //失败
        public static ApiResult<T> Fail(int code,string message)
        {
            return new ApiResult<T>(code, message, default);
        }
    }

    //给「无返回数据」的场景用（如只报错的异常中间件）
    public class ApiResult: ApiResult<object?>
    {
        private ApiResult(int code, string message, object? data):base(code, message, data) { }

        public static ApiResult Ok()
        {
            return new ApiResult(200, "success", null);
        }

        public static new ApiResult Fail(int code, string message)
        {
            return new ApiResult(code, message, null);
        }
    }
}
