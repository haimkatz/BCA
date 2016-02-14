using BCA.Models;
using BCA.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class PlayerNote : Page
    {
        public PlayerVM player { get; set; }
        public PlayerNote()
        {
            this.InitializeComponent();
            player = new PlayerVM();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
           

            base.OnNavigatedTo(e);

 player = (PlayerVM)e.Parameter;
           
        }

        private async void savebtn_Click(object sender, RoutedEventArgs e)
        {
            AZClient azc = new AZClient();
            var response = await azc.SavePlayerasync(player);
                if (response == false) {
                var dialog = new MessageDialog("Didn't work"); 
                await dialog.ShowAsync();
            }
        }
    }
}
