using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Plugin.GoogleClient;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace EssentialUIKit.Droid
{
    [Activity(Label = "Bets Friend", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize|ConfigChanges.Orientation)]

    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnActivityResult(int requestCode, Result resultCode, Android.Content.Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            GoogleClientManager.OnAuthCompleted(requestCode, resultCode, data);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {   
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            base.OnCreate(savedInstanceState);

            Forms.SetFlags("CollectionView_Experimental");

            Forms.Init(this, savedInstanceState);

            Syncfusion.XForms.Android.PopupLayout.SfPopupLayoutRenderer.Init();

            Syncfusion.XForms.Android.Core.Core.Init(this);
            GoogleClientManager.Initialize(this);

            this.LoadApplication(new App());

            // Change the status bar color
            this.SetStatusBarColor(Android.Graphics.Color.Argb(255, 0, 0, 0));

            // Enable scrolling to the page when the keyboard is enabled
            Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        }
    }
}