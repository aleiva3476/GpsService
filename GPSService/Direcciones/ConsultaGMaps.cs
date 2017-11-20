using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;

namespace GPSService
{
    class ConsultaGMaps
    {
        const string GOOGLE_API_KEY = "MyGoogleApiKey";

        /// <summary>
        /// Connect with google and pick up the address corresponding to the location
        /// </summary>
        /// <param name="idGps"></param>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <param name="delegadoFinLectura"></param>
        public static async void LeeDireccion(int idGps, double lng, double lat, Action<int, int> delegadoFinLectura)
        {
            
            int idDireccion = 0;
            NumberFormatInfo nfi = new NumberFormatInfo() { NumberDecimalSeparator = "." };
            string requestUri = string.Format(nfi, "https://maps.googleapis.com/maps/api/geocode/json?latlng={0},{1}&key={2}&esult_type=street_address", lat, lng, GOOGLE_API_KEY);

            using (var client = new HttpClient())
            {
                var request = await client.GetAsync(requestUri);
                var content = await request.Content.ReadAsStringAsync();
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(content)))
                {
                    ms.Position = 0;
                    var sr = new StreamReader(ms);
                    var myStr = sr.ReadToEnd();
                    if (myStr.Contains("\"status\" : \"OK\""))
                    {
                        ms.Position = 0;
                        var serializer = new DataContractJsonSerializer(typeof(RootObject));
                        RootObject ro = (RootObject)serializer.ReadObject(ms);
                        Direccion dir = new Direccion(ro);
                        idDireccion = SqlDireccion.Insert(dir);
                        if (idDireccion != 0)
                        {
                            delegadoFinLectura?.Invoke(idGps, idDireccion);
                        }
                    }
                }
            }
        }
    }
}
