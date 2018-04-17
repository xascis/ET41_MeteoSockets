using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using static ET41_MeteoSockets.GetSSID;

[assembly: Xamarin.Forms.Dependency(typeof(ET41_MeteoSockets.UWP.GetSSIDImplementation))]
namespace ET41_MeteoSockets.UWP
{
    class GetSSIDImplementation : ISSID
    {
        public string GetSSID()
        {
            string wifi = "Desconocida (UWP)";
            IReadOnlyList<ConnectionProfile> deviceAccountID = null;
            deviceAccountID = NetworkInformation.GetConnectionProfiles();
            if (deviceAccountID.Count != 0)
            {
                try
                {
                    foreach (var conProf in deviceAccountID)
                    {
                        if (conProf.IsWlanConnectionProfile)
                        {
                            WlanConnectionProfileDetails wlanConnectionProfileDetails = conProf.WlanConnectionProfileDetails;
                            if (wlanConnectionProfileDetails.GetConnectedSsid() != "")
                            {
                                wifi = wlanConnectionProfileDetails.GetConnectedSsid();
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Fallo recuperando información de la Wifi: " + ex.ToString());
                    throw;
                }
            }
            return wifi;
        }
    }
}
