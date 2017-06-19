using BCA.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace BCA.Repository
{
    class EBClient
    {
        private const string AuthHeader = "ede47f5a-64f1-476c-b87b-b5242c0a43aa";
        private const string urlbase = "https://erikberg.com/";
        private const string staturl = "http://www.baseball-reference.com/players/";
        private const string wikiurl = "https://en.wikipedia.org/wiki/";

        public static async Task GetGames(ObservableCollection<Event> games)
        {
            DateTime mydate = DateTime.Today;
             await GetGames(games, mydate);
        }
public static async Task GetGames(ObservableCollection<Event> games, DateTime mydate)
        {
            string datestring = mydate.Year.ToString() + mydate.Month.ToString("0#") + mydate.Day.ToString("0#");
            string apiextension = "events.json?date=" + datestring + "&sport=mlb";
            var jsonmessage = await CallEBAsync(apiextension);
            var serializer = new DataContractJsonSerializer(typeof(Games));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonmessage));
            var mygameWrapper = (Games)serializer.ReadObject(ms);
            games.Clear();
            var returnedgames = mygameWrapper.@event;
            foreach (Event e in returnedgames)
            {
                games.Add(e);
            }


        }
        public static  async Task<string> GetPLayers(ObservableCollection<Player> players,String teamid)
        {
            Team myteamWrapper = null;
            string mystring = String.Empty;
            int tries = 0;
            string apiextension = String.Format("mlb/roster/{0}.json",teamid);
           
            try {
                
                while (tries < 20)
                {
                    var jsonmessage = await CallEBAsync(apiextension);
                    var serializer = new DataContractJsonSerializer(typeof(Team));
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonmessage));

                    myteamWrapper = (Team)serializer.ReadObject(ms);
                    players.Clear();

                    if (myteamWrapper.players == null)
                    { tries += 1; }
                    else { tries = 30; }
                }
                if (tries==20)
                { return "no team"; }
                var returnedplayers =  myteamWrapper.players.Where(p => p.position != "P");
                             addplayers(players, returnedplayers);
               returnedplayers = myteamWrapper.players.Where(p => p.position == "P");
                addplayers(players, returnedplayers);
                return "OK";
            }
            catch (Exception ex) {
               return  mystring += " " + ex.Message; 

            }
            //foreach (Player e in returnedplayers)
            //{
            //    players.Add(e);
            //}

        }
        public static async Task<List<Team>> getteamlistasync()
        {
            string apiextension = "mlb/teams.json";
            var jsonmessage = await CallEBAsync(apiextension);
            var serializer = new DataContractJsonSerializer(typeof(List<Team>));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonmessage));
            var myteamWrapper = (List<Team>)serializer.ReadObject(ms);
            return myteamWrapper;
        }

        private  static void addplayers(ObservableCollection<Player> players, IEnumerable<Player> returnedplayers)
        {
            foreach (Player e in returnedplayers)
            {
                players.Add(e);
            }
        }
       
        private async static Task<string> CallEBAsync(string apiext)
         { 
             // Get the MD5 Hash 
             
             string completeUrl = String.Format("{0}{1}", urlbase,apiext); 
 
 
             // Call out to Marvel 
             HttpClient http = new HttpClient();
            http.DefaultRequestHeaders.Add("Authorization", "Bearer " + AuthHeader);
          //  http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Authorization","Bearer " + AuthHeader);
            http.DefaultRequestHeaders.Add("User-Agent","(katzhd@gmail.com)");
           var response = await http.GetAsync(completeUrl);
            return await response.Content.ReadAsStringAsync(); 
         }

        internal static string GetStatURL(string first_name, string last_name,int numb=1)
        {
           int maxlength = 5;
            int maxflenght = 2;
            if (last_name.Length < 5) maxlength = last_name.Length;
            if (first_name.Length < 2) maxflenght = first_name.Length;
            string response =  string.Format(staturl + "{0}/{1}{2}{3}.shtml", last_name.Substring(0, 1), last_name.Substring(0, maxlength),
                first_name.Substring(0, maxflenght),string.Format("{0:D2}",numb));
            response = RemoveDiacritics(response);
            return response.ToLower();
        }

        internal static string GetWikiURL(string first_name, string last_name)
        {
            string response = string.Format(wikiurl + "{0}_{1}", first_name, last_name);
            response = RemoveDiacritics(response);
            return response;
        }
        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        
    }
}
