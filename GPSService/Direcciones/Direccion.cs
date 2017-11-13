namespace GPSService
{
    class Direccion
    {
        public string Calle { get; set; }
        public string Numero { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public string Cpostal { get; set; }
        public string Pais { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }

        public Direccion(RootObject ro)
        {
            Result res = ro.results[0];

            this.Calle = LeeNombreTipo(res, "route");
            this.Numero = LeeNombreTipo(res, "street_number");
            this.Localidad = LeeNombreTipo(res, "locality");
            this.Provincia = LeeNombreTipo(res, "administrative_area_level_2");
            this.Cpostal = LeeNombreTipo(res, "postal_code");
            this.Pais = LeeNombreTipo(res, "country");

            this.Lat = res.geometry.location.lat;
            this.Lng = res.geometry.location.lng;
        }

        public override string ToString()
        {
            return $"{Calle} {Numero}, {Cpostal} {Localidad}, {Provincia}, {Pais}";
        }

        private static string LeeNombreTipo(Result res, string tipo)
        {
            AddressComponent address = res.address_components.Find(ac => ac.types.Contains(tipo));
            return address?.long_name ?? string.Empty;
        }
    }
}
