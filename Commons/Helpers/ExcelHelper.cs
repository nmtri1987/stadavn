using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using System.IO;
using Microsoft.SharePoint;

namespace RBVH.Stada.Intranet.Biz.Helpers
{
    public class ExcelHelper
    {
        public static string ConvertColumnIndexToLetter(int colIdx)
        {
            int dividend = colIdx;
            string columnName = String.Empty;
            int modifier;

            while (dividend > 0)
            {
                modifier = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modifier).ToString() + columnName;
                dividend = (int)((dividend - modifier) / 26);
            }

            return columnName;
        }

        public static string GetCellValue(WorkbookPart workbookPart, string sheetName, string addressName)
        {
            WorksheetPart worksheetPart = workbookPart.GetPartById(workbookPart.Workbook.Descendants<Sheet>().Where(e => e.Name.ToString().ToUpper().Equals(sheetName.ToUpper())).First().Id.Value) as WorksheetPart;

            Cell theCell = worksheetPart.Worksheet.Descendants<Cell>().Where(c => c.CellReference == addressName).FirstOrDefault();
            string value = "";

            if (theCell != null && theCell.CellValue != null)
            {
                value = theCell.CellValue.Text;
                if (theCell.DataType != null)
                {
                    switch (theCell.DataType.Value)
                    {
                        case CellValues.SharedString:

                            var stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                            if (stringTable != null)
                            {
                                value = stringTable.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
                            }
                            break;
                        case CellValues.Boolean:
                            switch (value)
                            {
                                case "0":
                                    value = "FALSE";
                                    break;
                                default:
                                    value = "TRUE";
                                    break;
                            }
                            break;
                    }
                }
            }
            return value;
        }

        public static void RemoveRow(WorkbookPart workbookPart, string sheetName, uint rowIndex)
        {
            WorksheetPart worksheetPart = workbookPart.GetPartById(workbookPart.Workbook.Descendants<Sheet>().Where(e => e.Name.ToString().ToUpper().Equals(sheetName.ToUpper())).First().Id.Value) as WorksheetPart;
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();

            Row currentRow = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).FirstOrDefault();
            sheetData.RemoveChild(currentRow);

            IEnumerable<Row> rows = worksheetPart.Worksheet.Descendants<Row>().Where(r => r.RowIndex.Value >= rowIndex + 1);
            foreach (Row row in rows)
            {
                uint newIndex = row.RowIndex - 1;
                string curRowIndex = row.RowIndex.ToString();
                string newRowIndex = newIndex.ToString();

                foreach (Cell cell in row.Elements<Cell>())
                {
                    // Update the references for the rows cells.
                    cell.CellReference = new StringValue(cell.CellReference.Value.Replace(curRowIndex, newRowIndex));
                }
                row.RowIndex = newIndex;
            }

            //worksheet.Save();
        }

        public static void DuplicateRow(WorkbookPart workbookPart, string sheetName, uint sourceRowIndex, uint destRowIndex)
        {
            WorksheetPart worksheetPart = workbookPart.GetPartById(workbookPart.Workbook.Descendants<Sheet>().Where(e => e.Name.ToString().ToUpper().Equals(sheetName.ToUpper())).First().Id.Value) as WorksheetPart;
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row currentRow = sheetData.Elements<Row>().Where(r => r.RowIndex == sourceRowIndex).FirstOrDefault();

            Row newRow = new Row();
            newRow = currentRow.CloneNode(true) as Row;

            foreach (Cell cell in newRow.Elements<Cell>())
            {
                // Update the references for the rows cells.
                cell.CellReference = new StringValue(cell.CellReference.Value.Replace(sourceRowIndex.ToString(), destRowIndex.ToString()));
            }
            newRow.RowIndex = destRowIndex;

            IEnumerable<Row> rows = worksheetPart.Worksheet.Descendants<Row>().Where(r => r.RowIndex.Value >= destRowIndex);
            foreach (Row row in rows)
            {
                uint newIndex = row.RowIndex + 1;
                string curRowIndex = row.RowIndex.ToString();
                string newRowIndex = newIndex.ToString();

                foreach (Cell cell in row.Elements<Cell>())
                {
                    // Update the references for the rows cells.
                    cell.CellReference = new StringValue(cell.CellReference.Value.Replace(curRowIndex, newRowIndex));
                }
                row.RowIndex = newIndex;
            }

            sheetData.InsertAfter(newRow, sheetData.Elements<Row>().Where(r => r.RowIndex == (destRowIndex - 1)).FirstOrDefault());

            //worksheet.Save();
        }

        public static WorksheetPart InsertWorksheet(WorkbookPart workbookPart, string sheetName)
        {
            // Add a new worksheet part to the workbook.
            WorksheetPart newWorksheetPart = null;
            newWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            newWorksheetPart.Worksheet = new Worksheet(new SheetData());
            newWorksheetPart.Worksheet.Save();

            Sheets sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
            string relationshipId = workbookPart.GetIdOfPart(newWorksheetPart);

            // Get a unique ID for the new sheet.
            uint sheetId = 1;
            if (sheets.Elements<Sheet>().Count() > 0)
            {
                sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }

            if (string.IsNullOrEmpty(sheetName))
            {
                sheetName = "Sheet" + sheetId;
            }

            // Append the new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
            sheets.Append(sheet);
            workbookPart.Workbook.Save();

            return newWorksheetPart;
        }

        public static void InsertCellValue(WorkbookPart workbookPart, string sheetName, string column, uint row, string cellValue, CellValues dataType)
        {
            WorksheetPart worksheetPart = null;
            if (!SheetExists(workbookPart, sheetName))
            {
                // Insert a new worksheet.
                worksheetPart = InsertWorksheet(workbookPart, sheetName);
            }
            else
            {
                Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().Where(e => e.Name.ToString().ToUpper().Equals(sheetName.ToUpper())).FirstOrDefault();
                if (sheet != null)
                {
                    worksheetPart = workbookPart.GetPartById(sheet.Id.Value) as WorksheetPart;
                }
            }

            Cell cell = InsertCellInWorksheet(column, row, worksheetPart);

            cell.CellValue = new CellValue(cellValue);
            cell.DataType = new EnumValue<CellValues>(dataType);

            //// Save the new worksheet.
            //worksheetPart.Worksheet.Save();
        }

        public static void InsertSharedText(WorkbookPart workbookPart, string sheetName, string column, uint row, string text)
        {
            // Get the SharedStringTablePart. If it does not exist, create a new one.
            SharedStringTablePart shareStringPart;
            if (workbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
            {
                shareStringPart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
            }
            else
            {
                shareStringPart = workbookPart.AddNewPart<SharedStringTablePart>();
            }

            // Insert the text into the SharedStringTablePart.
            int index = InsertSharedStringItem(text, shareStringPart);

            WorksheetPart worksheetPart = null;
            if (!SheetExists(workbookPart, sheetName))
            {
                // Insert a new worksheet.
                worksheetPart = InsertWorksheet(workbookPart, sheetName);
            }
            else
            {
                Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().Where(e => e.Name.ToString().ToUpper().Equals(sheetName.ToUpper())).FirstOrDefault();
                if (sheet != null)
                {
                    worksheetPart = workbookPart.GetPartById(sheet.Id.Value) as WorksheetPart;
                }
            }

            Cell cell = InsertCellInWorksheet(column, row, worksheetPart);

            cell.CellValue = new CellValue(index.ToString());
            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

            //// Save the new worksheet.
            //worksheetPart.Worksheet.Save();
        }

        public static void Save(WorkbookPart workbookPart, string sheetName)
        {
            WorksheetPart worksheetPart = null;
            Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().Where(e => e.Name.ToString().ToUpper().Equals(sheetName.ToUpper())).FirstOrDefault();
            if (sheet != null)
            {
                worksheetPart = workbookPart.GetPartById(sheet.Id.Value) as WorksheetPart;
                worksheetPart.Worksheet.Save();
            }
        }

        public static string CopyFile(string sourcePath, string destFolder, string newFileName)
        {
            if (!File.Exists(sourcePath))
            {
                throw new Exception("File does not exist.");
            }

            string destPath = "";
            try
            {
                DirectoryInfo destDirInfo = Directory.CreateDirectory(destFolder);
                if (string.IsNullOrEmpty(newFileName))
                {
                    newFileName = string.Format("{0}-{1}.xlsx", Path.GetFileNameWithoutExtension(sourcePath), DateTime.Now.Ticks);
                }

                destPath = Path.Combine(destDirInfo.FullName, newFileName);
                File.Copy(sourcePath, destPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return destPath;
        }

        public static string DownloadFile(string webUrl, string libName, string fileName, string destFolder, string newFileName)
        {
            string destinationFilePath = "";

            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                try
                {
                    using (SPSite spSite = new SPSite(webUrl))
                    {
                        using (SPWeb spWeb = spSite.OpenWeb())
                        {
                            SPList documentList = spWeb.Lists.TryGetList(libName);
                            SPFile templateFile = spWeb.GetFile(string.Format("{0}/{1}", libName, fileName));

                            if (string.IsNullOrEmpty(newFileName))
                            {
                                newFileName = string.Format("{0}-{1}.xlsx", Path.GetFileNameWithoutExtension(fileName), DateTime.Now.Ticks);
                            }

                            destinationFilePath = Path.Combine(destFolder, newFileName);
                            byte[] binary = templateFile.OpenBinary();

                            using (FileStream fs = new FileStream(destinationFilePath, FileMode.Create))
                            {
                                using (BinaryWriter writer = new BinaryWriter(fs))
                                {
                                    writer.Write(binary);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });

            return destinationFilePath;
        }

        public static void DeleteLibraryFile(string webUrl, string libName, string fileName)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                try
                {
                    using (SPSite spSite = new SPSite(webUrl))
                    {
                        using (SPWeb spWeb = spSite.OpenWeb())
                        {
                            SPList documentList = spWeb.Lists.TryGetList(libName);
                            SPFile templateFile = spWeb.GetFile(string.Format("{0}/{1}", libName, fileName));
                            spWeb.AllowUnsafeUpdates = true;
                            templateFile.Delete();
                            spWeb.AllowUnsafeUpdates = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
        }

        #region Private Methods
        private static bool SheetExists(WorkbookPart workbookPart, string sheetName)
        {
            if (workbookPart.Workbook.Descendants<Sheet>().Where(e => e.Name.ToString().ToUpper().Equals(sheetName.ToUpper())).Count() > 0)
            {
                return true;
            }

            return false;
        }

        // Given text and a SharedStringTablePart, creates a SharedStringItem with the specified text 
        // and inserts it into the SharedStringTablePart. If the item already exists, returns its index.
        private static int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        {
            // If the part does not contain a SharedStringTable, create one.
            if (shareStringPart.SharedStringTable == null)
            {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }

                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
            shareStringPart.SharedStringTable.Save();

            return i;
        }

        // Given a column name, a row index, and a WorksheetPart, inserts a cell into the worksheet. 
        // If the cell already exists, returns it. 
        private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = columnName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else
            {
                // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                    {
                        refCell = cell;
                        break;
                    }
                }

                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                //worksheet.Save();
                return newCell;
            }
        }

        public static void RemoveOldFiles(string folderPath, int numOfDiffDay)
        {
            try
            {
                DirectoryInfo info = new DirectoryInfo(folderPath);
                FileInfo[] files = info.GetFiles();

                foreach (FileInfo file in files)
                {
                    TimeSpan ts = DateTime.Now.Subtract(file.CreationTime);
                    if (ts.Days >= numOfDiffDay)
                    {
                        file.Delete();
                    }
                }
            }
            catch { }
        }
        #endregion
    }
}
