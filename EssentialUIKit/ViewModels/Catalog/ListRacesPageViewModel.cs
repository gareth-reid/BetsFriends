using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using EssentialUIKit.Models;
using EssentialUIKit.Models.Api;
using EssentialUIKit.Views.Catalog;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace EssentialUIKit.ViewModels.Catalog
{
    /// <summary>
    /// ViewModel for home page.
    /// </summary>
    [Preserve(AllMembers = true)]    
    public class ListRacesPageViewModel : BaseViewModel
    {
        public ListRacesPageViewModel()
        {
            if (Application.Current.Properties.ContainsKey("name"))
            {
                displayName = Application.Current.Properties["name"] as string;
            }
            else
            {
                displayName = "LOGIN";
            }
        }
        #region Fields
        private Venue venue;
        private string displayName;
        private Command backButtonCommand;
        #endregion

        #region Public properties                
        public Venue Venue
        {
            get
            {
                return this.venue;
            }

            set
            {
                if (this.venue == value)
                {
                    return;
                }

                this.venue = value;
                this.NotifyPropertyChanged();
            }
        }
        public string DisplayName
        {
            get
            {
                return this.displayName;
            }

            set
            {
                if (this.displayName == value)
                {
                    return;
                }

                this.displayName = value;
            }
        }

        public Command BackButtonCommand
        {
            get { return this.backButtonCommand ?? (this.backButtonCommand = new Command(this.BackButtonClicked)); }
        }

        private void BackButtonClicked(object obj)
        {
            Application.Current.MainPage.Navigation.PopAsync();
        }
        #endregion

    }
}