using Npgsql;
using System;
using System.Data;
using System.Data.Common;

namespace GPSService
{
    class AuxSql
    {
        const string DB_HOST = "localhost";
        const string DB_DATABASE = "GEO";
        const string DB_USER = "postgres";
        const string DB_PASSWORD = "postgres";
        const int DB_PORT = 5432;

        private static DbConnection conn;
        public static Exception Excepcion { get; set; }

        /// <summary>
        /// Test the connection to the database server
        /// </summary>
        /// <returns>Returns true if all ok, or false if fails</returns>
        public static bool TestServerConn()
        {
            try
            {
                NpgsqlConnectionStringBuilder csb = new NpgsqlConnectionStringBuilder()
                {
                    Host = DB_HOST,
                    Database = DB_DATABASE,
                    Username = DB_USER,
                    Password = DB_PASSWORD,
                    Port = DB_PORT,
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

        /// <summary>
        /// Returns a connection that uses the same parameters as the initial connection
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Create a DbCommand object and execute the function it receives as a parameter.
        /// It's just a container to simplify the code
        /// </summary>
        /// <param name="fnProccess">Function that will be executed. Receives the DbCommand object as parameter</param>
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


        /// <summary>
        /// Adds a parameter (with an optional value) to a DbCommand object
        /// </summary>
        /// <param name="cmd">DbCommand object</param>
        /// <param name="name">Parameter name</param>
        /// <param name="type">Parameter type</param>
        /// <param name="valor">Parameter value</param>
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

        /// <summary>
        /// Executes the SQL statement of a DbCommand object and returns an integer value
        /// </summary>
        /// <param name="cmd">DbCommand object that contains the SQL sentence</param>
        /// <returns></returns>
        public static int ReadInt(DbCommand cmd)
        {
            return AuxConvert.ToInt(cmd.ExecuteScalar());
        }
    }
}
