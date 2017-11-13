using System;
using System.Collections.Generic;
using System.Text;

namespace GPSService.Teltonika
{
    class FMPacket
    {
        const int LEN_TIMESTAMP = 8;
        const int LEN_PRIORIDAD = 1;
        const int LEN_LONGITUD = 4;
        const int LEN_LATITUD = 4;
        const int LEN_ALTITUD = 2;
        const int LEN_ANGULO = 2;
        const int LEN_SATELITES = 1;
        const int LEN_VELOCIDAD = 2;

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public string Imei { get; private set; }
        public DateTime Fecha { get; private set; } = UnixEpoch;
        public decimal Longitud { get; private set; }
        public decimal Latitud { get; private set; }
        public int Altitud { get; private set; }
        public int Angulo { get; private set; }
        public int Satelites { get; private set; }
        public int Velocidad { get; private set; }
        public int Prioridad { get; private set; }

        /// <summary>
        /// GPS Ident
        /// </summary>
        public int EventIOID { get; private set; }

        private List<FMIOElement> IO { get; set; } = new List<FMIOElement>();

        public string DataJson
        {
            get
            {
                FMDataJson json = new FMDataJson()
                {
                    Satelites = this.Satelites,
                    Altitud = this.Altitud,
                    Direccion = this.Angulo,
                    EventID = this.EventIOID,
                    Data = string.Join(",", this.IO)
                };
                return json.ToString();
            }
        }


        public FMPacket(string imei)
        {
            this.Imei = imei;
        }

        public static bool IsTeltonika(byte[] message) => message[0] == 0;
        public static bool IsTeltonikaFM2(byte[] message) => message[0] == 0 && message[1] == 0 && message[2] == 0 && message[3] == 0;

        public static string GetIMEI(byte[] message)
        {
            int len = -1;
            string imei = string.Empty;

            bool isT2 = IsTeltonikaFM2(message);
            if (isT2)
            {
                int offset = 4;
                len = ToInt(message, offset, 4);
                offset += 4;
                imei = Encoding.ASCII.GetString(message, offset, len);
            }
            else
            {
                len = (int)message[1];
                imei = Encoding.ASCII.GetString(message, 2, len);
            }

            return imei;
        }

        const int HEAD_LEN = 4 + 4 + 1; // 4 ceros + longitud paquete + codec ID
        
        public static List<FMPacket> DecodeMessage(byte[] message, string imei)
        {
            List<FMPacket> lista = new List<FMPacket>();

            int offset = HEAD_LEN;
            int numRegistros = ToInt(message, offset, 1);
            offset += 1;

            for (int i = 0; i < numRegistros; i++)
            {
                FMPacket p = new FMPacket(imei);
                offset = p.Lee(message, offset);
                lista.Add(p);
            }

            return lista;
        }


        private int Lee(byte[] message, int offset)
        {
            this.Fecha = ToDateTime(ToLong(message, offset, LEN_TIMESTAMP));
            offset += LEN_TIMESTAMP;

            this.Prioridad = ToInt(message, offset, LEN_PRIORIDAD);
            offset += LEN_PRIORIDAD;

            this.Longitud = Convert.ToDecimal(ToInt(message, offset, LEN_LONGITUD)) / 1e7M;
            offset += LEN_LONGITUD;

            this.Latitud = Convert.ToDecimal(ToInt(message, offset, LEN_LATITUD)) / 1e7M;
            offset += LEN_LATITUD;

            this.Altitud = ToInt(message, offset, LEN_ALTITUD);
            offset += LEN_ALTITUD;

            this.Angulo = ToInt(message, offset, LEN_ANGULO);
            offset += LEN_ANGULO;

            this.Satelites = ToInt(message, offset, LEN_SATELITES);
            offset += LEN_SATELITES;

            this.Velocidad = ToInt(message, offset, LEN_VELOCIDAD);
            offset += LEN_VELOCIDAD;

            this.EventIOID = ToInt(message, offset, 1);
            offset += 1;
            int totalEventos = ToInt(message, offset, 1);
            offset += 1;

            int numEventos = 0;

            for (int numBytes = 1; numBytes <= 8; numBytes *= 2)
            {
                if (numEventos < totalEventos)
                {
                    numEventos += ParseIoElements(message, ref offset, numBytes);
                }
                else
                {
                    offset += 1; // Saltamos el número de eventos de longitud numBytes
                }

            }
            return offset;
        }

        private int ParseIoElements(byte[] message, ref int offset, int numBytesDatos)
        {
            int numItems = ToInt(message, offset, 1);
            offset += 1;
            for (int i = 0; i < numItems; i++)
            {
                this.IO.Add(FMIOElement.Parse(message, offset, numBytesDatos));
                offset += 1 + numBytesDatos;
            }

            return numItems;

        }

        private static int ToInt(byte[] message, int offset, int len)
        {
            int valor = 0;
            for (int i = 0; i < len; i++)
            {
                valor = valor << 8;
                valor = valor | message[offset + i];
            }

            return valor;
        }

        private static long ToLong(byte[] message, int offset, int len)
        {
            long valor = 0;
            for (int i = 0; i < len; i++)
            {
                valor = valor << 8;
                valor = valor | message[offset + i];
            }

            return valor;
        }

        private static DateTime ToDateTime(long millis)
        {
            return UnixEpoch.AddMilliseconds(millis).ToLocalTime();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Fecha:  {0:dd-MM-yyyy HH-mm-ss}", Fecha);
            sb.Append(Environment.NewLine);
            sb.AppendFormat("Lat: {0:N7}   Lng: {1:N7}", Latitud, Longitud);
            sb.Append(Environment.NewLine);
            sb.AppendFormat("Alt: {0}   Ang: {1}   Sat: {2}   Vel: {3}", Altitud, Angulo, Satelites, Velocidad);
            sb.Append(Environment.NewLine);

            this.IO.ForEach(item => sb.AppendFormat("IO: {0} {1}{2}", item.ID, item.Valor, Environment.NewLine));

            return sb.ToString();
        }
    }
}
