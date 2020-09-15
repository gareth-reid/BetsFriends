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
using EssentialUIKit.Models.Api;

namespace EssentialUIKit.Views.Catalog
{
    /// <summary>
    /// Page to show the catalog list. 
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListQuaddiePage
    {
        
        private HttpClient _client = new HttpClient();

        public ObservableCollection<QuaddieGroup> QuaddieGroups { get; } = new ObservableCollection<QuaddieGroup>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ListVenuesPage" /> class.
        /// </summary>
        public ListQuaddiePage()
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
                var content = await _client.GetStringAsync(ApiDataService.GetQuaddieGroupsApi);
                var quaddies = JsonConvert.DeserializeObject<List<QuaddieGroup>>(content);

                quaddieGroupListView.ItemsSource = quaddies;
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

            var quaddieGroup = e.SelectedItem as QuaddieGroup;
            Application.Current.Properties["quaddieGroupId"] = quaddieGroup.QuaddieGroupId.ToString();

            var assembly = typeof(App).GetTypeInfo().Assembly;
            var pageName = "Views.Catalog.ListRacesPage";
            var template = new Template("Races", "List Races", pageName, false, "", true, quaddieGroup.Venue);
            
            Routing.RegisterRoute("ListRaces",
                assembly.GetType($"EssentialUIKit.{pageName}"));
            //Application.Current.MainPage.Navigation.PushAsync(new ListRacesPage(e.SelectedItem as Venue));

            Application.Current.MainPage.Navigation.PushAsync(new TemplateHostPage(template));

            //Navigation.PushAsync<ListRacesPageViewModel, ListRacesPage>((viewModel, page) => viewModel.Venue = e.SelectedItem as Venue);
            //return _pushCommand ?? (_pushCommand = new RelayCommand(() => Navigation.PushAsync<ListRacesPageViewModel>((viewModel, page) => viewModel.Venue = e.SelectedItem as Venue)));

        }
    }

    
}