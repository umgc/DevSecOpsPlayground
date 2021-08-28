using iTextSharp.text;
using iTextSharp.text.pdf;

// Based on the works located here: https://github.com/tossnet/Blazor-PDF/blob/master/Blazor-PDF/Blazor-PDF/PDF/PDFHeaderFooter.cs
namespace CaPPMS.Model
{
    public class PdfHeaderFooter : PdfPageEventHelper
    {
        private Font TitleFont { get; set; }

        private string HeaderText { get; set; }

        private string FooterText { get; set; }

        public PdfHeaderFooter(string headerText, string footerText, Font titleFont)
        {
            this.HeaderText = headerText;
            this.FooterText = footerText;
            this.TitleFont = titleFont;
        }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);

            var table = CreateTable(HeaderText, document.LeftMargin, document.RightMargin);

            table.WriteSelectedRows(0, -1, document.LeftMargin, document.Top, writer.DirectContent);
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);
            var table = CreateTable(FooterText, document.LeftMargin, document.RightMargin);
            table.WriteSelectedRows(0, -1, document.LeftMargin, document.Bottom, writer.DirectContent);
        }

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            base.OnOpenDocument(writer, document);
        }

        private PdfPTable CreateTable(string text, float leftMargin, float rightMargin)
        {
            var table = new PdfPTable(1)
            {
                TotalWidth = PageSize.Letter.Width - leftMargin - rightMargin,
                LockedWidth = true
            };

            var label = new Chunk(text, this.TitleFont);

            var cell = new PdfPCell(new Phrase(label))
            {
                BackgroundColor = new BaseColor(163, 6, 6),
                Border = 0,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_TOP,
                Padding = 4f
            };

            table.AddCell(cell);

            return table;
        }
    }
}
