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

namespace EssentialUIKit.Views.Catalog
{
    /// <summary>
    /// Page to show the catalog list. 
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListRunnersPage
    {
        //192.168.1.6
        private const string _betfairApi = "http://betsfriendsapi.azurewebsites.net/api/BFHorseRunners?";//?mock=true";
        private HttpClient _client = new HttpClient();
        private Race _race;
        public ObservableCollection<Runner> Runners { get; } = new ObservableCollection<Runner>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ListVenuesPage" /> class.
        /// </summary>
        public ListRunnersPage(Race race)
        {
            _race = race;
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
                var content = await _client.GetStringAsync(_betfairApi + "id=" + _race.Id);
                var runners = JsonConvert.DeserializeObject<List<string>>(content);
                
                foreach ( string r in runners)
                {
                    var runnerArray = r.Split('|');
                    var runner = new Runner(runnerArray[0].Trim(), runnerArray[1].Trim(), runnerArray[2].Trim(), runnerArray[3].Trim());
                    Runners.Add(runner);
                }                
                
                runnerListView.ItemsSource = Runners;
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
            var pageName = "ListRunnersPage";
            //var template = new Template("Races", "List Races", pageName, false, "", true);
            Routing.RegisterRoute("ListRunners",
                assembly.GetType($"EssentialUIKit.{pageName}"));
            //Navigation.PushAsync(new ListRacesPage(e.SelectedItem as Venue));
        }
    }

    public class Runner
    {
        public Runner(String name, string metaData, String id, String price)
        {
            Name = name;
            Id = id;
            BuildMetadata(metaData);
            Price = price;
        }
        public String Name { get; set; }
        public RunnerMetaData MetaData { get; set; }
        public string MetaDataDisplay
        {
            get { return MetaData.ToString(); }
        }

        public String Id { get; set; }
        public String Price { get; set; }

        public String DisplayPrice
        {
            get
            {
                return "$" + Price;
            }            
        }

        public void BuildMetadata(string metaData)
        {
            MetaData = new RunnerMetaData();
            var metaDataArray = metaData.Split('^');
            MetaData.Jockey = metaDataArray[0];
            MetaData.Trainer = metaDataArray[1];
            MetaData.Weight = metaDataArray[2];
            MetaData.Form = metaDataArray[3];
            MetaData.Barrier = metaDataArray[4];            
        }

    }

    public class RunnerMetaData
    {
        public string Jockey;
        public string Trainer;
        public string Weight;
        public string Form;
        public string Barrier;

        public override string ToString()
        {
            return "F: " + Form +
                 ", W: " + Weight +
                 ", B: (" + Barrier + ")" +
                 ", T: " + Trainer +
                 ", J: " + Jockey;
        }
    }
}