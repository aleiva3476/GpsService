using System;

namespace GPSService.Meiligao
{
    class MeiligaoPacket
    {
        public bool HeaderOK => this.Header == "$$";
        public string Header { get; set; } = string.Empty;
        public string ID { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string IMEI { get; set; } = string.Empty;
        public int Length { get; set; }
        public CommandTypes Command { get; set; }
        public int Flag { get; set; }
        public string HDOP { get; set; } = string.Empty;
        public string DataBytes { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public bool SateliteOK { get; set; }
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public decimal Altitud { get; set; }
        public decimal Velocidad { get; set; }
        public decimal Direccion { get; set; }
        public string VariacionMagnetica { get; set; } = string.Empty;
        public int Checksum { get; set; } = 0;
        public string TelAuthSMS { get; set; } = string.Empty;
        public string TelAuthCall { get; set; } = string.Empty;

        public string DataJson
        {
            get
            {
                MeiligaoDataJson json = new MeiligaoDataJson()
                {
                    Satelite = this.SateliteOK,
                    Altitud = this.Altitud,
                    Direccion = this.Direccion,
                    Comando = Convert.ToInt32(this.Command),
                    Data = this.DataBytes
                };
                return json.ToString();
            }
        }

        public MeiligaoPacket()
        {
        }
    }
}
