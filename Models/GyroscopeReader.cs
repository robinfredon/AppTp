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
    public static decimal accX = 0.0m;
    public static decimal accY = 0.0m;
    public static decimal accZ = 0.0m;
    public static decimal oldaccX;
    public static decimal oldaccY;
    public static decimal oldaccZ;
    private const int nbrDeciDebug = 4;
    private const int nbrDeci = 2;
    public static bool isStartedG = false;
    public static bool isHold = false;

    public GyroscopeReader()
    {
      // Register for reading changes, be sure to unsubscribe when finished
      Gyroscope.ReadingChanged += this.Gyroscope_ReadingChanged;
    }

    void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
    {
      if (!isStartedG)
      {
        var data = e.Reading;

        // Process Angular Velocity X, Y, and Z
        oldaccX = accX;
        oldaccY = accY;
        oldaccZ = accZ;
        accX = Convert(data.AngularVelocity.X);
        accY = Convert(data.AngularVelocity.Y);
        accZ = Convert(data.AngularVelocity.Z);

        // Log
        Log.Debug("Dev_Gyroscope", $"Reading Gyroscope: X: {data.AngularVelocity.X }, Y: {data.AngularVelocity.Y }, Z: {data.AngularVelocity.Z}");
        Log.Debug("Dev_Gyroscope", $"Round Gyroscope: X: {accX }, Y: {accY }, Z: {accZ}");

        isStartedG = true;
      }
    }

    private decimal Convert(float x)
    {
      if (isHold)
      {
        return Decimal.Round(new decimal(x), nbrDeci);
      }
      else
      {
        return Decimal.Truncate(new decimal(x));
      }
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
