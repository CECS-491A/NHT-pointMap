using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceLayer.Services;

namespace UnitTesting
{
    [TestClass]
    public class SessionServiceUT
    {
        SessionService ss;
        public SessionServiceUT()
        {
            //Arrange
            ss = new SessionService();
        }

        [TestMethod]
        public void generateSession()
        {
            //Act
            string s1 = ss.GenerateSession();
            string s2 = ss.GenerateSession();
            //Assert
            Assert.AreEqual(64, s1.Length);
            Assert.AreNotEqual(s1, s2);
        }
    }
}
