using System;
using System.Collections.Generic;
using System.Text;

namespace Druk.Common
{
    [Serializable]
    public class JsonR
    {
        #region //JsonR_Simple
        /// <summary>
        /// 页面数据传输过程中,用于存储外键对象名称
        /// </summary>
        [Serializable]
        public class JsonR_Simple
        {
            public JsonR_Simple() { id = 0; name = string.Empty; }
            public JsonR_Simple(int i, string n)
            {
                id = i;
                name = n;
            }
            /// <summary>
            /// ID
            /// </summary>
            public int id { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            public string name { get; set; }
        }

        #endregion

        #region //JsonR_SimpleCode
        [Serializable]
        public class JsonR_SimpleCode : JsonR_Simple
        {

            public string codeNo { get; set; }
        }
        #endregion

        #region //JsonR_KeyValue
        /// <summary>
        /// 页面数据传输过程中,用于储存键值对
        /// </summary>
        [Serializable]
        public class JsonR_KeyValue
        {
            public JsonR_KeyValue() { id = string.Empty; name = string.Empty; }
            public JsonR_KeyValue(string i, string n)
            {
                id = i;
                name = n;
            }
            /// <summary>
            /// ID
            /// </summary> 
            public string id { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            public string name { get; set; }
        }

        #endregion
    }
}
