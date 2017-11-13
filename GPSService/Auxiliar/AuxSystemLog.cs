using System;
using System.Diagnostics;
using System.Text;

namespace GPSService
{
    class AuxSystemLog
    {
        public static string AppName = "";

        public static void Information(string text, params object[] paramList)
        {
            Escribe(EventLogEntryType.Information, text, paramList);
        }

        public static void Warning(string text, params object[] paramList)
        {
            Escribe(EventLogEntryType.Warning, text, paramList);
        }
        public static void Error(string text, params object[] paramList)
        {
            Escribe(EventLogEntryType.Error, text, paramList);
        }

        public static void Escribe(EventLogEntryType tipoEvento, string text, params object[] paramList)
        {
            EventLog oEventLog = new EventLog();
            try
            {
                string nombreLog = "Application";

                if (string.IsNullOrEmpty(AuxSystemLog.AppName))
                {
                    AuxSystemLog.AppName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                }

                // Registro de la aplicación como un origen de eventos. 
                if (!EventLog.SourceExists(AuxSystemLog.AppName))
                {
                    EventLog.CreateEventSource(AuxSystemLog.AppName, nombreLog);
                }

                string textoLog = paramList == null ? text : string.Format(text, paramList);
                oEventLog.Source = AuxSystemLog.AppName;
                oEventLog.WriteEntry(textoLog, tipoEvento);
            }
            catch (ArgumentException) { }
            catch (InvalidOperationException) { }
            finally
            {
                oEventLog = null;
            }
        }

        public static void Escribe(Exception ex, string text)
        {
            StringBuilder sb = new StringBuilder();

            if (text != null)
            {
                sb.AppendFormat("{0}{1}", text, Environment.NewLine);
            }

            if (ex != null)
            {
                sb.AppendFormat("{0}{1}", ex.Message, Environment.NewLine);
                Exception inner = ex.InnerException;
                while (inner != null)
                {
                    sb.AppendFormat("[{0}] {1}{2}", inner.GetType(), inner.Message, Environment.NewLine);
                    inner = inner.InnerException;
                }
            }

            Escribe(EventLogEntryType.Error, sb.ToString());
        }

    }
}
