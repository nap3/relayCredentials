using System;
using System.IO;
using System.Net;

//http://blogs.msdn.com/b/rido/archive/2010/05/06/how-to-connect-to-tfs-through-authenticated-web-proxy.aspx
namespace relayCredentials
{
    public class AuthProxyModule : IWebProxy
    {
        /// <summary>
        /// 認証情報
        /// </summary>
        public ICredentials Credentials { get; set; }

        /// <summary>
        /// Proxy Server名を返す。
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        public Uri GetProxy(Uri destination)
        {
            return new Uri(Setting.ProxyServer, UriKind.Absolute);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AuthProxyModule()
        {
            //File.AppendAllText(Setting.LogFilePath, string.Format("コンストラクタ Setting.ProxyUser={0}\r\n", Setting.ProxyUser));
            Credentials = new NetworkCredential(Setting.ProxyUser, Setting.ProxyPassword);
        }

        /// <summary>
        /// host でプロキシ サーバーを使用しない場合は true。それ以外の場合は false。
        /// </summary>
        public bool IsBypassed(Uri host)
        {
            var bypassList = Setting.ProxyBypassList;
            if (bypassList == null)
            {
                return false;
            }

            //File.AppendAllText(Setting.LogFilePath, string.Format("DnsSafeHost={0}    AbsoluteUri={1}\r\n", host.DnsSafeHost, host.AbsoluteUri));

            //bypassListに*.host.comのようなワイルドカード付きホスト名が含まれていた場合は未対応
            foreach (var s in bypassList)
            {
                if (String.Equals(host.DnsSafeHost, s, StringComparison.CurrentCultureIgnoreCase))  //大文字小文字は区別しない。
                {
                    return true;
                }
            }

            return false;
        }
    }
}
