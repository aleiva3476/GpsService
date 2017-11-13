using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace GPSService.Meiligao
{
    [DataContract]
    class MeiligaoDataJson
    {
        [DataMember(Name = "sat")]
        public bool Satelite { get; set; }
        [DataMember(EmitDefaultValue = false, Name = "alt")]
        public decimal Altitud;
        [DataMember(EmitDefaultValue = false, Name = "cmd")]
        public decimal Comando;
        [DataMember(EmitDefaultValue = false, Name = "dir")]
        public decimal Direccion;
        [DataMember(EmitDefaultValue = false, Name = "dat")]
        public string Data { get; set; }


        public override string ToString()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(MeiligaoDataJson));
                ser.WriteObject(stream, this);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }
}
