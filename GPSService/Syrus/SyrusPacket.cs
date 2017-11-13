using System;

namespace GPSService.Syrus
{
    class SyrusPacket
    {
        public bool OK => !string.IsNullOrEmpty(ID);


        public int EventIndex { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public decimal Velocidad { get; set; }
        public int Rumbo { get; set; }
        public int PositionFixData { get; set; }
        public int AgeData { get; set; }

        // Valores extendidos
        public string ID { get; set; }
        public int Aceleracion { get; set; } // AC: Instant acceleration measured in Miles per hour per seconds.
        public int Altitud { get; set; } // AL: Altitude in meters above mean sea level (AMSL).
        public int Voltaje { get; set; } // BL: Battery voltage level. Milivolts.
        public string CellInfo { get; set; } // CF: Cell information.
        public string Contadores { get; set; } // CVxx: Counter xx value.
        public string PrecisionGPS { get; set; } // DOP: GPS dilution of precision.
        public string InputsOutputs { get; set; } // IO: Inputs and Outputs state.
        public string DireccionIP { get; set; } // IP: IP Address.
        public string Satelites { get; set; } // SV: Satellites in view.
        public string Odometro { get; set; } // VO: Virtual Odometer value. Meters.


        public DateTime FechaHoraGps
        {
            get
            {
                DateTime fecha = this.Fecha;
                fecha = fecha.AddHours(this.Hora.Hours);
                fecha = fecha.AddMinutes(this.Hora.Minutes);
                fecha = fecha.AddSeconds(this.Hora.Seconds);
                return fecha.ToLocalTime();
            }

        }

    public string DataJson
        {
            get
            {
                SyrusDataJson json = new SyrusDataJson()
                {
                    Satelites = this.Satelites,
                    Altitud = this.Altitud,
                    Direccion = this.Rumbo,
                    Aceleracion = this.Aceleracion,
                    Data = this.InputsOutputs
                };
                return json.ToString();
            }
        }

        public override string ToString()
        {
            return $"EV:{EventIndex} Fecha:{Fecha:dd-MM-yyyy} Hora:{Hora:hh\\:mm\\:ss}"
                + $"\r\nLat:{Latitud:N5} Long:{Longitud:N5}"
                + $"\r\nVel:{Velocidad:N2} km/h"
                + $"\r\nPosFixData:{PositionFixData}" +
                  $"\r\nAgeData:{AgeData}"
                + $"\r\nID: {ID}"
                + $"\r\nAceleracion: {Aceleracion}"
                + $"\r\nAltitud: {Altitud}"
                + $"\r\nVoltaje: {Voltaje}"
                + $"\r\nCellInfo: {CellInfo}"
                + $"\r\nContadores: {Contadores}"
                + $"\r\nPrecisionGPS: {PrecisionGPS}"
                + $"\r\nInputsOutputs: {InputsOutputs}"
                + $"\r\nDireccionIP: {DireccionIP}"
                + $"\r\nSatelites: {Satelites}"
                + $"\r\nOdometro: {Odometro}";
        }

    }
}
