using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Xml.Serialization;

namespace Druk.DB.Entity
{ 
    /// <summary>
    /// Sys_Params=系统参数配置表 实体定义类
    /// </summary>
    [Serializable, XmlRoot(ElementName = "SysParams")]
    [Table("Sys_Params")]
    public class Sys_Params : BaseEntity
    { 
        /// <summary>
        /// 类型，1文本，2富文本
        /// </summary>
        public virtual int type { get; set; }
        /// <summary>
        /// 参数名称
        /// </summary>
        public virtual string title { get; set; }

        /// <summary>
        /// 参数key
        /// </summary>
		public virtual string pName { get; set; } 
        /// <summary>
        /// 参数值
        /// </summary>
		public virtual string pValue { get; set; }
    }
}
