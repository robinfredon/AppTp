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
    public static decimal gyrX = 0.0m;
    public static decimal gyrY = 0.0m;
    public static decimal gyrZ = 0.0m;
    public static decimal oldgyrX;
    public static decimal oldgyrY;
    public static decimal oldgyrZ;
    public static decimal deltagyrX;
    public static decimal deltagyrY;
    public static decimal deltagyrZ;
    private const int nbrDeciDebug = 4;
    private const int nbrDeci = 2;
    public static bool isStartedG = false;
    public static bool isHold = false;
    public static bool isLaunchedG = false;

    public GyroscopeReader()
    {
      // Register for reading changes, be sure to unsubscribe when finished
      Gyroscope.ReadingChanged += this.Gyroscope_ReadingChanged;
    }

    // Handler
    void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
    {
      if (!isLaunchedG)
      {
        var data = e.Reading;

        // Process Angular Velocity X, Y, and Z
        oldgyrX = gyrX;
        oldgyrY = gyrY;
        oldgyrZ = gyrZ;
        gyrX = Convert(data.AngularVelocity.X);
        gyrY = Convert(data.AngularVelocity.Y);
        gyrZ = Convert(data.AngularVelocity.Z);

        computeDelta();

        // Log
        Log.Debug("Dev_Data_Gyroscope", $"Reading Gyroscope: X: {data.AngularVelocity.X }, Y: {data.AngularVelocity.Y }, Z: {data.AngularVelocity.Z}");
        Log.Debug("Dev_Data_Gyroscope", $"Round Gyroscope: X: {gyrX }, Y: {gyrY }, Z: {gyrZ}");

        isLaunchedG = true;
      }
    }

    // Compute each delta
    private void computeDelta()
    {
      deltagyrX = (oldgyrX - gyrX) * 100;
      deltagyrY = (oldgyrY - gyrY) * 100;
      deltagyrZ = (oldgyrZ - gyrZ) * 100;
    }

    // Convert from float to decimal
    private decimal Convert(float x)
    {
       return Decimal.Round(new decimal(x), nbrDeci);
    }

    // Check if movement is detected
    private void CheckMoving()
    {
      var deltas = deltagyrY + deltagyrX + deltagyrZ;
      if (deltas > 30 || deltas < -30)
      {
        isHold = true;
      }
      else
      {
        isHold = false;
      }

      Log.Debug("Dev_Data_Gyroscope", $"isHold = {isHold}");
      Log.Debug("Dev_Data_Gyroscope", $"Deltas = {deltas}");
    }

    // Toggle 
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
