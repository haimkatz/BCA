using BCA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace BCA.Repository
{
    class AZMClient
    {
        private MobileServiceClient client;
        private MobileServiceUser CurrentUser;
        // private const string urlbase = "http://localhost:63495/api/";
        private const string url = "https://gamenote2.azurewebsites.net/";
        private const string getteamapi = "Teams?teamid={0}";
        private const string geteventapi = "Events?eventid={0}";
        private string urlbase = "";// "api/";
        public AZMClient()
        {
            client = App.client;
        }

        public void AZMClientSetUser(MobileServiceUser user)
        {
            client.CurrentUser = user;
        }
        public AZMClient(MobileServiceClient iclient)
        {
            client = iclient;
        }
        private async Task<HttpResponseMessage> GetAZMAsync(string apiext)
        {
            // Get the MD5 Hash 

            string myuri = String.Format("{0}{1}", urlbase, apiext);


            // Call out to AZ 
            //using (var http = new HttpClient())
            //{

            //    http.BaseAddress = new Uri(completeUrl);

            //    // http.DefaultRequestHeaders.Add("Authorization", "Bearer " + AuthHeader);
            //    // http.DefaultRequestHeaders.Add("User-Agent", "(katzhd@gmail.com)");
            //    var response = await http.GetAsync(apiext);
            //    return response;
            //}
            var response = await client.InvokeApiAsync<HttpResponseMessage>(myuri, System.Net.Http.HttpMethod.Get, null);
            return response;
        }
        private async Task<HttpResponseMessage> GetAZMAsyncP(string apiext, IDictionary<string,string> param )
        {
            // Get the MD5 Hash 

            string myuri = String.Format("{0}{1}", urlbase, apiext);


            // Call out to AZ 
            //using (var http = new HttpClient())
            //{

            //    http.BaseAddress = new Uri(completeUrl);

            //    // http.DefaultRequestHeaders.Add("Authorization", "Bearer " + AuthHeader);
            //    // http.DefaultRequestHeaders.Add("User-Agent", "(katzhd@gmail.com)");
            //    var response = await http.GetAsync(apiext);
            //    return response;
            //}
            var response = await client.InvokeApiAsync<HttpResponseMessage>(myuri, System.Net.Http.HttpMethod.Get, param);
            return response;
        }

        public async  Task<TeamVM> GetTeambyIDasync(string team_id)
        {
            string apiextension = String.Format(getteamapi, team_id);
            IDictionary<string,string> param = new Dictionary<string, string>();
            param.Add("teamid",team_id);
        HttpResponseMessage jsonmessage =
            await client.InvokeApiAsync<HttpResponseMessage>(apiextension,HttpMethod.Get,param);
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



        public  async Task<HttpResponseMessage> saveTeamasync(TeamVM team)

        {
            //using (var http = new HttpClient())
            //{

            //    var myuri = new Uri(urlbase + String.Format("Teams/{0}", team.Id));

            //    // http.DefaultRequestHeaders.Add("Authorization", "Bearer " + AuthHeader);
            //    // http.DefaultRequestHeaders.Add("User-Agent", "(katzhd@gmail.com)");
            //    var response = await http.PutAsJsonAsync(myuri, team);
            //    return response;
            //}
            HttpResponseMessage jsonmessage =
                await client.InvokeApiAsync<TeamVM, HttpResponseMessage>(String.Format("Teams/{0}", team.Id),
                    team,new CancellationToken());
            return jsonmessage;
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
            foreach (Team t in allteams)
            {
                if (await GetTeambyIDasync(t.team_id) == null)
                {
                    { await createteamasync(t); }
                }
            }

        }

        private async  Task createteamasync(Team t)
        {
           

               String myuri = (urlbase + "Teams");

                // http.DefaultRequestHeaders.Add("Authorization", "Bearer " + AuthHeader);
                // http.DefaultRequestHeaders.Add("User-Agent", "(katzhd@gmail.com)");
                ////var response = await http.PostAsJsonAsync(myuri, t);
                var response =
                    await client.InvokeApiAsync<Team, HttpResponseMessage>(myuri, t, CancellationToken.None);
                if (response.IsSuccessStatusCode)
                {
                    Uri team = response.Headers.Location;

                }

            


        }

        private async  Task saveteamasync(Team t)
        {

            String myuri = (urlbase + "Teams");
                // http.DefaultRequestHeaders.Add("Authorization", "Bearer " + AuthHeader);
                // http.DefaultRequestHeaders.Add("User-Agent", "(katzhd@gmail.com)");
                //var response = await http.PutAsJsonAsync(myuri, t);
            var response =
                await
                    client.InvokeApiAsync<Team, HttpResponseMessage>(myuri, t, HttpMethod.Put, null,
                        CancellationToken.None);
                if (response.IsSuccessStatusCode)
                {
                    Uri team = response.Headers.Location;
                }
            
        }
        #region "Players"
        public async Task<bool> SavePlayerasync(PlayerVM p)
        {
            PlayerVM azplayer = await isPlayerinAzureasync(p.display_name, p.team_id);
            if (azplayer == null)
            {
                HttpResponseMessage response = await postplayerasync(p);
                return response.IsSuccessStatusCode;
            }
            else
            { return false; } //save with put method
        }
        public async Task<HttpResponseMessage> postplayerasync(PlayerVM p)
        {

            using (var http = new HttpClient())
            {

                String myuri = (urlbase + "Players");

                // http.DefaultRequestHeaders.Add("Authorization", "Bearer " + AuthHeader);
                // http.DefaultRequestHeaders.Add("User-Agent", "(katzhd@gmail.com)");
                //var response = await http.PostAsJsonAsync(myuri, p);
                var response =
                    await client.InvokeApiAsync<PlayerVM, HttpResponseMessage>(myuri, p, CancellationToken.None);
                return response;

            }

        }
        public async Task<PlayerVM> isPlayerinAzureasync(string displayname, string teamid)
        {
            IDictionary<string,string> param = new Dictionary<string, string>();
            param.Add("displayname", displayname);
            param.Add("teamid", teamid);
            //using (var http = new HttpClient())
            //{

                string apiextension = String.Format("Players?displayname={0}&teamid={1}", displayname, teamid);
                HttpResponseMessage jsonmessage = await GetAZMAsyncP("Players",param);
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
            //}
        }
        public async Task<bool> PutPLayerasync(PlayerVM player)
        {
          
                String myuri = string.Format("Players/{0}", player.id);
            // var response = await http.PutAsJsonAsync(myuri, player);
            var response =
                await
                    client.InvokeApiAsync<PlayerVM, HttpResponseMessage>(myuri, player, HttpMethod.Put,null,
                        CancellationToken.None);
            return response.IsSuccessStatusCode;
            }
        

        #endregion
        #region "events"
        public async Task<bool> Saveeventasync(Event e)
        {
            Event azevent = await isEventinAzureasync(e.event_id);
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

            //using (var http = new HttpClient())
            //{

                String myuri = (urlbase +"Events"); 

                // http.DefaultRequestHeaders.Add("Authorization", "Bearer " + AuthHeader);
                // http.DefaultRequestHeaders.Add("User-Agent", "(katzhd@gmail.com)");
                var response = await client.InvokeApiAsync<Event,HttpResponseMessage>(myuri, e);

                return response;

            //}

        }
        public async Task<Event> isEventinAzureasync(string eventid)
        {
            using (var http = new HttpClient())
            {
                string apiextension = String.Format(geteventapi, eventid);
                // string apiextension = String.Format("Events?eventid={0}",  eventid);
                HttpResponseMessage jsonmessage = await GetAZMAsync(apiextension);
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

        public async Task<MobileServiceUser> LoginAsync(MobileServiceAuthenticationProvider provider)
        {

            MobileServiceClient myservice = new MobileServiceClient(url);
            return await myservice.LoginAsync(provider,false);
        }

        public async Task<MobileServiceUser> RefreshTokenasync()
        {

            return await client.RefreshUserAsync();
        }

        public async Task<HttpResponseMessage> IsUserAuthenticated()
        {
            return await client.InvokeApiAsync<HttpResponseMessage>("Login",CancellationToken.None);
        }

        public async Task<UserInfo> getuserinfo()
        {
            // return  await client.InvokeApiAsync<UserInfo>("UserInfo", HttpMethod.Get, null);
            JArray jsonmessage =  (JArray) await client.InvokeApiAsync("/.auth/Me",HttpMethod.Get, null);


            {
                UserInfo u = jsonmessage[0].ToObject<UserInfo>();
                u.account_id = u.user_claims.Where(c => c.typ == "urn:microsoftaccount:id").FirstOrDefault().val;
                return u;
            }
            {
                return null;
            }
        }

        public bool istokenexpired()
        {
            return client.IsTokenExpired();
        }
        //try refresh again
        public async Task<bool> refreshtoken()
        {
            try
            {

                JObject refreshJson = (JObject) await client.InvokeApiAsync("/.auth/refresh",
                    HttpMethod.Get,
                    null);

                string newToken = refreshJson["authenticationToken"].Value<string>();
                client.CurrentUser.MobileServiceAuthenticationToken
                    = newToken;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
    }
}

}
