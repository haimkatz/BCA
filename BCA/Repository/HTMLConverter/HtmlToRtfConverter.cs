using Repository.HTMLConverter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace BCA.Repository.HTMLConverter
{

    public static class HtmlToRtfConverter
    {

        public static string ConvertHtmlToRtf(string htmlText,RichEditBox reb)
        {
            var xamlText = HtmlToXamlConverter.ConvertHtmlToXaml(htmlText, false);

            return ConvertXamlToRtf(xamlText);
        }

        private static string ConvertXamlToRtf(string xamlText)
        {
            var richTextBox = new RichTextBlock();
            if (string.IsNullOrEmpty(xamlText)) return "";

                      //Create a MemoryStream of the xaml content 

            using (var xamlMemoryStream = new MemoryStream())
            {
                using (var xamlStreamWriter = new StreamWriter(xamlMemoryStream))
                {
                    xamlStreamWriter.Write(xamlText);
                    xamlStreamWriter.Flush();
                    xamlMemoryStream.Seek(0, SeekOrigin.Begin);

                    //Load the MemoryStream into TextRange ranging from start to end of RichTextBox. 
                    richTextBox.Load(xamlMemoryStream, DataFormats.Xaml);
                }
            }

            using (var rtfMemoryStream = new MemoryStream())
            {

                textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                textRange.Save(rtfMemoryStream, DataFormats.Rtf);
                rtfMemoryStream.Seek(0, SeekOrigin.Begin);
                using (var rtfStreamReader = new StreamReader(rtfMemoryStream))
                {
                    return rtfStreamReader.ReadToEnd();
                }
            }

        }




    }
}



