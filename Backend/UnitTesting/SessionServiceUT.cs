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
            Guid guid1 = ss.generateSession();
            Guid guid2 = ss.generateSession();

            //Assert
            StringAssert.Contains(guid1.ToString(), guid2.ToString());
        }
    }
}
