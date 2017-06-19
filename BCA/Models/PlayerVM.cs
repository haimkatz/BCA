using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCA.Models
{
  
    public class PlayerVM : INotifyPropertyChanged { 
public string id { get; set; }
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
    public string team_id { get; set; }
    public string mobileteam_id { get; set; }
        [StringLength(1500)]
    private string _playernote { get; set; }
    public string playernote
        {
            get
            { return _playernote; }
            set
            {
                if (_playernote != value)
                {
                    _playernote = value;
                    NotifyPropertyChanged("playernote");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
public PlayerVM()
        {

        }
        public  PlayerVM (Player p,  string teamid)
        {
           age = p.age;
            birthdate = p.birthdate;
           birthplace = p.birthplace;
            display_name = p.display_name;
            first_name = p.first_name;
            height_cm = p.height_cm;
            height_formatted = p.height_formatted;
            height_in = p.height_in;
            height_m = p.height_m;
            last_name = p.last_name;
            playernote = p.playernote;
            position = p.position;
            uniform_number = p.uniform_number;
            weight_kg = p.weight_kg;
            weight_lb = p.weight_lb;
            team_id = teamid;
            mobileteam_id = "NotSet";
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
