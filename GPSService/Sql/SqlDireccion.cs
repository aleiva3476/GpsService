using System.Data;

namespace GPSService
{
    class SqlDireccion
    {
        public static int ReadPointDir(double lng, double lat, int metros = 5)
        {
            int id = 0;

            AuxSql.Exec(cmd =>
            {
                AuxSql.AddParam(cmd, "lat", DbType.Double, lat);
                AuxSql.AddParam(cmd, "lng", DbType.Double, lng);
                AuxSql.AddParam(cmd, "met", DbType.Int32, metros);
                cmd.CommandText = "SELECT id"
                    + " FROM direcciones"
                    + " WHERE ST_Distance(posicion ,ST_SetSRID(ST_MakePoint(:lng, :lat), 4326)::geography) < :met"
                    + " ORDER BY posicion <-> ST_SetSRID(ST_MakePoint(:lng, :lat), 4326)::geography LIMIT 1";
                id = AuxSql.ReadInt(cmd);
            });

            return id;
        }

        public static int Insert(Direccion dir)
        {
            int id = 0;
            AuxSql.Exec(cmd =>
            {
                AuxSql.AddParam(cmd, "lat", DbType.Double, dir.Lat);
                AuxSql.AddParam(cmd, "lng", DbType.Double, dir.Lng);
                AuxSql.AddParam(cmd, "calle", DbType.String, dir.Calle);
                AuxSql.AddParam(cmd, "numero", DbType.String, dir.Numero);
                AuxSql.AddParam(cmd, "localidad", DbType.String, dir.Localidad);
                AuxSql.AddParam(cmd, "provincia", DbType.String, dir.Provincia);
                AuxSql.AddParam(cmd, "cpostal", DbType.String, dir.Cpostal);
                AuxSql.AddParam(cmd, "pais", DbType.String, dir.Pais);
                cmd.CommandText = "INSERT INTO direcciones (posicion, calle, numero, localidad, provincia, cpostal, pais)"
                    + " VALUES"
                    + " (ST_SetSRID(ST_MakePoint(:lng, :lat), 4326), :calle, :numero, :localidad, :provincia, :cpostal, :pais) RETURNING id";
                id = AuxSql.ReadInt(cmd);
            });

            return id;
        }
    }
}
