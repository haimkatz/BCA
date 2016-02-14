using BCA.Models;
using BCA.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BCA
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NoteWF : Page
    {

        public PlayerVM player { get; set; }
        public NoteWF()
        {

            this.InitializeComponent();
            player = new PlayerVM();

        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {


            base.OnNavigatedTo(e);

            player = (PlayerVM)e.Parameter;
            //if (player.playernote != null)
            //{
            //    await SethtmlTextasync(player.playernote);
            //}

        }

        private async void savebtn_Click(object sender, RoutedEventArgs e)
        {
            AZClient azc = new AZClient();
            player.playernote = await Gethtmlasync();
            //var ms = new InMemoryRandomAccessStream();
            //using (ms)
            //{

            //    //pn.Document.SaveToStream(Windows.UI.Text.TextGetOptions.FormatRtf,ms);
            //    // Repository.HTMLConverter.HtmlToRtfConverter.ConvertHtmlToRtf.
            //}
            var response = await azc.SavePlayerasync(player);
            if (response == false)
            {
                var dialog = new MessageDialog("Didn't work");
                await dialog.ShowAsync();
            }
        }
        private async void getClipboard(object sender, RoutedEventArgs e)
        {
           String myresults;
            DataPackageView dataPackageView = Clipboard.GetContent();
            if (dataPackageView.Contains(StandardDataFormats.Html))
                {
                myresults = await dataPackageView.GetHtmlFormatAsync();
                           }
           else if (dataPackageView.Contains(StandardDataFormats.Text))
            {
                myresults = await dataPackageView.GetTextAsync();
            }
            else { myresults = string.Empty; }

            await SethtmlTextasync(myresults);

            //DataPackageView dataPackageView = Clipboard.GetContent();
            //if (dataPackageView.Contains(StandardDataFormats.Text))
            //{
            //    string text = await dataPackageView.GetTextAsync();
            //    // To output the text from this example, you need a TextBlock control
            //    //TextOutput.Text = "Clipboard now contains: " + text;
            //    //string ConvertRichTextBoxContentsToString(RichTextBlock rtb)
            //}
        }

        private void pn_Paste(object sender, TextControlPasteEventArgs e)
        {
            DataPackageView dataPackageView = Clipboard.GetContent();
            var text = dataPackageView.GetTextAsync();
            var rtf = dataPackageView.GetRtfAsync();
            var html = dataPackageView.GetHtmlFormatAsync();

            var a = e.ToString();
        }
        private async  Task  SethtmlTextasync(String mytext)
        {
            await pn.InvokeScriptAsync("setText", new string[] { mytext });
        }
        private async Task Sethtmlasync(string mytext)
               {
            await pn.InvokeScriptAsync("setHtml", new string[] { mytext });
        }

        private async Task<String> Gethtmlasync()
        {
            return await pn.InvokeScriptAsync("getHtml", null);
        }

        private async void pn_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            if (player != null)
            {
                if (player.playernote != null)
                {
                    await Sethtmlasync(player.playernote);
                }
            }
        }
    }
        //{

        //    TextRange textRange = new TextRange(rtb.Document.ContentStart,

        //        rtb.Document.ContentEnd);
        
        //    return textRange.Text;

        //}

    }

