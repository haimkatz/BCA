using BCA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.System;
using Windows.UI.Notifications;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace BCA.Repository
{
    class AZClient
    {
        public MobileServiceUser CurrentUser;
       // private const string urlbase = "http://localhost:63495/api/";
       private const string urlbase = "https://gamenote2.azurewebsites.net/api/";
        private const string getteamapi = "Teams?teamid={0}";
        private const string geteventapi = "Events?eventid={0}";

        public AZClient()
        {
            
        }

        public AZClient(MobileServiceUser user)
        {
            this.CurrentUser = user;
        }
        private  async Task<HttpResponseMessage> GetAZAsync(string apiext)
        {
            // Get the MD5 Hash 
           
            string completeUrl = String.Format("{0}{1}", urlbase, apiext);


            // Call out to AZ 
            using (var http = new HttpClient())
            {

             //   http.BaseAddress = new Uri(completeUrl);
             HttpRequestMessage rq = new HttpRequestMessage()
             {
                 RequestUri = new Uri(completeUrl),
                 Method = HttpMethod.Get
             };
                addauthheader(rq);
               

                // http.DefaultRequestHeaders.Add("Authorization", "Bearer " + AuthHeader);
                // http.DefaultRequestHeaders.Add("User-Agent", "(katzhd@gmail.com)");
                var response = await http.SendAsync(rq);
                return response;
            }
        }
        public  async Task<TeamVM> GetTeambyIDasync(string team_id)
        {
            string apiextension = String.Format(getteamapi,team_id);
            HttpResponseMessage jsonmessage = await GetAZAsync(apiextension);
            if (jsonmessage.IsSuccessStatusCode)
            {
               TeamVM team = await jsonmessage.Content.ReadAsAsync<TeamVM>();
                //var serializer = new DataContractJsonSerializer(typeof(List<Team>));
                //var jstring = await jsonmessage.Content.ReadAsStringAsync();
                //var ms = new MemoryStream(Encoding.UTF8.GetBytes(jstring));
                //var myteamWrapper = (List<Team>)serializer.ReadObject(ms);
                //return myteamWrapper.FirstOrDefault();
                return team;
            }

            else
                return null;

        }

      

        public   async Task<HttpResponseMessage> saveTeamasync(TeamVM team)

       {
            using (var http = new HttpClient())
            {

                var myuri = new Uri(urlbase + String.Format("Teams/{0}", team.Id));

                HttpRequestMessage rq = new HttpRequestMessage()
                {
                    RequestUri = myuri,
                    Method = HttpMethod.Post,
                    Content = new StringContent(getJsonfromEntity(team),Encoding.UTF8,"application/json")
                };
                addauthheader(rq);
                // http.DefaultRequestHeaders.Add("Authorization", "Bearer " + AuthHeader);
                // http.DefaultRequestHeaders.Add("User-Agent", "(katzhd@gmail.com)");
                var response = await http.SendAsync(rq);
                return response;
            }

        }

        private  async Task<string> getteamazidasync(string team_id)
        {
            try
            {
 TeamVM myteam = await GetTeambyIDasync(team_id);
                return myteam.Id;

            }
            catch (Exception)
            {

                return "Failed";
            }
           
            
        }

        public  async Task postteamsasync()
        {
            EBClient eb = new EBClient();
            List<Team> allteams = await EBClient.getteamlistasync();
            foreach(Team t in allteams)
            {
              if (await GetTeambyIDasync(t.team_id)== null)
                {
                    { await createteamasync(t); }
                }
            }

        }

        private async  Task createteamasync(Team t)
        {
            using (var http = new HttpClient())
            {

                var myuri = new Uri(urlbase + "Teams");

                HttpRequestMessage rq = new HttpRequestMessage()
                {
                    RequestUri = myuri,
                    Method = HttpMethod.Post,
                    Content = new StringContent(getJsonfromEntity(t), Encoding.UTF8, "application/json")
                };

                // http.DefaultRequestHeaders.Add("Authorization", "Bearer " + AuthHeader);
                // http.DefaultRequestHeaders.Add("User-Agent", "(katzhd@gmail.com)");
                var response = await http.SendAsync(rq);
                if (response.IsSuccessStatusCode)
                {
                    Uri team = response.Headers.Location;

                }

            }

           
        }

        private async  Task saveteamasync(Team t)
        {
            using (var http = new HttpClient())
            {
                var myuri = new Uri(urlbase + "Teams");

                HttpRequestMessage rq = new HttpRequestMessage()
                {
                    RequestUri = myuri,
                    Method = HttpMethod.Post,
                    Content = new StringContent(getJsonfromEntity(t), Encoding.UTF8, "application/json")
                };
                // http.DefaultRequestHeaders.Add("Authorization", "Bearer " + AuthHeader);
                // http.DefaultRequestHeaders.Add("User-Agent", "(katzhd@gmail.com)");
                var response = await http.SendAsync(rq);
                if (response.IsSuccessStatusCode)
                {
                    Uri team = response.Headers.Location;
                }
            }
        }
        #region "Players"
        public async Task<bool> SavePlayerasync (PlayerVM p)
        {
            PlayerVM azplayer = await isPlayerinAzureasync(p.display_name, p.team_id);
            if (azplayer == null) { HttpResponseMessage response = await postplayerasync(p);
                return response.IsSuccessStatusCode;
            }
            else
            { return false;  } //save with put method
        }
        public async Task<HttpResponseMessage> postplayerasync(PlayerVM p)        {
            
                using (var http = new HttpClient())
                {
               
                var myuri = new Uri(urlbase + "Players");

                HttpRequestMessage rq = new HttpRequestMessage()
                {
                    RequestUri = myuri,
                    Method = HttpMethod.Post,
                    Content = new StringContent(getJsonfromEntity(p), Encoding.UTF8, "application/json")
                };
                addauthheader(rq);
                // http.DefaultRequestHeaders.Add("Authorization", "Bearer " + AuthHeader);
                // http.DefaultRequestHeaders.Add("User-Agent", "(katzhd@gmail.com)");
                var response = await http.SendAsync(rq);
                return response;

                }

            }
            public async Task<PlayerVM> isPlayerinAzureasync(string displayname, string teamid)
        {
           
               
                string apiextension = String.Format("Players?displayname={0}&teamid={1}", displayname, teamid);
                HttpResponseMessage jsonmessage = await GetAZAsync(apiextension);
                if (jsonmessage.IsSuccessStatusCode)
                {
                 PlayerVM p = await jsonmessage.Content.ReadAsAsync<PlayerVM>();
                    //var serializer = new DataContractJsonSerializer(typeof(List<Team>));
                    //var jstring = await jsonmessage.Content.ReadAsStringAsync();
                    //var ms = new MemoryStream(Encoding.UTF8.GetBytes(jstring));
                    //var myteamWrapper = (List<Team>)serializer.ReadObject(ms);
                    //return myteamWrapper.FirstOrDefault();
                    return p;
                }

                else
                    return null;
            
        }
        public async Task<bool> PutPLayerasync(PlayerVM player)
        {
            using (var http = new HttpClient())
            {
               
                var myuri = new Uri(urlbase + string.Format("Players/{0}", player.id));

                HttpRequestMessage rq = new HttpRequestMessage()
                {
                    RequestUri = myuri,
                    Method = HttpMethod.Put,
                    Content = new StringContent(getJsonfromEntity(player), Encoding.UTF8, "application/json")
                };
                addauthheader(rq);

                var response = await http.SendAsync(rq);
                return response.IsSuccessStatusCode;
            }
        }

        #endregion
        #region "events"
        public async Task<bool> Saveeventasync(Event e)
        {
            Event azevent = await isEventinAzureasync( e.event_id);
            if (azevent == null)
            {
                HttpResponseMessage response = await posteventasync(e);
                return response.IsSuccessStatusCode;
            }
            else
            { return false; } //save with put method
        }
        public async Task<HttpResponseMessage> posteventasync(Event e)
        {

            using (var http = new HttpClient())
            {

                var myuri = new Uri(urlbase + "Events");

                // http.DefaultRequestHeaders.Add("Authorization", "Bearer " + AuthHeader);
                // http.DefaultRequestHeaders.Add("User-Agent", "(katzhd@gmail.com)");
                var response = await http.PostAsJsonAsync(myuri, e);

                return response;

            }

        }
        public async Task<Event> isEventinAzureasync( string eventid)
        {
            using (var http = new HttpClient())
            {
                string apiextension = String.Format(geteventapi, eventid);
               // string apiextension = String.Format("Events?eventid={0}",  eventid);
                HttpResponseMessage jsonmessage = await GetAZAsync(apiextension);
                if (jsonmessage.IsSuccessStatusCode)
                {
                    Event e = await jsonmessage.Content.ReadAsAsync<Event>();
                    //var serializer = new DataContractJsonSerializer(typeof(List<Team>));&eventid={1}"
                    //var jstring = await jsonmessage.Content.ReadAsStringAsync();
                    //var ms = new MemoryStream(Encoding.UTF8.GetBytes(jstring));
                    //var myteamWrapper = (List<Team>)serializer.ReadObject(ms);
                    //return myteamWrapper.FirstOrDefault();
                    return e;
                }

                else
                    return null;
            }
        }
        #endregion
        #region "Login"
        public async Task<MobileServiceUser> LoginAsync(MobileServiceAuthenticationProvider provider)
        {
            
           MobileServiceClient myservice =  new MobileServiceClient(urlbase);
           return await myservice.LoginAsync(provider);
        }

        private static void  addauthheader(HttpRequestMessage rq)
        {
            MobileServiceUser user = App.client.CurrentUser;
           rq.Headers.Add("X-ZUMO-FEATURES", "AT,QS");                    
            rq.Headers.Add("X-ZUMO-INSTALLATION-ID",
                "ff90f37e-0c03-4c52-a343-af711752e383");
            rq.Headers.Add("X-ZUMO-AUTH", user.MobileServiceAuthenticationToken);
           rq.Headers.Add("Accept", "application/json");
            rq.Headers.Add("User-Agent", "ZUMO/2.1");
            rq.Headers.Add("User-Agent",
                "(lang = Managed; os = Windows Store; os_version = --; arch = X86; version = 2.1.40707.0)");
            rq.Headers.Add("X-ZUMO-VERSION",
                "ZUMO/2.1(lang = Managed; os = Windows Store; os_version = --; arch = X86; version = 2.1.40707.0)");
            rq.Headers.Add("ZUMO-API-VERSION", "2.0.0");
            //rq.Headers.Add("content-type", "application/json");

        }
        #endregion
        #region "UserInfo"

    

        public async Task<UserInfo> isPUserinAzureasync(UserInfo userinfo)
        {


            string apiextension = String.Format("UserInfoes?account_id=" + userinfo.account_id);
            HttpResponseMessage jsonmessage = await GetAZAsync(apiextension);
            if (jsonmessage.IsSuccessStatusCode)
            {
                UserInfo u = await jsonmessage.Content.ReadAsAsync<UserInfo>();
                //var serializer = new DataContractJsonSerializer(typeof(List<Team>));
                //var jstring = await jsonmessage.Content.ReadAsStringAsync();
                //var ms = new MemoryStream(Encoding.UTF8.GetBytes(jstring));
                //var myteamWrapper = (List<Team>)serializer.ReadObject(ms);
                //return myteamWrapper.FirstOrDefault();
                return u;
            }

            else
                return await postUserasync(userinfo);

        }

        private async Task<UserInfo> postUserasync(UserInfo userinfo)
        {
            using (var http = new HttpClient())
            {

                var myuri = new Uri(urlbase + "UserInfoes");

                HttpRequestMessage rq = new HttpRequestMessage()
                {
                    RequestUri = myuri,
                    Method = HttpMethod.Post,
                    Content = new StringContent(getJsonfromEntity(userinfo), Encoding.UTF8, "application/json")
                };
                addauthheader(rq);
                // http.DefaultRequestHeaders.Add("Authorization", "Bearer " + AuthHeader);
                // http.DefaultRequestHeaders.Add("User-Agent", "(katzhd@gmail.com)");
                var response = await http.SendAsync(rq);
                UserInfo u = await response.Content.ReadAsAsync<UserInfo>();
                //var serializer = new DataContractJsonSerializer(typeof(List<Team>));
                //var jstring = await jsonmessage.Content.ReadAsStringAsync();
                //var ms = new MemoryStream(Encoding.UTF8.GetBytes(jstring));
                //var myteamWrapper = (List<Team>)serializer.ReadObject(ms);
                //return myteamWrapper.FirstOrDefault();
                return u;
            }
        }


        #endregion

        private static string getJsonfromEntity(Object myentity)
        {
          return  JsonConvert.SerializeObject(myentity, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
        }

    }

}
