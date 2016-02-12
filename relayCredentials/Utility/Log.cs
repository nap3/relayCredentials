using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace relayCredentials.Utility
{
    public static class Log
    {
        public enum Level
        {
            None = 1,
            Error = 5,
            Full = 10
        }

        /// <summary>
        /// マルチスレッド排他制御用ロック
        /// </summary>
        private static readonly ReaderWriterLock _rwLock = new ReaderWriterLock();

        /// <summary>
        /// ログ出力先
        /// </summary>
        public static string LogFilePath { get; set; }

        /// <summary>
        /// ログ出力Level（設定したログレベルと同じ、もしくはそれ以上のログを出力する。）
        /// </summary>
        public static Level WriteLevel { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        static Log()
        {
            LogFilePath = string.Empty;
            WriteLevel = Level.None;
        }

        /// <summary>
        /// ログ出力
        /// </summary>
        public static void Write(Level level, string str)
        {
            if (level < WriteLevel)
            {
                return;
            }

            WriteCore(Log.LogFilePath, str);
        }

        /// <summary>
        /// 強制的にログ出力する。
        /// </summary>
        /// <param name="path">ファイル名も含めたフルパスを指定する。</param>
        /// <param name="str">ログに出力する文字列</param>
        public static void ForceWrite(string path, string str)
        {
            //メソッド名の取得処理がずれるのであえてラップする。
            WriteCore(path, str);
        }

        private static void WriteCore(string path, string str)
        {
            _rwLock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                var methodname = new StackFrame(2).GetMethod().Name;
                File.AppendAllText(path, DateTime.Now + "\t" + methodname + "\t" + str +"\r\n");
            }
            finally
            {
                _rwLock.ReleaseWriterLock();
            }            
        }
        
    }
}
