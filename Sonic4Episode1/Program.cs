using System;

namespace Sonic4Episode1
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Sonic4Ep1())
            {
                game.SetControllerSource(new Win32ControllerSource());
                game.Run();
            }
        }
    }
}
