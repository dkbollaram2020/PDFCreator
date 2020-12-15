using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.IO;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.IO.Font;
using iText.Kernel.Geom;

namespace GeneratePdf
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"D:\code\dotnet\input.txt";

            PdfWriter writer = new PdfWriter("D:\\demo.pdf");
            PdfDocument pdf = new PdfDocument(writer);
            // Adding a new page 
            pdf.AddNewPage();
            Document document = new Document(pdf, PageSize.A4);

            List<string> commands = new List<string>();

            // loop through each of the lines in the input text file
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            Style style = new Style();
            Paragraph paragraph = new Paragraph();
            int extra = 0;
            foreach (string line in File.ReadLines(path))
            {
                // check if the string starts with . and so add to the commands and continue
                string txt = line;
                if (txt.StartsWith("."))
                {
                    string[] subs = txt.Split(' ');
                    if (subs.Length==2)
                    {
                        commands.Add(subs[0].Substring(1));
                        extra = Convert.ToInt32(subs[1]);
                    }
                    else
                    {
                        commands.Add(txt.Substring(1));

                    }
                    continue;
                }
                else
                {
                    commands.Add("text");
                }

                // if not, it is a simple text
                // switch based on all the commands
                // apply transformations on the text accordingly
                for (int i = 0; i < commands.Count; i++)
                {
                    if (string.IsNullOrEmpty(commands[i]))
                    {
                        continue;
                    }
                    
                    switch (commands[i])
                    {
                        case "large":
                            style = setStyles(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 35);
                            break;
                        case "normal":
                            style = setStyles(PdfFontFactory.CreateFont(StandardFonts.HELVETICA));
                            break;
                        case "paragraph":
                            document.Add(paragraph);
                            paragraph = new Paragraph();
                            paragraph.SetMaxWidth(document.GetPdfDocument().GetDefaultPageSize().GetWidth() - (document.GetLeftMargin() + document.GetRightMargin() + 40));
                            break;
                        case "italics":
                            style = setStyles(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_OBLIQUE));
                            break;
                        case "bold":
                            style = setStyles(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD));
                            break;
                        case "regular":
                            style = setStyles(PdfFontFactory.CreateFont(StandardFonts.HELVETICA));
                            break;
                        case "indent":
                            paragraph.SetMarginLeft(-40 * extra);
                            break;
                        case "fill":
                            //Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
                            //table.AddCell(paragraph);
                            paragraph.SetWidth(document.GetPdfDocument().GetDefaultPageSize().GetWidth() - (document.GetLeftMargin() + document.GetRightMargin()));
                            //paragraph.SetTextAlignment(TextAlignment.JUSTIFIED);
                            //Console.WriteLine(document.GetLeftMargin() + document.GetRightMargin());
                            //paragraph.SetMaxWidth(document.GetPdfDocument().GetDefaultPageSize().GetWidth() - (document.GetLeftMargin() + document.GetRightMargin()));

                            break;
                        case "nofill":
                            break;
                        case "text":
                            paragraph = formatText(line, style, paragraph);
                            break;
                        default:
                            break;
                    }
                    commands[i] = string.Empty;
                }
                // apply the operations on the text until the next line with command
            }
            document.Close();
            //Console.ReadLine();
        }

        private static Paragraph formatText(string line, Style style, Paragraph paragraph)
        {
            if (paragraph == null)
            {
                paragraph = new Paragraph();
            }
            return paragraph.Add(new Text(line).AddStyle(style));
        }

        private static Style setStyles(PdfFont font, int fontSize = 12)
        {
            Style style = new Style();
            style.SetFont(font).SetFontSize(fontSize);
            return style;
        }
    }
}
