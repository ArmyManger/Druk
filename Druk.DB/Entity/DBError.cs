using System;
using System.Collections.Generic;
using System.Text;

namespace Druk.DB.Entity
{
    #region //数据库异常对象
    /// <summary>
    /// 数据库异常对象
    /// </summary>
    public class DBError
    {
        public System.Data.SqlClient.SqlConnection Conn { get; set; }
        public bool IsTrans { get; set; }
        public string Sql { get; set; }
        public object Params { get; set; }
        public Exception Error { get; set; }
    } 
    #endregion
}
