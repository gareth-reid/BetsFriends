using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using EssentialUIKit.AppLayout.Models;
using EssentialUIKit.DataService;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using System.Reflection;
using EssentialUIKit.AppLayout.Views;
using EssentialUIKit.ViewModels.Catalog;

namespace EssentialUIKit.Views.Catalog
{
    /// <summary>
    /// Page to show the catalog list. 
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListVenuesPage
    {
        private const string _betfairApi = "http://betsfriendsapi.azurewebsites.net/api/BFHorseVenues?";//?mock=true";
        private HttpClient _client = new HttpClient();

        public ObservableCollection<Venue> Venues { get; } = new ObservableCollection<Venue>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ListVenuesPage" /> class.
        /// </summary>
        public ListVenuesPage()
        {
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
                var content = await _client.GetStringAsync(_betfairApi);
                var venues = JsonConvert.DeserializeObject<List<string>>(content);
                
                foreach ( string v in venues)
                {
                    var venueArray = v.Split('|');
                    var venue = new Venue(venueArray[0].Trim(), venueArray[1].Trim(), venueArray[2].Trim());
                    Venues.Add(venue);
                }                
                
                venueListView.ItemsSource = Venues;
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
            var pageName = "Views.Catalog.ListRacesPage";
            var template = new Template("Races", "List Races", pageName, false, "", true, e.SelectedItem as Venue);
            
            Routing.RegisterRoute("ListRaces",
                assembly.GetType($"EssentialUIKit.{pageName}"));
            //Application.Current.MainPage.Navigation.PushAsync(new ListRacesPage(e.SelectedItem as Venue));

            Application.Current.MainPage.Navigation.PushAsync(new TemplateHostPage(template));

            //Navigation.PushAsync<ListRacesPageViewModel, ListRacesPage>((viewModel, page) => viewModel.Venue = e.SelectedItem as Venue);
            //return _pushCommand ?? (_pushCommand = new RelayCommand(() => Navigation.PushAsync<ListRacesPageViewModel>((viewModel, page) => viewModel.Venue = e.SelectedItem as Venue)));

        }
    }

    public class Venue
    {
        public Venue(String name, String description, String id)
        {
            Name = name;
            Description = description;
            Id = id;
        }
        public String Name { get; set; }
        public String Description { get; set; }
        public String Id { get; set; }
    }
}