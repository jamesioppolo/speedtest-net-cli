using NUnit.Framework;
using Shouldly;
using SpeedtestNetCli.Model;
using System.Xml.Linq;

namespace SpeedtestNetCliTests
{
    [TestFixture]
    public class LocationTests
    {
        [Test]
        public void TestDistance()
        {
            var sampleLocation = new Location(XDocument.Parse("<server lat=\"-32.5799\" lon=\"115.8975\"/>").Root);
            var aarnetPerth = new Location(XDocument.Parse("<server lat=\"-31.9554\" lon=\"115.8585\"/>").Root);
            sampleLocation.DistanceTo(aarnetPerth).ShouldBe(69, 1);
        }
    }
}
