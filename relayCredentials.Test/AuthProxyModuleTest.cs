using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace relayCredentials.Test
{
    [TestClass]
    public class AuthProxyModuleTest
    {
        [TestMethod]
        public void CreateConfigFileTest()
        {
            if (File.Exists(Setting.SettingFilePath))
            {
                File.Delete(Setting.SettingFilePath);
            }

            //設定ファイル保存
            Setting.Save("http://proxy.com:8080", "user", "password", new[] { "aa.com", "bb.cc.com" });
            Assert.IsTrue(File.Exists(Setting.SettingFilePath));


            //設定ファイル再読み込み
            Setting.Clear();
            Setting.Reload();
            Assert.AreEqual(2, Setting.SettingVersion);
            Assert.AreEqual("http://proxy.com:8080", Setting.ProxyServer);
            Assert.AreEqual("user", Setting.ProxyUser);
            Assert.AreEqual("password", Setting.ProxyPassword);
            Assert.AreEqual("aa.com", Setting.ProxyBypassList[0]);
            Assert.AreEqual("bb.cc.com", Setting.ProxyBypassList[1]);

            StringAssert.Contains(Setting.SettingFilePath, "relayCredentialsSetting.xml");

            var w = new AuthProxyModule();
        }

        [TestMethod]
        public void 設定ファイルが一切ない時の動作()
        {
            if (File.Exists(Setting.SettingFilePath))
            {
                File.Delete(Setting.SettingFilePath);
            }

            if (File.Exists(@".\relaycredentials.test.dll.config"))
            {
                File.Delete(@".\relaycredentials.test.dll.config");
            }

            Assert.AreEqual(Path.GetFullPath(@".\relayCredentialsSetting.xml"), Setting.SettingFilePath);
        }

        [TestMethod]
        [DeploymentItem(@".\relayCredentials.Test\TestResource\XmlにBypassListなし\relayCredentialsSetting.xml")]
        public void XmlにBypassListなし()
        {
            Setting.Clear();
            Setting.Reload();


            Assert.AreEqual("http://proxy.com:8080", Setting.ProxyServer);
            Assert.AreEqual("user", Setting.ProxyUser);
            Assert.AreEqual("password", Setting.ProxyPassword);

            Assert.IsNull(Setting.ProxyBypassList);
            var proxy = new AuthProxyModule();
            Assert.IsFalse(proxy.IsBypassed(new Uri("http://aa.com")));
        }

        [TestMethod]
        [DeploymentItem(@".\relayCredentials.Test\TestResource\ProxyBypassTest\relayCredentialsSetting.xml")]
        public void ProxyBypassTest()
        {
            Setting.Clear();
            Setting.Reload();

            var proxy = new AuthProxyModule();
            Assert.IsTrue(proxy.IsBypassed(new Uri("http://aa.com")));
            Assert.IsTrue(proxy.IsBypassed(new Uri("http://bb.cc.com")));

            Assert.IsFalse(proxy.IsBypassed(new Uri("http://zz.aa.com")));
            Assert.IsFalse(proxy.IsBypassed(new Uri("http://cc.com")));
            Assert.IsFalse(proxy.IsBypassed(new Uri("http://ee.cc.com")));
        }

        [TestMethod]
        [DeploymentItem(@".\relayCredentials.Test\TestResource\ディレクトリ作成テスト\relayCredentialsSetting.xml")]
        public void ディレクトリ作成テスト()
        {
            Setting.Clear();
            Setting.Reload();

            var proxy = new AuthProxyModule();
            Assert.IsTrue(proxy.IsBypassed(new Uri("http://aa.com")));
            Assert.IsTrue(proxy.IsBypassed(new Uri("http://bb.cc.com")));

            Assert.IsFalse(proxy.IsBypassed(new Uri("http://zz.aa.com")));
            Assert.IsFalse(proxy.IsBypassed(new Uri("http://cc.com")));
            Assert.IsFalse(proxy.IsBypassed(new Uri("http://ee.cc.com")));
        }

        [TestMethod]
        public void Reload並列処理耐久Test()
        {
            const int count = 1000;
            const string actualUser = "ReloadUser";
            var result = new string[count];

            //設定ファイル保存
            Setting.Save("http://proxy.com:8080", actualUser, "password", new[] { "aa.com", "bb.cc.com" });
            Assert.IsTrue(File.Exists(Setting.SettingFilePath));


            Action<int> a = delegate(int n)
             {
                 Debug.Print(n.ToString("0000"));
                 Setting.Reload();

                 if (Setting.ProxyUser == actualUser)
                 {
                     result[n] = "OK";
                 }
                 else
                 {
                     result[n] = "NG";
                 }
             };

            //並列処理
            Parallel.For(0, count, a);

            foreach (var s in result)
            {
                Assert.AreEqual("OK", s);
            }
        }


    }
}
