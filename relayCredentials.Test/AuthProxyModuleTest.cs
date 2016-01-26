using System;
using System.IO;
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

			Setting.Save("http://proxy.com:8080", "user", "password", new []{"aa.com", "bb.cc.com"});
			Assert.IsTrue(File.Exists(Setting.SettingFilePath));

            Setting.Reload();
            Assert.AreEqual(1, Setting.SettingVersion);
            Assert.AreEqual("http://proxy.com:8080", Setting.ProxyServer);
            Assert.AreEqual("user", Setting.ProxyUser);
            Assert.AreEqual("password", Setting.ProxyPassword);
            Assert.AreEqual("aa.com", Setting.ProxyBypassList[0]);
            Assert.AreEqual("bb.cc.com", Setting.ProxyBypassList[1]);
		}

        [TestMethod]
        public void ファイルパスの確認()
        {
            var f = Setting.SettingFilePath;
            StringAssert.Contains(f, "relayCredentialsSetting.xml");
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

            Assert.AreEqual(Path.GetFullPath( @".\relayCredentialsSetting.xml"),Setting.SettingFilePath);
        }

        [TestMethod]
        [DeploymentItem(@".\relayCredentials.Test\TestResource\XmlにBypassListなし\relayCredentialsSetting.xml")]
        public void XmlにBypassListなし()
        {
            Setting.Reload();

            Assert.AreEqual(1, Setting.SettingVersion);
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
            Setting.Reload();
            
            var proxy = new AuthProxyModule();
            Assert.IsTrue(proxy.IsBypassed(new Uri("http://aa.com")));
            Assert.IsTrue(proxy.IsBypassed(new Uri("http://bb.cc.com")));

            Assert.IsFalse(proxy.IsBypassed(new Uri("http://zz.aa.com")));
            Assert.IsFalse(proxy.IsBypassed(new Uri("http://cc.com")));
            Assert.IsFalse(proxy.IsBypassed(new Uri("http://ee.cc.com")));
        }
	}
}
