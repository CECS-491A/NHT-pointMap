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

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

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
            string hash1 = ps.HashPasswordSHA1(password, null);
            string hash2 = ps.HashPasswordSHA1(password, null);

            string password = "12t3h0eu93h9ke";
            string hash3 = ps.HashPasswordSHA1(password, null);
            //Assert
            Assert.AreEqual(hash1,hash2);
            Assert.AreNotEqual(hash1, hash2);
        }
    }
}
