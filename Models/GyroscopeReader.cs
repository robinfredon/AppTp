using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content.Res;
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

    // COORD
    public static decimal gyrX = 0.0m;
    public static decimal gyrY = 0.0m;
    public static decimal gyrZ = 0.0m;
    public static decimal oldgyrX;
    public static decimal oldgyrY;
    public static decimal oldgyrZ;
    public static decimal deltagyrX;
    public static decimal deltagyrY;
    public static decimal deltagyrZ;

    // BOOL
    public static bool isStartedG = false;
    public static bool isHoldG = false;
    public static bool isRollG = false;
    public static bool isLaunchedG = false;
    public static bool isFirstRoll = true;
    public static bool isRollUpdated = false;

    // CONST
    private const int nbrDeciDebug = 4;
    private const int nbrDeci = 2;
    private const int limitBreakMove = 40;
    private const int limitBreakStand = 2;
    private const int limitBreakRoll = 200;

    // VAR
    public static int nbRowRoll = 0;

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

        CheckMoving();

        // Log
        Log.Debug("Dev_Data_Gyr_Data", $"Reading Gyroscope: X: {data.AngularVelocity.X }, Y: {data.AngularVelocity.Y }, Z: {data.AngularVelocity.Z}");
        Log.Debug("Dev_Data_Gyr_Data", $"Round Gyroscope: X: {gyrX }, Y: {gyrY }, Z: {gyrZ}");

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
      // Start & end management
      var deltas = deltagyrY + deltagyrX + deltagyrZ;
      if (isHoldG)
      {
        if (Math.Abs(deltas) < limitBreakStand)
        {
          isHoldG = false;
        }
      }
      else
      {
        if (Math.Abs(deltas) > limitBreakMove)
        {
          isHoldG = true;
        }
      }

      // Roll management
      if (isRollG)
      {
        if (Math.Abs(deltas) > limitBreakRoll)
        {
          isRollG = true;
          nbRowRoll++;
        }
        else
        {
          isRollG = false;
          nbRowRoll = 0;
        }
      }
      else
      {
        if (Math.Abs(deltas) > limitBreakRoll)
        {
          isRollG = true;
          nbRowRoll = 1;
          isFirstRoll = !isFirstRoll;
        }
      }
      isRollUpdated = true;

      Log.Debug("Dev_Data_Gyr_Move", $"isHoldG = {isHoldG}");
      Log.Debug("Dev_Data_Gyr_Move", $"isRoll G = {isRollG}");
      Log.Debug("Dev_Data_Gyr_Move", $"NbRowRoll = {nbRowRoll}");
      Log.Debug("Dev_Data_Gyr_Move", $"Deltas G = {deltas}");
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
