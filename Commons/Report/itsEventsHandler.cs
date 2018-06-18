using iTextSharp.text;
using iTextSharp.text.pdf;

namespace RBVH.Stada.Intranet.Biz.Report
{
    public class itsEventsHandler : PdfPageEventHelper
    {
        PdfTemplate total;
        BaseFont helv;

        // I am following a tutorial & they said that if I want to create headers/footers when each page is created 
        // that I should override the OnEndPage() not the OnStartPage() is that correct?
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            // Post: When each new page is created, add a header & footer image to the page. And set the top margin to 370px
            //       and the bottom margin to 664px.
            // Result: The function executes but the pdf's header image isn't visible & the footer looks resized(scaled up in size).

            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var directory = System.IO.Path.GetDirectoryName(path);
            //Footer Image 
            Image imgfoot = Image.GetInstance(directory + "/images/pdf/StadaFooter.jpg");
            //Header Image 
            Image imghead = Image.GetInstance(directory + "/images/pdf/StadaLogo.jpg");

            imgfoot.SetAbsolutePosition(0, 0);
            imgfoot.ScaleAbsolute(new Rectangle(document.PageSize.Width, 60));

            imghead.SetAbsolutePosition(0, 0);
            imghead.ScaleAbsolute(new Rectangle(document.PageSize.Width, 15));

            PdfContentByte cbhead = writer.DirectContent;
            PdfTemplate tp = cbhead.CreateTemplate(document.PageSize.Width, document.PageSize.Height); // units are in pixels but I'm not sure if thats the correct units
            tp.AddImage(imghead);

            PdfContentByte cbfoot = writer.DirectContent;
            PdfTemplate tpl = cbfoot.CreateTemplate(document.PageSize.Width, document.PageSize.Height);
            tpl.AddImage(imgfoot);

            cbhead.AddTemplate(tp, 0, document.PageSize.Height);
            cbfoot.AddTemplate(tpl, 0, 0);
            //cbhead.AddTemplate(tp, 0, 715);

            helv = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);

            /*PdfContentByte cb = writer.DirectContent;
            cbfoot.SaveState();
            document.SetMargins(35, 35, 100, 82);
            cb.RestoreState();*/

            //document.NewPage(); 
            base.OnStartPage(writer, document);
        }

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            total = writer.DirectContent.CreateTemplate(100, 100);
            total.BoundingBox = new Rectangle(-20, -20, 100, 100);
            helv = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
        }
    }
}
