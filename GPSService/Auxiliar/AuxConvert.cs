using System;

namespace GPSService
{
    class AuxConvert
    {
        private static System.Globalization.NumberFormatInfo numberFormat = new System.Globalization.NumberFormatInfo();

        public static decimal ToDecimal(string txtDecimal)
        {
            if (String.IsNullOrEmpty(txtDecimal))
            {
                return 0M;
            }

            decimal valor;
            try
            {
                numberFormat.NumberDecimalSeparator = ".";
                valor = Convert.ToDecimal(txtDecimal, (txtDecimal.IndexOf(',') == -1) ? numberFormat : null);
            }
            catch (FormatException)
            {
                return 0M;
            }

            return Math.Round(valor, 2);
        }

        public static int ToInt(object objInteger)
        {
            if (objInteger == null || objInteger == DBNull.Value)
            {
                return 0;
            }

            return ToInt(objInteger.ToString());
        }

        public static int ToInt(string txtInteger)
        {
            if (Int32.TryParse(txtInteger, out int valor))
            {
                return valor;
            }

            return 0;
        }
    }
}
