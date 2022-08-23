using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Druk.Common
{
    /// <summary>
    /// 导出
    /// </summary>
    public class DoExcel
    {
        #region //Excel

        #region //使用List<T>写入Excel
        /// <summary>
        /// 使用List<T>写入Excel
        /// </summary>
        public static bool Create<T>(List<T> list, string filePath, bool IsOverWrite = true, bool IsAppendTitle = true, string[] Headers = null, string fileName = "", bool isCsv = false)
        {
            if (isCsv)
            {
                return CreateCSV(list.ToDataTable<T>(), filePath, IsAppendTitle, IsOverWrite);
            }
            else
            {
                return Create(list.ToDataTable<T>(), filePath, IsOverWrite, IsAppendTitle, Headers, fileName);
            }
        }
        #endregion

        #region //使用List<T>写入Excel
        /// <summary>
        /// 使用List<T>写入Excel
        /// </summary>
        public static byte[] CreateReturnBytes<T>(List<T> list, bool IsAppendTitle = true, string[] Headers = null)
        {
            return CreateReturnBytes(list.ToDataTable<T>(), IsAppendTitle, Headers);
        }
        #endregion

        #region //从DataTable生成Excel
        /// <summary>
        /// 从DataTable生成Excel
        /// </summary>
        public static bool Create(DataTable dt, string filePath, bool IsOverWrite = true, bool IsAppendTitle = true, string[] Headers = null, string fileName = "", bool isCsv = false)
        {
            if (isCsv)
            {
                return CreateCSV(dt, filePath, IsAppendTitle, IsOverWrite);
            }
            try
            {
                dt = dt ?? new DataTable();
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                //FileInfo file = new FileInfo(DoPath.GetFullPath(filePath + fileName).TrimStart('/'));
                FileInfo file = new FileInfo(filePath + fileName);
                if (file.Exists && !IsOverWrite)
                {
                    file.Delete();
                }
                //创建Excel文件的对象
                HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
                //添加一个sheet
                ISheet sheet1 = book.CreateSheet("Sheet1");

                #region //书写标题
                if (IsAppendTitle)
                {
                    if (Headers == null || Headers.Length == 0) { Headers = dt.GetColumnNames().ToArray(); }

                    //给sheet1添加第一行的头部标题
                    IRow row1 = sheet1.CreateRow(0);

                    for (int i = 0; i < Headers.Length; i++)
                    {
                        row1.CreateCell(i).SetCellValue(Headers[i]);
                    }
                }
                #endregion

                #region //填充数据
                var k = IsAppendTitle ? 1 : 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //创建数据行
                    IRow rowtemp = sheet1.CreateRow(k);
                    rowtemp.CreateCell(0).SetCellValue((k).ToString());
                    k++;
                    //填充数据
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        var value = dt.Rows[i][j].ToString();
                        rowtemp.CreateCell(j).SetCellValue(value);
                    }
                }
                //foreach (DataRow row in dt.Rows)
                //{
                //    //创建数据行
                //    IRow rowtemp = sheet1.CreateRow(k);
                //    rowtemp.CreateCell(0).SetCellValue((k).ToString());
                //    k++;
                //    //填充数据
                //    int j = 1;
                //    foreach (var item in row.ItemArray)
                //    {
                //        rowtemp.CreateCell(j).SetCellValue(item.ToString());
                //        j++;
                //    }
                //}
                #endregion

                #region //写入文件
                using (MemoryStream ms = new MemoryStream())
                {
                    book.Write(ms);
                    ms.Seek(0, SeekOrigin.Begin);

                    using (FileStream FileStream = new FileStream(file.FullName, FileMode.Create, System.IO.FileAccess.Write))
                    {
                        byte[] bytes = new byte[ms.Length];
                        ms.Read(bytes, 0, (int)ms.Length);
                        FileStream.Write(bytes, 0, bytes.Length);
                        FileStream.Close();
                        ms.Close();
                    }
                }
                #endregion

                file.Refresh();
                return file.Exists;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
        #endregion

        #region //从DataTable生成Excel
        /// <summary>
        /// 从DataTable生成Excel
        /// </summary>
        public static byte[] CreateReturnBytes(DataTable dt, bool IsAppendTitle = true, string[] Headers = null)
        {
            try
            {

                dt = dt ?? new DataTable();
                //创建Excel文件的对象
                HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
                //添加一个sheet
                ISheet sheet1 = book.CreateSheet("Sheet1");

                #region //书写标题
                if (IsAppendTitle)
                {
                    if (Headers == null || Headers.Length == 0) { Headers = dt.GetColumnNames().ToArray(); }

                    //给sheet1添加第一行的头部标题
                    IRow row1 = sheet1.CreateRow(0);

                    for (int i = 0; i < Headers.Length; i++)
                    {
                        row1.CreateCell(i).SetCellValue(Headers[i]);
                    }
                }
                #endregion

                #region //填充数据
                var k = IsAppendTitle ? 1 : 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //创建数据行
                    IRow rowtemp = sheet1.CreateRow(k);
                    rowtemp.CreateCell(0).SetCellValue((k).ToString());
                    k++;
                    //填充数据
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        var value = dt.Rows[i][j].ToString();
                        rowtemp.CreateCell(j).SetCellValue(value);
                    }
                }
                #endregion
                #region //写入文件
                using (MemoryStream ms = new MemoryStream())
                {
                    book.Write(ms);

                    var t = ms.ToArray();
                    return t;
                }
                #endregion

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new byte[0];
            }
        }
        #endregion

        #region //保存Excel  流对象
        /// <summary>
        /// 保存图片 流对象
        /// </summary>
        /// <param name="fileUrl"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static bool SaveExcel(string fileUrl, Stream stream)
        {
            fileUrl = DoPath.GetFullPath(fileUrl);
            var file = new FileInfo(fileUrl);
            if (!file.Directory.Exists) { file.Directory.Create(); }
            if (file.Exists) { file.Delete(); }
            if (stream != null)
            {
                try
                {
                    byte[] Buf = new Byte[stream.Length];
                    stream.Read(Buf, 0, Buf.Length);
                    stream.Seek(0, SeekOrigin.Begin);
                    using (FileStream fs = new FileStream(fileUrl, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(Buf, 0, Buf.Length);
                        fs.Close();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return false;
        }
        #endregion

        #region //读取

        #region //DataSet from Excel
        /// <summary>
        /// DataSet from Excel
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="IsReadHiddenSheet"></param>
        /// <returns></returns>
        public static DataSet Read(string FilePath, bool IsReadHiddenSheet = true)
        {
            var ds = new DataSet();
            var workbook = GetExcelWorkBook(FilePath);
            if (workbook != null)
            {
                for (var i = 0; i < workbook.NumberOfSheets; i++)
                {
                    if (!IsReadHiddenSheet && workbook.IsSheetHidden(i)) { Console.WriteLine("HiddenSheet 不读取"); continue; }

                    var dt = GetDtBySheet(workbook.GetSheetAt(i), false) ?? new DataTable();

                    ds.Tables.Add(dt);
                }
            }
            return ds;
        }
        #endregion

        #region //DataTable from Excel
        /// <summary>
        /// DataTable from Excel
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="IsHasTitle"></param>
        /// <returns></returns>
        public static DataTable Read(string FilePath, bool IsHasTitle = true, int SheetIndex = 0)
        {
            var workbook = GetExcelWorkBook(FilePath);
            if (workbook != null)
            {
                return GetDtBySheet(workbook.GetSheetAt(SheetIndex), IsHasTitle);
            }

            return null;
        }
        #endregion

        #region //根据Excle文件链接读取Workbook并返回

        /// <summary>
        /// 根据Excle文件链接读取Workbook并返回
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static IWorkbook GetExcelWorkBook(string FilePath)
        {
            var file = new FileInfo(DoPath.GetFullPath(FilePath));
            if (file.Exists)
            {
                Stream fs = null;
                try
                {
                    #region //填充文件内容到IWorkbook
                    IWorkbook workbook = null;
                    using (fs = File.OpenRead(file.FullName))
                    {
                        switch (file.Extension.ToLower())
                        {
                            case ".xls": workbook = new HSSFWorkbook(fs); break; //2003版本
                            case ".xlsx": workbook = new XSSFWorkbook(fs); break; //2007以后的版本
                                                                                  //其他文件名的不管.返回null
                        }
                    }
                    return workbook;
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Close();
                        fs.Dispose();
                    }
                }
            }
            return null;
        }
        #endregion

        #region //根据Sheet填充DataTable
        /// <summary>
        /// 从Sheet中填充DataTable
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="IsHasTitle"></param>
        /// <returns></returns>
        public static DataTable GetDtBySheet(ISheet sheet, bool IsHasTitle = true)
        {

            if (sheet != null)
            {
                var dt = new DataTable();

                dt.TableName = sheet.SheetName;

                #region //为DataTable增加列
                IRow firstRow = sheet.GetRow(0);//第一行
                var ColCount = firstRow.LastCellNum;//到哪一列结束

                short FirstCellNum = firstRow.FirstCellNum; //第一行的开始列
                short LastCellNum = firstRow.LastCellNum;   //第一行的结束列
                if (IsHasTitle)
                {
                    firstRow.Cells.ForEach(cell =>
                    {
                        if (cell != null)
                        {
                            dt.Columns.Add(cell.StringCellValue ?? "Col" + cell.ColumnIndex, typeof(string));
                        }
                    });
                }
                else
                {
                    for (var i = 0; i < ColCount; i++) { dt.Columns.Add("Col" + i, typeof(string)); }
                }
                #endregion

                #region //为DataTable填充内容
                for (int i = IsHasTitle ? 1 : 0; i <= sheet.LastRowNum; i++)
                {
                    var row = sheet.GetRow(i); //当前CellRow行

                    var tempContext = new List<string>();

                    //LastCellNum是最后一列的后一列编号, 所以不是 <=
                    for (short ii = FirstCellNum; ii < LastCellNum; ii++)
                    {
                        var cell = row.GetCell(ii);
                        var text = string.Empty;
                        try
                        {
                            switch (cell.CellType)
                            {
                                case CellType.Numeric: text = cell.NumericCellValue.ToString(); break;
                                case CellType.Boolean: text = cell.BooleanCellValue.ToString(); break;
                                case CellType.Blank: text = string.Empty; break;
                                default: text = cell.StringCellValue; break;
                            }
                        }
                        catch { text = ""; }
                        tempContext.Add(text);
                    }
                    dt.Rows.Add(tempContext.ToArray());
                }
                #endregion

                #region //删除Excel读到数据的最后空行
                var RowCount = dt.Rows.Count;
                for (int i = RowCount - 1; i >= 0; i--)
                {
                    if (string.IsNullOrEmpty(string.Join("", dt.Rows[i].ItemArray)))
                    {
                        dt.Rows.RemoveAt(i); //删除该行
                    }
                    else
                    {
                        break;
                    }
                }
                #endregion

                return dt;
            }
            return null;
        }

        #endregion

        #endregion

        #endregion

        #region //Csv


        #region //使用List<T>生成CSV文件

        #region DataTable from Csv
        /// <summary>
        /// 将CSV文件的数据读取到DataTable中
        /// </summary>
        /// <param name="filepath">CSV文件路径</param>
        /// <returns>返回读取了CSV数据的DataTable</returns>
        public static DataTable OpenCSVFile(string filepath)
        {
            DataTable mycsvdt = new DataTable();
            string strpath = filepath; //csv文件的路径
            try
            {
                int intColCount = 0;
                bool blnFlag = true;

                DataColumn mydc;
                DataRow mydr;

                string strline;
                string[] aryline;
                StreamReader mysr = new StreamReader(strpath, System.Text.Encoding.Default);


                while ((strline = mysr.ReadLine()) != null)
                {
                    aryline = strToAry(strline);

                    //给datatable加上列名
                    if (blnFlag)
                    {
                        blnFlag = false;
                        intColCount = aryline.Length;
                        int col = 0;
                        for (int i = 0; i < aryline.Length; i++)
                        {
                            col = i + 1;
                            mydc = new DataColumn(col.ToString());
                            mycsvdt.Columns.Add(mydc);
                        }
                    }

                    //填充数据并加入到datatable中
                    mydr = mycsvdt.NewRow();
                    for (int i = 0; i < intColCount; i++)
                    {
                        mydr[i] = aryline[i];
                    }
                    mycsvdt.Rows.Add(mydr);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return mycsvdt;
        }

        /// <summary>
        /// 转换csv中的引号逗号
        /// </summary>
        /// <param name="strLine"></param>
        /// <returns></returns>
        private static string[] strToAry(string strLine)
        {
            string strItem = "";
            int iFenHao = 0;
            System.Collections.ArrayList lstStr = new System.Collections.ArrayList();
            for (int i = 0; i < strLine.Length; i++)
            {
                string strA = strLine.Substring(i, 1);
                if (strA == "\"")
                {
                    iFenHao = iFenHao + 1;
                }
                if (iFenHao == 2)
                {
                    iFenHao = 0;
                }
                if (strA == "," && iFenHao == 0)
                {
                    lstStr.Add(strItem);
                    strItem = "";
                }
                else
                {
                    strItem = strItem + strA;
                }
            }
            if (strItem.Length > 0)
                lstStr.Add(strItem);
            return (String[])lstStr.ToArray(typeof(string));
        }
        #endregion

        /// <summary>
        /// 使用List生成CSV文件
        /// </summary>
        public static bool CreateCSV<T>(List<T> list, string filePath, bool IsAppendTitle = true, bool IsOverWrite = true)
        {
            return CreateCSV(list.ToDataTable<T>(), filePath, IsAppendTitle, IsOverWrite);
        }
        #endregion

        #region //用datatable生成CSV文件
        /// <summary>
        /// 用datatable生成CSV文件
        /// </summary>
        public static bool CreateCSV(DataTable dt, string filePath, bool IsAppendTitle = true, bool IsOverWrite = true)
        {
            try
            {
                dt = dt ?? new DataTable();
                FileInfo file = new FileInfo(DoPath.GetFullPath(filePath));
                //文件存在并且不允许覆盖则返回true 否则删除文件
                //  if (file.Exists && !IsOverWrite) { return true; } else { file.Delete(); }

                var context = string.Empty;
                if (IsAppendTitle)
                {
                    context += GetCsvRow(dt.GetColumnNames().ToArray()) + "\n";
                    DoIOFile.Write(file.FullName, context, true, "UTF-8");
                    context = string.Empty;
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var row = dt.Rows[i];
                    context += GetCsvRow(row.ItemArray) + "\n";

                    if (context.Length >= 5000 || i == dt.Rows.Count - 1)
                    {
                        DoIOFile.Write(file.FullName, context, true, "UTF-8");
                        context = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            return true;
        }
        #endregion
        #region //Tools
        #region //将数组内容组合为双引号包裹的CSV行数据
        /// <summary>
        /// 将数组内容组合为双引号包裹的CSV行数据
        /// </summary>
        /// <param name="dataItems"></param>
        /// <returns></returns>
        static string GetCsvRow(object[] dataItems)
        {
            return string.Format("\"{0}\"", string.Join("\",\"", dataItems));
        }
        #endregion
        #endregion
        #endregion
    }
}
