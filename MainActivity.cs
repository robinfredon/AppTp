using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Support.V4.Content.Res;
using System;
using AppTP.Models;
using Android.Locations;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Android.Util;
using Android.Views;
using Java.Util.Zip;
using Android.Media;

namespace AppTP
{
  [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
  public class MainActivity : AppCompatActivity
  {
    /*private static TextView accView;
    private static TextView accViewX;
    private static TextView accViewY;
    private static TextView accViewZ;
    private static TextView gyrViewX;
    private static TextView gyrViewY;
    private static TextView gyrViewZ;*/
    private static GyroscopeReader gyrRead;
    private static AccelerometerReader accRead;
    private List<int> listShortVoice = new List<int>();
    private List<int> listLongVoice = new List<int>();
    private List<int> listCurrentVoice = new List<int>();
    private Boolean isPlaying = false;
    private int currentIdVoice = Resource.Id.bVoice1;
    private MediaPlayer mediaPlayer = new MediaPlayer();
    private Boolean isStarted = false;
    private int nbChoc;
    private int nbRoulis;

    protected override void OnCreate(Bundle savedInstanceState)
    {
      base.OnCreate(savedInstanceState);
      Xamarin.Essentials.Platform.Init(this, savedInstanceState);
      // Set our view from the "main" layout resource
      SetContentView(Resource.Layout.activity_main);

      // Init voices lists
      listShortVoice.AddRange(new int[] { Resource.Raw.Voice01_01, Resource.Raw.Voice01_02, Resource.Raw.Voice01_03, Resource.Raw.Voice01_04, Resource.Raw.Voice01_05, Resource.Raw.Voice01_06, Resource.Raw.Voice01_07, Resource.Raw.Voice01_08, Resource.Raw.Voice01_09, Resource.Raw.Voice01_10 });
      listLongVoice.AddRange(new int[] { Resource.Raw.Voice02_01, Resource.Raw.Voice02_02, Resource.Raw.Voice02_03, Resource.Raw.Voice02_04, Resource.Raw.Voice02_05, Resource.Raw.Voice02_06, Resource.Raw.Voice02_07, Resource.Raw.Voice02_08, Resource.Raw.Voice02_09, Resource.Raw.Voice02_10 });
      listCurrentVoice = listShortVoice;

      initVoice();

      // Add onClick Listener
      Button button = (Button)FindViewById(Resource.Id.bVoice1);
      button.Click += delegate { OnVoiceClick(button); };
      Button button2 = (Button)FindViewById(Resource.Id.bVoice2);
      button2.Click += delegate { OnVoiceClick(button2); };
      Button button3 = (Button)FindViewById(Resource.Id.bVoice3);
      button3.Click += delegate { OnVoiceClick(button3); };
      Button button4 = (Button)FindViewById(Resource.Id.bVoice4);
      button4.Click += delegate { OnVoiceClick(button4); };
      Button button5 = (Button)FindViewById(Resource.Id.bVoice5);
      button5.Click += delegate { OnVoiceClick(button5); };
      Button button6 = (Button)FindViewById(Resource.Id.bVoice6);
      button6.Click += delegate { OnVoiceClick(button6); };
      Button button7 = (Button)FindViewById(Resource.Id.bVoice7);
      button7.Click += delegate { OnVoiceClick(button7); };
      Button button8 = (Button)FindViewById(Resource.Id.bVoice8);
      button8.Click += delegate { OnVoiceClick(button8); };
      Button button9 = (Button)FindViewById(Resource.Id.bVoice9);
      button9.Click += delegate { OnVoiceClick(button9); };
      Button button10 = (Button)FindViewById(Resource.Id.bVoice10);
      button10.Click += delegate { OnVoiceClick(button10); };

      // SET UP for test_layout.xml view
      /*accView = FindViewById<TextView>(Resource.Id.accView);
      accViewX = FindViewById<TextView>(Resource.Id.accViewX);
      accViewY = FindViewById<TextView>(Resource.Id.accViewY);
      accViewZ = FindViewById<TextView>(Resource.Id.accViewZ);
      gyrViewX = FindViewById<TextView>(Resource.Id.gyrViewX);
      gyrViewY = FindViewById<TextView>(Resource.Id.gyrViewY);
      gyrViewZ = FindViewById<TextView>(Resource.Id.gyrViewZ);*/

      gyrRead = new GyroscopeReader();
      accRead = new AccelerometerReader();
      startTimer();
    }

    private void initVoice()
    {
      nbChoc = 0;
      nbRoulis = 0;
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
    {
      Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

      base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }

    public void OnVoiceClick(Button t_button)
    {
      Boolean notInclude = false;
      while (isPlaying)
      {
        //null
      }
      if(currentIdVoice != t_button.Id)
      {
        switch (t_button.Id)
        {
          case Resource.Id.bVoice1:
            listCurrentVoice = listShortVoice;
            break;
          case Resource.Id.bVoice2:
            listCurrentVoice = listLongVoice;
            break;
          default:
            notInclude = true;
            break;
        }

        if (notInclude)
        {
          Log.Warn("Dev_Voice", $"Voice {t_button.Text} not included");
        }
        else
        {
          initVoice();
          currentIdVoice = t_button.Id;
          Log.Info("Dev_Voice", $"New voice : {t_button.Text}");
        }
      }
      else
      {
        Log.Info("Dev_Voice", "Voice not changed");
      }
    }

    public void startTimer()
    {
      System.Timers.Timer Timer1 = new System.Timers.Timer();
      Timer1.Start(); 
      Timer1.Interval = 500;
      Timer1.Enabled = true;
      Timer1.AutoReset = true;
      Timer1.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
      {
        RunOnUiThread(() =>
        {
          Log.Debug("App_Dev", "RunOnUiThread()");

          AccelerometerReader.ToggleAccelerometer();
          /*accViewX.Text = "X: " + AccelerometerReader.accX.ToString();
          accViewY.Text = "Y: " + AccelerometerReader.accY.ToString();
          accViewZ.Text = "Z: " + AccelerometerReader.accZ.ToString();
          //AccelerometerReader.ToggleAccelerometer();*/

          GyroscopeReader.ToggleGyroscope();
          /*gyrViewX.Text = "G X: " + GyroscopeReader.accX.ToString();
          gyrViewY.Text = "G Y: " + GyroscopeReader.accY.ToString();
          gyrViewZ.Text = "G Z: " + GyroscopeReader.accZ.ToString();
          //GyroscopeReader.ToggleGyroscope();*/

          studyMove();
        });
      };
    }

    public void studyMove()
    {
      // Start
      if (!isStarted)
      {
        if(AccelerometerReader.accX != AccelerometerReader.oldaccX && AccelerometerReader.accY != AccelerometerReader.oldaccY)
        {
          //null
        }
      }
    }
  }
}
