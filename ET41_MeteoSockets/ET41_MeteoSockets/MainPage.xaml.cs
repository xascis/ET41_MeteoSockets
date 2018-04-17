using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ET41_MeteoSockets
{
    [ContentProperty("Source")]
    public class ImageResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
            {
                return null;
            }
            return ImageSource.FromResource(Source);
        }
    }
	public partial class MainPage : ContentPage
	{
        string wifi = "-";
        Comunicaciones coms = null;

		public MainPage()
		{
			InitializeComponent();
            RecuperaInfo.Clicked += RecuperaInfo_Clicked;

            var wifimanager = new GetSSID();
            wifi = wifimanager.GetSSIDName();
            conexInfo.Text = "Conexión Wifi SSID: " + wifi;

            coms = new Comunicaciones();
            coms.Connect("kona2.alc.upv.es", 8081);
		}

        private async void RecuperaInfo_Clicked(object sender, EventArgs e)
        {
            //var result = await DisplayAlert("Confirmación", "¿Estás seguro que quieres contectar?", "Sí", "No");
            var dialogo = await UserDialogs.Instance.LoginAsync(new LoginConfig
            {
                Message = "Introduce tu nombre de usuario y contraseña",
                OkText = "Conecta",
                CancelText = "Cancela",
                LoginPlaceholder = "Nombre usuario",
                PasswordPlaceholder = "Contraseña"
            }, null);

            if (dialogo.Ok && dialogo.LoginText == "admin" && dialogo.Password == "aaaa")
            {
                conexInfo.Text = await coms.Enviar(0, 0, 0);
                infoMeteo.Text = coms.informacion;

                manecilla.AnchorY = 0.9;
                //manecilla.Rotation = coms.dirvent;
                bool x = await manecilla.RotateTo(coms.dirvent, 2000, Easing.BounceOut);
            } else
            {
                var toastConf = new ToastConfig("Usuario y/o contraseña incorrecta");
                toastConf.SetDuration(3000);
                toastConf.SetBackgroundColor(System.Drawing.Color.FromArgb(12,131,193));
                UserDialogs.Instance.Toast(toastConf);
            }
            
        }
	}
}
