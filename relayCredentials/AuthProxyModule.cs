using System;
using System.Net;

//http://blogs.msdn.com/b/rido/archive/2010/05/06/how-to-connect-to-tfs-through-authenticated-web-proxy.aspx
namespace relayCredentials
{
    public class AuthProxyModule : IWebProxy
    {
        public ICredentials Credentials { get; set; }


        public Uri GetProxy(Uri destination)
        {
            return new Uri(Setting.ProxyServer, UriKind.Absolute);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AuthProxyModule()
        {
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

            //bypassListに*host.comのようなワイルドカード付きホスト名が含まれていた場合は未対応
            foreach (var s in bypassList)
            {
                if (host.DnsSafeHost == s)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
