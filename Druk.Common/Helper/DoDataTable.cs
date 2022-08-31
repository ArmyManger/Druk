using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Druk.Common
{
    public static class DoDataTable
    {
        #region //获取Dt的所有列名
        /// <summary>
        /// 获取Dt的所有列名
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <returns></returns>
        public static List<string> GetColumnNames(this DataTable dt)
        {
            var result = new List<string>();
            dt ??= new DataTable();
            //循环数据表,获取列名
            foreach (DataColumn col in dt.Columns)
            {
                result.Add(col.ColumnName.ToLower());
            }
            return result;
        }
        #endregion

        #region //DataTable <=> List<T>

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this T entity)
        {
            return new List<T>() { entity }.ToDataTable();
        }


        /// <summary>
        /// 转化一个DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this List<T> list)
        {
            //获得反射的入口
            Type type = typeof(T);
            var Props = type.GetProperties();
            var dt = new DataTable();
            //把所有的public属性加入到集合 并添加DataTable的列
            foreach (var p in Props)
            {
                Type proType = p.PropertyType;
                if (proType.IsClass && proType != typeof(string)) { dt.Columns.Add(p.Name, typeof(string)); continue; }
                if (proType.UnderlyingSystemType.ToString().ToLower().Contains("system.datetime")) { dt.Columns.Add(p.Name, typeof(DateTime)); continue; }
                if (proType.IsEnum) { dt.Columns.Add(p.Name, typeof(int)); continue; }
                dt.Columns.Add(p.Name, proType);
            }

            foreach (var item in list)
            {
                //创建一个DataRow实例
                DataRow row = dt.NewRow();
                //给row 赋值
                T o = item;
                Array.ForEach(Props,
                    p =>
                    {
                        var value = p.GetValue(o, null);
                        row[p.Name] = p.PropertyType.IsClass && p.PropertyType != typeof(string) ? (value ?? "").ToJson() : value;
                    }
                );
                //加入到DataTable
                dt.Rows.Add(row);
            }
            return dt;
        }



        /// <summary>
        /// DataTable 转换为List 集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable dt) where T : class, new()
        {

            //List<T> list = new List<T>();
            //if (dt == null || dt.Rows.Count == 0) 
            //    return list;
            //DataTableEntityBuilder<T> eblist = DataTableEntityBuilder<T>.CreateBuilder(dt.Rows[0]);
            //foreach (DataRow info in dt.Rows) 
            //    list.Add(eblist.Build(info));
            //dt.Dispose(); 
            //dt = null;
            //return list;

            var List = new List<T>();
            //创建一个属性的列表
            var prolist = new List<PropertyInfo>();
            //获取TResult的类型实例  反射的入口
            var propList = typeof(T).GetProperties();
            //获得TResult 的所有的Public 属性 并找出TResult属性和DataTable的列名称相同的属性(PropertyInfo) 并加入到属性列表 
            Array.ForEach(propList, p => { if (dt.Columns.IndexOf(p.Name) != -1) prolist.Add(p); });
            var CurrentProName = string.Empty;

            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    T entity = new T();
                    prolist.ForEach(p =>
                    {
                        CurrentProName = p.Name;
                        var value = row[p.Name];
                        if (value is System.DBNull)
                        {
                            value = "";
                            if (p.PropertyType.IsNumericType()) value = 0;
                            if (p.PropertyType == typeof(DateTime)) value = Config.DefaultDateTime;
                            if (p.PropertyType == typeof(bool)) value = false;  //先默认给 false
                            if (p.PropertyType == typeof(decimal)) value = 0m;
                        }
                        //如果该属性是实体类型
                        p.SetValue(entity, (p.PropertyType.IsClass && p.PropertyType != typeof(string) ? Newtonsoft.Json.JsonConvert.DeserializeObject(value.ToString(), p.PropertyType) : value));
                    });
                    List.Add(entity);
                }
                return List;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToSimple("表格转换对象列表时, [" + CurrentProName + "]属性报错"));
                return null;
            }
        }




        /// <summary>
        /// 将集合类转换成DataTable
        /// </summary>
        /// <param name="list">集合</param>
        /// <returns></returns>
        public static DataTable ToDataTableTow(IList list)
        {
            var result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    result.Columns.Add(pi.Name, pi.PropertyType);
                }

                foreach (object t in list)
                {
                    var tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        object obj = pi.GetValue(t, null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }

        /**/
        /// <summary>
        /// 将泛型集合类转换成DataTable
        /// </summary>
        /// <typeparam name="T">集合项类型</typeparam>
        /// <param name="list">集合</param>
        /// <returns>数据集(表)</returns>
        public static DataTable ToDataTable<T>(IList<T> list)
        {
            return ToDataTable(list, null);
        }

        /**/
        /// <summary>
        /// 将泛型集合类转换成DataTable
        /// </summary>
        /// <typeparam name="T">集合项类型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="propertyName">需要返回的列的列名</param>
        /// <returns>数据集(表)</returns>
        public static DataTable ToDataTable<T>(IList<T> list, params string[] propertyName)
        {
            var propertyNameList = new List<string>();
            if (propertyName != null)
                propertyNameList.AddRange(propertyName);

            var result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    if (propertyNameList.Count == 0)
                    {
                        result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                    else
                    {
                        if (propertyNameList.Contains(pi.Name))
                            result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                }

                foreach (T t in list)
                {
                    var tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        if (propertyNameList.Count == 0)
                        {
                            object obj = pi.GetValue(t, null);
                            tempList.Add(obj);
                        }
                        else
                        {
                            if (propertyNameList.Contains(pi.Name))
                            {
                                object obj = pi.GetValue(t, null);
                                tempList.Add(obj);
                            }
                        }
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }

        #endregion

        #region //DataRow[] List<DataRow> => DataTable
        /// <summary>
        /// 将Datarow[] 转换为表
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static DataTable ToTable(this DataRow[] Obj, params string[] ColName)
        {
            return Obj.ToList().ToTable(ColName);
        }

        /// <summary>
        /// 将List<DataRow> 转换为表
        /// </summary>
        /// <param name="obj">数据行集合</param>
        /// <param name="colName">列集合</param>
        /// <returns></returns>
        public static DataTable ToTable(this List<DataRow> obj, params string[] colName)
        {

            colName = colName.ToLower();
            if (obj.Count > 0)
            {
                var result = obj[0].Table.Clone();
                foreach (DataRow row in obj)
                {
                    result.Rows.Add(row.ItemArray);
                }
                var removeCol = new List<string>();

                if (colName.Length > 0) //如果有特意指定的列,则删除其他列,为空时输出全部
                {
                    foreach (DataColumn colInfo in result.Columns)
                    {
                        if (!colName.Contains(colInfo.ColumnName.ToLower()))
                        {
                            removeCol.Add(colInfo.ColumnName.ToLower());
                        }
                    }
                    removeCol.ForEach(o => result.Columns.Remove(o));
                }
                return result;
            }

            return new DataTable();
        }

        #endregion

        #region //DataTable  某一列输出为List<string>
        /// <summary>
        /// DataTable  某一列输出  
        /// </summary>
        /// <param name="DT"></param>
        /// <param name="ColumnName">列名称</param>
        /// <param name="IsQuChong">是否去重</param>
        /// <returns></returns>
        public static List<string> ToList_ByOneColumn(this DataTable DT, string ColumnName, bool IsQuChong = false)
        {
            if (DT != null)
            {
                if (DT.Columns.Contains(ColumnName))
                {
                    if (IsQuChong)
                    {
                        HashSet<string> hashSet = new HashSet<string>();
                        Array.ForEach(DT.Select(), row => { hashSet.Add(row[ColumnName].ToString()); });
                        return hashSet.ToList();
                    }
                    else
                    {
                        return DT.Select().Select(o => o[ColumnName].ToString()).ToList();
                    }
                }
            }
            return null;
        }

        #endregion

        #region //DataTable =>　Dictionary<string, string>

        /// <summary>
        /// 从DateTable 中挑选出两列组成键值对
        /// </summary>
        /// <param name="DT"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ToDictionary(this DataTable DT, string Key, string Value, bool IsToLower = true)
        {
            var result = new Dictionary<string, string>();
            var cols = DT.GetColumnNames();
            if (cols.Contains(Key.ToLower()) && cols.Contains(Value.ToLower()))
            {
                DT.Select().ToList().ForEach(row =>
                {
                    var Dic_key = IsToLower ? row[Key].ToString().ToLower() : row[Key].ToString();
                    if (!result.ContainsKey(Dic_key))
                    {
                        result.Add(Dic_key, row[Value].ToString());
                    }
                });
            }
            return result;
        }

        #endregion

        #region //DataTable =>　List<Dictionary<string, string>>
        /// <summary>
        /// 从DateTable转换为List
        /// </summary>
        /// <param name="DT"></param>
        /// <param name="IsToLower"></param>
        /// <returns></returns>
        public static List<Dictionary<string, string>> ToDictionaryList(this DataTable DT, bool IsToLower = true)
        {
            var result = new List<Dictionary<string, string>>();
            foreach (DataRow row in DT.Rows)
            {
                result.Add(row.ToDictionary(false));
            }
            return result;
        }

        #endregion

        #region //DataRow => Dictionary<string, string>

        public static Dictionary<string, string> ToDictionary(this DataRow Row, bool IsToLower = true)
        {
            var result = new Dictionary<string, string>();
            foreach (DataColumn col in Row.Table.Columns)
            {
                result[IsToLower ? col.ColumnName.ToLower() : col.ColumnName] = (Row[col.ColumnName] ?? "").ToString();
            }
            return result;
        }
        #endregion
    }

    public class DataTableEntityBuilder<Entity>
    {
        private static readonly MethodInfo getValueMethod = typeof(DataRow).GetMethod("get_Item", new Type[] { typeof(int) });
        private static readonly MethodInfo isDBNullMethod = typeof(DataRow).GetMethod("IsNull", new Type[] { typeof(int) });
        private delegate Entity Load(DataRow dataRecord);

        private Load handler;
        private DataTableEntityBuilder() { }

        public Entity Build(DataRow dataRecord)
        {
            return handler(dataRecord);
        }
        public static DataTableEntityBuilder<Entity> CreateBuilder(DataRow dataRecord)
        {
            DataTableEntityBuilder<Entity> dynamicBuilder = new DataTableEntityBuilder<Entity>();
            DynamicMethod method = new DynamicMethod("DynamicCreateEntity", typeof(Entity), new Type[] { typeof(DataRow) }, typeof(Entity), true);
            ILGenerator generator = method.GetILGenerator();
            LocalBuilder result = generator.DeclareLocal(typeof(Entity));
            generator.Emit(OpCodes.Newobj, typeof(Entity).GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);

            for (int i = 0; i < dataRecord.ItemArray.Length; i++)
            {
                PropertyInfo propertyInfo = typeof(Entity).GetProperty(dataRecord.Table.Columns[i].ColumnName);

                Label endIfLabel = generator.DefineLabel();
                if (propertyInfo != null && propertyInfo.GetSetMethod() != null)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, isDBNullMethod);
                    generator.Emit(OpCodes.Brtrue, endIfLabel);
                    generator.Emit(OpCodes.Ldloc, result);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, getValueMethod);
                    generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                    generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
                    generator.MarkLabel(endIfLabel);
                }
            }
            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);
            dynamicBuilder.handler = (Load)method.CreateDelegate(typeof(Load));
            return dynamicBuilder;
        }
    }
}
