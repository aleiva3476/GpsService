using System;
using System.Diagnostics;

namespace GPSService
{
    /// <summary>
    /// This class contains methods for inserting records into the system event log
    /// </summary>
    class AuxSystemLog
    {
        public static string AppName = "";

        /// <summary>
        /// Writes an information record
        /// If there are optional parameters, string.Format () will be used to format the text that it receives as the first parameter
        /// </summary>
        /// <param name="text">Text that we are going to record</param>
        /// <param name="paramList">Optional params.</param>
        public static void Information(string text, params object[] paramList)
        {
            WriteEvent(EventLogEntryType.Information, text, paramList);
        }

        /// <summary>
        /// Writes a warning record
        /// If there are optional parameters, string.Format () will be used to format the text that it receives as the first parameter
        /// </summary>
        /// <param name="text">Text that we are going to record</param>
        /// <param name="paramList">Optional params.</param>
        public static void Warning(string text, params object[] paramList)
        {
            WriteEvent(EventLogEntryType.Warning, text, paramList);
        }

        /// <summary>
        /// Writes an error record
        /// If there are optional parameters, string.Format () will be used to format the text that it receives as the first parameter
        /// </summary>
        /// <param name="text">Text that we are going to record</param>
        /// <param name="paramList">Optional params.</param>
        public static void Error(string text, params object[] paramList)
        {
            WriteEvent(EventLogEntryType.Error, text, paramList);
        }

        /// <summary>
        /// Writes a record into the system event log
        /// If there are optional parameters, string.Format () will be used to format the text that it receives as the first parameter
        /// </summary>
        /// <param name="tipoEvento">Event type</param>
        /// <param name="text">Text that we are going to record</param>
        /// <param name="paramList">Optional params.</param>
        private static void WriteEvent(EventLogEntryType tipoEvento, string text, params object[] paramList)
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

        //public static void WriteEvent(Exception ex, string text)
        //{
        //    StringBuilder sb = new StringBuilder();

        //    if (text != null)
        //    {
        //        sb.AppendFormat("{0}{1}", text, Environment.NewLine);
        //    }

        //    if (ex != null)
        //    {
        //        sb.AppendFormat("{0}{1}", ex.Message, Environment.NewLine);
        //        Exception inner = ex.InnerException;
        //        while (inner != null)
        //        {
        //            sb.AppendFormat("[{0}] {1}{2}", inner.GetType(), inner.Message, Environment.NewLine);
        //            inner = inner.InnerException;
        //        }
        //    }

        //    WriteEvent(EventLogEntryType.Error, sb.ToString());
        //}

    }
}
