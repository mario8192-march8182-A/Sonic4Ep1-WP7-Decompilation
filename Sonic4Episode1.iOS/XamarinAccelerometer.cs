using System;
using Sonic4Episode1.Abstraction;
using Xamarin.Essentials;

namespace Sonic4Episode1.iOS
{
    public class XamarinAccelerometer : IAccelerometer
    {
        public XamarinAccelerometer()
        {
        }

        public event EventHandler<AccelerometerChangeEventArgs> ReadingChanged;


        public bool IsSupported()
        {
            return true;
        }

        public bool Initialize()
        {
            Accelerometer.ReadingChanged += OnReadingChanged;
            Accelerometer.Start(SensorSpeed.Game);
            return true;
        }

        private void OnReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            ReadingChanged?.Invoke(this, new AccelerometerChangeEventArgs() { X = e.Reading.Acceleration.X, Y = e.Reading.Acceleration.Y, Z = e.Reading.Acceleration.Z });
        }
    }
}
