using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MStack.Core.ComponetModels
{
    /// <summary>
    /// 分页数据返回对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Page<T>
    {
        /// <summary>
        /// 分页的结果信息
        /// </summary>
        public Paging Paging { get; set; }

        /// <summary>
        /// 分页之后的数据集合
        /// </summary>
        public List<T> Records { get; set; }
    }

    public class Paging
    {
        public Paging()
        {
            PageIndex = 1;
            PageSize = 10;
        }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPage { get { return Total / PageSize + (Total % PageSize > 0 ? 1 : 0); } }
        public int Total { get; set; }
    }
}
