using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BCA.Models
{
    class UserInfo
    
    {

        public string access_token { get; set; }
        public string expires_on { get; set; }
        public string refresh_token { get; set; }
        public string account_id { get; set; }
       
        public ICollection<user_claim> user_claims { get; set; }
        public string user_id { get; set; }
       
        public string Id { get; set; }

     }



    public class user_claim 
    {
      
        public string userinfo_id { get; set; }


        public string typ { get; set; }
        public string val { get; set; }
      
       

        public string Id { get; set; }

     

    }
}
