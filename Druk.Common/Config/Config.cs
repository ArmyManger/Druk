using System;
using System.Collections.Generic;
using System.Text;

namespace Druk.Common
{
    public class Config
    {
        #region //默认值设定

        /// <summary>
        /// 项目编号
        /// </summary>
        public const int ProjectID = 1;
        /// <summary>
        /// 默认时间(最小时间)
        /// </summary>
        public static DateTime DefaultDateTime = new DateTime(1900, 1, 1);
        /// <summary>
        /// 默认编码方式 UTF-8
        /// </summary>
        public const string DefaultEncoding = "UTF-8";
        /// <summary>
        /// 默认列表页显示条数
        /// </summary>
        public const int DefaultPageSize = 10;

        #endregion
    }
}
