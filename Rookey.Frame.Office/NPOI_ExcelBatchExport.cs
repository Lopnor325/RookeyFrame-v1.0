/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using NPOI.HSSF.UserModel;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Data;
using System.IO;

namespace Rookey.Frame.Office
{
    /// <summary>
    /// Excel导出分批处理
    /// </summary>
    public class NPOI_ExcelBatchExport
    {
        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="dtSource"></param>
        /// <param name="filePath"></param>
        public static void ExportExcel(DataTable dtSource, string filePath)
        {
            IWorkbook excelWorkbook = OpenOrCreateExcelFile(filePath);
            InsertRow(dtSource, excelWorkbook);
            int sheetNum = excelWorkbook.NumberOfSheets;
            for (int i = 0; i < sheetNum; i++)
            {
                ISheet sheet = excelWorkbook.GetSheetAt(i);
                NPOI_ExcelHelper.AutoSizeColumns(sheet);
            }
            SaveExcelFile(excelWorkbook, filePath);
        }

        /// <summary>
        /// 保存Excel文件
        /// </summary>
        /// <param name="excelWorkBook"></param>
        /// <param name="filePath"></param>
        protected static void SaveExcelFile(IWorkbook excelWorkBook, string filePath)
        {
            FileStream file = null;
            try
            {
                file = new FileStream(filePath, FileMode.Create);
                excelWorkBook.Write(file);
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
        }

        /// <summary>
        /// 创建Excel文件
        /// </summary>
        /// <param name="filePath"></param>
        protected static IWorkbook OpenOrCreateExcelFile(string filePath)
        {
            FileStream fs = null;
            if (!File.Exists(filePath))
            {
                string dir = Path.GetDirectoryName(filePath); //文件所有文件夹
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir); //文件所在文件夹不存在则新建
                fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                MemoryStream ms = new MemoryStream();
                IWorkbook tempWorkbook = Path.GetExtension(filePath).ToLower() == "xlsx" ? (IWorkbook)(new XSSFWorkbook()) : new HSSFWorkbook();
                ISheet sheet = tempWorkbook.CreateSheet("Sheet1");
                tempWorkbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                byte[] data = ms.ToArray();
                fs.Write(data, 0, data.Length);
                fs.Flush();
                fs.Close();
            }
            fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
            IWorkbook workbook = Path.GetExtension(filePath).ToLower() == "xlsx" ? (IWorkbook)(new XSSFWorkbook(fs)) : new HSSFWorkbook(new POIFSFileSystem(fs));
            return workbook;
        }

        /// <summary>
        /// 创建excel表头
        /// </summary>
        /// <param name="excelSheet"></param>
        /// <param name="columns"></param>
        protected static void CreateHeader(ISheet excelSheet, DataColumnCollection columns)
        {
            if (excelSheet.LastRowNum == 0)
            {
                IRow newRow = excelSheet.CreateRow(0);
                foreach (DataColumn column in columns)
                    newRow.CreateCell(column.Ordinal).SetCellValue(column.Caption);
            }
        }

        /// <summary>
        /// 插入数据行
        /// </summary>
        protected static void InsertRow(DataTable dtSource, IWorkbook excelWorkbook)
        {
            int sheetCount = 1;
            ISheet newsheet = null;
            //循环数据源导出数据集
            newsheet = excelWorkbook.GetSheetAt(excelWorkbook.NumberOfSheets - 1);
            int rowCount = newsheet.LastRowNum;
            CreateHeader(newsheet, dtSource.Columns);
            foreach (DataRow dr in dtSource.Rows)
            {
                rowCount++;
                //超出65530条数据 创建新的工作簿
                if (rowCount == 65530)
                {
                    rowCount = 1;
                    sheetCount++;
                    newsheet = excelWorkbook.CreateSheet("Sheet" + sheetCount);
                    CreateHeader(newsheet, dtSource.Columns);
                }
                IRow newRow = newsheet.CreateRow(rowCount);
                InsertCell(dtSource, dr, newRow, newsheet, excelWorkbook);
            }
        }

        /// <summary>
        /// 导出数据行
        /// </summary>
        /// <param name="dtSource"></param>
        /// <param name="drSource"></param>
        /// <param name="currentExcelRow"></param>
        /// <param name="excelSheet"></param>
        /// <param name="excelWorkBook"></param>
        protected static void InsertCell(DataTable dtSource, DataRow drSource, IRow currentExcelRow, ISheet excelSheet, IWorkbook excelWorkBook)
        {
            for (int cellIndex = 0; cellIndex < dtSource.Columns.Count; cellIndex++)
            {
                //列名称
                string columnsName = dtSource.Columns[cellIndex].ColumnName;
                ICell newCell = null;
                System.Type rowType = drSource[cellIndex].GetType();
                string drValue = drSource[cellIndex].ToString().Trim();
                switch (rowType.ToString())
                {
                    case "System.String"://字符串类型
                        drValue = drValue.Replace("&", "&");
                        drValue = drValue.Replace(">", ">");
                        drValue = drValue.Replace("<", "<");
                        newCell = currentExcelRow.CreateCell(cellIndex);
                        newCell.SetCellValue(drValue);
                        break;
                    case "System.DateTime"://日期类型
                        DateTime dateV;
                        DateTime.TryParse(drValue, out dateV);
                        newCell = currentExcelRow.CreateCell(cellIndex);
                        newCell.SetCellValue(dateV);
                        //格式化显示
                        ICellStyle cellStyle = excelWorkBook.CreateCellStyle();
                        IDataFormat format = excelWorkBook.CreateDataFormat();
                        cellStyle.DataFormat = format.GetFormat("yyyy-mm-dd hh:mm:ss");
                        newCell.CellStyle = cellStyle;
                        break;
                    case "System.Boolean"://布尔型
                        bool boolV = false;
                        bool.TryParse(drValue, out boolV);
                        newCell = currentExcelRow.CreateCell(cellIndex);
                        newCell.SetCellValue(boolV);
                        break;
                    case "System.Int16"://整型
                    case "System.Int32":
                    case "System.Int64":
                    case "System.Byte":
                        int intV = 0;
                        int.TryParse(drValue, out intV);
                        newCell = currentExcelRow.CreateCell(cellIndex);
                        newCell.SetCellValue(intV.ToString());
                        break;
                    case "System.Decimal"://浮点型
                    case "System.Double":
                        double doubV = 0;
                        double.TryParse(drValue, out doubV);
                        newCell = currentExcelRow.CreateCell(cellIndex);
                        newCell.SetCellValue(doubV);
                        break;
                    case "System.DBNull"://空值处理
                        newCell = currentExcelRow.CreateCell(cellIndex);
                        newCell.SetCellValue("");
                        break;
                    default:
                        throw (new Exception(rowType.ToString() + "：类型数据无法处理!"));
                }
            }
        }
    }
}
