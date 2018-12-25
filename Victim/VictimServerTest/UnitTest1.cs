using NUnit.Framework;

namespace Tests
{
    public class Tests
    {
        Victim.VictimServer vs;
        string password_1;
        [SetUp]
        public void Setup()
        {
            password_1 = "123456654321";
            vs = new Victim.VictimServer(8000, password_1);
        }

        [Test]
        public void valid_passwordTest()
        {
            Assert.IsTrue(vs.valid_password(password_1));
            Assert.IsFalse(vs.valid_password("123456"));
        }
    }
}