using System;
using System.Security.Cryptography;
using System.Text;

namespace relayCredentials.Utility
{
    //チュートリアル : RSA キー コンテナの作成とエクスポート
    //https://msdn.microsoft.com/ja-jp/library/2w117ede(v=vs.100).aspx

    //管理者権限のcmdで実行する必要あり。

    //rem aspnet_regiis.exeの場所に移動（2.0以降から存在している）
    //cd \WINDOWS\Microsoft.Net\Framework\v4.0.*

    //rem NT AUTHORITY\NETWORK SERVICEにMyKeysキーコンテナの読み取りアクセス許可を与える。（aspx使用時などのアクセス権限がない場合は必要）
    //aspnet_regiis -pa "MyKeys" "NT AUTHORITY\NETWORK SERVICE"

    //rem キーコンテナ（Machineキー）をエクスポート
    //aspnet_regiis -px "MyKeys" "C:\Credentials\keys.xml" -pri

    //rem キーコンテナ削除
    //aspnet_regiis -pz "MyKeys"

    //rem キーコンテナインポート
    //aspnet_regiis -pi "MyKeys" "C:\Credentials\keys.xml"

    public class CipherRsa
    {
        private RSACryptoServiceProvider _rsa;

        /// <summary>
        /// XML形式の公開鍵
        /// </summary>
        public string PublicKey { get; private set; }

        /// <summary>
        /// XML形式の秘密鍵
        /// </summary>
        public string PrivateKey { get; private set; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="containerName">キーコンテナ名</param>
        public CipherRsa(string containerName)
        {
            var cp = new CspParameters();

            //キーコンテナを作成（すでに存在している場合は変更なし。）
            cp.KeyContainerName = containerName;
            cp.Flags = CspProviderFlags.UseMachineKeyStore;
            _rsa = new RSACryptoServiceProvider(cp);

            PublicKey = _rsa.ToXmlString(false);
            PrivateKey = _rsa.ToXmlString(true);
        }

        /// <summary>
        /// キーコンテナに格納された公開鍵を使って文字列を暗号化する
        /// </summary>
        /// <param name="str">暗号化する文字列</param>
        /// <returns>暗号化された文字列</returns>
        public string Encrypt(string str)
        {
            var data = Encoding.UTF8.GetBytes(str);
            var encryptedData = _rsa.Encrypt(data, false);
            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// 公開鍵を使って文字列を暗号化する
        /// </summary>
        /// <param name="str">暗号化する文字列</param>
        /// <param name="publicKey">暗号化に使用する公開鍵(XML形式)</param>
        /// <returns>暗号化された文字列</returns>
        public static string Encrypt(string str, string publicKey)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);
            var data = Encoding.UTF8.GetBytes(str);
            var encryptedData = rsa.Encrypt(data, false);
            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// キーコンテナに格納された秘密鍵を使って、文字列を復号化する。復号に失敗するとCryptographicExceptionが発生する。
        /// </summary>
        /// <param name="str">Encryptメソッドにより暗号化された文字列</param>
        /// <returns>復号化された文字列</returns>
        public string Decrypt(string str)
        {
            //復号化する
            var data = Convert.FromBase64String(str);
            var decryptedData = _rsa.Decrypt(data, false);
            return Encoding.UTF8.GetString(decryptedData);
        }

        /// <summary>
        /// 秘密鍵を使って文字列を復号化する。復号に失敗するとCryptographicExceptionが発生する。
        /// </summary>
        /// <param name="str">Encryptメソッドにより暗号化された文字列</param>
        /// <param name="privateKey">復号化に必要な秘密鍵(XML形式)</param>
        /// <returns>復号化された文字列</returns>
        public static string Decrypt(string str, string privateKey)
        {
            //RSACryptoServiceProviderオブジェクトの作成
            var rsa = new System.Security.Cryptography.RSACryptoServiceProvider();

            //秘密鍵を指定
            rsa.FromXmlString(privateKey);

            //復号化する文字列をバイト配列に
            var data = System.Convert.FromBase64String(str);

            //復号化する
            var decryptedData = rsa.Decrypt(data, false);

            //結果を文字列に変換
            return Encoding.UTF8.GetString(decryptedData);
        }

        /// <summary>
        /// キーコンテナを削除する
        /// </summary>
        public void DeleteKeyContainer()
        {
            //キーコンテナを削除
            _rsa.PersistKeyInCsp = false;
            _rsa.Clear();
        }
    }
}
