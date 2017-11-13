using System.Collections.Generic;
using System.Text;

namespace GPSService.Syrus
{
    class SyrusProcessor
    {
        List<SyrusPacket> mensajes = new List<SyrusPacket>();

        public static List<SyrusPacket> DecodeMessage(byte[] datos)
        {
            SyrusProcessor proc = new SyrusProcessor();
            string texto = Encoding.ASCII.GetString(datos);
            List<string> lista = LeeMensajes(texto);
            foreach (string msg in lista)
            {
                SyrusPacket p = SyrusDecoder.Decode(msg);
                if (p.OK)
                {
                    proc.mensajes.Add(p);
                }
            }

            return proc.mensajes;
        }

        private static List<string> LeeMensajes(string texto)
        {
            List<string> lista = new List<string>();
            int posIni = texto.IndexOf(">");
            int posFin = posIni;
            while (posIni != -1 && posFin != -1)
            {
                posFin = texto.IndexOf("<", posIni + 1);
                if (posFin != -1)
                {
                    lista.Add(texto.Substring(posIni + 1, posFin - posIni - 1));
                    posIni = texto.IndexOf(">", posFin + 1);
                }
            }

            return lista;
        }

        public static bool IsSyrus(byte[] datos)
        {
            string texto = Encoding.ASCII.GetString(datos);
            int posIni = texto.IndexOf(">");
            int posFin = texto.IndexOf("<", posIni + 1);

            return posIni != -1 && posFin != -1;
        }

        public static string GetIDFromRXART(byte[] datos)
        {
            // >RXART;1.3.42;ID=356612021088358<

            string idGps = string.Empty;
            string texto = Encoding.ASCII.GetString(datos).Replace(">", string.Empty).Replace("<", string.Empty);
            if (texto.StartsWith("RXART"))
            {
                string[] partes = texto.Split(';');
                if (partes.Length == 3)
                {
                    string[] idPartes = partes[2].Split('=');
                    if (idPartes.Length == 2)
                    {
                        idGps = idPartes[1];
                    }
                }
            }

            return idGps;
        }
    }
}
