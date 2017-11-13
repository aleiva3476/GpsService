using System;
using System.Data;
using System.Text;

namespace GPSService
{
    class SqlGps
    {
        public double lat;
        public double lng;
        public string imei;
        public DateTime fecha;
        public decimal velocidad;
        public string datosJson;

        public void InsertaLectura()
        {
            AuxSql.Exec(cmd =>
            {
                AuxSql.AddParam(cmd, "imei", DbType.String, imei);
                AuxSql.AddParam(cmd, "fecha", DbType.DateTime, fecha);
                AuxSql.AddParam(cmd, "lng", DbType.Double, lng);
                AuxSql.AddParam(cmd, "lat", DbType.Double, lat);
                AuxSql.AddParam(cmd, "velocidad", DbType.Decimal, velocidad);

                StringBuilder sb = new StringBuilder();
                sb.Append("INSERT INTO gps (imei, fecha, posicion, velocidad, datos) VALUES");
                sb.AppendFormat(" (:imei, :fecha, ST_SetSRID(ST_MakePoint(:lng, :lat), 4326), :velocidad, '{0}'::jsonb)", datosJson);
                sb.Append(" RETURNING id");
                cmd.CommandText = sb.ToString();

                int id = AuxSql.ReadInt(cmd);
                this.LeeDireccion(id, lng, lat);
            });

        }

        private void LeeDireccion(int idGps, double lng, double lat)
        {
            int idDireccion = SqlDireccion.ReadPointDir(lng, lat);
            if (idDireccion != 0)
            {
                ActualizaDireccion(idGps, idDireccion);
            }
            else
            {
                ConsultaGMaps.LeeDireccion(idGps, lng, lat, ActualizaDireccion);
            }
        }

        private static void ActualizaDireccion(int idGps, int idDireccion)
        {
            AuxSql.Exec(cmd =>
            {
                cmd.CommandText = $"UPDATE gps SET direccion= {idDireccion} WHERE id = {idGps}";
                cmd.ExecuteNonQuery();
            });
        }

        public static void InsertaError(byte[] datos)
        {
            AuxSql.Exec(cmd =>
            {
                AuxSql.AddParam(cmd, "datos", DbType.Binary, datos);
                cmd.CommandText = "INSERT INTO gps_error (datos) VALUES (:datos)";
                cmd.ExecuteNonQuery();
            });
        }
    }
}
