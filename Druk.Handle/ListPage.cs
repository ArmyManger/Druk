using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Druk.Handle
{
    /// <summary>
    /// List分页处理
    /// </summary>
    public class ListPage<T>
    {
        /// <summary>
        /// 页数
        /// </summary>
        public int PageCount { get; set; }
        /// <summary>
        /// 当前页码
        /// </summary>
        public int CurrPage { get; set; }
        /// <summary>
        /// 一页多少条
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 数据源信息
        /// </summary>
        private List<T> DataSource { get; set; }
        /// <summary>
        /// 获取数据源和一页多少
        /// </summary>
        /// <param name="List"></param>
        /// <param name="PageSize"></param>
        public ListPage(List<T> List, int PageSize = 4, int CurrPage = 0)
        {
            DataSource = List;
            this.PageSize = PageSize;
            this.PageCount = (int)Math.Ceiling((decimal)DataSource.Count / PageSize);
            this.CurrPage = CurrPage;

        }
        //是否有下一页
        public bool HasNextPage
        {
            get { return CurrPage < this.PageCount; }
        }
        /// <summary>
        /// 直接获取下一页
        /// </summary>
        /// <returns></returns>
        public List<T> NextPage()
        {
            var data = DataSource.Skip((CurrPage) * PageSize).Take(PageSize).ToList();
            CurrPage++;
            return data;
        }
        /// <summary>
        /// 获取指定页
        /// </summary>
        /// <returns></returns>
        public List<T> getPage(int CurrPage)
        {
            var data = DataSource.Skip((CurrPage) * PageSize).Take(PageSize).ToList();
            return data;
        }
    }
}
