using NUnit.Framework;
using Services.Services;

namespace UnitTesting
{
    public class Tests
    {
        SessionService session;   

        [SetUp]
        public void Setup()
        {
            session = new SessionService();
        }

        [Test]
        public void getFooTest()
        {
            Assert.AreEqual("foo", session.getFoo());
        }
    }
}