using System;
using System.Collections.Generic;
using System.Text;

namespace Druk.Common
{
    public static class DoURL
    {
        #region //生成短连接
        public static string GetShortURL(string targetURL)
        {
            var result = DoWebRequest.SendRequest_Get("http://goo.gd/action/json.php?source=1681459862&url_long=" + DoWebRequest.UrlEncode(targetURL), "");
            var obj = result.ToObjectFromJson<ShortURL_Entity>();
            return obj != null ? obj.urls[0].url_short : "";
        }
        #endregion
    }

    #region //临时模型
    class ShortURL_Entity
    {
        public List<ShortURL_Entity_2> urls { get; set; }
    }
    class ShortURL_Entity_2
    {
        public bool result { get; set; }
        public string url_short { get; set; }
        public string url_long { get; set; }
        public string object_type { get; set; }
        public int type { get; set; }
        public string object_id { get; set; }
    }
    #endregion
}
