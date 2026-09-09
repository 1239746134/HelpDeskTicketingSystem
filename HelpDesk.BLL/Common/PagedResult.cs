using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.BLL.Common
{
    /// <summary>
    /// 分页包装
    /// </summary>
    public class PagedResult<T>
    {
        //当前页的数据列表
        public IReadOnlyList<T> Data { get; }

        //当前页码
        public int Page { get; }

        //每页条数
        public int PageSize { get; }

        //总条数
        public int TotalCount { get; }

        //总页数
        public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

        public PagedResult(IReadOnlyList<T> data, int page, int pageSize, int totalCount)
        {
            this.Data = data;
            this.Page = page;
            this.PageSize = pageSize;
            this.TotalCount = totalCount;
        }

        public static PagedResult<T> Create(IReadOnlyList<T> data, int page, int pageSize, int totalCount)
            => new PagedResult<T>(data, page, pageSize, totalCount);
    }
}
