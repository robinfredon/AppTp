using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;

namespace AppTP.Models
{
  public class AccelerometerReader
  {
    // Set speed  delay for monitoring changes.
    static SensorSpeed speed = SensorSpeed.UI;
    public static float accX = 0.0F;
    public static float accY = 0.0F;
    public static float accZ = 0.0F;
    public static float oldaccX;
    public static float oldaccY;
    public static float oldaccZ;

    public AccelerometerReader()
    {
      // Register for reading changes, be sure to unsubscribe when finished
      Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
    }

    void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
    {
      var data = e.Reading;

      oldaccX = accX;
      oldaccY = accY;
      oldaccZ = accZ;
      accX = data.Acceleration.X;
      accY = data.Acceleration.Y;
      accZ = data.Acceleration.Z;

      Log.Debug("Dev_Accelerometer", $"Reading Accelerometer: X: {data.Acceleration.X }, Y: {data.Acceleration.Y }, Z:{data.Acceleration.Z}");
      // Process Acceleration X, Y, and Z
    }

    public static void ToggleAccelerometer()
    {
      try
      {
        if (Accelerometer.IsMonitoring)
        {
          Log.Debug("Dev_Accelerometer", "Stop Accelerometer");
          Accelerometer.Stop();
          //Accelerometer.ReadingChanged -= Accelerometer_ReadingChanged;
        }
        else
        {
          Log.Debug("Dev_Accelerometer", "Start Accelerometer");
          Accelerometer.Start(speed);
          //Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
        }
      }
      catch (FeatureNotSupportedException fnsEx)
      {
        // Feature not supported on device
      }
      catch (Exception ex)
      {
        // Other error has occurred
      }
    }

  }
}
