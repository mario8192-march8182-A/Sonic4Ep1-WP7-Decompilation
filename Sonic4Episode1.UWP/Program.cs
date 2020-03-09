using System;
using System.Collections.Generic;
using Windows.Storage;

namespace Sonic4Episode1.UWP
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var factory = new MonoGame.Framework.GameFrameworkViewSource<Sonic4Ep1>((a, s) =>
            {
                a.SetAccelerometer(new UwpAccelerometer());
                a.SetControllerSource(new UwpControllerSource());
                a.SetSaveContentPath(ApplicationData.Current.RoamingFolder.Path);
            });
            Windows.ApplicationModel.Core.CoreApplication.Run(factory);
        }
    }
}

