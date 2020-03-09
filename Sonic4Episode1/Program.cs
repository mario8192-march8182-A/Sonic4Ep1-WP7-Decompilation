using System;
using System.IO;

namespace Sonic4Episode1
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new Sonic4Ep1())
            {
                game.SetControllerSource(new Win32ControllerSource());
                game.SetSaveContentPath(Directory.GetCurrentDirectory());
                game.Run();
            }
        }
    }
}
