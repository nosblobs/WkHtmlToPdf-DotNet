﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WkHtmlToPdfDotNet;

namespace WkHtmlToPdfDotNet.TestThreadSafe
{
    public class Program
    {
        static SynchronizedConverter converter;

        public static void Main(string[] args)
        {
            converter = new SynchronizedConverter(new PdfTools());

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Grayscale,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings() { Top = 10 },
                },
                Objects = {
                    new ObjectSettings() {
                        Page = "http://www.color-hex.com/"
                    }
                }
            };

            Task.Run(() => Action(doc));

            var doc2 = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    PaperSize = PaperKind.A4Small
                },

                Objects = {
                    new ObjectSettings()
                    {
                        Page = "http://google.com/",
                    }
                }
            };


            Task.Run(() => Action(doc2));

            Console.ReadKey();
        }

        private static void Action(HtmlToPdfDocument doc)
        {
            byte[] pdf = converter.Convert(doc);

            if (!Directory.Exists("Files"))
            {
                Directory.CreateDirectory("Files");
            }

            using (var stream = new FileStream(Path.Combine("Files", DateTime.UtcNow.Ticks.ToString() + ".pdf"), FileMode.Create))
            {
                stream.Write(pdf, 0, pdf.Length);
            }
        }
    }
}
