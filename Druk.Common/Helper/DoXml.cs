using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Druk.Common.Helper
{
    public static class DoXml
    {
        /// <summary>
        /// 创建XML文档
        /// </summary>
        /// <param name="name">根节点名称</param>
        /// <param name="type">根节点的一个属性值</param>
        /// <returns></returns>
        /// moss中调用方法：创建的文件如果要存到moss的文档库中,则：
        ///          XmlDocument doc = XmlOperate.CreateXmlDocument("project", "T");
        ///            在此可嵌入增加子节点方法,如AddTaskNode(taskObj, ref doc); ..
        ///          byte[] fileContent = Encoding.UTF8.GetBytes(doc.OuterXml);
        ///          folder.Files.Add("name.xml", fileContent, true);
        ///          web.Update();
        /// .net中调用方法：写入文件中,则：
        ///          document = XmlOperate.CreateXmlDocument("sex", "sexy");
        ///          document.Save("c:/bookstore.xml");         
        public static XmlDocument CreateXmlDocument(string name, string type)
        {
            XmlDocument doc = null;
            XmlElement rootEle = null;
            try
            {
                doc = new XmlDocument();
                doc.LoadXml("<" + name + "/>");
                rootEle = doc.DocumentElement;
                rootEle.SetAttribute("type", type);
            }
            catch (Exception er)
            {
                throw er;
            }
            return doc;
        }


        /// <summary>
        /// 获取Xml结果中对应节点的值
        /// </summary>
        ///  <param name="_resultXml"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public static string GetXmlValue(this XDocument _resultXml, string nodeName)
        {
            if (_resultXml == null || _resultXml.Element("xml") == null
                || _resultXml.Element("xml").Element(nodeName) == null)
            {
                return "";
            }
            return _resultXml.Element("xml").Element(nodeName).Value;
        }

        /// <summary>
        /// 在根节点下增加子元素
        /// </summary>
        /// <param name="document"></param>
        /// <param name="nodeName"></param>
        /// <param name="type"></param>
        /// 调用方法：
        ///      document = xmloper.CreateXmlDocument("animal", "carnivore");
        ///      XmlOperate.AddNewNode1(ref document, "carnivore", "high");
        public static void AddNewNode1(ref XmlDocument document, string nodeName, string type)
        {
            XmlElement taskEle = null;
            try
            {
                taskEle = document.CreateElement(nodeName);
                taskEle.SetAttribute("type", type);
                document.DocumentElement.AppendChild((XmlNode)taskEle);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 在元素下增加子元素
        /// </summary>
        /// <param name="element"></param>
        /// <param name="nodeName"></param>
        /// <param name="type"></param>
        /// 调用方法：
        ///       XmlDocument document = new XmlDocument();
        ///       先取到相应的元素,然后调用该方法在该元素下增加子元素
        ///       XmlElement root = (XmlElement)document.SelectSingleNode("//animal/third");
        ///       XmlOperate.AddNewNode2(ref root,"thaw","boost investor confidence");
        ///注意上面的"//animal/third"也可换成"workflow[@type='T' and @isSpecial='1']"这种形式用来获取带有相应属性的元素
        public static void AddNewNode2(ref XmlElement element, string nodeName, string type)
        {
            XmlElement taskEle = null;
            try
            {
                taskEle = element.OwnerDocument.CreateElement(nodeName);
                taskEle.SetAttribute("type", type);
                element.AppendChild((XmlNode)taskEle);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 获取类型为制定值的一组节点
        /// </summary>
        /// <param name="type">类型值。E.G.workflow[@type='T' and @isSpecial='1']</param>
        /// <returns></returns>
        /// 注意：返回的 XmlNodeList 类型是个类似于arraylist的类型,所以要得到它的值只能遍历
        public static XmlNodeList GetDesiredNode(string type)
        {
            XmlDocument document = new XmlDocument();
            return document.SelectNodes("type");
        }


        /// <summary>
        /// 抓取网页上的xml文档赋值给XmlDocument对象
        /// </summary>
        /// <param name="url">网页的url（网页的内容必须是xml格式的）</param>
        /// <returns></returns>
        public static XmlDocument GetXMLDocumentFromWebPage(string url)
        {
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);

            myRequest.Method = "POST";
            myRequest.ContentType = "application/x-www-form-urlencoded";
            myRequest.ContentLength = 0;

            // Get response 
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.Default);
            string content = reader.ReadToEnd();

            XmlDocument document = new XmlDocument();
            document.LoadXml(content);
            return document;
        }

        /// <summary>
        /// 获取服务器上指定文件的xml文件内容
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static string GetXMLFile(string location)
        {
            XmlDocument document = new XmlDocument();
            document.Load(@"G:\ttt.xml");
            return document.InnerXml;
        }

        //获取sharepoint中指定文件的内容
        //public string GetProjectWorkflowUrl(SPFile file)
        //{      
        //    byte[] content = file.OpenBinary();
        //    contentStr = Encoding.UTF8.GetString(content);
        //    return contentStr;       
        //}

        //#region GetDataSetByXml

        ///// <summary>
        ///// 读取xml直接返回DataSet 
        ///// </summary>
        ///// <param name="strXmlPath">xml文件相对路径</param>
        ///// <returns></returns>
        //public static DataSet GetDataSetByXml(string strXmlPath)
        //{
        //    try
        //    {
        //        DataSet ds = new DataSet();
        //        ds.ReadXml(GetXmlFullPath(strXmlPath));
        //        if (ds.Tables.Count > 0)
        //        {
        //            return ds;
        //        }
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //        throw ex;                
        //    }
        //}
        //#endregion

        //#region GetDataViewByXml
        ///// <summary>
        ///// 读取Xml返回一个经排序或筛选后的DataView 
        ///// </summary>
        ///// <param name="strXmlPath">xml文件相对路径</param>
        ///// <param name="strWhere">筛选条件,如："name = ＇kgdiwss＇"</param>
        ///// <param name="strSort">排序条件,如："Id desc"</param>
        ///// <returns></returns>        
        //public static DataView GetDataViewByXml(string strXmlPath, string strWhere, string strSort)
        //{
        //    try
        //    {
        //        DataSet ds = new DataSet();
        //        ds.ReadXml(GetXmlFullPath(strXmlPath));
        //        DataView dv = new DataView(ds.Tables[0]);
        //        if (strSort != null)
        //        {
        //            dv.Sort = strSort;
        //        }
        //        if (strWhere != null)
        //        {
        //            dv.RowFilter = strWhere;
        //        }
        //        return dv;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}
        //#endregion

        //#region WriteXmlByDataSet
        ///// 
        ///// 向Xml文件插入一行数据 
        ///// 
        ///// xml文件相对路径 
        ///// 要插入行的列名数组,如：string[] Columns = {"name","IsMarried"}; 
        ///// 要插入行每列的值数组,如：string[] ColumnValue={"明天去要饭","false"}; 
        ///// 成功返回true,否则返回false 
        //public static bool WriteXmlByDataSet(string strXmlPath, string[] Columns, string[] ColumnValue)
        //{
        //    try
        //    {
        //        //根据传入的XML路径得到.XSD的路径,两个文件放在同一个目录下 
        //        string strXsdPath = strXmlPath.Substring(0, strXmlPath.IndexOf(".")) + ".xsd";
        //        DataSet ds = new DataSet();
        //        //读xml架构,关系到列的数据类型 
        //        ds.ReadXmlSchema(GetXmlFullPath(strXsdPath));
        //        ds.ReadXml(GetXmlFullPath(strXmlPath));
        //        DataTable dt = ds.Tables[0];
        //        //在原来的表格基础上创建新行 
        //        DataRow newRow = dt.NewRow();
        //        //循环给一行中的各个列赋值 
        //        for (int i = 0; i < Columns.Length; i++)
        //        {
        //            newRow[Columns[i]] = ColumnValue[i];
        //        }
        //        dt.Rows.Add(newRow);
        //        dt.AcceptChanges();
        //        ds.AcceptChanges();
        //        ds.WriteXml(GetXmlFullPath(strXmlPath));
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
        //#endregion

        //#region UpdateXmlRow
        ///// 
        ///// 更行符合条件的一条Xml记录 
        ///// 
        ///// XML文件路径 
        ///// 列名数组 
        ///// 列值数组 
        ///// 条件列名 
        ///// 条件列值 
        ///// 
        //public static bool UpdateXmlRow(string strXmlPath, string[] Columns, string[] ColumnValue, string strWhereColumnName, string strWhereColumnValue)
        //{
        //    try
        //    {
        //        string strXsdPath = strXmlPath.Substring(0, strXmlPath.IndexOf(".")) + ".xsd";
        //        DataSet ds = new DataSet();
        //        //读xml架构,关系到列的数据类型 
        //        ds.ReadXmlSchema(GetXmlFullPath(strXsdPath));
        //        ds.ReadXml(GetXmlFullPath(strXmlPath));
        //        //先判断行数 
        //        if (ds.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //            {
        //                //如果当前记录为符合Where条件的记录 
        //                if (ds.Tables[0].Rows[i][strWhereColumnName].ToString().Trim().Equals(strWhereColumnValue))
        //                {
        //                    //循环给找到行的各列赋新值 
        //                    for (int j = 0; j < Columns.Length; j++)
        //                    {
        //                        ds.Tables[0].Rows[i][Columns[j]] = ColumnValue[j];
        //                    }
        //                    //更新DataSet 
        //                    ds.AcceptChanges();
        //                    //重新写入XML文件 
        //                    ds.WriteXml(GetXmlFullPath(strXmlPath));
        //                    return true;
        //                }
        //            }
        //        }
        //        return false;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
        //#endregion

        //#region DeleteXmlRowByIndex
        ///// 
        ///// 通过删除DataSet中iDeleteRow这一行,然后重写Xml以实现删除指定行 
        ///// 
        ///// 
        ///// 要删除的行在DataSet中的Index值 
        //public static bool DeleteXmlRowByIndex(string strXmlPath, int iDeleteRow)
        //{
        //    try
        //    {
        //        DataSet ds = new DataSet();
        //        ds.ReadXml(GetXmlFullPath(strXmlPath));
        //        if (ds.Tables[0].Rows.Count > 0)
        //        {
        //            //删除符号条件的行 
        //            ds.Tables[0].Rows[iDeleteRow].Delete();
        //        }
        //        ds.WriteXml(GetXmlFullPath(strXmlPath));
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
        //#endregion

        //#region DeleteXmlRows
        ///// 
        ///// 删除strColumn列中值为ColumnValue的行 
        ///// 
        ///// xml相对路径 
        ///// 列名 
        ///// strColumn列中值为ColumnValue的行均会被删除 
        ///// 
        //public static bool DeleteXmlRows(string strXmlPath, string strColumn, string[] ColumnValue)
        //{
        //    try
        //    {
        //        DataSet ds = new DataSet();
        //        ds.ReadXml(GetXmlFullPath(strXmlPath));
        //        //先判断行数 
        //        if (ds.Tables[0].Rows.Count > 0)
        //        {
        //            //判断行多还是删除的值多,多的for循环放在里面 
        //            if (ColumnValue.Length > ds.Tables[0].Rows.Count)
        //            {
        //                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //                {
        //                    for (int j = 0; j < ColumnValue.Length; j++)
        //                    {
        //                        if (ds.Tables[0].Rows[i][strColumn].ToString().Trim().Equals(ColumnValue[j]))
        //                        {
        //                            ds.Tables[0].Rows[i].Delete();
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                for (int j = 0; j < ColumnValue.Length; j++)
        //                {
        //                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //                    {
        //                        if (ds.Tables[0].Rows[i][strColumn].ToString().Trim().Equals(ColumnValue[j]))
        //                        {
        //                            ds.Tables[0].Rows[i].Delete();
        //                        }
        //                    }
        //                }
        //            }
        //            ds.WriteXml(GetXmlFullPath(strXmlPath));
        //        }
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
        //#endregion

        //#region DeleteXmlAllRows
        ///// 
        ///// 删除所有行 
        ///// 
        ///// XML路径 
        ///// 
        //public static bool DeleteXmlAllRows(string strXmlPath)
        //{
        //    try
        //    {
        //        DataSet ds = new DataSet();
        //        ds.ReadXml(GetXmlFullPath(strXmlPath));
        //        //如果记录条数大于0 
        //        if (ds.Tables[0].Rows.Count > 0)
        //        {
        //            //移除所有记录 
        //            ds.Tables[0].Rows.Clear();
        //        }
        //        //重新写入,这时XML文件中就只剩根节点了 
        //        ds.WriteXml(GetXmlFullPath(strXmlPath));
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
        //#endregion

        //#region GetXmlFullPath
        ///// 
        ///// 返回完整路径 
        ///// 
        ///// Xml的路径 
        ///// 
        //public static string GetXmlFullPath(string strPath)
        //{
        //    //如果路径中含有:符号,则认定为传入的是完整路径 
        //    if (strPath.IndexOf(":") > 0)
        //    {
        //        return strPath;
        //    }
        //    else
        //    {
        //        //返回完整路径 
        //        return System.Web.HttpContext.Current.Server.MapPath(strPath);
        //    }
        //}
        //#endregion

        //#region //WebConfig

        ///// <summary>
        ///// 根据key读取WebConfig配置文件ConnectionStrings
        ///// </summary>
        ///// <param name="key">key</param>
        ///// <returns>value</returns>
        //public static string ReadWebConfigConnectionStrings(string key)
        //{
        //    try
        //    {
        //        string value = ConfigurationManager.ConnectionStrings[key].ConnectionString;
        //        if (null != value)
        //        {
        //            return value;
        //        }
        //        else
        //        {
        //            return "";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return "";
        //        throw ex;
        //    }

        //}
        ///// <summary>
        ///// 根据key读取WebConfig配置文件AppSettings
        ///// </summary>
        ///// <param name="key">key</param>
        ///// <returns>value</returns>
        //public static string ReadWebConfigAppSettings(string key)
        //{
        //    try
        //    {
        //        string value = System.Configuration.ConfigurationManager.AppSettings[key];
        //        if (null != value)
        //        {
        //            return value;
        //        }
        //        else
        //        {
        //            return "";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return "";
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// 根据key读取WebConfig配置文件AppSettings
        ///// </summary>
        ///// <param name="key">key</param>
        ///// <returns>value</returns>
        //public static int ReadWebConfigAppSettingsToInt(string key, int defval)
        //{
        //    try
        //    {
        //        string value = System.Configuration.ConfigurationManager.AppSettings[key];
        //        if (null != value)
        //        {
        //            return Convert.ToInt32(value);
        //        }
        //        else
        //        {
        //            return defval;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return defval;
        //        throw ex;
        //    }
        //}

        //#endregion



        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性名，非空时返回该属性值，否则返回串联值</param>
        /// <returns>string</returns>
        /**************************************************
         * 使用示列:
         * XmlHelper.Read(path, "/Node", "")
         * XmlHelper.Read(path, "/Node/Element[@Attribute='Name']", "Attribute")
         ************************************************/
        public static string Read(string path, string node, string attribute)
        {
            string value = "";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                value = (attribute.Equals("") ? xn.InnerText : xn.Attributes[attribute].Value);
            }
            catch { }
            return value;
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">节点</param>
        /// <param name="element">元素名，非空时插入新元素，否则在该元素中插入属性</param>
        /// <param name="attribute">属性名，非空时插入该元素属性值，否则插入元素值</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        /**************************************************
         * 使用示列:
         * XmlHelper.Insert(path, "/Node", "Element", "", "Value")
         * XmlHelper.Insert(path, "/Node", "Element", "Attribute", "Value")
         * XmlHelper.Insert(path, "/Node", "", "Attribute", "Value")
         ************************************************/
        public static void Insert(string path, string node, string element, string attribute, string value)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                if (element.Equals(""))
                {
                    if (!attribute.Equals(""))
                    {
                        XmlElement xe = (XmlElement)xn;
                        xe.SetAttribute(attribute, value);
                    }
                }
                else
                {
                    XmlElement xe = doc.CreateElement(element);
                    if (attribute.Equals(""))
                        xe.InnerText = value;
                    else
                        xe.SetAttribute(attribute, value);
                    xn.AppendChild(xe);
                }
                doc.Save(path);
            }
            catch { }
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性名，非空时修改该节点属性值，否则修改节点值</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        /**************************************************
         * 使用示列:
         * XmlHelper.Insert(path, "/Node", "", "Value")
         * XmlHelper.Insert(path, "/Node", "Attribute", "Value")
         ************************************************/
        public static void Update(string path, string node, string attribute, string value)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                XmlElement xe = (XmlElement)xn;
                if (attribute.Equals(""))
                    xe.InnerText = value;
                else
                    xe.SetAttribute(attribute, value);
                doc.Save(path);
            }
            catch { }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性名，非空时删除该节点属性值，否则删除节点值</param>
        /// <returns></returns>
        /**************************************************
         * 使用示列:
         * XmlHelper.Delete(path, "/Node", "")
         * XmlHelper.Delete(path, "/Node", "Attribute")
         ************************************************/
        public static void Delete(string path, string node, string attribute)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                XmlElement xe = (XmlElement)xn;
                if (attribute.Equals(""))
                    xn.ParentNode.RemoveChild(xn);
                else
                    xe.RemoveAttribute(attribute);
                doc.Save(path);
            }
            catch { }
        }

    }
}
