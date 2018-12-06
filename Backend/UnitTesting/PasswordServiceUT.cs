using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceLayer.Services;

namespace UnitTesting
{
    /// <summary>
    /// Summary description for PasswordServiceUT
    /// </summary>
    [TestClass]
    public class PasswordServiceUT
    {
        PasswordService ps;
        public PasswordServiceUT()
        {
            //Arrange
            ps = new PasswordService();
        }

        [TestMethod]
        public void hashPassword()
        {
            //Act
            string password = "buasd78324yas";
            byte[] salt = ps.GenerateSalt();
            string hash1 = ps.HashPassword(password, salt);
            string hash2 = ps.HashPassword(password, salt);

            password = "uibava97s133";
            string hash3 = ps.HashPassword(password, salt);



            //Assert
            Assert.AreEqual(hash1, hash2);
            Assert.AreNotEqual(hash1, hash3);
        }

        [TestMethod]
        public void HashPasswordSHA1()
        {
            //Act
            string password = "password";
            byte[] salt = ps.GenerateSalt();
            string hash1 = ps.HashPasswordSHA1(password, salt);
            string hash2 = ps.HashPasswordSHA1(password, salt);

            password = "12t3h0eu93h9ke";
            salt = ps.GenerateSalt();
            string hash3 = ps.HashPasswordSHA1(password, salt);
            //Assert
            Assert.AreEqual(hash1,hash2);
            Assert.AreNotEqual(hash1, hash3);
        }

        [TestMethod]
        public void CheckPasswordPwned()
        {
            Assert.AreNotEqual(0, ps.CheckPasswordPwned("password"));
            Assert.AreEqual(0, ps.CheckPasswordPwned("ASDfas!@fdasf!223gs3"));
        }

        [TestMethod]
        public void QueryPwnedApi()
        {
            string prefix = ps.HashPasswordSHA1("password", null).Substring(0, 5);
            string prefix2 = ps.HashPasswordSHA1("letgooooo", null).Substring(0, 5);

            Assert.AreEqual(true, isEqual(ps.QueryPwnedApi(prefix), ps.QueryPwnedApi(prefix)));
            Assert.AreEqual(false, isEqual(ps.QueryPwnedApi(prefix), ps.QueryPwnedApi(prefix2)));

        }

        private static bool isEqual(string[] arr1, string[] arr2)
        {
            if (arr1.Length != arr2.Length)
                return false;
            for(int i =0; i < arr1.Length; i++)
            {
                if (arr1[i] != arr2[i])
                    return false;
            }
            return true;
        }
    }
}
