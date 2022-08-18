using System;
using System.Collections.Generic;
using System.Text;

namespace Druk.Common.Entity
{
    #region //日志
    [Serializable]
    public class LogEntity
    {
        public string Program { get; set; }
        public string ThreadName { get; set; }
        public Druk.Common.ComEnum.LogLevel Level { get; set; }
        public object Body { get; set; }
        public DateTime Time { get; set; }
        public DateTime HandleTime { get; set; } //处理时间
    }

    #endregion

    #region //对象修改

    public class LogEntityModifiy
    {
        public LogEntityModifiy() { UpdateTime = DateTime.Now; Remark = string.Empty; }
        public int ID { get; set; }
        public object User { get; set; }
        public Druk.Common.ComEnum.EntityType Entity { get; set; }
        public object OldEntity { get; set; }
        public object NewEntity { get; set; }
        public ComEnum.EntityMethod Method { get; set; }
        public string Remark { get; set; }
        public DateTime UpdateTime { get; set; }
    }
    #endregion

    #region// 接口请求处理失败
    public class LogEntityApiRequestError
    {
        public DateTime DateKey { get; set; }
        public string apiUrl { get; set; }
        public string JsonText { get; set; }
        public string description { get; set; }

    }
    #endregion
    #region //接口访问记录

    public class LogVisitPV : VisitPV
    {


        public int UserID;
        public string UserCodeNo;
        public string UserName;
        public string Token;
        public string ProfileToken;

        public DateTime BeginTime;
        public DateTime EndTime;

        public string PageURL;
        public string ParamsGet;
        public string ParamsPost;
        public string Method;
        public string IP;

        public int ResultCode;
        public string ResultMessage;
        public string ResultBody;

        public int HttpCodeStatus;
        public string Exception;
    }
    #endregion

    #region //PV记录

    public class VisitPV
    {

    }

    #endregion
}
