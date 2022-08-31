using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Xml.Serialization;

namespace Druk.DB.Entity.CMS
{
    [Serializable, XmlRoot(ElementName = "Admin")]
    [Table("t_User")]
    public class User : BaseEntity
    {
    }
}
