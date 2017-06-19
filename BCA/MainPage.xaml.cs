using BCA.Models;
using BCA.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.MobileServices;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BCA
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    enum theclickedbutton : byte
    {
        stat,
        wiki,
        note,
        detail,
        team,
        game,
        note_ro,
        blank
    }

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
        private string butshow { get; set; }
        private bool _panestate = true;
        private int statnum { get; set; }
        private AZMClient client;
        private AZClient azc = new AZClient();
       // private AZMClient azc = new AZMClient();

        private UserInfo thisuser;
        private bool panestate
        {
            get { return _panestate; }
            set
            {
                if (value == false)
                {
                    AwayTBtn.Visibility = Visibility.Collapsed;
                    HomeTBtn.Visibility = Visibility.Collapsed;
                    butshow = '\uE0E3'.ToString();
                }
                else
                {
                    AwayTBtn.Visibility = Visibility.Visible;
                    HomeTBtn.Visibility = Visibility.Visible;
                    butshow = '\uE0E2'.ToString();
                    ;
                }
                _panestate = value;
            }
        }

        // 1 stat  2 wiki 3 note 4 5 detail 6 team
        public MainPage()
        {
            this.InitializeComponent();
            games = new ObservableCollection<Event>();
            hplayers = new ObservableCollection<Player>();
            aplayers = new ObservableCollection<Player>();
            selectedevent = new Event();
            this.Loaded += (sender, e) => Page_Loaded(sender, e);
            PageFrame.Navigate(typeof (BlankPage));
            buttonClicked = theclickedbutton.blank;
            hometeamname = new Fuckbinding();
            awayteamname = new Fuckbinding();
            hometeamname.lastname = "Home";
            awayteamname.lastname = "Away";
            this.HomeTBtn.Visibility = Visibility.Collapsed;
            this.AwayTBtn.Visibility = Visibility.Collapsed;

            statnum = 1;
          
        }
        // Define a member variable for storing the signed-in user. 
        private MobileServiceUser user;
        public async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ProgRing.IsActive = true;
                this.ProgRing.Visibility = Visibility.Visible;
                this.MySplitView.IsPaneOpen = true;
                //Title.Text = "event fired";
                await EBClient.GetGames(games);
                await AuthenticateAsync();
                if (games.Count == 0)
                {
                    txtFeedback.Text = "No games for this date,  choose another date.";

                }
                HamburgerButton.IsEnabled = false;
            }
            catch (Exception ex3)
            {

                this.txtFeedback.Text = "No Internet";
                // throw;
            }
            finally
            {
                this.ProgRing.IsActive = false;
                this.ProgRing.Visibility = Visibility.Collapsed;
            }

        }

        private void GameLV_ItemClick(object sender, ItemClickEventArgs e)
        {
            txtFeedback.Text = String.Empty;
            //    ListViewItem lvi = (ListViewItem)e.ClickedItem;
            //    ListView lv = (ListView)sender;
            var selectedGame = (Event) e.ClickedItem;
            //this.HomeTBtn.Content = selectedGame.home_team.city;
            //this.HomeTBtn.CommandParameter = selectedGame.home_team.team_id;
            //this.AwayTBtn.Content = selectedGame.away_team.city;
            //this.AwayTBtn.CommandParameter = selectedGame.away_team.team_id;
            HomeTBtn.Visibility = Visibility.Visible;
            AwayTBtn.Visibility = Visibility.Visible;
            this.HPlayers.Visibility = Visibility.Collapsed;
            this.APlayers.Visibility = Visibility.Visible;
            getallplayers(selectedGame.home_team.team_id, selectedGame.away_team.team_id);
            this.HomeTBtn.IsEnabled = true;

            AwayTBtn.IsEnabled = false;
            APlayers.Background = this.APlayers.Background;
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
            // ****************
            //ListViewItem lvi1 = (ListViewItem)lv.Items[0];
            //lvi1.TabIndex = lvi.TabIndex;
            //lvi.TabIndex = 0;
            HamburgerButton.IsEnabled = true;
        }

        private void HomeTBtn_Click(object sender, RoutedEventArgs e)
        {
            this.HomeTBtn.IsEnabled = !HomeTBtn.IsEnabled;
            this.AwayTBtn.IsEnabled = !AwayTBtn.IsEnabled;

            if (this.APlayers.Visibility == Visibility.Collapsed)
            {
                APlayers.Visibility = Visibility.Visible;
                HomeTBtn.Background = new SolidColorBrush(Colors.Beige);
                AwayTBtn.Background = APlayers.Background;
                selectedteamid = selectedevent.away_team.team_id;
            }
            else
            {
                APlayers.Visibility = Visibility.Collapsed;
                HomeTBtn.Background = APlayers.Background;
                AwayTBtn.Background = new SolidColorBrush(Colors.Beige);
                selectedteamid = selectedevent.home_team.team_id;
            }

            if (this.HPlayers.Visibility == Visibility.Collapsed)
            {
                HPlayers.Visibility = Visibility.Visible;
            }
            else
            {
                HPlayers.Visibility = Visibility.Collapsed;
            }
            //  var clickedbutton = (Button)sender;

            if (buttonClicked == theclickedbutton.team)
            {
                naviateteamnoteasync();
            }
            //  await Repository.EBClient.GetPLayers(aplayers, (string)clickedbutton.CommandParameter);
        }

        private async void getallplayers(string homeid, string awayid)
        {
            string myresponse = await Repository.EBClient.GetPLayers(hplayers, homeid);
            string myresponse1 = await Repository.EBClient.GetPLayers(aplayers, awayid);
            if (myresponse != "OK" | myresponse1 != "OK")
            {
                this.txtFeedback.Text = "Problem getting player information, try again later";
            }
            else this.txtFeedback.Text = String.Empty;
        }

        private void Statbtn_Click(object sender, RoutedEventArgs e)
        {
            txtFeedback.Text = string.Empty;
            MenuFlyoutItem mybutton = (MenuFlyoutItem) sender;
            string myparam = (string) mybutton.CommandParameter;
            switch (myparam)
            {
                case "S":
                    buttonClicked = theclickedbutton.stat;
                    StatInc.Visibility = Visibility.Visible;
                    break;
                case "W":
                    buttonClicked = theclickedbutton.wiki;
                    StatInc.Visibility = Visibility.Collapsed;
                    break;
                case "B":
                    buttonClicked = theclickedbutton.blank;
                    StatInc.Visibility = Visibility.Collapsed;
                    break;
                case "N":
                    buttonClicked = theclickedbutton.note;
                    StatInc.Visibility = Visibility.Collapsed;
                    break;
                case "G":
                    buttonClicked = theclickedbutton.game;
                    StatInc.Visibility = Visibility.Collapsed;
                    navigategamenoteasync();
                    break;
                case "D":
                    buttonClicked = theclickedbutton.detail;
                    StatInc.Visibility = Visibility.Collapsed;
                    break;
                case "T":
                    buttonClicked = theclickedbutton.team;
                    StatInc.Visibility = Visibility.Collapsed;
                    naviateteamnoteasync();
                    break;
                case "R":
                    buttonClicked = theclickedbutton.note_ro;
                    StatInc.Visibility = Visibility.Collapsed;
                    break;
            }

            var playerclicked = getplayerdisplayed();
            NavigateFrame(playerclicked);
        }

        private async void naviateteamnoteasync()
        {
            TeamVM team = await azc.GetTeambyIDasync(selectedteamid);
            PageFrame.Navigate(typeof (TeamWF), team);
        }

        private Player getplayerdisplayed()
        {
            Player playerclicked;
            if (HPlayers.Visibility == Visibility.Visible)
            {
                if (HPlayers.SelectedItem == null)
                {
                    playerclicked = (Player) HPlayers.Items[0];
                }
                else
                {
                    playerclicked = (Player) HPlayers.SelectedItem;
                }
            }
            else
            {
                if (APlayers.SelectedItem == null)
                {
                    playerclicked = (Player) APlayers.Items[0];
                }

                else
                {
                    playerclicked = (Player) APlayers.SelectedItem;
                }
            }
            return playerclicked;
        }

        private void Players_ItemClick(object sender, ItemClickEventArgs e)
        {
            var playerclicked = (Player) e.ClickedItem;
            statnum = 1;
            NavigateFrame(playerclicked);


        }

        private async void NavigateFrame(Player playerclicked)
        {
            string myurl;
            switch (buttonClicked)
            {
                case theclickedbutton.stat:
                    myurl = Repository.EBClient.GetStatURL(playerclicked.first_name, playerclicked.last_name);
                    PageFrame.Navigate(typeof (StatPage), myurl);
                    break;
                case theclickedbutton.wiki:
                    myurl = Repository.EBClient.GetWikiURL(playerclicked.first_name, playerclicked.last_name);
                    PageFrame.Navigate(typeof (StatPage), myurl);
                    break;
                case theclickedbutton.blank:
                    myurl = "";
                    PageFrame.Navigate(typeof (BlankPage));
                    break;
                case theclickedbutton.detail:
                    myurl = "ms-appx-web:///assets/htmlEditor/editor.html";
                    PageFrame.Navigate(typeof (StatPage), myurl);
                    break;
                case theclickedbutton.note:
                case theclickedbutton.note_ro:
                    myurl = "";

                    if (playerclicked != null)
                    {
                        PlayerVM p = new PlayerVM(playerclicked, selectedteamid);
                        //  AZMClient azc = new AZMClient();
                      //  AZClient azc = new AZClient();
                        try

                        {


                            PlayerVM pinazure = await azc.isPlayerinAzureasync(p.display_name, selectedteamid);
                            if (pinazure == null)
                            { 
                                PageFrame.Navigate(typeof (NoteWF), p);
                            }
                            else
                            {
                                if (buttonClicked == theclickedbutton.note)
                                {
                                    PageFrame.Navigate(typeof (NoteWF), pinazure);
                                }
                                else
                                {
                                    PageFrame.Navigate(typeof (StatPage), pinazure.playernote);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            this.txtFeedback.Text = "Problem getting note from internet.";
                        }
                    }

                    else
                    {
                        PageFrame.Navigate(typeof (NoteWF));
                    }
                    break;

            }

        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            string a = "Hello Menu";
        }

        private void toggleSV_Click(object sender, RoutedEventArgs e)
        {
            this.MySplitView.IsPaneOpen = !this.MySplitView.IsPaneOpen;
            panestate = !panestate;
            toggleSV.Content = butshow;
        }

        private async void Gamedate_OnDateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            try
            {
                this.txtFeedback.Text = String.Empty;
                this.ProgRing.Visibility = Visibility.Visible;
                ProgRing.IsActive = true;
                await EBClient.GetGames(games, e.NewDate.Date);
                if (games.Count == 0)
                {
                    txtFeedback.Text = "No games for this date,  choose another date.";

                }
            }
            catch (Exception)
            {

                this.txtFeedback.Text = "No Internet";
                // throw;
            }
            finally
            {
                this.ProgRing.IsActive = false;
                this.ProgRing.Visibility = Visibility.Collapsed;
            }
        }

        private async void navigategamenoteasync()
        {
            string myurl = "";

            if (selectedevent.event_id != null)
            {
                Event e = selectedevent; //new Event(selectedevent, selectedteamid);
                                         // AZMClient azc = new AZMClient();
             //   AZClient azc = new AZClient();
                try

                {
                    Event pinazure = await azc.isEventinAzureasync(e.event_id);
                    if (pinazure == null)
                    {
                        PageFrame.Navigate(typeof (GameNote), e);
                    }
                    else
                    {
                        PageFrame.Navigate(typeof (GameNote), pinazure);
                    }
                }
                catch
                {
                    this.txtFeedback.Text = "Problem getting note from internet.";
                }
            }

            else
            {
                this.txtFeedback.Text = "No game selected";
            }

        }

        private void BtnGamesToggle_OnClick(object sender, RoutedEventArgs e)
        {
            Button mybtn = (Button) sender;
            if (GameLV.Visibility == Visibility.Collapsed)
            {
                GameLV.Visibility = Visibility.Visible;
                mybtn.Content = '\uE09D'.ToString();
            }
            else
            {
                GameLV.Visibility = Visibility.Collapsed;
                mybtn.Content = '\uE014'.ToString();
            }
        }

        private void IncreaseStat(object sender, RoutedEventArgs e)
        {
            statnum += 1;
            string myurl;
            var playerclicked = getplayerdisplayed();
            myurl = Repository.EBClient.GetStatURL(playerclicked.first_name, playerclicked.last_name, statnum);
            PageFrame.Navigate(typeof (StatPage), myurl);

        }

        // Define a member variable for storing the signed-in user. 


        // Define a method that performs the authentication process
        // using a Facebook sign-in. 
        private async System.Threading.Tasks.Task<bool> AuthenticateAsync()
        {
            string message;
            bool success = false;
           client = new AZMClient();
            // This sample uses the Microsoft provider.
            var provider = MobileServiceAuthenticationProvider.MicrosoftAccount;

            // Use the PasswordVault to securely store and access credentials.
            PasswordVault vault = new PasswordVault();
            PasswordCredential credential = null;

            try
            {
                // Try to get an existing credential from the vault.
                credential = vault.FindAllByResource(provider.ToString()).FirstOrDefault();
            }
            catch (Exception)
            {
                // When there is no matching resource an error occurs, which we ignore.
            }

            if (credential != null)
            {
                // Create a user from the stored credentials.
                user = new MobileServiceUser(credential.UserName);
                credential.RetrievePassword();
                user.MobileServiceAuthenticationToken = credential.Password;

                // Set the user from the stored credentials.
                client.AZMClientSetUser(user);

                // Consider adding a check to determine if the token is 
                // expired, as shown in this post: http://aka.ms/jww5vp.
                //  Check if there is authorization
                try
                {
                 if ( client.istokenexpired())
                    {
                        await client.refreshtoken();
                    }
                    await client.IsUserAuthenticated();
                    message = savecredentials(vault, credential, provider);
                    success = true;
                    
                }
                //if not try to refresh token
                catch ( MobileServiceInvalidOperationException ex)
               {
                    if (ex.Response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        try
                        {
                            //user = await client.RefreshTokenasync();
                            //   client.AZMClientSetUser(user);
                            success = await NewLoginAsync(vault, credential, provider);
                            message = savecredentials(vault, credential, provider);
                            success = true;
                           
                        }
                        catch (Exception ex1)
                        {
                            
                            success= await NewLoginAsync(vault, credential, provider);
                            message =  savecredentials(vault, credential, provider);
                          
                        }


                }
               

                
                message = string.Format("Cached credentials for user - {0}", user.UserId);
            }
            else
            {
                try
                {
                    // Login with the identity provider.
                    success = await NewLoginAsync(vault, credential, provider);


                    // Create and store the user credentials.


                    message = this.savecredentials(vault, credential, provider);
                }
                catch (MobileServiceInvalidOperationException)
                {
                    message = "You must log in. Login Required";
                }
            }
            var userInfo = await client.getuserinfo();


            await callDialogasync(success, message);
            
            return success;
        }

        private async Task callDialogasync(bool success, string message)
        {
            var dialog = new MessageDialog(message);
            dialog.Commands.Add(new UICommand("OK"));
            if (success)
            {
                dialog.Title = "Successful Login";
            }
            else
            {
                dialog.Title = "You were not able to login to your provider";}
            await dialog.ShowAsync();
        }

        private async System.Threading.Tasks.Task<bool> NewLoginAsync(PasswordVault vault, PasswordCredential credential,
           MobileServiceAuthenticationProvider provider )
        {
            user = await client
                       .LoginAsync(provider);

            // Create and store the user credentials.
            savecredentials(vault, credential, provider);
            return true;
           
                }

        private string savecredentials(PasswordVault vault, PasswordCredential credential, MobileServiceAuthenticationProvider provider)
        {
            // Create and store the user credentials.
            credential = new PasswordCredential(provider.ToString(),
                user.UserId, user.MobileServiceAuthenticationToken);
            vault.Add(credential);
            client.AZMClientSetUser(user);
            
            return  string.Format("You are now logged in - {0}", user.UserId);
        }

        private  void Logon_OnClick(object sender, RoutedEventArgs e)
        {
          var myres =   AuthenticateAsync().Result;
        }
    }
    }
