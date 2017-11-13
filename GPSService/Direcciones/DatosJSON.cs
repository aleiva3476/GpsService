using System.Collections.Generic;
using System.Runtime.Serialization;

// http://json2csharp.com/

namespace GPSService
{
    [DataContract]
    public class AddressComponent
    {
        [DataMember]
        public string long_name { get; set; }
        [DataMember]
        public string short_name { get; set; }
        [DataMember]
        public List<string> types { get; set; }
    }

    [DataContract]
    public class Location
    {
        [DataMember]
        public double lat { get; set; }
        [DataMember]
        public double lng { get; set; }
    }

    [DataContract]
    public class Northeast
    {
        [DataMember]
        public double lat { get; set; }
        [DataMember]
        public double lng { get; set; }
    }

    [DataContract]
    public class Southwest
    {
        [DataMember]
        public double lat { get; set; }
        [DataMember]
        public double lng { get; set; }
    }

    [DataContract]
    public class Viewport
    {
        [DataMember]
        public Northeast northeast { get; set; }
        [DataMember]
        public Southwest southwest { get; set; }
    }

    [DataContract]
    public class Northeast2
    {
        [DataMember]
        public double lat { get; set; }
        [DataMember]
        public double lng { get; set; }
    }

    [DataContract]
    public class Southwest2
    {
        [DataMember]
        public double lat { get; set; }
        [DataMember]
        public double lng { get; set; }
    }

    [DataContract]
    public class Bounds
    {
        [DataMember]
        public Northeast2 northeast { get; set; }
        [DataMember]
        public Southwest2 southwest { get; set; }
    }

    [DataContract]
    public class Geometry
    {
        [DataMember]
        public Location location { get; set; }
        [DataMember]
        public string location_type { get; set; }
        [DataMember]
        public Viewport viewport { get; set; }
        [DataMember]
        public Bounds bounds { get; set; }
    }

    [DataContract]
    public class Result
    {
        [DataMember]
        public List<AddressComponent> address_components { get; set; }
        [DataMember]
        public string formatted_address { get; set; }
        [DataMember]
        public Geometry geometry { get; set; }
        [DataMember]
        public string place_id { get; set; }
        [DataMember]
        public List<string> types { get; set; }
    }

    [DataContract]
    public class RootObject
    {
        [DataMember]
        public List<Result> results { get; set; }
        [DataMember]
        public string status { get; set; }
    }
}
