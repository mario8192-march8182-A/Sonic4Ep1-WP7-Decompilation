using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Microsoft.Xna.Framework;
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
    public class Sonic4Episode1Activity : AndroidGameActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            var g = new Sonic4Ep1();
            SetContentView((View)g.Services.GetService(typeof(View)));
            g.Run();
        }
    }
}

