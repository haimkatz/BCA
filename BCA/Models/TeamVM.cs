using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCA.Models
{
    public class TeamVM : INotifyPropertyChanged
    {
        public string Id { get; set; }
        public string team_id { get; set; }
        public string abbreviation { get; set; }
        public bool active { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string conference { get; set; }
        public string division { get; set; }
        public string site_name { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string full_name { get; set; }
      
       private string _gamenotes { get; set; }
        public string gamenotes
        {
            get
            { return _gamenotes; }
            set
            {
                if (_gamenotes != value)
                {
                    _gamenotes = value;
                    NotifyPropertyChanged("playernote");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public TeamVM()
        {

        }
        public TeamVM(Team t, string teamid)
        {
            team_id = t.team_id;
        
        abbreviation =t.abbreviation;
            active = t.active;
            first_name = t.first_name;
        last_name = t.last_name;
       conference = t.conference;
        division = t.division;
      site_name = t.site_name;
        city = t.city;
       state = t.state;
            full_name = t.full_name;

        Id = "NotSet";
        }
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

    }

}