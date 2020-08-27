using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using EssentialUIKit.AppLayout.Models;
using EssentialUIKit.AppLayout.Views;
using EssentialUIKit.DataService;
using EssentialUIKit.ViewModels;
using EssentialUIKit.ViewModels.Catalog;
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
        private const string _betfairApi = "http://betsfriendsapi.azurewebsites.net/api/BFRaces?";
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
        }

        
        public ListRacesPage()
        {            
            InitializeComponent();                        
            SetActivity(true);
            _venue = ((ListRacesPageViewModel)BindingContext).Venue;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                SetActivity(true);
                var content = await _client.GetStringAsync(_betfairApi + "id=" + _venue.Id);
                var races = JsonConvert.DeserializeObject<List<Tuple< DateTime, String>>>(content);
                
                foreach (Tuple<DateTime, String> r in races)
                {
                    var racetTime = r.Item1.ToLocalTime().ToString("dd/MM hh:mm tt");
                    var raceArray = r.Item2.Split('|');
                    var race = new Race(raceArray[0].Trim(),
                        racetTime, raceArray[2].Trim());
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
            var assembly = typeof(App).GetTypeInfo().Assembly;
            var pageName = "Views.Catalog.ListRunnersPage";
            var template = new Template("Runners", "List Runners", pageName, false, "", true, e.SelectedItem as Race);

            Routing.RegisterRoute("ListRunners",
                assembly.GetType($"EssentialUIKit.{pageName}"));
            ///Navigation.PushAsync(new ListRacesPage(e.SelectedItem as Venue));

            Navigation.PushAsync(new TemplateHostPage(template));            
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