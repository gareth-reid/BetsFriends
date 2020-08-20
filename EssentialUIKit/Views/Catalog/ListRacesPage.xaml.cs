using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using EssentialUIKit.DataService;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace EssentialUIKit.Views.Catalog
{
    /// <summary>
    /// Page to show the catalog list. 
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListRacesPage
    {
        private const string _betfairApi = "http://192.168.1.6:7071/api/BFRaces?mock=true&";
        private HttpClient _client = new HttpClient();
        private Venue _venue;
        public ObservableCollection<Race> Races { get; } = new ObservableCollection<Race>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ListVenuesPage" /> class.
        /// </summary>
        public ListRacesPage(Venue venue)
        {
            _venue = venue;
            InitializeComponent();
            SetActivity(true);
            //this.BindingContext = CatalogDataService.Instance.CatalogPageViewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                SetActivity(true);
                var content = await _client.GetStringAsync(_betfairApi + "id=" + _venue.Id);
                var races = JsonConvert.DeserializeObject<List<string>>(content);
                
                foreach ( string r in races)
                {
                    var raceArray = r.Split('|');
                    var race = new Race(raceArray[0].Trim(), raceArray[1].Trim(), raceArray[2].Trim());
                    Races.Add(race);
                }                
                
                raceListView.ItemsSource = Races;
                //BindingContext = this;
            }
            catch (Exception e)
            {
                int i = 0;
            }
            finally
            {
                SetActivity(false);
            }

            
        }

        public void SetActivity(bool value)
        {
            activity.IsVisible = value;
            activity.IsRunning = value;
            activity.IsEnabled = value;
            //OnPropertyChanged();
        }

        private void ListView_OnSelectionChanged(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return;
            }

            Navigation.PushAsync(new ListRunnersPage(e.SelectedItem as Race));
        }
    }

    public class Race
    {
        public Race(String name, String time, String id)
        {
            Name = name;
            Time = time;
            Id = id;
        }
        public String Name { get; set; }
        public String Time { get; set; }
        public String Id { get; set; }
    }
}