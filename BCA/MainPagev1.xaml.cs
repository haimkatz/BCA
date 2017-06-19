using BCA.Models;
using BCA.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BCA
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
     enum  theclickedbutton:byte
        {stat,wiki,note,detail,team,blank}
public sealed partial class MainPage : Page
    {
        public ObservableCollection<Event> games { get; set; }
        public ObservableCollection<Player> hplayers { get; set; }
        public ObservableCollection<Player> aplayers { get; set; }
        public Event selectedevent { get; set; }
        public Fuckbinding awayteamname { get; set; }
        public Fuckbinding hometeamname { get; set; }
        public string selectedteamid { get; set; }
        //public string hometeamid { get; set; }
        private theclickedbutton buttonClicked { get; set; }
        // 1 stat  2 wiki 3 note 4 5 detail 6 team
        public MainPage()
        {
            this.InitializeComponent();
            games = new ObservableCollection<Event>();
            hplayers = new ObservableCollection<Player>();
            aplayers = new ObservableCollection<Player>();
            selectedevent = new Event();
            this.Loaded += (sender, e) => Page_Loaded(sender, e);
            PageFrame.Navigate(typeof(BlankPage));
            buttonClicked = theclickedbutton.blank;
            hometeamname = new Fuckbinding();
            awayteamname = new Fuckbinding();
            hometeamname.lastname = "Home";
            awayteamname.lastname = "Away";
        }
        public async void Page_Loaded(object sender, RoutedEventArgs e)
       {
            try
            {
  //Title.Text = "event fired";
            await EBClient.GetGames(games);
            }
            catch (Exception)
            {
                var dialog = new MessageDialog("No Internet");
                await dialog.ShowAsync();
                // throw;
            }
          
        }

        private void GameLV_ItemClick(object sender, ItemClickEventArgs e)
        {
        //    ListViewItem lvi = (ListViewItem)e.ClickedItem;
        //    ListView lv = (ListView)sender;
            var selectedGame = (Event)e.ClickedItem;
            //this.HomeTBtn.Content = selectedGame.home_team.city;
            //this.HomeTBtn.CommandParameter = selectedGame.home_team.team_id;
            //this.AwayTBtn.Content = selectedGame.away_team.city;
            //this.AwayTBtn.CommandParameter = selectedGame.away_team.team_id;

           this.HPlayers.Visibility = Visibility.Collapsed;
            this.APlayers.Visibility = Visibility.Visible;
            getallplayers(selectedGame.home_team.team_id, selectedGame.away_team.team_id);
            this.HomeTBtn.IsEnabled = true;

            this.AwayTBtn.IsEnabled = false;
            this.AwayTBtn.Background = this.APlayers.Background;
            if (selectedevent.event_id != null)
            {
                games.Add(selectedevent);
            }
          //  changeproperties(selectedGame, selectedevent);
            selectedevent = selectedGame;
          hometeamname.lastname = selectedGame.home_team.last_name;
          selectedteamid = selectedGame.away_team.team_id;
          awayteamname.lastname = selectedGame.away_team.last_name;
          //  awayteamid = selectedGame.away_team.team_id;
                    games.Remove(selectedGame);
            //ListViewItem lvi1 = (ListViewItem)lv.Items[0];
            //lvi1.TabIndex = lvi.TabIndex;
            //lvi.TabIndex = 0;
        }

        private  void HomeTBtn_Click(object sender, RoutedEventArgs e)
        {
            this.HomeTBtn.IsEnabled = !HomeTBtn.IsEnabled;
            this.AwayTBtn.IsEnabled = !AwayTBtn.IsEnabled;
           
            if (this.APlayers.Visibility == Visibility.Collapsed) {
                APlayers.Visibility = Visibility.Visible;
                HomeTBtn.Background = new SolidColorBrush(Colors.Beige);
             AwayTBtn.Background    = APlayers.Background;
                selectedteamid = selectedevent.away_team.team_id;
            }
            else
            { APlayers.Visibility = Visibility.Collapsed;
               HomeTBtn.Background     = APlayers.Background;
            AwayTBtn.Background  = new SolidColorBrush(Colors.Beige);
                selectedteamid = selectedevent.home_team.team_id;
            }

            if (this.HPlayers.Visibility == Visibility.Collapsed) { HPlayers.Visibility = Visibility.Visible; }
            else
            { HPlayers.Visibility = Visibility.Collapsed; }
            //  var clickedbutton = (Button)sender;


            //  await Repository.EBClient.GetPLayers(aplayers, (string)clickedbutton.CommandParameter);
        }
        private async void getallplayers(string homeid, string awayid)
        {
            await Repository.EBClient.GetPLayers(hplayers, homeid);
            await Repository.EBClient.GetPLayers(aplayers, awayid);
        }

        private  void Statbtn_Click(object sender, RoutedEventArgs e)
        {
            Button mybutton = (Button)sender;
            string myparam = (string)mybutton.CommandParameter;
            switch (myparam)
            {
                case "S":
                    buttonClicked = theclickedbutton.stat;
                 break;
                case "W":
                    buttonClicked = theclickedbutton.wiki;

                    break;
                case "B":
                    buttonClicked = theclickedbutton.blank;
                    break;
                case "N":
                    buttonClicked = theclickedbutton.note;
                    break;
                case "D":
                    buttonClicked = theclickedbutton.detail;
                    break;
            }
            var playerclicked = getplayerdisplayed();
            NavigateFrame(playerclicked);
        }
        private Player getplayerdisplayed()
        { Player playerclicked;
            if (HPlayers.Visibility == Visibility.Visible) playerclicked = (Player)HPlayers.SelectedItem;
            else playerclicked = (Player)APlayers.SelectedItem;
            return playerclicked;
        }
        private void Players_ItemClick(object sender, ItemClickEventArgs e)
        {
            var playerclicked = (Player)e.ClickedItem;
            NavigateFrame(playerclicked);
                    
            
        }
        private async void NavigateFrame(Player playerclicked)
        {
            string myurl;
            switch (buttonClicked)
            {
                case theclickedbutton.stat:
                    myurl = Repository.EBClient.GetStatURL(playerclicked.first_name, playerclicked.last_name);
                    PageFrame.Navigate(typeof(StatPage), myurl);
                    break;
                case theclickedbutton.wiki:
                    myurl = Repository.EBClient.GetWikiURL(playerclicked.first_name, playerclicked.last_name);
                    PageFrame.Navigate(typeof(StatPage), myurl);
                    break;
                case theclickedbutton.blank:
                    myurl = "";
                    PageFrame.Navigate(typeof(BlankPage));
                    break;
                case theclickedbutton.detail:
                    myurl = "ms-appx-web:///assets/htmlEditor/editor.html";
                    PageFrame.Navigate(typeof(StatPage), myurl);
                    break;
                case theclickedbutton.note:
                    myurl = "";

                    if (playerclicked != null)
                    {
                        PlayerVM p = new PlayerVM(playerclicked, selectedteamid);
                        AZClient azc = new AZClient();

                        PlayerVM pinazure = await azc.isPlayerinAzureasync(p.display_name, selectedteamid);
                        if (pinazure == null) { PageFrame.Navigate(typeof(NoteWF), p); }
                        else { PageFrame.Navigate(typeof(NoteWF), pinazure); }
                    }
                    else { PageFrame.Navigate(typeof(NoteWF)); }
                    break;
            }

        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
