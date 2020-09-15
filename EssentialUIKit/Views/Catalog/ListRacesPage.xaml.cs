using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Acr.UserDialogs;
using EssentialUIKit.AppLayout.Models;
using EssentialUIKit.AppLayout.Views;
using EssentialUIKit.Views;
using EssentialUIKit.Controls;
using EssentialUIKit.DataService;
using EssentialUIKit.Models.Api;
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
            PageNameLabel.Text = venue.Description;

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
                var content = await _client.GetStringAsync(ApiDataService.HorseRaces + "id=" + _venue.BfEventId);
                var races = JsonConvert.DeserializeObject<List<Tuple< DateTime, String>>>(content);
                
                foreach (Tuple<DateTime, String> r in races)
                {
                    var racetTime = r.Item1.ToLocalTime().ToString("dd/MM hh:mm tt");
                    var raceArray = r.Item2.Split('|');
                    var race = new Race(raceArray[0].Trim(),
                        racetTime, raceArray[2].Trim());
                    Races.Add(race);
                }

                Races[races.Count() - 1].SelectedColor = "LightGray";
                Races[races.Count() - 1].SelectedText = "Leg 1";
                Races[races.Count() - 2].SelectedColor = "LightGray";
                Races[races.Count() - 1].SelectedText = "Leg 2";
                Races[races.Count() - 3].SelectedColor = "LightGray";
                Races[races.Count() - 1].SelectedText = "Leg 3";
                Races[races.Count() - 4].SelectedColor = "LightGray";
                Races[races.Count() - 1].SelectedText = "Leg 4";
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

        
        private async void ListView_OnSelectionChanged(object sender, SelectedItemChangedEventArgs e)
        {
        }

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            
            if (e.Item == null)
            {
                return;
            }
            var assembly = typeof(App).GetTypeInfo().Assembly;
            var pageName = "Views.Catalog.ListRunnersPage";
            var template = new Template("Runners", "List Runners", pageName, false, "", true, e.Item as Race);

            Routing.RegisterRoute("ListRunners",
                assembly.GetType($"EssentialUIKit.{pageName}"));
            ///Navigation.PushAsync(new ListRacesPage(e.SelectedItem as Venue));

            Application.Current.MainPage.Navigation.PushAsync(new TemplateHostPage(template));
        }


        async void OnImageNameTapped(object sender, EventArgs args)
        {            
            var quaddieGroupId = Application.Current.Properties["quaddieGroupId"].ToString();
            var quaddieContent = await _client.GetStringAsync(ApiDataService.GetQuaddieGroupsApi + "qgId=" + quaddieGroupId);
            var quaddieList = JsonConvert.DeserializeObject<List<QuaddieGroup>>(quaddieContent);
            var quaddie = quaddieList.First();

            QuaddieView.Show(quaddie);
            string content = "";
            
            {
                //sr.Runner.Race
            }
            //sfPopupView.ShowPopUp(new List<Frame>()); // content: quaddie.Selections.First().Runner.Race.Description + "\n" + quaddie.Selections.First().Runner.Name + " (" + quaddie.Selections.First().User.Name +")\nNext Line");
        }
    }

    /*protected override bool OnBackButtonPressed()
    {
        if (SettingsView.IsVisible)
        {
            SettingsView.Hide();
            return true;
        }

        return base.OnBackButtonPressed();
    }*/

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
        public String SelectedColor { get; set; }
        public String SelectedText { get; set; }

    }
}