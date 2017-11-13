using System;
using System.Text;

// Based on: https://github.com/brimzi/meitrack-protocols

namespace GPSService.Meiligao
{
    class MeiligaoProcessor
    {
        MeiligaoPacket mp = new MeiligaoPacket();

        public static MeiligaoPacket DecodeMessage(byte[] datos)
        {
            MeiligaoProcessor proc = new MeiligaoProcessor();
            proc.Decode(datos);
            return proc.mp;
        }

        #region · Métodos ·

        public static bool IsMeiligao(byte[] datos)
        {
            return datos.Length >= 14 && Encoding.ASCII.GetString(datos, 0, 2) == "$$";
        }

        private void Decode(byte[] datos)
        {
            if (datos.Length < 14)
            {
                return;
            }

            this.mp.Checksum = this.GetChecksum(datos);
            this.mp.Header = Encoding.ASCII.GetString(datos, 0, 2);
            this.mp.ID = this.GetID(datos);
            this.mp.Length = this.GetLength(datos);
            this.mp.Command = this.GetCommand(datos);
         
            this.mp.Flag = datos[13] - 0x00;

            switch (this.mp.Command)
            {
                case CommandTypes.GET_SN_IMEI:
                    this.GetSnImei(datos);
                    break;
                case CommandTypes.GET_REPORT_TIME_INTERVAL:
                    this.mp.Flag = this.GetInt(datos, 14, 15);
                    break;
                case CommandTypes.GET_AUTHORIZED_PHONE:
                    this.GetAuthorizedPhone(datos);
                    break;

                case CommandTypes.SET_REPORT_TIME_INTERVAL_RESULT:
                    this.mp.DataBytes = this.GetInt(datos, 14, 15).ToString();
                    break;

                case CommandTypes.REPORT:
                    this.GetGPSData(datos);
                    break;
                case CommandTypes.ALARM:
                    this.GetGPSData(datos, 1);
                    break;
            }
        }

        #endregion

        #region · Métodos auxiliares ·

        private string GetDataSection(byte[] datos, int offset = 0)
        {
            var dataCount = datos.Length - 13 - 2 - 2;
            if (dataCount < 13)
            {
                return string.Empty;
            }
            return Encoding.ASCII.GetString(datos, 13 + offset, dataCount);
        }

        private int GetInt(byte[] datos, int firstByte, int secondByte)
        {
            if (BitConverter.IsLittleEndian)
            {
                var tmp = firstByte;
                firstByte = secondByte;
                secondByte = tmp;
            }
            var commandBytes = new[] { datos[firstByte], datos[secondByte] };
            return Convert.ToInt32(BitConverter.ToUInt16(commandBytes, 0));
        }

        private string GetID(byte[] datos)
        {
            string rtVal = BitConverter.ToString(datos, 4, 7).Replace("-", "");
            int posF = rtVal.ToLower().IndexOf('f');
            if (posF != -1)
            {
                rtVal = rtVal.ToLower().Remove(posF);
            }
            return rtVal;
        }

        private int GetLength(byte[] datos)
        {
            return this.GetInt(datos, 2, 3);
        }

        private CommandTypes GetCommand(byte[] datos)
        {
            CommandTypes cmd = CommandTypes.NONE;
            int valor = this.GetInt(datos, 11, 12);
            CommandTypes.TryParse(valor.ToString(), out cmd);
            return cmd;
        }

        private void GetSnImei(byte[] datos)
        {
            var dataSection = this.GetDataSection(datos);
            var dataComponents = dataSection.Split(',');
            if (dataComponents.Length == 0)
            {
                return;
            }
            this.mp.SerialNumber = dataComponents[0];
            this.mp.IMEI = dataComponents[1];
        }

        private void GetAuthorizedPhone(byte[] datos)
        {
            var dataSection = this.GetDataSection(datos);
            if (string.IsNullOrEmpty(dataSection))
            {
                return;
            }

            this.mp.TelAuthSMS = dataSection.Substring(0, 16);
            this.mp.TelAuthCall = dataSection.Substring(16, 16);
        }

        private void GetGPSData(byte[] datos, int offset = 0)
        {
            var dataSection = this.GetDataSection(datos, offset);
            var dataComponents = dataSection.Split('|');
            if (dataComponents.Length == 0)
            {
                return;
            }

            this.GetGPRMC(dataComponents[0]);
            this.mp.HDOP = dataComponents[1];
            this.mp.Altitud = AuxConvert.ToDecimal(dataComponents[2]);

            this.mp.DataBytes = Convert.ToString(Convert.ToInt32(dataComponents[3], 16), 2).PadLeft(16, '0'); 
        }

        private void GetGPRMC(string sentenciaGPRMC)
        {
            var stringParts = sentenciaGPRMC.Split(',');
            this.mp.Fecha = this.GetDateTime(stringParts[8], stringParts[0]);
            this.mp.SateliteOK = this.GetGPSStatus(stringParts[1]);
            this.mp.Latitud = this.GetLatitud(stringParts[2], stringParts[3]);
            this.mp.Longitud = this.GetLongitud(stringParts[4], stringParts[5]);
            this.mp.Velocidad = this.GetVelocidad(stringParts[6]);
            this.mp.Direccion = this.GetDireccion(stringParts[7]);
            this.mp.VariacionMagnetica = stringParts[9];
        }

        private DateTime GetDateTime(string datePart, string timePart)
        {
            //hhmmss
            var hour = AuxConvert.ToInt(timePart.Substring(0, 2));
            var min = AuxConvert.ToInt(timePart.Substring(2, 2));
            var secParts = timePart.Substring(4, 4).Split('.');
            var sec = AuxConvert.ToInt(secParts[0]);
            var secDecimal = AuxConvert.ToInt(secParts[1]) * 10;

            //ddmmyy
            var day = AuxConvert.ToInt(datePart.Substring(0, 2));
            var month = AuxConvert.ToInt(datePart.Substring(2, 2));
            var year = AuxConvert.ToInt("20" + datePart.Substring(4, 2));

            return new DateTime(year, month, day, hour, min, sec, secDecimal);
        }

        private bool GetGPSStatus(string statusString)
        {
            return statusString.ToLowerInvariant() == "a";
        }

        private decimal GetLatitud(string stringPart, string hemisphere)
        {
            int hemIndicator = 1;
            if (hemisphere.ToLowerInvariant() == "s")
            {
                hemIndicator = -1;
            }

            decimal deg = AuxConvert.ToDecimal(stringPart.Substring(0, 2));
            decimal min = AuxConvert.ToDecimal(stringPart.Substring(2));
            decimal lat = (deg + (min / 60)) * hemIndicator;

            return lat;
        }

        private decimal GetLongitud(string stringPart, string sideofMeridian)
        {
            int sideIndicator = 1;
            if (sideofMeridian.ToLowerInvariant() == "w")
            {
                sideIndicator = -1;
            }


            decimal deg = AuxConvert.ToDecimal(stringPart.Substring(0, 3));
            decimal min = AuxConvert.ToDecimal(stringPart.Substring(3));
            decimal longit = (deg + (min / 60)) * sideIndicator;

            return longit;
        }

        private decimal GetVelocidad(string speedString)
        {
            return AuxConvert.ToDecimal(speedString);
        }

        private decimal GetDireccion(string directionString)
        {
            return AuxConvert.ToDecimal(directionString);
        }

        private int GetChecksum(byte[] bytes)
        {
            var calculatedChecksum = MeiligaoChecksum.CalculaChecksum(bytes, 0, bytes.Length - 4);
            var checksumInPacket = new[] { bytes[bytes.Length - 4], bytes[bytes.Length - 3] };
            var c = BitConverter.ToUInt16(calculatedChecksum, 0);
            var d = BitConverter.ToUInt16(checksumInPacket, 0);
            if (c != d)
            {
                return 0;
            }
            return AuxConvert.ToInt(c);
        }

        #endregion
    }
}
