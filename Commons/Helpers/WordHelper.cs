using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DocumentFormat.OpenXml.Office2010.Word.DrawingShape;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;

namespace RBVH.Stada.Intranet.Biz.Helpers
{
    public static class WordHelper
    {
        private const string WORKING_HOUR_IN_PDF = "8";
        public static MemoryStream CreateOverTimeWorkApplication(OverTimeManagement overTimeManagement)
        {
            MemoryStream ms = new MemoryStream();

            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                string templateFileName = "OverTimeWorkApplicationTemplate.docx";
                string tempFolderPath = SPUtility.GetVersionedGenericSetupPath(@"TEMPLATE\LAYOUTS\RBVH.Stada.Intranet.ReportTemplates\OverTimeWorkApplication", 15);
                Directory.CreateDirectory(tempFolderPath);
                RemoveOldFiles(tempFolderPath, 1);

                string destFilePath = "";
                string newFileName = string.Format("{0}-{1}.docx", "OverTimeWorkApplication", DateTime.Now.Ticks);
                destFilePath = DownloadFile(SPContext.Current.Site.RootWeb.Url, "Shared Documents", templateFileName, tempFolderPath, newFileName);

                try
                {
                    using (WordprocessingDocument wordProcessingDoc = WordprocessingDocument.Open(destFilePath, true))
                    {
                        List<SdtContentCell> requestedby1 = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByNameFromTable("requestedby");
                        requestedby1[0].FillTextBox(!string.IsNullOrEmpty(overTimeManagement.Requester.LookupValue) ? overTimeManagement.Requester.LookupValue : " ");

                        List<SdtBlock> requestedby2 = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByName("requestedby");
                        requestedby2[0].FillTextBox(!string.IsNullOrEmpty(overTimeManagement.Requester.LookupValue) ? overTimeManagement.Requester.LookupValue : " ");

                        List<SdtContentCell> department = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByNameFromTable("department");
                        department[0].FillTextBox(!string.IsNullOrEmpty(overTimeManagement.CommonDepartment1066.LookupValue) ? overTimeManagement.CommonDepartment1066.LookupValue : " ");

                        var fromDate = overTimeManagement.OverTimeManagementDetailList.Min(x => x.OvertimeFrom).ToString("dd/MM/yyyy");
                        List<SdtContentCell> from = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByNameFromTable("from");
                        from[0].FillTextBox(!string.IsNullOrEmpty(fromDate) ? fromDate : " ");

                        var toDate = overTimeManagement.OverTimeManagementDetailList.Max(x => x.OvertimeTo).ToString("dd/MM/yyyy");
                        List<SdtContentCell> to = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByNameFromTable("to");
                        to[0].FillTextBox(!string.IsNullOrEmpty(toDate) ? toDate : " ");

                        List<SdtContentCell> place = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByNameFromTable("place");
                        place[0].FillTextBox(!string.IsNullOrEmpty(overTimeManagement.Place) ? overTimeManagement.Place : " ");

                        List<SdtContentCell> quantity = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByNameFromTable("quantity");
                        quantity[0].FillTextBox(overTimeManagement.SumOfEmployee + " ");

                        List<SdtContentCell> serving = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByNameFromTable("serving");
                        serving[0].FillTextBox(overTimeManagement.SumOfMeal + " ");
                        List<SdtContentCell> others = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByNameFromTable("others");
                        others[0].FillTextBox(!string.IsNullOrEmpty(overTimeManagement.OtherRequirements) ? overTimeManagement.OtherRequirements : " ");

                        List<SdtBlock> tables = wordProcessingDoc.MainDocumentPart.Document.GetTextBoxByName("table1");
                        if (tables != null && tables.Count() > 0)
                        {
                            string templateStr = File.ReadAllText(Path.Combine(SPUtility.GetVersionedGenericSetupPath(@"TEMPLATE\LAYOUTS\RBVH.Stada.Intranet.ReportTemplates\XML", 15), "overtimetabletemplate.xml"));
                            Table newTable = new Table(templateStr);

                            TableRow rowTemplate = newTable.Descendants<TableRow>().Last();
                            List<string> propertyList = new List<string>() { };
                            for (int i = 0; i < overTimeManagement.OverTimeManagementDetailList.Count; i++)
                            {
                                var detail = overTimeManagement.OverTimeManagementDetailList[i];

                                TableRow newTableRow = new TableRow();
                                newTableRow.TableRowProperties = new TableRowProperties(rowTemplate.TableRowProperties.OuterXml);

                                TableCell cellTemplate1 = rowTemplate.Descendants<TableCell>().ElementAt(0);
                                TableCell newTableCell1 = new TableCell(cellTemplate1.OuterXml);
                                newTableCell1.Descendants<Text>().First().Text = (i + 1).ToString();
                                newTableRow.Append(newTableCell1);

                                TableCell cellTemplate2 = rowTemplate.Descendants<TableCell>().ElementAt(1);
                                TableCell newTableCell2 = new TableCell(cellTemplate2.OuterXml);
                                newTableCell2.Descendants<Text>().First().Text = detail.Employee.LookupValue;
                                newTableRow.Append(newTableCell2);

                                TableCell cellTemplate3 = rowTemplate.Descendants<TableCell>().ElementAt(2);
                                TableCell newTableCell3 = new TableCell(cellTemplate3.OuterXml);
                                newTableCell3.Descendants<Text>().First().Text = detail.EmployeeID.LookupValue;
                                newTableRow.Append(newTableCell3);

                                TableCell cellTemplate4 = rowTemplate.Descendants<TableCell>().ElementAt(3);
                                TableCell newTableCell4 = new TableCell(cellTemplate4.OuterXml);
                                newTableCell4.Descendants<Text>().First().Text = WORKING_HOUR_IN_PDF;
                                newTableRow.Append(newTableCell4);

                                TableCell cellTemplate5 = rowTemplate.Descendants<TableCell>().ElementAt(4);
                                TableCell newTableCell5 = new TableCell(cellTemplate5.OuterXml);
                                newTableCell5.Descendants<Text>().First().Text = detail.OvertimeHours;
                                newTableRow.Append(newTableCell5);

                                TableCell cellTemplate6 = rowTemplate.Descendants<TableCell>().ElementAt(5);
                                TableCell newTableCell6 = new TableCell(cellTemplate6.OuterXml);
                                newTableCell6.Descendants<Text>().First().Text = detail.Task == null ? "" : detail.Task;
                                newTableRow.Append(newTableCell6);

                                TableCell cellTemplate7 = rowTemplate.Descendants<TableCell>().ElementAt(6);
                                TableCell newTableCell7 = new TableCell(cellTemplate7.OuterXml);
                                newTableCell7.Descendants<Text>().First().Text = detail.CompanyTransport;
                                newTableRow.Append(newTableCell7);

                                TableCell cellTemplate8 = rowTemplate.Descendants<TableCell>().ElementAt(7);
                                TableCell newTableCell8 = new TableCell(cellTemplate8.OuterXml);
                                newTableCell8.Descendants<Text>().First().Text = "";
                                newTableRow.Append(newTableCell8);

                                newTable.Append(newTableRow);
                            }

                            newTable.Descendants<TableRow>().ElementAt(1).Remove();
                            tables[0].Parent.InsertAfter(newTable, tables[0]);
                            tables[0].Remove();
                        }

                        wordProcessingDoc.MainDocumentPart.Document.Save();
                    }

                    using (FileStream file = new FileStream(destFilePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] bytes = new byte[file.Length];
                        file.Read(bytes, 0, (int)file.Length);
                        ms.Write(bytes, 0, (int)file.Length);
                    }
                }
                catch { }
            });

            return ms;
        }

        private static bool IsSameDay(OverTimeManagement overtime)
        {
            var minDay = overtime.OverTimeManagementDetailList.Min(x => x.OvertimeFrom);

            var maxDay = overtime.OverTimeManagementDetailList.Max(x => x.OvertimeTo);

            return minDay.Day == maxDay.Day;
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
                                newFileName = string.Format("{0}-{1}.docx", Path.GetFileNameWithoutExtension(fileName), DateTime.Now.Ticks);
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

        public static Table GetTableByCaption(this Document document, string caption)
        {
            var table = document.Body.Descendants<Table>()
                           .Where(t => t.Descendants<TableCaption>().Any(c => c != null && c.Val == caption))
                           .FirstOrDefault();

            List<TableCaption> aa = document.Body.Descendants<TableCaption>().ToList();

            return table;
        }

        public static void FillTableCell(this TableCell cell, string newText)
        {
            // Retrieve old paragraph to get the current style of text
            Paragraph oldParagraph = cell.Elements<Paragraph>().FirstOrDefault();

            // Create a new text
            var paragraph = CreateParagraph(oldParagraph, newText);

            // Replace old text with new one
            if (oldParagraph != null)
            {
                cell.ReplaceChild(paragraph, oldParagraph);
            }
            else
            {
                cell.Append(paragraph);
            }
        }

        public static List<SdtBlock> GetTextBoxByName(this Document document, string textboxName)
        {
            IEnumerable<SdtBlock> textBoxCollection = document.Body.Descendants<SdtBlock>()
                                .Where(sdtBlock => sdtBlock.Descendants<SdtContentBlock>().Any(sdtContent => sdtContent.InnerText == textboxName));

            if (textBoxCollection != null && textBoxCollection.Count() > 0)
            {
                return textBoxCollection.ToList();
            }

            return new List<SdtBlock>();
        }

        public static List<SdtBlock> GetTextBoxByNameFromTable1(this Document document, string textboxName)
        {
            IEnumerable<Table> tables = document.Body.Descendants<Table>();
            if (tables != null)
            {
                foreach (var table in tables)
                {
                    IEnumerable<SdtBlock> textBoxCollection = table.Descendants<SdtBlock>().Where(sdtBlock => sdtBlock.Descendants<SdtContentBlock>().Any(sdtContent => sdtContent.InnerText == textboxName));
                    if (textBoxCollection != null)
                    {
                        return textBoxCollection.ToList();
                    }
                }
            }

            return new List<SdtBlock>();
        }

        public static List<SdtContentCell> GetTextBoxByNameFromTable(this Document document, string textboxName)
        {
            IEnumerable<SdtContentCell> textBoxCollection = document.Body.Descendants<SdtContentCell>().Where(sdtContent => sdtContent.InnerText == textboxName);

            if (textBoxCollection != null && textBoxCollection.Count() > 0)
            {
                return textBoxCollection.ToList();
            }

            return new List<SdtContentCell>();
        }

        public static List<SdtContentCell> GetCheckBoxByNameFromTable(this Document document, string checkboxText)
        {
            IEnumerable<SdtContentCell> checkboxCollection = document.Body.Descendants<SdtContentCell>().Where(sdtContent => sdtContent.InnerText == checkboxText);

            if (checkboxCollection != null && checkboxCollection.Count() > 0)
            {
                return checkboxCollection.ToList();
            }

            return new List<SdtContentCell>();
        }

        /// <summary>
        /// Get a shape in a word document by its name
        /// </summary>
        /// <param name="document">word document to find the shape</param>
        /// <param name="shapeName">name of the shape</param>
        /// <returns></returns>
        public static List<WordprocessingShape> GetShapeByName(this Document document, string shapeName)
        {
            var drawing = document.Body.Descendants<Drawing>()
                                       .Where(d => d.Descendants<DocProperties>().Any(p => p.Name == shapeName))
                                       .FirstOrDefault();
            if (drawing != null)
            {
                return drawing.Descendants<WordprocessingShape>().ToList();
            }

            return new List<WordprocessingShape>();
        }

        public static void FillTextBox(this SdtContentCell content, string newText)
        {
            if (content != null)
            {
                // Retrieve old paragraph to get the current style of text
                Paragraph oldParagraph = content.Descendants<TableCell>().FirstOrDefault().Elements<Paragraph>().FirstOrDefault();

                // Create a new text
                var paragraph = CreateParagraph(oldParagraph, newText);

                // Replace old text with new one
                if (oldParagraph != null)
                {
                    content.Descendants<TableCell>().FirstOrDefault().ReplaceChild(paragraph, oldParagraph);
                }
                else
                {
                    content.Descendants<TableCell>().FirstOrDefault().Append(paragraph);
                }
            }
        }

        public static void FillTextBox(this SdtContentCell content, string newText, JustificationValues alignment)
        {
            if (content != null)
            {
                // Retrieve old paragraph to get the current style of text
                Paragraph oldParagraph = content.Descendants<TableCell>().FirstOrDefault().Elements<Paragraph>().FirstOrDefault();

                // Create a new text
                var paragraph = CreateParagraph(oldParagraph, newText, alignment);

                // Replace old text with new one
                if (oldParagraph != null)
                {
                    content.Descendants<TableCell>().FirstOrDefault().ReplaceChild(paragraph, oldParagraph);
                }
                else
                {
                    content.Descendants<TableCell>().FirstOrDefault().Append(paragraph);
                }
            }
        }

        public static void FillTextBox(this SdtBlock sdtBlock, string newText)
        {
            // Get Textbox content
            SdtContentBlock content = sdtBlock.Descendants<SdtContentBlock>().FirstOrDefault();

            if (content != null)
            {
                // Retrieve old paragraph to get the current style of text
                Paragraph oldParagraph = content.Elements<Paragraph>().FirstOrDefault();

                // Create a new text
                var paragraph = CreateParagraph(oldParagraph, newText);

                // Replace old text with new one
                if (oldParagraph != null)
                {
                    content.ReplaceChild(paragraph, oldParagraph);
                }
                else
                {
                    content.Append(paragraph);
                }
            }
        }

        /// <summary>
        /// Fill a textbox with a text
        /// </summary>
        /// <param name="textbox">textbox to fill</param>
        /// <param name="newText">text to fill</param>
        public static void FillTextBox(this WordprocessingShape textbox, string newText)
        {
            // Get Textbox content
            TextBoxContent content = textbox.Descendants<TextBoxContent>().FirstOrDefault();

            if (content != null)
            {
                // Retrieve old paragraph to get the current style of text
                Paragraph oldParagraph = content.Elements<Paragraph>().FirstOrDefault();

                // Create a new text
                var paragraph = CreateParagraph(oldParagraph, newText);

                // Replace old text with new one
                if (oldParagraph != null)
                {
                    content.ReplaceChild(paragraph, oldParagraph);
                }
                else
                {
                    content.Append(paragraph);
                }
            }
        }

        private static Paragraph CreateParagraph(Paragraph oldParagraphToGetStyle, string newText)
        {
            // Create a new text
            var newParagraph = new Paragraph();
            var run = new Run() { RunProperties = new RunProperties() };
            var text = new Text();
            text.Text = newText;
            run.Append(text);
            
            newParagraph.Append(run);

            // Retrieve old paragraph to get the current style of text
            Paragraph oldParagraph = oldParagraphToGetStyle;
            Run oldRun = null;

            if (oldParagraph != null)
            {
                oldRun = oldParagraph.Elements<Run>().FirstOrDefault();
            }

            if (oldRun != null && oldRun.RunProperties != null)
            {
                run.RunProperties = (RunProperties)oldRun.RunProperties.CloneNode(true);
            }

            return newParagraph;
        }

        private static Paragraph CreateParagraph(Paragraph oldParagraphToGetStyle, string newText, JustificationValues alignment)
        {
            // Create a new text
            var newParagraph = new Paragraph();
            var run = new Run() { RunProperties = new RunProperties() };
            var text = new Text();
            text.Text = newText;
            run.Append(text);

            ParagraphProperties paragraphProperties = new ParagraphProperties();
            Justification justification = new Justification() { Val = alignment };
            paragraphProperties.Append(justification);

            newParagraph.Append(paragraphProperties);
            newParagraph.Append(run);

            // Retrieve old paragraph to get the current style of text
            Paragraph oldParagraph = oldParagraphToGetStyle;
            Run oldRun = null;

            if (oldParagraph != null)
            {
                oldRun = oldParagraph.Elements<Run>().FirstOrDefault();
            }

            if (oldRun != null && oldRun.RunProperties != null)
            {
                run.RunProperties = (RunProperties)oldRun.RunProperties.CloneNode(true);
            }

            return newParagraph;
        }
    }
}
