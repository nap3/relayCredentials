using System;
using System.IO;
using System.Configuration;
using System.Security.Cryptography;
using System.Threading;
using relayCredentials.Utility;

namespace relayCredentials
{
    public class Setting
    {
        private const bool EMBED_BOM = true;
        private static SettingInfo _info = new SettingInfo();

        public static int SettingVersion { get { return _info.SettingVersion; } }
        public static string ProxyServer { get { return _info.ProxyServer; } }
        public static string ProxyUser { get { return _info.ProxyUser; } }


        public static string ProxyPassword
        {
            get
            {
                var pass = _info.ProxyPassword;
                if (!string .IsNullOrEmpty(_info.EncryptedProxyPassword))
                {
                    try
                    {
                        var rsa = new CipherRsa(_info.KeyContainerName);
                        pass = rsa.Decrypt(_info.EncryptedProxyPassword);
                    }
                    catch (CryptographicException)
                    {
                        Log.Write(Log.Level.Error, "Proxyパスワードの復号に失敗");
                    }
                }
                return pass;
            }
        }

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
        /// コンストラクタ
        /// </summary>
        static Setting()
        {
            Reload();
            Log.Write(Log.Level.Full, "**************** Settingコンストラクタ ****************");
        }

        /// <summary>
        /// すべての設定値をクリアする。
        /// </summary>
        public static void Clear()
        {
            _info.Clear();
        }

        /// <summary>
        /// ProxyServerが設定されていない場合に設定ファイルから設定値を読み込む。（設定を変えたいときは先にClearメソッドを呼ぶ必要あり。）
        /// </summary>
        public static void Reload()
        {
            if (!File.Exists(Setting.SettingFilePath)|| !string.IsNullOrEmpty(Setting.ProxyServer))
            {
                return;
            }

            try
            {
                Setting.Clear();
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(SettingInfo));
                using (var sr = new StreamReader(Setting.SettingFilePath, new System.Text.UTF8Encoding(EMBED_BOM)))
                {
                    //XMLファイルから読み込み、デシリアライズする
                    _info = (SettingInfo)serializer.Deserialize(sr);
                }

                if (_info.Update())
                {
                    Setting.Save();
                }
                //ログ出力の設定
                Log.WriteLevel = _info.LogLevel;
                var directory = Path.GetDirectoryName(Setting.SettingFilePath) ?? string.Empty;
                Log.LogFilePath = Path.Combine(directory, "relayCredentialsLog.txt");
            }
            catch (Exception ex)
            {
                Util.WriteStackTrace(ex);
                throw;
            }
        }

        /// <summary>
        /// 設定値を設定ファイルに書き込む
        /// </summary>
        public static void Save()
        {
            using (var sw = new StreamWriter(Setting.SettingFilePath, false, new System.Text.UTF8Encoding(EMBED_BOM)))
            {
                //シリアライズして、XMLファイルに保存する
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(SettingInfo));
                serializer.Serialize(sw, _info);
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

            Setting.Save();
        }
    }//class



    /// <summary>
    /// XMLに保存する設定値を保持するクラス
    /// </summary>
    [Serializable]
    public class SettingInfo
    {
        /// <summary>
        /// 設定ファイルのバージョン（SettingInfoのフィールドを追加した時にインクリメントする）
        /// </summary>

        public int SettingVersion { get; set; }

        public string ProxyServer { get; set; }
        public string ProxyUser { get; set; }
        public string ProxyPassword { get; set; }
        public string EncryptedProxyPassword { get; set; }
        public string KeyContainerName { get { return "relayCredentials"; } }
        public string[] BypassList { get; set; }
        public Log.Level LogLevel { get; set; }

        public SettingInfo()
        {
            SettingVersion = 2;
            this.LogLevel = Log.Level.None;
        }

        public bool Update()
        {
            var updated = false;

            if (this.SettingVersion == 1)
            {
                this.LogLevel = Log.Level.None;
                this.SettingVersion = 2;
                updated = true;
            }

            if (!string.IsNullOrEmpty(this.ProxyPassword) && string.IsNullOrEmpty(this.EncryptedProxyPassword))
            {
                var rsa = new CipherRsa(this.KeyContainerName);
                this.EncryptedProxyPassword = rsa.Encrypt(this.ProxyPassword);
                this.ProxyPassword = string.Empty;
                updated = true;
            }

            return updated;
        }

        /// <summary>
        /// すべての設定値をクリアする。
        /// </summary>
        public void Clear()
        {
            ProxyServer = string.Empty;
            ProxyUser = string.Empty;
            ProxyPassword = string.Empty;
            BypassList = new string[] { };
            this.LogLevel = Log.Level.None;
        }
    }
}

