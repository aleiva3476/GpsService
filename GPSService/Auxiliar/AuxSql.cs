using Npgsql;
using System;
using System.Data;
using System.Data.Common;

namespace GPSService
{
    class AuxSql
    {
        private static DbConnection conn;
        public static Exception Excepcion { get; set; }

        public static bool TestServerConn(string servidor, string baseDatos, string usuario, string password, int puerto = 0)
        {
            try
            {
                NpgsqlConnectionStringBuilder csb = new NpgsqlConnectionStringBuilder()
                {
                    Host = servidor,
                    Database = baseDatos,
                    Username = usuario,
                    Password = password,
                    Port = puerto == 0 ? 5432 : puerto,
                    CommandTimeout = 20,
                    PersistSecurityInfo = true,
                };
                AuxSql.conn = new NpgsqlConnection(csb.ToString());
                AuxSql.conn.Open();
                if (AuxSql.conn != null)
                {
                    AuxSql.conn.Close();
                }
            }
            catch (ArgumentException ex)
            {
                AuxSql.conn = null;
                AuxSql.Excepcion = ex;
            }
            catch (NpgsqlException ex)
            {
                AuxSql.conn = null;
                AuxSql.Excepcion = ex;
            }

            return AuxSql.conn != null;
        }

        private static DbConnection DuplicateConn()
        {
            if (AuxSql.conn is ICloneable origPg)
            {
                DbConnection nueva = origPg.Clone() as NpgsqlConnection;
                nueva.Open();
                return nueva;
            }

            return null;
        }

        public static void Exec(Action<DbCommand> fnProccess)
        {
            using (DbConnection conn = AuxSql.DuplicateConn())
            {
                using (DbCommand cmd = conn.CreateCommand())
                {
                    fnProccess(cmd);
                }
            }
        }


        public static void AddParam(DbCommand cmd, string name, DbType type, object valor = null)
        {
            if (cmd == null)
            {
                return;
            }

            name = name.ToLowerInvariant();
            if (!cmd.Parameters.Contains(name))
            {
                DbParameter param = cmd.CreateParameter();
                param.ParameterName = name;
                param.DbType = type;
                param.Value = valor ?? DBNull.Value;
                cmd.Parameters.Add(param);
            }
            else
            {
                cmd.Parameters[name].Value = valor ?? DBNull.Value;
            }

            return;
        }

        public static int ReadInt(DbCommand cmd)
        {
            return AuxConvert.ToInt(cmd.ExecuteScalar());
        }



    }
}
