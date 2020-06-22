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
    
    // COORD
    public static decimal accX = 0.0m;
    public static decimal accY = 0.0m;
    public static decimal accZ = 1.0m;
    public static decimal oldAccX;
    public static decimal oldAccY;
    public static decimal oldAccZ;
    public static decimal deltaAccX;
    public static decimal deltaAccY;
    public static decimal deltaAccZ;
    public static decimal oldDeltaX;
    public static decimal oldDeltaY;
    public static decimal oldDeltaZ;

    // BOOLEAN
    public static bool isLaunchedA = false;
    public static bool isStartedA = false;
    public static bool isHoldA = false;
    public static bool isChock = false;
    public static bool isChocUpdated = false;

    // CONST
    private const int nbrDeciDebug = 4;
    private const int nbrDeci = 2;
    private const int limitBreakMove = 40;
    private const int limitBreakStand = 2;
    private const int limitChoc = 50;
    public const int timeIntervalMs = 500;

    // VAR
    public static int nbChock = 0;
    public static int nbTickLin = 0;

    public AccelerometerReader()
    {
      // Register for reading changes, be sure to unsubscribe when finished
      Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
    }

    void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
    {
      if (!isLaunchedA)
      {
        var data = e.Reading;

        // Process Acceleration X, Y, and Z
        oldAccX = accX;
        oldAccY = accY;
        oldAccZ = accZ;
        accX = Convert(data.Acceleration.X);
        accY = Convert(data.Acceleration.Y);
        accZ = Convert(data.Acceleration.Z);
        computeDelta();

        CheckMoving();

        // Log
        Log.Debug("Dev_Data_Acc_Delta", $"Delta Accelerometer: X: {deltaAccX }, Y: {deltaAccY }, Z: {deltaAccZ}");
        Log.Debug("Dev_Data_Acc_Coord", $"Reading Accelerometer: X: {data.Acceleration.X }, Y: {data.Acceleration.Y }, Z: {data.Acceleration.Z}");
        Log.Debug("Dev_Data_Acc_Coord", $"Round Accelerometer: X: {accX }, Y: {accY }, Z: {accZ}");

        isLaunchedA = true;
      }
    }

    // Compute each delta
    private void computeDelta()
    {
      oldDeltaX = deltaAccX;
      oldDeltaY = deltaAccY;
      oldDeltaZ = deltaAccZ;
      deltaAccX = (oldAccX - accX) * 100;
      deltaAccY = (oldAccY - accY) * 100;
      deltaAccZ = (oldAccZ - accZ) * 100;

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
      var deltas = deltaAccY + deltaAccX + deltaAccZ;
      if (isHoldA)
      {
        if(Math.Abs(deltaAccZ) < limitBreakStand && Math.Abs(deltaAccX) < limitBreakStand && Math.Abs(deltaAccY) < limitBreakStand)
        {
          isHoldA = false;
        }
      }
      else
      {
        if(!isStartedA)
        {
          if (Math.Abs(deltas) > limitBreakMove || Math.Abs(deltaAccX) > limitBreakMove || Math.Abs(deltaAccY) > limitBreakMove)
          {
            isHoldA = true;
          }
        }
        else
        {
          if(Math.Abs(deltas) > limitBreakStand)
          {
            isHoldA = true;
          }
        }
      }

      var oldDeltas = oldDeltaY + oldDeltaX + oldDeltaZ;
      // Choc management
      if (isHoldA && isStartedA && nbTickLin > 1)
      {
        if((Math.Sign(deltas) != Math.Sign(oldDeltas)) && !isChock && Math.Abs(deltas) > limitChoc)
        {
          isChock = true;
          nbChock = (nbChock+1) % 4;
        }
        else
        {
          isChock = false;
        }
      }

      isChocUpdated = true;

      Log.Debug("Dev_Data_Acc_Move", $"isHoldA = {isHoldA}");
      Log.Debug("Dev_Data_Acc_Move", $"Deltas A = {deltas}");
      Log.Debug("Dev_Data_Acc_Move", $"OldDeltas A = {oldDeltas}");
      Log.Debug("Dev_Data_Acc_Move", $"isChockA = {isChock}");
      Log.Debug("Dev_Data_Acc_Move", $"nbChoc = {nbChock}");
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
