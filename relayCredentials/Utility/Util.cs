using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace relayCredentials
{
    class Util
    {
        /// <summary>
        /// ログの出力先
        /// </summary>
        public static string StackTraceFileFullName
        {
            get
            {
                var directory = Path.GetDirectoryName(Setting.SettingFilePath) ?? string.Empty;
                return Path.Combine(directory, "StackTrace.txt");
            }
        }

        public static void WriteStackTrace(Exception ex)
        {
            var sb = new StringBuilder();
            sb.AppendLine("*********************************************");
            sb.AppendLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            sb.Append("メソッド名：");
            sb.AppendLine(new StackFrame(1).GetMethod().Name);

            sb.AppendLine(ex.GetType().FullName);
            sb.Append("　");
            sb.AppendLine(ex.Message);
            if (ex.InnerException != null)
            {
                sb.Append("InnerException：");
                sb.AppendLine(ex.InnerException.ToString());
                sb.Append("　");
                sb.AppendLine(ex.InnerException.Message);
            }
            sb.AppendLine("StackTrace：");
            sb.AppendLine(Environment.StackTrace);
            sb.AppendLine();
            File.AppendAllText(StackTraceFileFullName, sb.ToString());
        }
    }
}
