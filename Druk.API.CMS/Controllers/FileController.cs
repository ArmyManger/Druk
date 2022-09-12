using Druk.API.CMS.Util;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Druk.API.CMS.Controllers
{
    /// <summary>
    /// 文件控制器
    /// </summary>
    public class FileController : BaseController
    {
        #region //上传文件
        /// <summary>
        /// 上传文件 
        /// </summary>
        /// <returns></returns>
        [Util.NoLogin]
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json", "multipart/form-data")]//此处为新增
        public object uploadImage()
        {
            var fileexeLimit = new string[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            return new Handle.Util.FileNginx().Upload(HttpContext.Request, new Handle.Util.FileConfig().Fixed_Path_Image, fileexeLimit, "Image");
        }




        #endregion

        #region //上传Excel
        /// <summary>
        /// 上传Excel
        /// </summary>
        /// <returns></returns>
        [Util.NoLogin]
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json", "multipart/form-data")]//此处为新增
        public object uploadExcel()
        {
            var fileexeLimit = new string[] { ".xls", ".xlsx" };
            return new Handle.Util.FileNginx().Upload(HttpContext.Request, new Handle.Util.FileConfig().Fixed_Path_Excel, fileexeLimit, "Excel");
        }
        #endregion
    }
}
