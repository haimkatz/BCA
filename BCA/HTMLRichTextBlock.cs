using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Windows.Data.Xml.Dom;
using Windows.Data.Xml.Xsl;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Markup;
using Windows.Storage;

namespace BCA
{

    public class Properties : DependencyObject 
     { 
         public static readonly DependencyProperty HtmlProperty =
             DependencyProperty.RegisterAttached("Html", typeof(string), typeof(Properties), new PropertyMetadata(null, HtmlChanged)); 
 
         public static void SetHtml(DependencyObject obj, string value)
         { 
             obj.SetValue(HtmlProperty, value); 
         } 
 
 
         public static string GetHtml(DependencyObject obj)
         { 
             return (string)obj.GetValue(HtmlProperty); 
         } 

         private static async void HtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
         { 
             // Get the target RichTextBlock 
             RichEditBox richText = d as RichEditBox; 
             if (richText == null) return; 
 
 
             // Wrap the value of the Html property in a div and convert it to a new RichTextBlock 
             string xhtml = string.Format("<div>{}</div>", e.NewValue as string); 
             xhtml = xhtml.Replace("\r", "").Replace("\n", "<br />"); 
          RichEditBox newRichText = null; 
             if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) 
             { 
                 // In design mode we swallow all exceptions to make editing more friendly 
                 string xaml = ""; 
                 try 
                 { 
                     xaml = await ConvertHtmlToXamlRichTextBlock(xhtml); 
                     newRichText = (RichEditBox)XamlReader.Load(xaml); 
                 } 
               catch (Exception ex) 
                 { 
                     string errorxaml = string.Format(@" 
                         <RichTextBlock  
                          xmlns='http://schemas.microsoft.com/winfx/2/xaml/presentation' 
                          xmlns:x='http://schemas.microsoft.com/winfx/2/xaml' 
                         > 
                             <Paragraph>An exception occurred while converting HTML to XAML: {0}</Paragraph> 
                             <Paragraph /> 
                             <Paragraph>HTML:</Paragraph> 
                           <Paragraph>{1}</Paragraph> 
                             <Paragraph /> 
                             <Paragraph>XAML:</Paragraph> 
                             <Paragraph>{2}</Paragraph> 
                         </RichTextBlock>",
                         ex.Message,
                         EncodeXml(xhtml),
                         EncodeXml(xaml)); 
                     newRichText = (RichEditBox)XamlReader.Load(errorxaml); 
               } // Display a friendly error in design mode. 
             } 
             else 
             { 
                 // When not in design mode, we let the application handle any exceptions 
                 string xaml = ""; 
                 try 
                 { 
                     xaml = await ConvertHtmlToXamlRichTextBlock(xhtml); 
                     newRichText = (RichEditBox)XamlReader.Load(xaml); 
               } 
                 catch (Exception ex) 
                 { 
                     string errorxaml = string.Format(@" 
                         <RichTextBlock  
                          xmlns='http://schemas.microsoft.com/winfx/2/xaml/presentation' 
                          xmlns:x='http://schemas.microsoft.com/winfx/2/xaml' 
                         > 
                             <Paragraph>Cannot convert HTML to XAML. Please ensure that the HTML content is valid.</Paragraph> 
                             <Paragraph /> 
                         <Paragraph>HTML:</Paragraph> 
                             <Paragraph>{0}</Paragraph> 
                         </RichTextBlock>",
                         EncodeXml(xhtml)); 
                     newRichText = (RichEditBox)XamlReader.Load(errorxaml); 
                 } // Display a friendly error in design mode. 
             } 

 
           // Move the blocks in the new RichTextBlock to the target RichTextBlock 
           // richText.Document.Clear(); 
           //if (newRichText != null) 
           //  { 
           //    for (int i = newRichText.Blocks.Count - 1; i >=0 ; i--) 
           //      { 
           //        Block b = newRichText.Blocks[i]; 
           //          newRichText.Blocks.RemoveAt(i); 
           //          richText.Blocks.Insert(0, b); 
           //      } 
           //  } 
        } 
 
 
         private static string EncodeXml(string xml)
         { 
             string encodedXml = xml.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;"); 
             return encodedXml; 
         } 
 
 
         private static XsltProcessor Html2XamlProcessor; 
 
 
        private static async Task<string> ConvertHtmlToXamlRichTextBlock(string xhtml)
         { 
             // Load XHTML fragment as XML document 
             XmlDocument xhtmlDoc = new XmlDocument(); 
             xhtmlDoc.LoadXml(xhtml); 
 
 
             if (Html2XamlProcessor == null) 
             {
                // Read XSLT. In design mode we cannot access the xslt from the file system (with Build Action = Content),  
                // so we use it as an embedded resource instead: 
                //Assembly assembly = typeof(Properties).GetTypeInfo().Assembly; 
                //using (Stream stream = ("WinRT_RichTextBlock.Html2Xaml.RichTextBlockHtml2Xaml.xslt")) 
                string xsltfile = @"Assets\Html2Xaml.RichTextBlockHtml2Xaml.xslt";
                StorageFolder InstallationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                StorageFile file = await InstallationFolder.GetFileAsync(xsltfile);
                using (Stream stream = await file.OpenStreamForReadAsync())

                {
                    StreamReader reader = new StreamReader(stream);
                    string content = await reader.ReadToEndAsync();
                    XmlDocument html2XamlXslDoc = new XmlDocument();
                    html2XamlXslDoc.LoadXml(content);
                    Html2XamlProcessor = new XsltProcessor(html2XamlXslDoc);
                } 
             } 

 
             // Apply XSLT to XML 
             string xaml = Html2XamlProcessor.TransformToString(xhtmlDoc.FirstChild); 
             return xaml; 
         } 
 
 
     } 
 } 

