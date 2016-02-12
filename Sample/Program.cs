using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using relayCredentials;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var wc = new WebClient();
            var u = new Uri("http://dummy.com");
            var proxy = wc.Proxy.GetProxy(u).AbsoluteUri;
            Console.WriteLine("proxy=" + proxy);


            Console.WriteLine("3秒後にhtmlの読み込みを試みます。");
            System.Threading.Thread.Sleep(3000);

            var response = wc.DownloadString("http://google.com");
            Console.WriteLine(string.Empty);
            Console.WriteLine(response);

            System.Threading.Thread.Sleep(3000);
        }
    }
}
