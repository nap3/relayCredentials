using System;
using System.IO;
using System.Net;
using relayCredentials.Utility;

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
            var fullAssemblyNmae = this.GetType().Assembly.Location;
            var s = string.Format("Assembly={0}    Setting.ProxyUser={1}", fullAssemblyNmae, Setting.ProxyUser);
            Log.Write(Log.Level.Full,s);
            
            try
            {
                Credentials = new NetworkCredential(Setting.ProxyUser, Setting.ProxyPassword);
            }
            catch (Exception ex)
            {
                Util.WriteStackTrace(ex);
                throw;
            }

        }

        /// <summary>
        /// host でプロキシ サーバーを使用しない場合は true。それ以外の場合は false。
        /// </summary>
        public bool IsBypassed(Uri host)
        {
            var bypassList = Setting.ProxyBypassList;
            if (bypassList == null)
            {
                var a = string.Format("IsBypassed={0}\tDnsSafeHost={1}\tAbsoluteUri={2}", "false", host.DnsSafeHost, host.AbsoluteUri);
                Log.Write(Log.Level.Full, a);
                return false;
            }

            var result = false;
            //bypassListに*.host.comのようなワイルドカード付きホスト名が含まれていた場合は未対応
            foreach (var s in bypassList)
            {
                if (String.Equals(host.DnsSafeHost, s, StringComparison.CurrentCultureIgnoreCase))  //大文字小文字は区別しない。
                {
                    result =  true;
                }
            }

            var logStr = string.Format("IsBypassed={0}\tDnsSafeHost={1}\tAbsoluteUri={2}", result, host.DnsSafeHost, host.AbsoluteUri);
            Log.Write(Log.Level.Full, logStr);
            return result;
        }
    }
}
