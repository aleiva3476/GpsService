using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace GPSService.Teltonika
{
    [DataContract]
    class FMDataJson
    {
        [DataMember(Name = "sat")]
        public int Satelites { get; set; }
        [DataMember(EmitDefaultValue = false, Name = "alt")]
        public int Altitud;
        [DataMember(EmitDefaultValue = false, Name = "dir")]
        public int Direccion;
        [DataMember(EmitDefaultValue = false, Name = "evID")]
        public int EventID;
        [DataMember(EmitDefaultValue = false, Name = "dat")]
        public string Data { get; set; }


        public override string ToString()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(FMDataJson));
                ser.WriteObject(stream, this);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }
}
