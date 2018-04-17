using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace ET41_MeteoSockets
{
    class Comunicaciones
    {
        Socket _socket = null;
        static ManualResetEvent _clientDone = new ManualResetEvent(false);
        const int TIMEOUT_MS = 5000;
        const int MAX_BUFF_SIZE = 2048;
        public const String serverIpAddress = "192.168.1.2";
        public const int IPPORT = 8081;
        bool conectat = false;
        public double dirvent = 0.0;
        public string informacion;

        public async Task<string> Connect(string hostName, int portNumber)
        {
            string result = string.Empty;
            if (conectat == true) return result;
            await Task.Run(() =>
            {
                DnsEndPoint hostEntry = new DnsEndPoint(hostName, portNumber);
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
                socketEventArg.RemoteEndPoint = hostEntry;

                socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(
                    delegate(object s, SocketAsyncEventArgs e)
                    {
                        result = e.SocketError.ToString();
                        _clientDone.Set();
                    });

                _clientDone.Reset();
                _socket.ConnectAsync(socketEventArg);
                _clientDone.WaitOne(TIMEOUT_MS);

                if (result == "Success") conectat = true;
                else conectat = false;
            });

            return result;
        }

        public async Task<string> Send(byte[] data)
        {
            string response = "Timeout de conexión";
            await Task.Run(() =>
            {
                if (_socket != null)
                {
                    SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
                    socketEventArg.RemoteEndPoint = _socket.RemoteEndPoint;
                    socketEventArg.UserToken = null;

                    socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(
                        delegate(object s, SocketAsyncEventArgs e)
                        {
                            response = e.SocketError.ToString();
                            _clientDone.Set();
                        });

                    socketEventArg.SetBuffer(data, 0, data.Length);
                    _clientDone.Reset();
                    _socket.SendAsync(socketEventArg);
                    _clientDone.WaitOne(TIMEOUT_MS);
                }
                else
                {
                    response = "Conexión no establecida";
                }
            });


            return response;
        }

        public async Task<string> Receive()
        {
            string response = "Timeout de conexión";
            await Task.Run(() =>
            {
                // We are receiving over an established socket connection
                if (_socket != null)
                {
                    // Create SocketAsyncEventArgs context object
                    SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
                    socketEventArg.RemoteEndPoint = _socket.RemoteEndPoint;

                    // Setup the buffer to receive the data
                    socketEventArg.SetBuffer(new Byte[MAX_BUFF_SIZE], 0, MAX_BUFF_SIZE);

                    // Inline event handler for the Completed event.
                    // Note: This even handler was implemented inline in order to make
                    // this method self-contained.
                    socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(
                        delegate(object s, SocketAsyncEventArgs e)
                        {
                            if (e.SocketError == SocketError.Success)
                            {
                                // Retrieve the data from the buffer
                                response = Encoding.UTF8.GetString(e.Buffer, e.Offset + 8, e.BytesTransferred - 8);
                                response = response.Trim('\0');
                            }
                            else
                            {
                                response = e.SocketError.ToString();
                            }

                            _clientDone.Set();
                        });

                    // Sets the state of the event to nonsignaled, causing threads to block
                    _clientDone.Reset();

                    // Make an asynchronous Receive request over the socket
                    _socket.ReceiveAsync(socketEventArg);

                    // Block the UI thread for a maximum of TIMEOUT_MILLISECONDS milliseconds.
                    // If no response comes back within this time then proceed
                    _clientDone.WaitOne(TIMEOUT_MS);
                }
                else
                {
                    response = "Conexión no establecida";
                }
            });


            return response;
        }

        public async Task<string> Enviar(byte orden, byte datos1, byte datos2)
        {
            string valret = "";
            informacion = "";

            if (conectat == false) return "Sin conexión";
            try
            {
                byte[] buffer = new byte[]
                {
                    28, 0, 0, 0, //tam_datagrama
                    2, 0, 0, 0, //tipo_datagrama
                    0x9F, 0, 0, 0, //Clave inicio
                    10, 0, 0, 0, //Orden
                    0, 0, 0, 0, //Datos 1
                    0, 0, 0, 0, //Datos 2
                    0xA3, 0, 0, 0, //Clave final
                };

                buffer[6] = orden;

                string resposta;
                resposta = await Send(buffer);
                valret = "Mensaje enviado\nResposta: " + resposta;

                if (true)
                {
                    String texte;
                    texte = await Receive();
                    AnalizarDatos(texte);
                }

                return valret;
            }
            catch (IOException e)
            {
                valret = "Error de envio\n" + e.Message.ToString();
            }

            return "OK";
        }

        public string AnalizarDatos(string texte)
        {
            string result = string.Empty;
            // da error
            if (texte == "Timeout de conexión")
            {
                informacion = texte;
                return result;
            }

            string[] vents = new string[]
                {"Tramuntana", "Gregal", "Llevant", "Xaloc", "Migjorn", "Llebeig", "Ponent", "Mestral"};
            string[] elements = texte.Split(' ');
            double vent, rafaga;
            vent = Convert.ToDouble(elements[0]) * 1.8;
            rafaga = Convert.ToDouble(elements[1]) * 1.8;
            result += "Viento: " + vent.ToString("0.0") + " km/h, Ráfaga: " + rafaga.ToString("0.0") + "km/h, ";
            dirvent = Convert.ToDouble(elements[2]);
            int angle = ((((int) (dirvent + 22.5)) % 360) / 45);
            result += vents[angle] + "\n";
            result += "T. Exterior: " + elements[3] + "ºC, T. Interior: " + elements[11] +
                      "ºC\nHumedad: " + elements[4] + "%\nLluvia hoy: " + elements[5] + " mm (Intensitat: " +
                      elements[10] + " mm/h)";

            informacion = result;

            return result;
        }
    }
}