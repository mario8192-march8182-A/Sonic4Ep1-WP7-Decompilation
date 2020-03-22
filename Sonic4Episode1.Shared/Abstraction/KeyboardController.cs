using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Sonic4Episode1.Abstraction
{
    public class KeyboardController : Controller
    {
        public override void SetVibration(ushort left, ushort right) { }
        public override void SetVibrationEnabled(bool enabled) { }

        public override void UpdateControllerReading(ref ControllerReading reading)
        {
            var state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Up))
                reading.direction |= ControllerConsts.UP;
            if (state.IsKeyDown(Keys.Down))
                reading.direction |= ControllerConsts.DOWN;
            if (state.IsKeyDown(Keys.Left))
                reading.direction |= ControllerConsts.LEFT;
            if (state.IsKeyDown(Keys.Right))
                reading.direction |= ControllerConsts.RIGHT;
            if (state.IsKeyDown(Keys.A) || state.IsKeyDown(Keys.S) || state.IsKeyDown(Keys.D))
                reading.direction |= ControllerConsts.JUMP_BUTTON;
            if (state.IsKeyDown(Keys.W))
                reading.direction |= ControllerConsts.SUPER_SONIC;
            if (state.IsKeyDown(Keys.Escape))
                reading.direction |= ControllerConsts.START;
        }
    }
}
