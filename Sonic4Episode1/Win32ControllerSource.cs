using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using Sonic4Episode1.Abstraction;

namespace Sonic4Episode1
{
    class Win32Controller : Controller
    {
        private int _index;
        private bool _doVibrate = true;
        private KeyboardController _keyboardController;

        public Win32Controller(int index)
        {
            this._index = index;

            if (index == 0)
            {
                _keyboardController = new KeyboardController();
            }
        }

        public override void SetVibrationEnabled(bool enabled)
        {
            _doVibrate = enabled;
            if (!_doVibrate)
                GamePad.SetVibration(_index, 0, 0);
        }

        public override void SetVibration(ushort left, ushort right)
        {
            if (_doVibrate)
                GamePad.SetVibration(_index, (float)left / ushort.MaxValue, (float)right / ushort.MaxValue);
        }

        public override void UpdateControllerReading(ref ControllerReading reading)
        {
            var state = GamePad.GetState(_index, GamePadDeadZone.Circular);

            if (_index == 0)
                _keyboardController.UpdateControllerReading(ref reading);
            else if (!state.IsConnected)
                return;

            reading.alx = (short)(state.ThumbSticks.Left.X * short.MaxValue);
            reading.aly = (short)(state.ThumbSticks.Left.Y * short.MaxValue);

            reading.arx = (short)(state.ThumbSticks.Right.X * short.MaxValue);
            reading.ary = (short)(state.ThumbSticks.Right.Y * short.MaxValue);

            if (state.Buttons.A == ButtonState.Pressed)
                reading.direction |= (ControllerConsts.JUMP_BUTTON | ControllerConsts.CONFIRM);
            if (state.Buttons.B == ButtonState.Pressed)
                reading.direction |= (ControllerConsts.JUMP_BUTTON | ControllerConsts.CANCEL);

            if (state.Buttons.Y == ButtonState.Pressed || state.Buttons.X == ButtonState.Pressed)
                reading.direction |= ControllerConsts.SUPER_SONIC;

            if (state.Buttons.Start == ButtonState.Pressed)
                reading.direction |= ControllerConsts.START;

            if (state.DPad.Left == ButtonState.Pressed)
                reading.direction |= ControllerConsts.LEFT;
            if (state.DPad.Up == ButtonState.Pressed)
                reading.direction |= ControllerConsts.UP;
            if (state.DPad.Down == ButtonState.Pressed)
                reading.direction |= ControllerConsts.DOWN;
            if (state.DPad.Right == ButtonState.Pressed)
                reading.direction |= ControllerConsts.RIGHT;
        }
    }

    class Win32ControllerSource : IControllerSource
    {
        private List<Controller> controllers;
        private bool hasKeyboard = false;
        private int controllerIndex = 0;

        public Controller this[int index] => controllers.ElementAtOrDefault(index);
        public int Count => controllers.Count;

        public Win32ControllerSource()
        {
            controllers = new List<Controller>(4);
            for (int i = 0; i < 4; i++)
            {
                controllers.Add(new Win32Controller(i));
            }
        }

        public void Update()
        {
            foreach (var item in controllers)
            {
                item?.UpdateControllerReading();
            }
        }
    }
}
