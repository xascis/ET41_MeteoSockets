using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using static ET41_MeteoSockets.GetSSID;

[assembly: Xamarin.Forms.Dependency(typeof(ET41_MeteoSockets.Droid.GetSSIDImplementation))]
namespace ET41_MeteoSockets.Droid
{
    class GetSSIDImplementation : ISSID
    {
        public string GetSSID()
        {
            string wifi = "Desconocida (Android)";
            WifiManager wifiManager = (WifiManager)(Application.Context.GetSystemService(Context.WifiService));
            if (wifiManager != null)
            {
                wifi = wifiManager.ConnectionInfo.SSID;
            }
            return wifi;
        }
    }
}