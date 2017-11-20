using GPSService.Meiligao;
using GPSService.Syrus;
using GPSService.Teltonika;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Threading;

namespace GPSService
{
    public partial class GpsService : ServiceBase
    {
        private readonly int SERVICE_PORT = 9081;
        private readonly IPAddress IP_ADDRESS = IPAddress.Any;

        private TcpListener listener;
        private Thread hilo;
        private ManualResetEvent shutdownEvent = new ManualResetEvent(false);

        public GpsService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            AuxSystemLog.AppName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

            bool conexionOk = AuxSql.TestServerConn();
            if (!conexionOk)
            {
                string textoError = "Error conectando con la base de datos";
                if (AuxSql.Excepcion != null)
                {
                    textoError = $"Error conectando con la base de datos\r\n{AuxSql.Excepcion.Message}";
                }
                AuxSystemLog.Error(textoError);
                throw new Exception(textoError);
            }

            this.listener = new TcpListener(IP_ADDRESS, SERVICE_PORT);
            if (listener == null)
            {
                string textoError = $"Error creando TcpListener en {IP_ADDRESS}:{SERVICE_PORT}";
                AuxSystemLog.Error(textoError);
                throw new Exception(textoError);
            }

            this.hilo = new Thread(Execute)
            {
                IsBackground = true,
                Name = "GPSService thread"
            };
            this.hilo.Start();
        }

        protected override void OnStop()
        {
            try
            {
                shutdownEvent.Set();
                this.listener.Stop();
                if (!hilo.Join(3000)) // Esperamos 3 segundos
                {
                    hilo.Abort();
                }
            }
            catch (ThreadInterruptedException) { }
            finally
            {
                this.ExitCode = 0;
            }
        }

        private void Execute()
        {
            this.listener.Start();
            while (!shutdownEvent.WaitOne(0))
            {
                try
                {
                    TcpClient client = this.listener.AcceptTcpClient();
                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleConnection));
                    clientThread.Start(client);
                }
                catch (SocketException) { }
            }

        }

        private void HandleConnection(object client)
        {
            const int TAM_BUFFER = 2048;
            TcpClient tcpCli = client as TcpClient;
            NetworkStream clientStream = tcpCli.GetStream();
            clientStream.ReadTimeout = 5000;

            string teltonikaImei = string.Empty;
            byte[] message = new byte[TAM_BUFFER];
            List<byte> contenido = new List<byte>();
            int bytesRead = 0;

            try
            {
                bytesRead = clientStream.Read(message, 0, TAM_BUFFER); //blocks until a client sends a message
                for (int i = 0; i < bytesRead; i++)
                {
                    contenido.Add(message[i]);
                }
            }
            catch (Exception)
            {
                bytesRead = 0;
            }

            if (bytesRead == 0)
            {
                clientStream.Close();
                tcpCli.Close();
                return;
            }

            if (FMPacket.IsTeltonika(message))
            {
                teltonikaImei = FMPacket.GetIMEI(message);
                contenido.Clear();
                try
                {
                    message[0] = 0x01;
                    clientStream.Write(message, 0, 1);
                    clientStream.Flush();
                }
                catch (System.IO.IOException) { }
            }

            Array.Clear(message, 0, TAM_BUFFER);
            try
            {
                bytesRead = clientStream.Read(message, 0, TAM_BUFFER);
                while (bytesRead > 0)
                {
                    for (int i = 0; i < bytesRead; i++)
                    {
                        contenido.Add(message[i]);
                    }
                    Array.Clear(message, 0, TAM_BUFFER);
                    bytesRead = clientStream.Read(message, 0, TAM_BUFFER);
                }
            }
            catch (Exception) { }

            message = contenido.ToArray();
            bytesRead = message.Length;


            if (bytesRead > 0)
            {
                Array.Resize(ref message, bytesRead);
                if (FMPacket.IsTeltonika(message))
                {
                    var listaFM = FMPacket.DecodeMessage(message, teltonikaImei);
                    try
                    {
                        byte[] resp = Int2BigEndian(listaFM.Count);
                        clientStream.Write(resp, 0, resp.Length);
                        clientStream.Flush();
                    }
                    catch (System.IO.IOException) { }

                    foreach (FMPacket fmP in listaFM)
                    {
                        this.InsertaLectura(fmP);
                    }
                }
                else if (MeiligaoProcessor.IsMeiligao(message))
                {
                    MeiligaoPacket meiP = MeiligaoProcessor.DecodeMessage(message);
                    if (meiP.HeaderOK && meiP.Checksum != 0)
                    {
                        this.InsertaLectura(meiP);
                    }
                    else
                    {
                        SqlGps.InsertaError(message);
                    }
                }
                else if (SyrusProcessor.IsSyrus(message))
                {
                    //AuxSystemLog.Information("SYRUS: [{0}]{1}{2}", message.Length, Environment.NewLine, BitConverter.ToString(message));
                    string idGps = SyrusProcessor.GetIDFromRXART(message);
                    if (!string.IsNullOrEmpty(idGps))
                    {
                        try
                        {
                            byte[] resp = System.Text.Encoding.ASCII.GetBytes(idGps);
                            clientStream.Write(resp, 0, resp.Length);
                            clientStream.Flush();
                        }
                        catch (System.IO.IOException) { }
                    }
                    else
                    {
                        var listaSyrus = SyrusProcessor.DecodeMessage(message);
                        foreach (SyrusPacket syrP in listaSyrus)
                        {
                            if (syrP.OK)
                            {
                                this.InsertaLectura(syrP);
                                try
                                {
                                    byte[] resp = System.Text.Encoding.ASCII.GetBytes(syrP.ID);
                                    clientStream.Write(resp, 0, resp.Length);
                                    clientStream.Flush();
                                }
                                catch (System.IO.IOException) { }
                            }
                            else
                            {
                                AuxSystemLog.Warning("SYRUS-ERROR: [{0}]{1}{2}", message.Length, Environment.NewLine, BitConverter.ToString(message));
                            }
                        }
                    }
                }
                else
                {
                    AuxSystemLog.Warning("ERROR: [{0}]{1}{2}", message.Length, Environment.NewLine, BitConverter.ToString(message));
                    SqlGps.InsertaError(message);
                }
            }

            clientStream.Close();
            tcpCli.Close();
        }

        private void InsertaLectura(MeiligaoPacket packet)
        {
            SqlGps gps = new SqlGps()
            {
                lng = Convert.ToDouble(packet.Longitud),
                lat = Convert.ToDouble(packet.Latitud),
                imei = packet.ID,
                fecha = packet.Fecha,
                velocidad = packet.Velocidad,
                datosJson = packet.DataJson
            };

            gps.InsertaLectura();

        }

        private void InsertaLectura(SyrusPacket packet)
        {
            SqlGps gps = new SqlGps()
            {
                lng = Convert.ToDouble(packet.Longitud),
                lat = Convert.ToDouble(packet.Latitud),
                imei = packet.ID,
                fecha = packet.FechaHoraGps,
                velocidad = packet.Velocidad,
                datosJson = packet.DataJson
            };

            gps.InsertaLectura();
        }

        private void InsertaLectura(FMPacket packet)
        {
            SqlGps gps = new SqlGps()
            {
                lng = Convert.ToDouble(packet.Longitud),
                lat = Convert.ToDouble(packet.Latitud),
                imei = packet.Imei,
                fecha = packet.Fecha,
                velocidad = packet.Velocidad,
                datosJson = packet.DataJson
            };

            gps.InsertaLectura();
        }

        private static byte[] Int2BigEndian(int data)
        {
            byte[] b = new byte[4];
            b[0] = (byte)(((uint)data >> 24) & 0xFF);
            b[1] = (byte)(((uint)data >> 16) & 0xFF);
            b[2] = (byte)(((uint)data >> 8) & 0xFF);
            b[3] = (byte)data;
            return b;
        }

    }
}
