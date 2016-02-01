using System;
using System.IO;
using System.Configuration;

namespace relayCredentials
{
	public class Setting
    {
        private const bool EMBED_BOM = true;

        public static int SettingVersion { get { return _info.SettingVersion; } }
        public static string ProxyServer { get { return _info.ProxyServer; } }
        public static string ProxyUser { get { return _info.ProxyUser; } }
        public static string ProxyPassword { get { return _info.ProxyPassword; } }
        public static string[] ProxyBypassList { get { return _info.BypassList; } }

        /// <summary>
        /// 設定ファイルのパス
        /// </summary>
        public static string SettingFilePath
        {
            get
            {
                var c = ConfigurationManager.AppSettings.Get("relayCredentialsConfigPath");
                if (string.IsNullOrEmpty(c))
                {
                    c = @".\relayCredentialsSetting.xml";
                }

                return Path.GetFullPath(c);
            }
        }

        /// <summary>
        /// ログの出力先（今のところデバッグ専用）
        /// </summary>
        public static string LogFilePath
        {
            get
            {
                var directory = Path.GetDirectoryName(Setting.SettingFilePath) ?? string.Empty;
                return Path.Combine(directory, "relayCredentialsLog.txt");
            }
        }

        private static SettingInfo _info = new SettingInfo();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        static Setting()
        {
            Reload();
        }

        /// <summary>
        /// すべての設定値をクリアする。
        /// </summary>
        public static void Clear()
        {
            _info.Clear();
        }

        /// <summary>
        /// 設定ファイルから設定値を読み込む
        /// </summary>
        public static void Reload()
        {
            Setting.Clear();
            if (!File.Exists(Setting.SettingFilePath))
            {
                return;
            }

            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(SettingInfo));
            using (var sr = new StreamReader(Setting.SettingFilePath, new System.Text.UTF8Encoding(EMBED_BOM)))
            {
                var i = (SettingInfo)serializer.Deserialize(sr);
                //XMLファイルから読み込み、デシリアライズする
                _info = i;
            }
        }

        /// <summary>
        /// 設定値を設定ファイルに書き込む
        /// </summary>
        public static void Save(string proxyServer, string proxyUser, string proxyPassword, string[] bypassList)
        {
            _info.ProxyServer = proxyServer;
            _info.ProxyUser = proxyUser;
            _info.ProxyPassword = proxyPassword;
            _info.BypassList = bypassList;

            using (var sw = new StreamWriter(Setting.SettingFilePath, false, new System.Text.UTF8Encoding(EMBED_BOM)))
            {
                //シリアライズして、XMLファイルに保存する
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(SettingInfo));
                serializer.Serialize(sw, _info);
            }
        }
    }//class




    /// <summary>
    /// XMLに保存する設定値を保持するクラス
    /// </summary>
    [Serializable]
    public class SettingInfo
    {
        //SettingInfoのフィールドを追加した場合にインクリメントする
        public int SettingVersion { get { return 1; } }

        public string ProxyServer { get; set; }
        public string ProxyUser { get; set; }
        public string ProxyPassword { get; set; }
        public string[] BypassList { get; set; }


        /// <summary>
        /// すべての設定値をクリアする。
        /// </summary>
        public void Clear()
        {
            ProxyServer = string.Empty;
            ProxyUser = string.Empty;
            ProxyPassword = string.Empty;
            BypassList = new string[]{};
        }
    }
}

