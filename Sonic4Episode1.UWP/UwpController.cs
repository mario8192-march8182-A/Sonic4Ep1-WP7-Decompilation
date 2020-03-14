using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sonic4Episode1.Abstraction;
using Windows.Gaming.Input;

/*                              */
/*      A RAGING HOMOSEXUAL     */
/*           PRESENTS           */
/*                              */

namespace Sonic4Episode1.UWP
{
    public class UwpController : Controller
    {
        public Gamepad pad;

        public UwpController(Gamepad gamepad)
        {
            pad = gamepad;
        }

        public override void SetVibration(ushort left, ushort right)
        {
            pad.Vibration = new GamepadVibration()
            {
                LeftMotor = (float)left / ushort.MaxValue,
                RightMotor = (float)right / ushort.MaxValue
            };
        }

        public override void SetVibrationEnabled(bool enabled)
        {
            // ?
        }

        protected override void UpdateControllerReading(ref ControllerReading padData)
        {
            var reading = pad.GetCurrentReading();

            padData.alx = (short)(reading.LeftThumbstickX * short.MaxValue);
            padData.aly = (short)(reading.LeftThumbstickY * short.MaxValue);

            padData.arx = (short)(reading.RightThumbstickX * short.MaxValue);
            padData.ary = (short)(reading.RightThumbstickY * short.MaxValue);

            if (reading.Buttons.HasFlag(GamepadButtons.A))
                padData.direction |= (ControllerConsts.JUMP_BUTTON | ControllerConsts.CONFIRM);
            if (reading.Buttons.HasFlag(GamepadButtons.B))
                padData.direction |= (ControllerConsts.JUMP_BUTTON | ControllerConsts.CANCEL);

            if (reading.Buttons.HasFlag(GamepadButtons.Y) || reading.Buttons.HasFlag(GamepadButtons.X))
                padData.direction |= ControllerConsts.SUPER_SONIC;

            if (reading.Buttons.HasFlag(GamepadButtons.Menu))
                padData.direction |= ControllerConsts.START;

            if (reading.Buttons.HasFlag(GamepadButtons.DPadLeft))
                padData.direction |= ControllerConsts.LEFT;
            if (reading.Buttons.HasFlag(GamepadButtons.DPadUp))
                padData.direction |= ControllerConsts.UP;
            if (reading.Buttons.HasFlag(GamepadButtons.DPadDown))
                padData.direction |= ControllerConsts.DOWN;
            if (reading.Buttons.HasFlag(GamepadButtons.DPadRight))
                padData.direction |= ControllerConsts.RIGHT;
        }
    }
}
