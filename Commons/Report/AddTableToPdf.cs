using iTextSharp.text;
using iTextSharp.text.pdf;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.IO;
using System.Linq;

namespace RBVH.Stada.Intranet.Biz.Report
{
    public static class AddTableToPdf
    {
        private const string WORKING_HOUR_IN_PDF = "8";
        private const int EXTRA_ITEM = 5;

        public static MemoryStream CreateReportFromExisted(string filePath, OverTimeManagement OverTimeManagement)
        {
            PdfReader reader = new PdfReader(filePath);
            Rectangle size = reader.GetPageSizeWithRotation(1);
            MemoryStream ms = new MemoryStream();

            using (Document document = new Document())
            {
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                // Open the Document for writing
                int p = 1;
                document.Open();

                PdfContentByte cb = writer.DirectContent;

                //Get the page to work with
                PdfImportedPage page = writer.GetImportedPage(reader, p);

                var pageRotation = reader.GetPageRotation(p);
                var pageHeight = reader.GetPageSizeWithRotation(p).Height;
                var itemPerPage = 18;
                var pageNum = 0;
                if (OverTimeManagement.OverTimeManagementDetailList.Count - itemPerPage <= 0)
                {
                    pageNum = 1;
                }
                else
                {
                    pageNum = (OverTimeManagement.OverTimeManagementDetailList.Count - itemPerPage) / (itemPerPage + EXTRA_ITEM) + 1;
                    if ((OverTimeManagement.OverTimeManagementDetailList.Count - itemPerPage) % itemPerPage != 0)
                    {
                        pageNum++;
                    }
                }
              

                for (int i = 0; i < pageNum; i++)
                {
                    if (i == 0)
                    {
                        document.SetPageSize(size);

                        document.NewPage();

                        cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);

                        CreateTableMasterData(document, writer, OverTimeManagement);

                        CreateOvertimeDetail(document, writer, OverTimeManagement, i, itemPerPage);
                    }
                    else
                    {
                        document.SetPageSize(size);
                        document.NewPage();

                        cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);

                        CreateOvertimeDetail(document, writer, OverTimeManagement, i, itemPerPage);
                    }
                    if (i == pageNum - 1)
                    {

                        CreateFooter(document, writer, OverTimeManagement);
                    }
                }
            }
            reader.Close();
            return ms;
        }

        public static MemoryStream CreateStreamFile(string fileUrl)
        {
            var waterMarkText = string.Empty;

            PdfReader reader = new PdfReader(fileUrl);
            PdfReader.unethicalreading = true;

            Rectangle size = reader.GetPageSizeWithRotation(1);
            Document document = new Document(size);
            MemoryStream ms = new MemoryStream();

            PdfWriter writer = PdfWriter.GetInstance(document, ms);

            document.Open();

            PdfContentByte cb = writer.DirectContent;

            for (int pageNumber = 1; pageNumber <= reader.NumberOfPages; pageNumber++)
            {
                PdfImportedPage page = writer.GetImportedPage(reader, pageNumber);
                var pageRotation = reader.GetPageRotation(pageNumber);
                var pageHeight = reader.GetPageSizeWithRotation(pageNumber).Height;

                float textX, textY, textRotation;

                if (pageRotation == 90 || pageRotation == 270)
                {
                    document.SetPageSize(size.Rotate());
                    document.NewPage();

                    cb.AddTemplate(page, 0, -1f, 1f, 0, 0, pageHeight);

                    textX = document.PageSize.Width / 2;
                    textY = 30;
                    textRotation = 0;
                }
                else
                {
                    document.SetPageSize(size);
                    document.NewPage();

                    cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);

                    textX = 30;
                    textY = document.PageSize.Height / 2;
                    textRotation = 90f;
                }

                PdfGState graphicsState = new PdfGState();
                graphicsState.FillOpacity = 0.4F;
                //set graphics state to pdfcontentbyte
                cb.SetGState(graphicsState);
                //Font, font size and color has to be set for the PDFContentByte before writing any text
                BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);
                cb.SetColorFill(BaseColor.DARK_GRAY);
                cb.SetFontAndSize(baseFont, 8);
                cb.BeginText();
                cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, waterMarkText, textX, textY, textRotation);
                cb.EndText();
            }

            document.Close();

            return ms;
        }

        private static void CreateOvertimeDetail(Document document, PdfWriter writer, OverTimeManagement overTimeManagement, int indexFrom, int itemPerPage)
        {
            // content table
            PdfPTable table = new PdfPTable(8) { HorizontalAlignment = Element.ALIGN_CENTER, WidthPercentage = 80, HeaderRows = 2 };
            float[] widths = new float[] { 5f, 30f, 10f, 12f, 15f, 30f, 14f, 14f };
            table.SetWidths(widths);
            BaseFont bf = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\ARIALUNI.TTF", BaseFont.IDENTITY_H, true);
            Font normalFont = new Font(bf, 10, Font.NORMAL, BaseColor.BLACK);
            Font boldFont = new Font(bf, 10, Font.BOLD, BaseColor.BLACK);
            Font boldItalicFont = new Font(bf, 10, Font.BOLDITALIC, BaseColor.BLACK);

            table.DefaultCell.Phrase = new Phrase() { Font = normalFont };

            //Master Table
            //PdfPTable masterTable = new PdfPTable(4) { HorizontalAlignment = Element.ALIGN_LEFT, WidthPercentage = 80 };
            //float[] widthsmasterTable = new float[] { 30f, 40f, 30f, 20f };
            //masterTable.SetWidths(widthsmasterTable);

            //header
            CreateTableHeader(table, boldFont, boldItalicFont);

            //content
            CreateTableContent(table, normalFont, overTimeManagement, indexFrom, itemPerPage);

            table.TotalWidth = document.PageSize.Width - 40f;
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            if (indexFrom == 0)
            {
                table.WriteSelectedRows(0, -1, 20f, document.PageSize.Height - 200f, writer.DirectContent);
            }
            else
            {
                table.WriteSelectedRows(0, -1, 20f, document.PageSize.Height - 110f, writer.DirectContent);
            }



            //masterTable.TotalWidth = document.PageSize.Width - 40f;
            //masterTable.HorizontalAlignment = Element.ALIGN_CENTER;
            //masterTable.WriteSelectedRows(0, -1, 20f, document.PageSize.Height - 100f, writer.DirectContent);
        }

        private static void CreateTableHeader(PdfPTable table, Font boldFont, Font boldItalicFont)
        {
            TableHeader[] headerTitles = new TableHeader[] {
                       new TableHeader {BoldTitle= "Stt"},
                        new TableHeader {BoldTitle= "Họ và tên",},
                        new TableHeader {BoldTitle= "Mã NV",},
                       new TableHeader {BoldTitle=  "Số giờ làm việc trong ngày (giờ)"} ,
                       new TableHeader {BoldTitle=  "Số giờ làm thêm",ItalicTitle=" Từ:…h Đến:…h" },
                    new TableHeader {BoldTitle= "Nội dung công việc"},
                      new TableHeader {BoldTitle=   "Xe đưa đón"},
                      new TableHeader {BoldTitle=   "Chữ ký người lao động"} };
            foreach (var headerTitle in headerTitles)
            {
                Chunk[] chunks = { new Chunk(headerTitle.BoldTitle, boldFont), new Chunk(headerTitle.ItalicTitle, boldItalicFont) };
                PdfPCell cell;
                if (headerTitle.ColSpan > 0)
                {
                    cell = GetCell(chunks, headerTitle.ColSpan, 1);
                }
                else
                {
                    cell = GetCell(chunks, 1, 2);
                }

                cell.BackgroundColor = new BaseColor(214, 227, 188);

                table.AddCell(cell);
            }

            // Inner middle row.
            //PdfPCell cellMidle1 = GetCell("HM", boldFont);
            //cellMidle1.BackgroundColor = new BaseColor(196, 188, 150);
            //table.AddCell(cellMidle1);
            //PdfPCell cellMidle2 = GetCell("KD", boldFont);
            //cellMidle2.BackgroundColor = new BaseColor(196, 188, 150);
            //table.AddCell(cellMidle2);
        }

        private static bool IsSameDay(OverTimeManagement overtime)
        {
            var minDay = overtime.OverTimeManagementDetailList.Min(x => x.OvertimeFrom);

            var maxDay = overtime.OverTimeManagementDetailList.Max(x => x.OvertimeTo);

            return minDay.Day == maxDay.Day;
        }

        private static void CreateTableMasterData(Document document, PdfWriter writer, OverTimeManagement overtime)
        {
            BaseFont bf = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\ARIALUNI.TTF", BaseFont.IDENTITY_H, true);
            Font normalFont = new Font(bf, 10, Font.NORMAL, BaseColor.BLACK);
            Font boldFont = new Font(bf, 10, Font.BOLD, BaseColor.BLACK);
            Font boldItalicFont = new Font(bf, 10, Font.BOLDITALIC, BaseColor.BLACK);

            PdfPTable table = new PdfPTable(4) { HorizontalAlignment = Element.ALIGN_LEFT, WidthPercentage = 80 };
            float[] widthsmasterTable = new float[] { 18f, 42f, 15f, 45f};
            table.SetWidths(widthsmasterTable);

            PdfPCell cellMidle1 = GetCell("Người Đề Nghị: ", boldItalicFont);
            cellMidle1.Border = 0;
            cellMidle1.BackgroundColor = BaseColor.WHITE;
            cellMidle1.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellMidle1);

            cellMidle1 = GetCell(overtime.Requester.LookupValue, normalFont);
            cellMidle1.Border = 0;
            cellMidle1.BackgroundColor = BaseColor.WHITE;
            cellMidle1.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellMidle1);

            cellMidle1 = GetCell("Bộ Phận:", boldItalicFont);
            cellMidle1.Border = 0;
            cellMidle1.BackgroundColor = BaseColor.WHITE;
            cellMidle1.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellMidle1);

            cellMidle1 = GetCell(overtime.CommonDepartment1066.LookupValue, normalFont);
            cellMidle1.Border = 0;
            cellMidle1.BackgroundColor = BaseColor.WHITE;
            cellMidle1.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellMidle1);

            cellMidle1 = GetCell("Thời Gian: ", boldItalicFont);
            cellMidle1.Border = 0;
            cellMidle1.BackgroundColor = BaseColor.WHITE;
            cellMidle1.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellMidle1);

            if (IsSameDay(overtime))
            {
                cellMidle1 = GetCell(overtime.CommonDate.ToString("dd/MM/yyyy"), normalFont);
                cellMidle1.Border = 0;
                cellMidle1.BackgroundColor = BaseColor.WHITE;
                cellMidle1.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cellMidle1);
            }
            else
            {
                var from = overtime.OverTimeManagementDetailList.Min(x => x.OvertimeFrom).ToString("dd/MM/yyyy");
                var to = overtime.OverTimeManagementDetailList.Max(x => x.OvertimeTo).ToString("dd/MM/yyyy");

                cellMidle1 = GetCell("Từ: " + from + "  Đến: " + to, normalFont);
                cellMidle1.Border = 0;
                cellMidle1.BackgroundColor = BaseColor.WHITE;
                cellMidle1.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cellMidle1);
            }

            cellMidle1 = GetCell("Địa Điểm: ", boldItalicFont);
            cellMidle1.Border = 0;
            cellMidle1.BackgroundColor = BaseColor.WHITE;
            cellMidle1.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellMidle1);

            cellMidle1 = GetCell(overtime.Place, normalFont);
            cellMidle1.Border = 0;
            cellMidle1.BackgroundColor = BaseColor.WHITE;
            cellMidle1.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellMidle1);

            cellMidle1 = GetCell("Số Lượng: ", boldItalicFont);
            cellMidle1.Border = 0;
            cellMidle1.BackgroundColor = BaseColor.WHITE;
            cellMidle1.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellMidle1);

            cellMidle1 = GetCell(overtime.SumOfEmployee + "", normalFont);
            cellMidle1.Border = 0;
            cellMidle1.BackgroundColor = BaseColor.WHITE;
            cellMidle1.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellMidle1);

            cellMidle1 = GetCell("Số Suất Ăn: ", boldItalicFont);
            cellMidle1.Border = 0;
            cellMidle1.BackgroundColor = BaseColor.WHITE;
            cellMidle1.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellMidle1);

            cellMidle1 = GetCell(overtime.SumOfMeal + "", normalFont);
            cellMidle1.Border = 0;
            cellMidle1.BackgroundColor = BaseColor.WHITE;
            cellMidle1.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellMidle1);

            cellMidle1 = GetCell("Yêu Cầu Khác: ", boldItalicFont);
            cellMidle1.Border = 0;
            cellMidle1.BackgroundColor = BaseColor.WHITE;
            cellMidle1.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellMidle1);

            cellMidle1 = GetCell(overtime.OtherRequirements, normalFont);
            cellMidle1.Border = 0;
            cellMidle1.Colspan = 3;
            cellMidle1.BackgroundColor = BaseColor.WHITE;
            cellMidle1.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellMidle1);

            cellMidle1 = GetCell("", boldItalicFont);
            cellMidle1.Border = 0;
            cellMidle1.BackgroundColor = BaseColor.WHITE;
            cellMidle1.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellMidle1);

            cellMidle1 = GetCell("", boldItalicFont);
            cellMidle1.Border = 0;
            cellMidle1.BackgroundColor = BaseColor.WHITE;
            cellMidle1.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellMidle1);

            table.TotalWidth = document.PageSize.Width - 40f;
            Console.WriteLine(table.TotalWidth);
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.WriteSelectedRows(0, -1, 20f, document.PageSize.Height - 100f, writer.DirectContent);
        }

        private static void CreateFooter(Document document, PdfWriter writer, OverTimeManagement overtime)
        {
            BaseFont bf = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\ARIALUNI.TTF", BaseFont.IDENTITY_H, true);
            Font normalFont = new Font(bf, 10, Font.NORMAL, BaseColor.BLACK);

            PdfPTable table = new PdfPTable(4) { HorizontalAlignment = Element.ALIGN_LEFT, WidthPercentage = 80 };
            float[] widthsmasterTable = new float[] { 30f, 30f, 30f, 30f };
            table.SetWidths(widthsmasterTable);

            PdfPCell cellMidle1 = GetCell("Bình Dương, ngày......tháng......năm......", normalFont);
            cellMidle1.BackgroundColor = BaseColor.WHITE;
            cellMidle1.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellMidle1.Colspan = 4;
            cellMidle1.Border = 0;
            table.AddCell(cellMidle1);

            cellMidle1 = GetCell("Người Đề Nghị", normalFont);
            cellMidle1.BackgroundColor = BaseColor.WHITE;
            cellMidle1.HorizontalAlignment = Element.ALIGN_MIDDLE;
            cellMidle1.Border = 0;
            table.AddCell(cellMidle1);

            cellMidle1 = GetCell("PT. Bộ Phận", normalFont);
            cellMidle1.BackgroundColor = BaseColor.WHITE;
            cellMidle1.HorizontalAlignment = Element.ALIGN_MIDDLE;
            cellMidle1.Border = 0;
            table.AddCell(cellMidle1);

            cellMidle1 = GetCell("Người Sử Dụng Lao Động", normalFont);
            cellMidle1.BackgroundColor = BaseColor.WHITE;
            cellMidle1.HorizontalAlignment = Element.ALIGN_MIDDLE;
            cellMidle1.Border = 0;
            table.AddCell(cellMidle1);

            cellMidle1 = GetCell("BCH Công Đoàn", normalFont);
            cellMidle1.BackgroundColor = BaseColor.WHITE;
            cellMidle1.HorizontalAlignment = Element.ALIGN_MIDDLE;
            cellMidle1.Border = 0;
            table.AddCell(cellMidle1);

            cellMidle1 = GetCell(" ", normalFont);
            cellMidle1.BackgroundColor = BaseColor.WHITE;
            cellMidle1.HorizontalAlignment = Element.ALIGN_MIDDLE;
            cellMidle1.Border = 0;
            cellMidle1.Colspan = 4;
            cellMidle1.Rowspan = 4;
            table.AddCell(cellMidle1);

            cellMidle1 = GetCell(" ", normalFont);
            cellMidle1.BackgroundColor = BaseColor.WHITE;
            cellMidle1.HorizontalAlignment = Element.ALIGN_MIDDLE;
            cellMidle1.Border = 0;
            cellMidle1.Colspan = 4;
            table.AddCell(cellMidle1);

            cellMidle1 = GetCell(overtime.Requester.LookupValue, normalFont);
            cellMidle1.BackgroundColor = BaseColor.WHITE;
            cellMidle1.HorizontalAlignment = Element.ALIGN_MIDDLE;
            cellMidle1.Border = 0;
            table.AddCell(cellMidle1);

            cellMidle1 = GetCell("", normalFont);
            cellMidle1.BackgroundColor = BaseColor.WHITE;
            cellMidle1.HorizontalAlignment = Element.ALIGN_MIDDLE;
            cellMidle1.Border = 0;
            cellMidle1.Colspan = 3;
            table.AddCell(cellMidle1);

            table.TotalWidth = document.PageSize.Width - 40f;
            table.HorizontalAlignment = Element.ALIGN_LEFT;
            table.WriteSelectedRows(0, -1, 20f, document.PageSize.Height - 680f, writer.DirectContent);
        }

        private static void CreateTableContent(PdfPTable table, Font normalFont, OverTimeManagement OverTimeManagement, int indexFrom, int itemPerPage)
        {
            int index = 0;

            var overTimes = OverTimeManagement.OverTimeManagementDetailList.OrderBy(x => x.OvertimeFrom).GroupBy(x => x.OvertimeHours).Select(n => new
            {
                OverTimeHours = n.Key,
                OverTimeHoursCount = n.Count()
            });

            var itemCount = OverTimeManagement.OverTimeManagementDetailList.Count();
            int runFrom = 0, runTo = 0;

            if (indexFrom > 0)
            {
                itemPerPage = itemPerPage + EXTRA_ITEM;
                runFrom = indexFrom * itemPerPage - EXTRA_ITEM;
            }
            else
            {
                runFrom = indexFrom * itemPerPage;
            }

            runTo = runFrom + itemPerPage;
            
            if (itemCount - runTo < 0)
            {
                runTo = itemCount;
            }

            for (int i = runFrom; i < runTo; i++)
            {
                var detail = OverTimeManagement.OverTimeManagementDetailList[i];
                index = i + 1;

                PdfPCell cellMidle1 = GetCell(index++.ToString(), normalFont);
                cellMidle1.FixedHeight = 15f;
                cellMidle1.BackgroundColor = BaseColor.WHITE;
                table.AddCell(cellMidle1);

                cellMidle1 = GetCell(detail.Employee.LookupValue, normalFont);
                cellMidle1.BackgroundColor = BaseColor.WHITE;
                cellMidle1.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cellMidle1);

                cellMidle1 = GetCell(detail.EmployeeID.LookupValue, normalFont);
                cellMidle1.BackgroundColor = BaseColor.WHITE;
                table.AddCell(cellMidle1);

                cellMidle1 = GetCell(WORKING_HOUR_IN_PDF, normalFont);
                cellMidle1.BackgroundColor = BaseColor.WHITE;
                table.AddCell(cellMidle1);

                cellMidle1 = GetCell(detail.OvertimeHours, normalFont);
                cellMidle1.BackgroundColor = BaseColor.WHITE;
                table.AddCell(cellMidle1);

                var task = detail.Task == null ? "" : detail.Task.Length > 25 ? detail.Task.Substring(0, 24) + "..." : detail.Task;
                cellMidle1 = GetCell(task, normalFont);
                cellMidle1.BackgroundColor = BaseColor.WHITE;
                cellMidle1.PaddingTop = 0;
                table.AddCell(cellMidle1);

                cellMidle1 = GetCell(detail.CompanyTransport, normalFont);
                cellMidle1.BackgroundColor = BaseColor.WHITE;
                table.AddCell(cellMidle1);

                //cellMidle1 = GetCell(detail.KD, normalFont);
                //cellMidle1.BackgroundColor = BaseColor.WHITE;
                //table.AddCell(cellMidle1);

                cellMidle1 = GetCell("", normalFont);
                cellMidle1.BackgroundColor = BaseColor.WHITE;
                table.AddCell(cellMidle1);
            }
        }

        private static PdfPCell GetCell(string text, Font normalFont)
        {
            return GetCell(text, normalFont, 1, 1);
        }

        private static PdfPCell GetCell(string text, Font normalFont, int colSpan, int rowSpan)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, normalFont));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Rowspan = rowSpan;
            cell.Colspan = colSpan;

            return cell;
        }

        private static PdfPCell GetCell(Chunk[] chunks)
        {
            return GetCell(chunks, 1, 1);
        }

        private static PdfPCell GetCell(Chunk[] chunks, int colSpan, int rowSpan)
        {
            Phrase phrase = new Phrase();
            foreach (var chunk in chunks)
            {
                phrase.Add(chunk);
            }
            PdfPCell cell = new PdfPCell(phrase);
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Rowspan = rowSpan;
            cell.Colspan = colSpan;

            return cell;
        }
    }
}
