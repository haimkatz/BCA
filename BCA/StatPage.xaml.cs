﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class StatPage : Page
    {
        public StatPage()
        {
            this.InitializeComponent();
        }
      protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string myurl = (string)e.Parameter;
            if (myurl!= null)
                if(myurl.ToLower().StartsWith("http"))
            {
                StatWView.Source = new Uri(myurl, UriKind.Absolute);
            }
                else
                {
                    StatWView.NavigateToString(myurl);
                }
            
        }

        public void Renavigate(string myurl)
        {
            this.StatWView.Source = new Uri(myurl, UriKind.Absolute);
        }
    }
}
