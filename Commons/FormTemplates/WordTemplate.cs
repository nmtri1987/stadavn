using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office2010.Word;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.SharePoint;
using RBVH.Core.SharePoint;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RBVH.Stada.Intranet.Biz.FormTemplates
{
    /// <summary>
    /// WordTemplate
    /// </summary>
    public class WordTemplate
    {
        #region Constatns

        /// <summary>
        /// F0FE
        /// </summary>
        public const string CheckedSymbolChar = "F0FE";

        /// <summary>
        /// 
        /// </summary>
        public const string UncheckedSymbolChar = "F06F";

        #endregion

        #region Properties
        public SPWeb CurrentWeb { get; set; }
        #endregion

        #region Constructors
        public WordTemplate(SPWeb currentWeb)
        {
            this.CurrentWeb = currentWeb;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Create Dictionary object from properties of source object.
        /// </summary>
        /// <param name="entityObject">The Dictionary object.</param>
        /// <returns>The Dictionary object.</returns>
        private Dictionary<string, object> GetDictionaryFromObject(object entityObject)
        {
            if (entityObject == null)
            {
                return new Dictionary<string, object>();
            }
            var typeInfo = entityObject.GetType();
            var listOfProperties = typeInfo.GetProperties();
            var result = new Dictionary<string, object>();
            foreach (var property in listOfProperties)
            {
                result.Add(property.Name, property.GetValue(entityObject, new object[] { }));
            }

            return result;
        }

        /// <summary>
        /// Create array string from properties of source object.
        /// </summary>
        /// <param name="entityObject">The Dictionary object.</param>
        /// <returns>The array string.</returns>
        private string[] GetStringValuesFromObject(object entityObject)
        {
            string[] stringValues = null;

            if (entityObject != null)
            {
                var typeInfo = entityObject.GetType();
                var arrayProperties = typeInfo.GetProperties();
                if (arrayProperties != null && arrayProperties.Length > 0)
                {
                    stringValues = new string[arrayProperties.Length];
                    for (int i = 0; i < arrayProperties.Length; i++)
                    {
                        var objectValue = arrayProperties[i].GetValue(entityObject, new object[] { });
                        stringValues[i] = objectValue != null ? objectValue.ToString() : string.Empty;
                    }
                }
            }

            return stringValues;
        }

        /// <summary>
        /// Fill data object into word file that has arrBytesSource data.
        /// </summary>
        /// <param name="dataInputObject"></param>
        /// <param name="arrBytesSource"></param>
        /// <returns>The MemoryStream object.</returns>
        public Stream FillDataObject(object dataInputObject, byte[] arrBytesSource)
        {
            MemoryStream memoryStream = null;

            try
            {
                if (dataInputObject != null)
                {
                    if (arrBytesSource != null && arrBytesSource.Length > 0)
                    {
                        #region Fix bug TFS #1971
                        //memoryStream = new MemoryStream(arrBytesSource, true);
                        memoryStream = new MemoryStream();
                        memoryStream.Write(arrBytesSource, 0, arrBytesSource.Length);
                        memoryStream.Position = 0;
                        #endregion

                        // Use OpenXML to process
                        var dictionary = GetDictionaryFromObject(dataInputObject);
                        using (WordprocessingDocument word = WordprocessingDocument.Open(memoryStream, true))
                        {
                            var part = word.MainDocumentPart;
                            var elements = part.Document.Descendants<SdtElement>().ToList();
                            foreach (SdtElement element in elements)
                            {
                                SdtAlias alias = element.Descendants<SdtAlias>().FirstOrDefault();
                                if (alias != null)
                                {
                                    // Get title of content control
                                    var title = alias.Val.Value;
                                    if (dictionary.ContainsKey(title))
                                    {
                                        object value = dictionary[title];

                                        if (value != null)
                                        {
                                            if (element.ToString().Equals("DocumentFormat.OpenXml.Wordprocessing.SdtRun"))
                                            {
                                                SdtRun run = element as SdtRun;
                                                if (run != null)
                                                {
                                                    if (value is bool)
                                                    {
                                                        SdtContentCheckBox contentCheckBox = run.Descendants<SdtContentCheckBox>().FirstOrDefault();
                                                        if (contentCheckBox != null)
                                                        {
                                                            if (string.Compare(value.ToString(), Boolean.TrueString, true) == 0)
                                                            {
                                                                contentCheckBox.Checked.Val = string.Compare(value.ToString(), Boolean.TrueString, true) == 0 ? OnOffValues.One : OnOffValues.Zero;
                                                                SdtContentRun contentRun = run.Descendants<SdtContentRun>().FirstOrDefault();
                                                                if (contentRun != null)
                                                                {
                                                                    Run xRun = contentRun.Descendants<Run>().FirstOrDefault();
                                                                    if (xRun != null)
                                                                    {
                                                                        SymbolChar checkedSymbolChar = xRun.Descendants<SymbolChar>().FirstOrDefault();
                                                                        if (checkedSymbolChar != null)
                                                                        {
                                                                            checkedSymbolChar.Char = new HexBinaryValue(CheckedSymbolChar);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        SdtContentRun contentRun = run.Descendants<SdtContentRun>().FirstOrDefault();
                                                        Run xRun = contentRun.Descendants<Run>().FirstOrDefault();
                                                        if (xRun == null)
                                                        {
                                                            contentRun.AppendChild(new Run());
                                                            xRun = contentRun.Descendants<Run>().FirstOrDefault();
                                                        }
                                                        Text text = xRun.Descendants<Text>().FirstOrDefault();
                                                        if (text == null)
                                                        {
                                                            xRun.AppendChild(new Text(value.ToString()));
                                                            text = xRun.Descendants<Text>().FirstOrDefault();
                                                        }
                                                        text.Text = value.ToString();
                                                    }
                                                }
                                            }
                                            else if (element.ToString().Equals("DocumentFormat.OpenXml.Wordprocessing.SdtBlock"))
                                            {
                                                SdtBlock block = element as SdtBlock;
                                                if (block != null)
                                                {
                                                    if (value is bool)
                                                    {
                                                        SdtContentCheckBox contentCheckBox = block.Descendants<SdtContentCheckBox>().FirstOrDefault();
                                                        if (contentCheckBox != null)
                                                        {
                                                            if (string.Compare(value.ToString(), Boolean.TrueString, true) == 0)
                                                            {
                                                                contentCheckBox.Checked.Val = string.Compare(value.ToString(), Boolean.TrueString, true) == 0 ? OnOffValues.One : OnOffValues.Zero;
                                                                SdtContentRun contentRun = block.Descendants<SdtContentRun>().FirstOrDefault();
                                                                if (contentRun != null)
                                                                {
                                                                    Run xRun = contentRun.Descendants<Run>().FirstOrDefault();
                                                                    if (xRun != null)
                                                                    {
                                                                        SymbolChar checkedSymbolChar = xRun.Descendants<SymbolChar>().FirstOrDefault();
                                                                        if (checkedSymbolChar != null)
                                                                        {
                                                                            checkedSymbolChar.Char = new HexBinaryValue(CheckedSymbolChar);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        SdtContentBlock contentBlock = block.Descendants<SdtContentBlock>().FirstOrDefault();
                                                        Run xRun = contentBlock.Descendants<Run>().FirstOrDefault();
                                                        if (xRun == null)
                                                        {
                                                            contentBlock.AppendChild(new Run());
                                                            xRun = contentBlock.Descendants<Run>().FirstOrDefault();
                                                        }
                                                        Text text = xRun.Descendants<Text>().FirstOrDefault();
                                                        if (text == null)
                                                        {
                                                            xRun.AppendChild(new Text(value.ToString()));
                                                            text = xRun.Descendants<Text>().FirstOrDefault();
                                                        }
                                                        text.Text = value.ToString();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            part.Document.Save();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                throw ex;
            }

            return memoryStream;
        }

        /// <summary>
        /// FillDataObject
        /// </summary>
        /// <param name="dataInputObject"></param>
        /// <param name="stream"></param>
        public void FillDataObject(object dataInputObject, Stream stream)
        {
            if (dataInputObject != null)
            {
                if (stream != null && stream.CanRead && stream.CanWrite)
                {
                    // Use OpenXML to process
                    var dictionary = GetDictionaryFromObject(dataInputObject);
                    using (WordprocessingDocument word = WordprocessingDocument.Open(stream, true))
                    {
                        var part = word.MainDocumentPart;
                        var elements = part.Document.Descendants<SdtElement>().ToList();
                        foreach (SdtElement element in elements)
                        {
                            SdtAlias alias = element.Descendants<SdtAlias>().FirstOrDefault();
                            if (alias != null)
                            {
                                // Get title of content control
                                var title = alias.Val.Value;
                                if (dictionary.ContainsKey(title))
                                {
                                    object value = dictionary[title];
                                    if (element.ToString().Equals("DocumentFormat.OpenXml.Wordprocessing.SdtRun"))
                                    {
                                        SdtRun run = element as SdtRun;
                                        if (run != null)
                                        {
                                            SdtContentRun contentRun = run.Descendants<SdtContentRun>().FirstOrDefault();
                                            Run xRun = contentRun.Descendants<Run>().FirstOrDefault();
                                            if (xRun == null)
                                            {
                                                contentRun.AppendChild(new Run());
                                                xRun = contentRun.Descendants<Run>().FirstOrDefault();
                                            }
                                            Text text = xRun.Descendants<Text>().FirstOrDefault();
                                            if (text == null)
                                            {
                                                xRun.AppendChild(new Text(value.ToString()));
                                                text = xRun.Descendants<Text>().FirstOrDefault();
                                            }
                                            text.Text = value.ToString();
                                        }
                                    }
                                    else if (element.ToString().Equals("DocumentFormat.OpenXml.Wordprocessing.SdtBlock"))
                                    {
                                        SdtBlock block = element as SdtBlock;
                                        if (block != null)
                                        {
                                            SdtContentBlock contentBlock = block.Descendants<SdtContentBlock>().FirstOrDefault();
                                            Run xRun = contentBlock.Descendants<Run>().FirstOrDefault();
                                            if (xRun == null)
                                            {
                                                contentBlock.AppendChild(new Run());
                                                xRun = contentBlock.Descendants<Run>().FirstOrDefault();
                                            }
                                            Text text = xRun.Descendants<Text>().FirstOrDefault();
                                            if (text == null)
                                            {
                                                xRun.AppendChild(new Text(value.ToString()));
                                                text = xRun.Descendants<Text>().FirstOrDefault();
                                            }
                                            text.Text = value.ToString();
                                        }
                                    }
                                }
                            }
                        }

                        part.Document.Save();
                    }
                }
            }
        }

        /// <summary>
        /// Fill data from lists of items into sourceStream with tableName tag and list of column names.
        /// </summary>
        /// <param name="items">The list of data items that need fill into sourceStream.</param>
        /// <param name="sourceStream">The stream source that is needed to fill.</param>
        /// <param name="tableName">The name of control that contrain for data table.</param>
        /// <param name="columns">The list of column name.</param>
        public void FillDataListOfObject(IEnumerable items, Stream sourceStream, string tableName, string[] columns, string[] widths = null)
        {
            if (items != null)
            {
                if (sourceStream != null)
                {
                    //use OpenXml to process
                    using (WordprocessingDocument word = WordprocessingDocument.Open(sourceStream, true))
                    {
                        var part = word.MainDocumentPart;
                        // SdtBlock block = part.Document.Body.Descendants<SdtBlock>().Where(p => p.SdtProperties.GetFirstChild<Tag>().Val == tableName).FirstOrDefault();
                        SdtElement tableElement = part.Document.Body.Descendants<SdtBlock>().Where(p => p.SdtProperties.GetFirstChild<Tag>().Val == tableName).FirstOrDefault();
                        if (tableElement == null)
                        {
                            tableElement = part.Document.Body.Descendants<SdtRun>().Where(p => p.SdtProperties.GetFirstChild<Tag>().Val == tableName).FirstOrDefault();
                        }

                        if (tableElement != null)
                        {
                            var table = NewTable(columns, widths);

                            #region Fill data
                            foreach (var item in items)
                            {
                                TableRow dataRow = NewEmptyRow();

                                string[] stringValues = GetStringValuesFromObject(item);
                                if (widths != null)
                                {
                                    for (var i = 0; i < columns.Length; i++)
                                    {
                                        var value = stringValues[i];
                                        //create cell object and its properties
                                        TableCell tableCellData = NewTableCellData(value, widths[i]);
                                        // add to row
                                        dataRow.Append(tableCellData);
                                    }
                                }
                                else
                                {
                                    for (var i = 0; i < columns.Length; i++)
                                    {
                                        var value = stringValues[i];
                                        //create cell object and its properties
                                        TableCell tableCellData = NewTableCellData(value);
                                        // add to row
                                        dataRow.Append(tableCellData);
                                    }
                                }
                                
                                table.Append(dataRow);
                            }
                            #endregion

                            tableElement.RemoveAllChildren();
                            tableElement.Append(table);
                            part.Document.Save();
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Initialize new cell header object.
        /// </summary>
        /// <param name="textValue">The content of cell.</param>
        /// <returns>The TableCell object.</returns>
        public virtual TableCell NewTableCellHeader(string textValue, string width = "")
        {
            //create cell object and its properties
            TableCell tableCell = new TableCell();

            TableCellProperties tableCellProperties = new TableCellProperties();
            //ConditionalFormatStyle conditionalFormatStyle = new ConditionalFormatStyle() { Val = "001000000000" };
            if (!string.IsNullOrEmpty(width))
            {
                TableCellWidth tableCellWidth = new TableCellWidth() { Width = width, Type = TableWidthUnitValues.Dxa };
                tableCellProperties.Append(tableCellWidth);
            }

            //tableCellProperties.Append(hideMark);

            //create paragrpah object and its properties
            Paragraph paragraph = new Paragraph();

            ParagraphProperties paragraphProperties = new ParagraphProperties();
            Justification justification = new Justification() { Val = JustificationValues.Center };

            paragraphProperties.Append(justification);

            //create Run and Text 
            Run run = new Run();
            DocumentFormat.OpenXml.Wordprocessing.RunProperties runProperties = new DocumentFormat.OpenXml.Wordprocessing.RunProperties();
            runProperties.Append(new DocumentFormat.OpenXml.Wordprocessing.Bold());
            Text text = new Text();
            //add content in Text
            text.Text = textValue;
            // Set RunProperties
            run.RunProperties = runProperties;
            //add Text to Run
            run.Append(text);

            //add Run to paragraph
            paragraph.Append(paragraphProperties);
            paragraph.Append(run);

            //add Paragraph to cell
            tableCell.Append(tableCellProperties);
            tableCell.Append(paragraph);

            return tableCell;
        }

        /// <summary>
        /// Initialize new cell content object.
        /// </summary>
        /// <param name="textValue">The content of cell.</param>
        /// <returns>The TableCell object.</returns>
        public virtual TableCell NewTableCellData(string textValue, string width = "")
        {
            //create cell object and its properties
            TableCell tableCell = new TableCell();

            TableCellProperties tableCellProperties = new TableCellProperties();
            ConditionalFormatStyle conditionalFormatStyle = new ConditionalFormatStyle() { Val = "001000000000" };
            
            HideMark hideMark = new HideMark();

            tableCellProperties.Append(conditionalFormatStyle);
            // Set Width
            string specificWidth = "2394";
            specificWidth = !string.IsNullOrEmpty(width) ? width : specificWidth;
            TableCellWidth tableCellWidth = new TableCellWidth() { Width = specificWidth, Type = TableWidthUnitValues.Dxa };
            tableCellProperties.Append(tableCellWidth);

            tableCellProperties.Append(hideMark);

            //create paragrpah object and its properties
            Paragraph paragraph = new Paragraph() { RsidParagraphAddition = "004D1DA5", RsidRunAdditionDefault = "004D1DA5" };

            ParagraphProperties paragraphProperties = new ParagraphProperties();
            Justification justification = new Justification() { Val = JustificationValues.Center };

            paragraphProperties.Append(justification);

            //create Run and Text 
            Run run = new Run();
            Text text = new Text();
            //add content in Text
            text.Text = textValue;

            //add Text to Run
            run.Append(text);

            //add Run to paragraph
            paragraph.Append(paragraphProperties);
            paragraph.Append(run);

            //add Paragraph to cell
            tableCell.Append(tableCellProperties);
            tableCell.Append(paragraph);

            return tableCell;
        }

        /// <summary>
        /// Initialize new header row object.
        /// </summary>
        /// <param name="columns">The list of column names.</param>
        /// <returns>The DocumentFormat.OpenXml.Wordprocessing.TableRow object.</returns>
        public virtual TableRow NewTableRowHeader(string[] columns, string[] widths)
        {
            TableRow rowHeader = new TableRow();

            TableRowHeight tableRowHeight = new TableRowHeight() { HeightType = HeightRuleValues.Auto };
            TableRowProperties tableRowProperties = new TableRowProperties();
            tableRowProperties.Append(tableRowHeight);
            rowHeader.TableRowProperties = tableRowProperties;
            if (widths != null)
            {
                for (var i = 0; i < columns.Length; i++)
                {
                    TableCell cellHeader = NewTableCellHeader(columns[i], widths[i]);
                    rowHeader.Append(cellHeader);
                }
            }
            else
            {
                for (var i = 0; i < columns.Length; i++)
                {
                    TableCell cellHeader = NewTableCellHeader(columns[i]);
                    rowHeader.Append(cellHeader);
                }
            }
            
            return rowHeader;
        }

        /// <summary>
        /// Initialize new TableRow object.
        /// </summary>
        /// <returns>The DocumentFormat.OpenXml.Wordprocessing.TableRow object.</returns>
        public virtual TableRow NewEmptyRow()
        {
            TableRow newRow = new TableRow();

            TableRowHeight tableRowHeight = new TableRowHeight() { HeightType = HeightRuleValues.Auto };
            TableRowProperties tableRowProperties = new TableRowProperties();
            tableRowProperties.Append(tableRowHeight);
            newRow.TableRowProperties = tableRowProperties;

            return newRow;
        }

        /// <summary>
        /// Create new Table object and add header row into table with columns.
        /// </summary>
        /// <param name="columns">The list of column names.</param>
        /// <returns>The DocumentFormat.OpenXml.Wordprocessing.Table object.</returns>
        public virtual Table NewTable(string[] columns, string[] widths = null)
        {
            //create table object 
            Table table = new Table();

            // Create the table properties
            TableProperties tblProperties = new TableProperties();

            // Create Table Borders
            TableBorders tblBorders = new TableBorders();

            TopBorder topBorder = new TopBorder();
            topBorder.Val = new EnumValue<BorderValues>(BorderValues.Thick);
            topBorder.Color = "000000";
            tblBorders.AppendChild(topBorder);

            BottomBorder bottomBorder = new BottomBorder();
            bottomBorder.Val = new EnumValue<BorderValues>(BorderValues.Thick);
            bottomBorder.Color = "000000";
            tblBorders.AppendChild(bottomBorder);

            RightBorder rightBorder = new RightBorder();
            rightBorder.Val = new EnumValue<BorderValues>(BorderValues.Thick);
            rightBorder.Color = "000000";
            tblBorders.AppendChild(rightBorder);

            LeftBorder leftBorder = new LeftBorder();
            leftBorder.Val = new EnumValue<BorderValues>(BorderValues.Thick);
            leftBorder.Color = "000000";
            tblBorders.AppendChild(leftBorder);

            InsideHorizontalBorder insideHBorder = new InsideHorizontalBorder();
            insideHBorder.Val = new EnumValue<BorderValues>(BorderValues.Thick);
            insideHBorder.Color = "000000";
            tblBorders.AppendChild(insideHBorder);

            InsideVerticalBorder insideVBorder = new InsideVerticalBorder();
            insideVBorder.Val = new EnumValue<BorderValues>(BorderValues.Thick);
            insideVBorder.Color = "000000";
            tblBorders.AppendChild(insideVBorder);

            // Add the table borders to the properties
            tblProperties.AppendChild(tblBorders);

            // Create Table Width
            TableWidth tableWidth = new TableWidth() { Width = "100%", Type = TableWidthUnitValues.Pct };
            // Add the table width to the properties
            tblProperties.AppendChild(tableWidth);

            // Create Table height
            TableRowHeight tableRowHeight = new TableRowHeight() { HeightType = HeightRuleValues.Auto };
            // Add the table height to the properties
            tblProperties.AppendChild(tableRowHeight);

            TableStyle tableStyle = new TableStyle() { Val = "TableGrid" };
            tblProperties.AppendChild(tableStyle);

            TableLook tableLook = new TableLook() { Val = "04A0" };
            tblProperties.AppendChild(tableLook);

            // Add the table properties to the table
            table.AppendChild(tblProperties);

            //// aaa
            //TableGrid tableGrid = new TableGrid();
            //for (int i = 0; i < columns.Count(); i++)
            //{
            //    //GridColumn gridcol = new GridColumn() { Width = i == 0 ? "800" : "1500" };
            //    GridColumn gridcol = new GridColumn() { Width = "800"};
            //    tableGrid.Append(gridcol);
            //}
            //table.Append(tableGrid);
            //// aaa

            TableRow rowHeader = NewTableRowHeader(columns, widths);
            table.Append(rowHeader);

            return table;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public byte[] GetContentFile(SPFile file)
        {
            return file.OpenBinary();
        }

        public SPFile GetFile(string fileUrl)
        {
            return this.CurrentWeb.GetFile(fileUrl);
        }

        public void CopyTo(SPFile file, string strNewUrl, bool bOverWrite)
        {
            file.CopyTo(strNewUrl, bOverWrite);
        }

        /// <summary>
        /// Upload file to web with url of file.
        /// </summary>
        /// <param name="urlOfFile">The url of file.</param>
        /// <param name="arrBytesSource">The array bute of file.</param>
        public void UploadFile(string urlOfFile, byte[] arrBytesSource)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                using (var site = new SPSite(this.CurrentWeb.Url))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;
                        web.Files.Add(urlOfFile, arrBytesSource, true);
                        web.AllowUnsafeUpdates = false;
                    }
                }
            });
        }

        public void UploadFile(string urlOfFile, Stream stream)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                using (var site = new SPSite(this.CurrentWeb.Url))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;
                        web.Files.Add(urlOfFile, stream, true);
                        web.AllowUnsafeUpdates = false;
                    }
                }
            });
        }
        #endregion
    }
}
