using NUnit.Framework;
using PointMap.Services;

namespace Tests
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
        public void getFoo()
        {
            Assert.That("foo", Is.EqualTo(session.getFoo()));
        }
    }
}