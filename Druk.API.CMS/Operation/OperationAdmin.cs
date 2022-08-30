using Druk.API.CMS.Util;
using Druk.Common.Entity;
using Druk.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Druk.API.CMS.Operation
{
    /// <summary>
    /// 业务处理类
    /// </summary>
    public class OperationAdmin
    {
        /// <summary>
        /// 添加会员
        /// </summary>
        /// <param name="adminPjson"></param>
        /// <returns></returns>
        public static bool Add(AdminPJson adminPjson)
        {
            var adminModel = new DB.Entity.Admin()
            {
                name = adminPjson.name,
                pwd = adminPjson.pwd,
                nickName = adminPjson.nickName,
                phone = adminPjson.phone
            };
            var add = DB.Config.Conn_Druk.AddModel<DB.Entity.Admin>(adminModel);
            if (add == null)
            {
                return false;
            }
            return true;
        }
    }
}
