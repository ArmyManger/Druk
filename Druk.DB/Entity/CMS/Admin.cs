using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Xml.Serialization;

namespace Druk.DB.Entity
{
    /// <summary>
    /// 管理员类
    /// </summary>
    [Serializable, XmlRoot(ElementName = "Admin")]
    [Table("t_Admin")]
    public class Admin : BaseEntity
    {
        public virtual string name { get; set; }

        public virtual string nickName { get; set; }

        public virtual string pwd { get; set; }

        public virtual string phone { get; set; }
    }
}
