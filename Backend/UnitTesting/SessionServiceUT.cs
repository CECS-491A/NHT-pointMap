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
            string s1 = ss.generateSession();
            string s2 = ss.generateSession();
            //Assert
            Assert.AreEqual(16, s1.Length);
            Assert.AreNotEqual(s1, s2);
        }
    }
}
