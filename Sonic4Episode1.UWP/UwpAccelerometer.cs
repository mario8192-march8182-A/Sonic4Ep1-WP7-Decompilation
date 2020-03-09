using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sonic4Episode1.Abstraction;
using Windows.Devices.Sensors;

namespace Sonic4Episode1.UWP
{
    class UwpAccelerometer : IAccelerometer
    {
        private Accelerometer _accelerometer;

        public event EventHandler<AccelerometerChangeEventArgs> ReadingChanged;

        public UwpAccelerometer()
        {
            _accelerometer = Accelerometer.GetDefault();
        }

        public bool IsSupported()
        {
            return _accelerometer != null;
        }

        public bool Initialize()
        {
            _accelerometer.ReadingChanged += OnReadingChanged;
            return true;
        }

        private void OnReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            ReadingChanged?.Invoke(this, new AccelerometerChangeEventArgs()
            {
                X = (float)args.Reading.AccelerationX,
                Y = (float)args.Reading.AccelerationY,
                Z = (float)args.Reading.AccelerationZ
            });
        }
    }
}
