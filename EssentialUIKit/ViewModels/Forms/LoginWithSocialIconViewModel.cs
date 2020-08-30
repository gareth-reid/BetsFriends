using System;
using System.Net.Http;
using System.Threading.Tasks;
using EssentialUIKit.AppLayout.Views;
using EssentialUIKit.DataService;
using EssentialUIKit.Views.Catalog;
using EssentialUIKit.Views.Forms;
using Newtonsoft.Json;
using Plugin.GoogleClient;
using Plugin.GoogleClient.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace EssentialUIKit.ViewModels.Forms
{
    public class AuthNetwork
    {
        public string Name { get; set; }

        public string Icon { get; set; }

        public string Background { get; set; }

        public string Foreground { get; set; }
    }
    /// <summary>
    /// ViewModel for login with social icon page.
    /// </summary>
    [Preserve(AllMembers = true)]
    public class LoginWithSocialIconViewModel : LoginViewModel
    {
        #region Fields
        
        private HttpClient _client = new HttpClient();
        private string password;
        private string email;
        public INavigation Navigation { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance for the <see cref="LoginWithSocialIconViewModel" /> class.
        /// </summary>
        public LoginWithSocialIconViewModel()
        {
            this.LoginCommand = new Command(this.LoginClicked);
            this.SignUpCommand = new Command(this.SignUpClicked);
            this.ForgotPasswordCommand = new Command(this.ForgotPasswordClicked);
            this.FaceBookLoginCommand = new Command(this.FaceBookClicked);
            this.TwitterLoginCommand = new Command(this.TwitterClicked);
            this.GmailLoginCommand = new Command<AuthNetwork>(async (data) => await LoginGoogleAsync(data));

        }

        #endregion
        #region GOOGLE
        IGoogleClientManager _googleService = CrossGoogleClient.Current;

        async Task LoginGoogleAsync(AuthNetwork authNetwork)
        {
            try
            {
                /*if (!string.IsNullOrEmpty(_googleService.AccessToken))
                {
                    //Always require user authentication
                    _googleService.Logout();
                }*/

                EventHandler<GoogleClientResultEventArgs<GoogleUser>> userLoginDelegate = null;
                userLoginDelegate = async (object sender, GoogleClientResultEventArgs<GoogleUser> e) =>
                {
                    switch (e.Status)
                    {
                        case GoogleActionStatus.Completed:
#if DEBUG
                            var googleUserString = JsonConvert.SerializeObject(e.Data);
                            int d = 0;
                            //Debug.WriteLine($"Google Logged in succesfully: {googleUserString}");
#endif
                            int i = 0;
                            /*var socialLoginData = new NetworkAuthData
                            {
                                Id = e.Data.Id,
                                Logo = authNetwork.Icon,
                                Foreground = authNetwork.Foreground,
                                Background = authNetwork.Background,
                                Picture = e.Data.Picture.AbsoluteUri,
                                Name = e.Data.Name,
                            };*/

                            await App.Current.MainPage.Navigation.PushModalAsync(new HomePage());
                            break;
                        case GoogleActionStatus.Canceled:
                            await App.Current.MainPage.DisplayAlert("Google Auth", "Canceled", "Ok");
                            break;
                        case GoogleActionStatus.Error:
                            await App.Current.MainPage.DisplayAlert("Google Auth", "Error", "Ok");
                            break;
                        case GoogleActionStatus.Unauthorized:
                            await App.Current.MainPage.DisplayAlert("Google Auth", "Unauthorized", "Ok");
                            break;
                    }

                    _googleService.OnLogin -= userLoginDelegate;
                };

                _googleService.OnLogin += userLoginDelegate;

                await _googleService.LoginAsync();
            }
            catch (Exception ex)
            {
                //Debug.WriteLine(ex.ToString());
            }
        }
        #endregion
        #region property

        
        /// <summary>
        /// Gets or sets the property that is bound with an entry that gets the password from user in the login page.
        /// </summary>
        public string Email
        {
            get
            {
                return this.email;
            }

            set
            {
                if (this.email == value)
                {
                    return;
                }

                this.email = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the property that is bound with an entry that gets the password from user in the login page.
        /// </summary>
        public string Password
        {
            get
            {
                return this.password;
            }

            set
            {
                if (this.password == value)
                {
                    return;
                }

                this.password = value;
                this.NotifyPropertyChanged();
            }
        }

        #endregion

        #region Command

        /// <summary>
        /// Gets or sets the command that is executed when the Log In button is clicked.
        /// </summary>
        public Command LoginCommand { get; set; }

        /// <summary>
        /// Gets or sets the command that is executed when the Sign Up button is clicked.
        /// </summary>
        public Command SignUpCommand { get; set; }

        /// <summary>
        /// Gets or sets the command that is executed when the Forgot Password button is clicked.
        /// </summary>
        public Command ForgotPasswordCommand { get; set; }

        /// <summary>
        /// Gets or sets the command that is executed when the facebook login button is clicked.
        /// </summary>
        public Command FaceBookLoginCommand { get; set; }

        /// <summary>
        /// Gets or sets the command that is executed when the twitter login button is clicked.
        /// </summary>
        public Command TwitterLoginCommand { get; set; }

        /// <summary>
        /// Gets or sets the command that is executed when the gmail login button is clicked.
        /// </summary>
        public Command GmailLoginCommand { get; set; }

        #endregion

        #region methods

        /// <summary>
        /// Invoked when the Log In button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private void LoginClicked(object obj)
        {
            var api = ApiDataService.CheckUserApi + "un=" + email + "&pw=" + password;
            string name = _client.GetStringAsync(api).Result.ToString();
            if (name != "")
            {
                Application.Current.Properties.Add("name", name);
                App.Current.MainPage = new NavigationPage(new HomePage());
            }
            else
            {
                App.Current.MainPage = new NavigationPage(new LoginWithSocialIconPage());

            }

            //Navigation.PushAsync(new HomePage());            
            // Do something
        }

        /// <summary>
        /// Invoked when the Sign Up button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private void SignUpClicked(object obj)
        {
            App.Current.MainPage = new NavigationPage(new SimpleSignUpPage());
        }

        /// <summary>
        /// Invoked when the Forgot Password button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private async void ForgotPasswordClicked(object obj)
        {
            var label = obj as Label;
            label.BackgroundColor = Color.FromHex("#70FFFFFF");
            await Task.Delay(100);
            label.BackgroundColor = Color.Transparent;
        }

        /// <summary>
        /// Invoked when facebook login button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private void FaceBookClicked(object obj)
        {
            // Do something
        }

        /// <summary>
        /// Invoked when twitter login button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private void TwitterClicked(object obj)
        {
            // Do something
        }

        /// <summary>
        /// Invoked when gmail login button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private void GmailClicked(object obj)
        {
            
        }

        #endregion
    }
}
