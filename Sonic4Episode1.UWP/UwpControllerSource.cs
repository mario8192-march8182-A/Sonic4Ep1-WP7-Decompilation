using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Sonic4Episode1.Abstraction;
using Windows.Devices.Input;
using Windows.Gaming.Input;

namespace Sonic4Episode1.UWP
{
    class UwpControllerSource : IControllerSource
    {
        private List<Controller> controllers;
        private bool hasKeyboard = false;
        private int controllerIndex = 0;

        public Controller this[int index] => controllers.ElementAtOrDefault(index);
        public int Count => controllers.Count;

        public UwpControllerSource()
        {
            controllers = new List<Controller>(4);
        }

        public void Update()
        {
            if (!hasKeyboard)
            {
                var state = Keyboard.GetState();
                if (state.GetPressedKeys().Any())
                {
                    controllerIndex++;
                    hasKeyboard = true;
                    controllers.Add(new KeyboardController());
                }
            }

            foreach (var pad in Gamepad.Gamepads)
            {
                if (pad.GetCurrentReading().Buttons != 0 && !controllers.OfType<UwpController>().Any(c => c.pad == pad))
                {
                    controllers.Add(new UwpController(pad));
                }
            }

            foreach (var item in controllers)
            {
                item?.UpdateControllerReading();
            }
        }
    }
}
