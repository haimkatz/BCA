using BCA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.IO;


namespace BCA.Repository
{
    class AZClient
    {
        private const string urlbase = "http://localhost:63494/api/";
        private const string getteamapi = "Teams?teamid={0}";
     
        private async static Task<HttpResponseMessage> GetAZAsync(string apiext)
        {
            // Get the MD5 Hash 
           
            string completeUrl = String.Format("{0}{1}", urlbase, apiext);


            // Call out to AZ 
            using (var http = new HttpClient())
            {

                http.BaseAddress = new Uri(completeUrl);

                // http.DefaultRequestHeaders.Add("Authorization", "Bearer " + AuthHeader);
                // http.DefaultRequestHeaders.Add("User-Agent", "(katzhd@gmail.com)");
                var response = await http.GetAsync(apiext);
                return response;
            }
        }
        public async static Task<Team> GetTeambyIDasync(string team_id)
        {
            string apiextension = String.Format(getteamapi,team_id);
            HttpResponseMessage jsonmessage = await GetAZAsync(apiextension);
            if (jsonmessage.IsSuccessStatusCode)
            {
               Team team = await jsonmessage.Content.ReadAsAsync<Team>();
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

      public static async Task postteamsasync()
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

        private async static Task createteamasync(Team t)
        {
            using (var http = new HttpClient())
            {

                var myuri = new Uri(urlbase + "Teams");

                // http.DefaultRequestHeaders.Add("Authorization", "Bearer " + AuthHeader);
                // http.DefaultRequestHeaders.Add("User-Agent", "(katzhd@gmail.com)");
                var response = await http.PostAsJsonAsync(myuri, t);
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

                    // http.DefaultRequestHeaders.Add("Authorization", "Bearer " + AuthHeader);
                    // http.DefaultRequestHeaders.Add("User-Agent", "(katzhd@gmail.com)");
                    var response = await http.PostAsJsonAsync(myuri, p);
                return response;

                }

            }
            public async Task<PlayerVM> isPlayerinAzureasync(string displayname, string teamid)
        {
            using (var http = new HttpClient())
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
        }


        #endregion
        }

}
