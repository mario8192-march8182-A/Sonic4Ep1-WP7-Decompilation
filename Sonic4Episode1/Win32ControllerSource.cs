using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Sonic4Episode1.Abstraction;

namespace Sonic4Episode1
{
    class Win32Controller : Controller
    {
        public int index;
        private bool doVibrate = true;

        public Win32Controller(int index)
        {
            this.index = index;
        }

        public override void SetVibrationEnabled(bool enabled)
        {
            this.doVibrate = enabled;
        }

        public override void SetVibration(ushort left, ushort right)
        {
            if (this.doVibrate)
                GamePad.SetVibration(index, (float)left / ushort.MaxValue, (float)right / ushort.MaxValue);
        }

        protected override void UpdateControllerReading(ref ControllerReading reading)
        {
            var state = GamePad.GetState(index, GamePadDeadZone.Circular);

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

            for (int i = 0; i < 4; i++)
            {
                if (GamePad.GetState(i).IsButtonDown(Buttons.Start) && !controllers.OfType<Win32Controller>().Any(c => c.index == i))
                {
                    controllers.Add(new Win32Controller(i));
                }
            }

            foreach (var item in controllers)
            {
                item?.UpdateControllerReading();
            }
        }
    }
}
