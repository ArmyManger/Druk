using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Druk.Common
{
    public partial class ComEnum
    {
        #region //日志 队列

        /// <summary>
        /// 日志等级
        /// </summary>
        public enum LogLevel
        {
            [Description("信息")]
            Info = 1,
            [Description("调试")]
            Debug = 2,
            [Description("警告")]
            Warn = 3,
            [Description("异常")]
            Error = 4,
            [Description("毁灭")]
            Fatal = 5,
        }
        /// <summary>
        /// 队列类型 根据此枚举获取队列名
        /// </summary>
        public enum RabbitQueue
        {
            [Description("Sql.Normal.Normal")]
            日志_常规 = 1,
            [Description("Sql.PV.CrmPV")]
            日志_主接口_访问 = 2,
            [Description("Sql.Operate.CrmOperate")]
            日志_主接口_操作修改 = 3,
            [Description("Sql.PV.ExternalPV")]
            日志_外部接口访问日志 = 4,

        }

        #endregion
         
        #region //对象类型
        /// <summary>
        /// 数据库对象类型, 主要用于修改和启用禁用时的日志存储和后期处理
        /// </summary>
        public enum EntityType
        {
        }
        #endregion

        #region //操作类型
        /// <summary>
        /// 操作类型
        /// </summary>
        public enum EntityMethod
        {
            [Description("添加")]
            添加 = 1,
            [Description("删除")]
            删除 = 2,
            [Description("修改信息")]
            修改信息 = 3,
            [Description("启用")]
            启用 = 4,
            [Description("禁用")]
            禁用 = 5,
        }
        #endregion

        #region //文件上传模式
        /// <summary>
        /// 上传模式
        /// </summary>
        public enum UploadMode
        { 
            [Description("本地文件服务器")]
            本地文件服务器 = 1,
            [Description("阿里OSS")]
            阿里OSS = 2
        }
        #endregion


        #region 项目状态码
        /// <summary>
        /// 项目状态码
        /// </summary>
        public enum Code
        {
            #region 普通操作
            [Description("操作成功")]
            操作成功 = 0,
            [Description("操作失败")]
            操作失败 = 1,
            [Description("参数异常")]
            参数异常 = 2,
            [Description("未找到对象")]
            未找到对象 = 3,
            [Description("必填字段为空")]
            必填字段为空 = 4,
            [Description("输入Json与模型要求不一致")]
            输入Json与模型要求不一致 = 5,
            [Description("已有同名数据,请避免数据重复")]
            已有同名参数 = 6,
            [Description("条件判断未通过，刷新重试")]
            条件判断未通过 = 7,
            [Description("当前对象为失效或者禁用状态")]
            当前对象失效 = 8,
            [Description("请求成功")]
            请求成功 = 9,
            [Description("请求失败")]
            请求失败 = 10,
            [Description("程序异常")]
            程序异常 = 500,
            #endregion 
        } 
        #endregion
    }
}
