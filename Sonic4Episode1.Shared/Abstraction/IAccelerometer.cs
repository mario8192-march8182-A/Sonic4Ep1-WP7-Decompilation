using System;
using System.Collections.Generic;
using System.Text;

namespace Sonic4Episode1.Abstraction
{
    public class AccelerometerChangeEventArgs
    {
        public float X;
        public float Y;
        public float Z;
    }

    public interface IAccelerometer
    {
        event EventHandler<AccelerometerChangeEventArgs> ReadingChanged;

        bool IsSupported();
        bool Initialize();
    }
}
