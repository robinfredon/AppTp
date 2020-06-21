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
  public class GyroscopeReader
  {
    // Set speed  delay for monitoring changes.
    static SensorSpeed speed = SensorSpeed.UI;
    public static float accX;
    public static float accY;
    public static float accZ;

    public GyroscopeReader()
    {
      // Register for reading changes, be sure to unsubscribe when finished
      Gyroscope.ReadingChanged += this.Gyroscope_ReadingChanged;
    }

    void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
    {
      var data = e.Reading;
      // Process Angular Velocity X, Y, and Z
      accX = data.AngularVelocity.X;
      accY = data.AngularVelocity.Y;
      accZ = data.AngularVelocity.Z;
      Log.Debug("Dev_Gyroscope", $"Reading Gyroscope: X: {data.AngularVelocity.X }, Y: {data.AngularVelocity.Y }, Z:{data.AngularVelocity.Z}");

    }

    public static void ToggleGyroscope()
    {
      try
      {
        if (Gyroscope.IsMonitoring)
        {
          Log.Debug("Dev_Gyroscope", "Stop Gyroscope");
          Gyroscope.Stop();
        }
        else
        {
          Gyroscope.Start(speed);
          Log.Debug("Dev_Gyroscope", "Start Gyroscope");
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
