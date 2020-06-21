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
    // COORD
    public static decimal accX = 0.0m;
    public static decimal accY = 0.0m;
    public static decimal accZ = 0.0m;
    public static decimal oldaccX;
    public static decimal oldaccY;
    public static decimal oldaccZ;
    public static decimal deltaaccX;
    public static decimal deltaaccY;
    public static decimal deltaaccZ;
    // BOOL
    public static bool isLaunchedA = false;
    public static bool isStartedA = false;
    public static bool isHoldA = false;
    // CONST
    private const int nbrDeciDebug = 4;
    private const int nbrDeci = 2;
    private const int limitBreakMove = 30;

    public AccelerometerReader()
    {
      // Register for reading changes, be sure to unsubscribe when finished
      Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
    }

    // Handler
    void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
    {
      if (!isLaunchedA)
      {
        var data = e.Reading;

        // Process Acceleration X, Y, and Z
        oldaccX = accX;
        oldaccY = accY;
        oldaccZ = accZ;
        accX = Convert(data.Acceleration.X);
        accY = Convert(data.Acceleration.Y);
        accZ = Convert(data.Acceleration.Z);

        /*computeDelta();

        CheckMoving();*/

        // Log
        Log.Debug("Dev_Data_Accelerometer", $"Reading Accelerometer: X: {data.Acceleration.X }, Y: {data.Acceleration.Y }, Z: {data.Acceleration.Z}");
        Log.Debug("Dev_Data_Accelerometer", $"Round Accelerometer: X: {accX }, Y: {accY }, Z: {accZ}");

        isLaunchedA = true;
      }
    }

    // Compute each delta
    private void computeDelta()
    {
      deltaaccX = (oldaccX - accX) * 100;
      deltaaccY = (oldaccY - accY) * 100;
      deltaaccZ = (oldaccZ - accZ) * 100;
    }

    // Convert from float to decimal
    private decimal Convert(float x)
    {
        return Decimal.Round(new decimal(x), nbrDeci);
    }

    // Check if movement is detected
    private void CheckMoving()
    {
      var deltas = deltaaccY + deltaaccX + deltaaccZ;
      if (deltas > limitBreakMove || deltas < -limitBreakMove)
      {
        isHoldA = true;
      }
      else
      {
        isHoldA = false;
      }

      Log.Debug("Dev_Move_Accelerometer", $"isHoldA = {isHoldA}");
      Log.Debug("Dev_Move_Accelerometer", $"Deltas = {deltas}");
    }

    // Toggle 
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
