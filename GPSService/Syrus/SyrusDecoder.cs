using System;

namespace GPSService.Syrus
{
    class SyrusDecoder
    {
        private string mensaje;
        private string inicioMensaje;

        public SyrusDecoder()
        {
        }

        public SyrusDecoder(string msg)
        {
            this.Mensaje = msg;
        }

        private string Mensaje
        {
            get
            {
                return mensaje;
            }
            set
            {
                mensaje = value ?? string.Empty;
                inicioMensaje = mensaje.Split(';')[0];
            }
        }

        private int EventIndex
        {
            get
            {
                return GetInt(3, 2);
            }
        }

        private DateTime Fecha
        {
            get
            {
                DateTime fecha = new DateTime(1980, 1, 6);
                int semanas = GetInt(5, 4);
                int diaSemana = GetInt(9, 1);
                int offset = (DayOfWeek)diaSemana - fecha.DayOfWeek;

                return fecha.AddDays(7 * semanas).AddDays(offset);
            }
        }

        private TimeSpan Hora
        {
            get
            {
                return new TimeSpan(0, 0, GetInt(10, 5));
            }
        }

        private decimal Latitud
        {
            get
            {
                int ent = GetInt(15, 3);
                int dec = GetInt(18, 5);
                return decimal.Parse($"{ent},{dec}", new System.Globalization.CultureInfo("es-ES"));
            }
        }

        private decimal Longitud
        {
            get
            {
                int ent = GetInt(23, 4);
                int dec = GetInt(27, 5);
                return decimal.Parse($"{ent},{dec}", new System.Globalization.CultureInfo("es-ES"));
            }
        }

        private decimal Velocidad
        {
            get
            {
                int mph = GetInt(32, 3);
                return Math.Round(1.6093440M * mph, 2); // 1 mph = 1,6093440 kph
            }
        }

        private int Rumbo
        {
            get
            {
                return GetInt(35, 3);
            }
        }


        private int PositionFixData
        {
            get
            {
                return GetInt(38, 1);
            }
        }

        private int AgeData
        {
            get
            {
                return GetInt(39, 1);
            }
        }

        private int GetInt(int startIndex, int length)
        {
            int.TryParse(GetString(startIndex, length) ?? "0", out int valor);
            return valor;
        }

        private string GetString(int startIndex, int length)
        {
            if (inicioMensaje.Length >= startIndex + length)
            {
                return inicioMensaje.Substring(startIndex, length);
            }

            return null;
        }

        private int ToInt(string texto)
        {
            int.TryParse(texto ?? "0", out int valor);
            return valor;
        }

        private void ValoresExtendidos(SyrusPacket p)
        {
            string[] items = this.mensaje.Split(';');
            for (int i = 1; i < items.Length; i++)
            {
                string[] partes = items[i].Split('=');
                if (partes.Length > 1)
                {
                    switch (partes[0])
                    {
                        case "ID": p.ID = partes[1]; break;
                        case "AC": p.Aceleracion = ToInt(partes[1]); break;
                        case "AL": p.Altitud = ToInt(partes[1]); break;
                        case "BL": p.Voltaje = ToInt(partes[1]); break;
                        case "CF": p.CellInfo = partes[1]; break;
                        case "CVxx": break;
                        case "DOP": p.PrecisionGPS = partes[1]; break;
                        case "IO": p.InputsOutputs = partes[1]; break;
                        case "IP": p.DireccionIP = partes[1]; break;
                        case "SV": p.Satelites = partes[1]; break;
                        case "VO": p.Odometro = partes[1]; break;
                    }
                }
            }
        }

        public static SyrusPacket Decode(string msg)
        {
            if (!msg.StartsWith("REV"))
            {
                return new SyrusPacket();
            }

            SyrusDecoder dec = new SyrusDecoder(msg);
            SyrusPacket p = new SyrusPacket()
            {
                EventIndex = dec.EventIndex,
                Fecha = dec.Fecha,
                Hora = dec.Hora,
                Latitud = dec.Latitud,
                Longitud = dec.Longitud,
                Velocidad = dec.Velocidad,
                Rumbo = dec.Rumbo,
                PositionFixData = dec.PositionFixData,
                AgeData = dec.AgeData
            };

            dec.ValoresExtendidos(p);

            return p;
        }
    }
}
