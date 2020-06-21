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
using Java.Lang;
using Xamarin.Essentials;

namespace AppTP.Models
{
  public class AccelerometerReader
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
    public static bool isStartedA = false;
    public static bool isHold = false;

    public AccelerometerReader()
    {
      // Register for reading changes, be sure to unsubscribe when finished
      Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
    }

    void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
    {
      if (!isStartedA)
      {
        var data = e.Reading;

        // Process Acceleration X, Y, and Z
        oldaccX = accX;
        oldaccY = accY;
        oldaccZ = accZ;
        accX = Convert(data.Acceleration.X);
        accY = Convert(data.Acceleration.Y);
        accZ = Convert(data.Acceleration.Z);

        CheckMoving();
        // Log
        Log.Debug("Dev_Accelerometer", $"Reading Accelerometer: X: {data.Acceleration.X }, Y: {data.Acceleration.Y }, Z: {data.Acceleration.Z}");
        Log.Debug("Dev_Accelerometer", $"Round Accelerometer: X: {accX }, Y: {accY }, Z: {accZ}");

        isStartedA = true;
      }
    }

    private decimal Convert(float x)
    {
      if(isHold)
      {
        return Decimal.Round(new decimal(x), nbrDeci);
      }
      else
      {
        return Decimal.Truncate(new decimal(x));
      }
    }

    private void CheckMoving()
    {
      if(oldaccX == accX && oldaccY == accY)
      {
        isHold = false;
      }
      else
      {
        isHold = true;
      }

      Log.Debug("Dev_Accelerometer", $"isHold = {isHold}");
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
      catch (System.Exception ex)
      {
        // Other error has occurred
      }
    }

  }
}
