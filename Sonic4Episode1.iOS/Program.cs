using System;
using Foundation;
using UIKit;

namespace Sonic4Episode1.iOS
{
    [Register("AppDelegate")]
    class Program : UIApplicationDelegate
    {
        private static Sonic4Ep1 game;

        internal static void RunGame()
        {
            game = new Sonic4Ep1();
            game.SetAccelerometer(new XamarinAccelerometer());
            game.SetSaveContentPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            game.Run();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            UIApplication.Main(args, null, "AppDelegate");
        }

        public override void FinishedLaunching(UIApplication app)
        {
            RunGame();
        }
    }
}
