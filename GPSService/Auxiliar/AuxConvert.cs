using System;

namespace GPSService
{
    class AuxConvert
    {
        private static System.Globalization.NumberFormatInfo numberFormat = new System.Globalization.NumberFormatInfo();

        /// <summary>
        /// Converts a string to decimal value
        /// </summary>
        /// <param name="txtDecimal">String that contains a decimal value</param>
        /// <returns>Decimal value, or zero if the string does not correspond to a decimal value</returns>
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

        /// <summary>
        /// Converts a object to integer value
        /// </summary>
        /// <param name="objInteger">Object that represents an integer value</param>
        /// <returns>Integer value, or zero if the object is not an whole value</returns>
        public static int ToInt(object objInteger)
        {
            if (objInteger == null || objInteger == DBNull.Value)
            {
                return 0;
            }

            return ToInt(objInteger.ToString());
        }

        /// <summary>
        /// Converts a string to integer value
        /// </summary>
        /// <param name="txtDecimal">String that contains a integer value</param>
        /// <returns>Integer value, or zero if the string does not correspond to a integer value</returns>
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
