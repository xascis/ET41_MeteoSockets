using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ET41_MeteoSockets
{
    public class GetSSID
    {
        public string wifi = "-";

        public interface ISSID
        {
            string GetSSID();
        }

        public string GetSSIDName()
        {
            wifi = DependencyService.Get<ISSID>().GetSSID();
            return wifi;
        }
    }
}
