using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Sonic4Episode1.iOS;

namespace Sonic4Episode1.Android
{
    [Activity(Label = "Sonic4Episode1.Android",
        MainLauncher = true,
        Icon = "@drawable/icon", Theme = "@style/Theme.Splash",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance, 
        ScreenOrientation = ScreenOrientation.Landscape, 
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize | ConfigChanges.ScreenLayout)]
    public class Activity1 : Microsoft.Xna.Framework.AndroidGameActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            var g = new Sonic4Ep1();
            g.SetSaveContentPath(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData));
            SetContentView((View)g.Services.GetService(typeof(View)));
            g.Run();
        }
    }
}

