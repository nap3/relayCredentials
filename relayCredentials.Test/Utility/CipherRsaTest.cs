using System;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using relayCredentials.Utility;

namespace relayCredentials.Test.Utility
{
    [TestClass]
    public class CipherRsaTest
    {
        [TestMethod]
        public void CipherRsa全体確認()
        {
            const string input = "abcdefg";

            var rsa = new CipherRsa("alltest");
            var publicKey = rsa.PublicKey;
            var privateKey = rsa.PrivateKey;

            var enc1 = rsa.Encrypt(input);
            var enc2 = rsa.Encrypt(input);
            var enc3 = CipherRsa.Encrypt(input, publicKey);
            var enc4 = CipherRsa.Encrypt(input, publicKey);
            Assert.AreNotEqual(enc1, enc2);
            Assert.AreNotEqual(enc3, enc4);
            Assert.AreNotEqual(enc1, enc3);    //暗号文はすべて異なる値になる。

            var plain1 = rsa.Decrypt(enc1);
            var plain2 = CipherRsa.Decrypt(enc1,privateKey);
            var plain3 = rsa.Decrypt(enc3);
            var plain4 = CipherRsa.Decrypt(enc3, privateKey);
            Assert.AreEqual(input, plain1);
            Assert.AreEqual(input, plain2);
            Assert.AreEqual(input, plain3);
            Assert.AreEqual(input, plain4);

            var rsa2 = new CipherRsa("alltest");
            var enc5 = rsa2.Encrypt(input);
            var plain5 = CipherRsa.Decrypt(enc5, privateKey); //rsa2ではないrsaのprivateKeyで復号
            Assert.AreEqual(input, plain5);
            
            rsa.DeleteKeyContainer();   //コンテナ削除

            var rsa3 = new CipherRsa("alltest");
            var enc6 = rsa3.Encrypt(input);

            var plain6 = CipherRsa.Decrypt(enc5, privateKey); //rsa2ではないrsaのprivateKeyで復号   //コンテナ削除した後も復号可能
            //var plain7 = CipherRsa.Decrypt(enc6, privateKey); //rsa2ではないrsaのprivateKeyで復号   //例外発生
            Assert.AreEqual(input, plain6);
            //Assert.AreEqual(input, plain7);
            
            rsa3.DeleteKeyContainer();
        }

        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void CipherRsa復号失敗()
        {
            const string input = "abcdefg";

            var rsa = new CipherRsa("alltest");
            var privateKey = rsa.PrivateKey;
            var enc1 = rsa.Encrypt(input);

            rsa.DeleteKeyContainer();

            var rsa2 = new CipherRsa("alltest");
            var enc2 = rsa2.Encrypt(input);

            var plain1 = CipherRsa.Decrypt(enc1, privateKey);
            Assert.AreEqual(input, plain1);

            var plain2 = CipherRsa.Decrypt(enc2, privateKey); //コンテナ削除前のprivateKeyでは例外が発生する。
        }
    }
}
