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
using Acr.UserDialogs;
using EssentialUIKit.Models.Api;

namespace EssentialUIKit.Views.Catalog
{
    /// <summary>
    /// Page to show the catalog list. 
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListVenuesPage
    {

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
                var content = await _client.GetStringAsync(ApiDataService.HorseVenuesApi);
                var venues = JsonConvert.DeserializeObject<List<string>>(content);

                foreach (string v in venues)
                {
                    var venueArray = v.Split('|');
                    int eventId;
                    int.TryParse(venueArray[2].Trim(), out eventId);
                    var venue = new Venue() { Name = venueArray[0].Trim(), Description = venueArray[1].Trim(), BfEventId = eventId };
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
            var pageName = "Views.Catalog.ListRacesPage";

            Routing.RegisterRoute("ListRaces",
                assembly.GetType($"EssentialUIKit.{pageName}"));

            PromptResult pResult = await UserDialogs.Instance.PromptAsync(new PromptConfig
            {
                InputType = InputType.Name,
                OkText = "Create",
                Title = "Give Quaddie a Name",
            });
            if (pResult.Ok && !string.IsNullOrWhiteSpace(pResult.Text))
            {
                if (Application.Current.Properties.ContainsKey("name"))
                {
                    var user = Application.Current.Properties["name"] as string;
                    var venue = e.Item as Venue;
                    var template = new Template("Races", "List Races", pageName, false, "", true, venue);
                    Application.Current.Properties["quaddieGroupId"] = await _client.GetStringAsync(ApiDataService.CreateQuaddieGroupApi + "desc=" + pResult.Text + "&user=" + user + "&vId=" + venue.BfEventId);
                    await Application.Current.MainPage.Navigation.PushAsync(new TemplateHostPage(template));
                }
            }
            //Navigation.PushAsync<ListRacesPageViewModel, ListRacesPage>((viewModel, page) => viewModel.Venue = e.SelectedItem as Venue);
            //return _pushCommand ?? (_pushCommand = new RelayCommand(() => Navigation.PushAsync<ListRacesPageViewModel>((viewModel, page) => viewModel.Venue = e.SelectedItem as Venue)));
        }


    }
}


   