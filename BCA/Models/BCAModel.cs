using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCA.Models
{
    class BCAModel
    {
        public ObservableCollection<Team> teams { get; set; }
        public ObservableCollection<Player> players { get; set; }
        public ObservableCollection<Event> events { get; set; }
    }

    public class Team
    {
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
       public string gamenotes { get; set; }
        public virtual ObservableCollection<Player> players { get; set; }
    }

    public class Player
    {  
        public string last_name { get; set; }
        public string first_name { get; set; }
        public string display_name { get; set; }
        public string birthdate { get; set; }
        public int age { get; set; }
        public string birthplace { get; set; }
        public int height_in { get; set; }
        public double height_cm { get; set; }
        public double height_m { get; set; }
        public string height_formatted { get; set; }
        public int weight_lb { get; set; }
        public double weight_kg { get; set; }
        public string position { get; set; }
        public int uniform_number { get; set; }
      public string playernote { get; set; }
    }
    public class AwayTeam: INotifyPropertyChanged
    {
        public string team_id { get; set; }
        public string abbreviation { get; set; }
        public bool active { get; set; }
        public string first_name { get; set; }
        private string _lastname;
        public string last_name
        {
            get
            { return _lastname; }
            set
            {
                if (_lastname != value)
                {
                    _lastname = value;
                    NotifyPropertyChanged("lastname");
                }
            }
        }
        public string conference { get; set; }
        public string division { get; set; }
        public string site_name { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string full_name { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

    }

    public class HomeTeam:INotifyPropertyChanged
    {
        public string team_id { get; set; }
        public string abbreviation { get; set; }
        public bool active { get; set; }
        public string first_name { get; set; }
    private string _lastname;
    public string last_name
    {
        get
        { return _lastname; }
        set
        {
            if (_lastname != value)
            {
                _lastname = value;
                NotifyPropertyChanged("lastname");
            }
        }
    }
    public string conference { get; set; }
        public string division { get; set; }
        public string site_name { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string full_name { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

    }

    public class Site
    {
        public int capacity { get; set; }
        public string surface { get; set; }
        public string name { get; set; }
        public string state { get; set; }
        public string city { get; set; }
    }

    public class Event:INotifyPropertyChanged
    {
        public string event_id { get; set; }
        public string event_status { get; set; }
        public string sport { get; set; }
        public string start_date_time { get; set; }
        public string season_type { get; set; }
        public AwayTeam away_team { get; set; }
        public HomeTeam home_team { get; set; }
        public Site site { get; set; }
        public List<int> away_period_scores { get; set; }
        public List<int> home_period_scores { get; set; }
        public int away_points_scored { get; set; }
        public int home_points_scored { get; set; }
        public virtual string display_time
        {
            get
            { int timestart = start_date_time.IndexOf("T");
            
                return start_date_time.Substring(timestart + 1, 5);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

    }

    public class Games
    {
        public string events_date { get; set; }
        public List<Event> @event { get; set; }
    }
    public class Fuckbinding : INotifyPropertyChanged
    {

        private string _lastname;
        public String lastname
        {
            get { return _lastname; }
            set
            {
                if (_lastname != value)
                    _lastname = value;
                NotifyPropertyChanged("lastname");
            }
            
        }

          public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}