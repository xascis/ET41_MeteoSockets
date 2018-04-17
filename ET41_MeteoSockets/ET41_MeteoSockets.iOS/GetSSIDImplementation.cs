using System;
using System.Collections.Generic;
using System.Text;
using static ET41_MeteoSockets.GetSSID;

[assembly: Xamarin.Forms.Dependency(typeof(ET41_MeteoSockets.iOS.GetSSIDImplementation))]
namespace ET41_MeteoSockets.iOS
{
    class GetSSIDImplementation : ISSID
    {
        public string GetSSID()
        {
            return "No implementado (iOS)";
        }
    }
}
