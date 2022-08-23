using Druk.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Druk.DataCache.Base
{
    public class StaticCache
    {
        /// <summary>
        /// 公用字典列表存储
        /// </summary>
        private static Dictionary<string, (string json, DateTime time)> Dic_List = new Dictionary<string, (string json, DateTime time)>();

        /// <summary>
        /// 公用字典存储对象
        /// </summary>
        private static Dictionary<string, Dictionary<int, (string json, DateTime time)>> Dic_Entity = new Dictionary<string, Dictionary<int, (string json, DateTime time)>>();


        #region //程序初始化静态缓存
        /// <summary>
        /// 程序初始化静态缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">缓存对象 List</param>
        /// <param name="cacheName">缓存名称</param>
        /// <param name="seconds">缓存秒数</param>
        internal static void SetList<T>(List<T> list, string cacheName, int seconds = 30) where T : DB.Entity.BaseEntity
        {
            //如果两个总体对象为null， 就初始化一下
            if (Dic_List == null) { Dic_List = new Dictionary<string, (string, DateTime)>(); }
            if (Dic_Entity == null) { Dic_Entity = new Dictionary<string, Dictionary<int, (string, DateTime)>>(); }

            if (!string.IsNullOrEmpty(cacheName) && list != null)
            {
                //过期时间
                var time = DateTime.Now.AddSeconds(seconds);

                //列表缓存更新
                Dic_List[cacheName] = (list.ToJson(), time);

                //实体缓存更新
                //var dic_Entity_Temp = new Dictionary<int, (string, DateTime)>();
                //foreach (var entity in list)
                //{
                //    dic_Entity_Temp.Add(entity.id, (entity.ToJson(), time));
                //}
                //Dic_Entity[CacheName] = dic_Entity_Temp;
            }
        }
        #endregion

        #region //根据缓存名称获取内容
        /// <summary>
        /// 根据缓存名称获取内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        internal static List<T> GetList<T>(string cacheName) where T : DB.Entity.BaseEntity
        {
            if (Dic_List != null)
            {
                if (Dic_List.ContainsKey(cacheName)) //包含此缓存
                {
                    var body = Dic_List[cacheName];
                    if (body.time >= DateTime.Now) //时间还有效
                    {
                        var str = body.json;
                        if (!string.IsNullOrEmpty(str))
                        {
                            return str.ToObjectFromJson<List<T>>();
                        }
                    }
                }
            }
            return null;
        }
        #endregion
    }
}
