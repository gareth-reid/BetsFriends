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
using Acr.UserDialogs;
using EssentialUIKit.Models.Api;

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
            PageNameLabel.Text = race.Name;
            SetActivity(true);
            //this.BindingContext = CatalogDataService.Instance.CatalogPageViewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                SetActivity(true);
                var content = await _client.GetStringAsync(ApiDataService.HorseRunnerApi + "id=" + _race.Id);                
                var runners = JsonConvert.DeserializeObject<List<string>>(content);


                var quaddieGroupId = Application.Current.Properties["quaddieGroupId"].ToString();
                var quaddieContent = await _client.GetStringAsync(ApiDataService.GetQuaddieGroupsApi + "qgId=" + quaddieGroupId);
                var quaddieList = JsonConvert.DeserializeObject<List<QuaddieGroup>>(quaddieContent);
                var quaddie = quaddieList.First();
                var user = Application.Current.Properties.ContainsKey("name") ? Application.Current.Properties["name"].ToString() : "";

                int i = 0;
                foreach ( string r in runners)
                {
                    var runnerArray = r.Split('|');
                    var runner = new Runner(runnerArray[0].Trim(), runnerArray[1].Trim(), runnerArray[2].Trim(), runnerArray[3].Trim());
                    var runnerSelected = quaddie.Selections.FirstOrDefault(selection =>
                         selection.Runner.BfSelectionId.ToString() == runner.Id);
                    Runners.Add(runner);
                    if (runnerSelected != null)
                    {
                        Runners[i].SelectedColor = "LightGray";
                        Runners[i].SelectedText = runnerSelected.User.Name;
                    }
                    i++;
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

        private async void ListView_OnSelectionChanged(object sender, SelectedItemChangedEventArgs e)
        {
        }

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
            {
                return;
            }
            
            var runner = e.Item as Runner;
            var quaddieGroupId = Application.Current.Properties["quaddieGroupId"] as string;
            var user = Application.Current.Properties["name"] as string;

            if (quaddieGroupId == null)
            {
                await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
                {
                    OkText = "Ok",
                    CancelText = "Cancel",
                    Title = "No Quaddie Selected"
                });
            }
            else
            {
                Boolean ok = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
                {
                    OkText = "Ok",
                    CancelText = "Cancel",
                    Title = "Runner Selected: " + runner.Name
                });
                if (ok)
                {

                    if (Application.Current.Properties.ContainsKey("name"))
                    {
                        var content = await _client.GetStringAsync(ApiDataService.QuaddieBuilderApi + "qgId=" + quaddieGroupId + "&selectionId=" + runner.Id + "&user=" + user);
                        int i = 0;
                    }
                }
            }

            
            return;
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
        public String SelectedColor { get; set; }
        public String SelectedText { get; set; }
        
        public String DisplayPrice
        {
            get
            {
                return "$" + (Price == "" || Price == null ? "11" : Price);
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