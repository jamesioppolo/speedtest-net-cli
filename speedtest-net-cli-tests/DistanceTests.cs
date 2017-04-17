using NUnit.Framework;
using Shouldly;
using SpeedtestNetCli.Utilities;

namespace SpeedtestNetCliTests
{
    [TestFixture]
    public class DistanceTests
    {
        [Test]
        public void TestDistance()
        {
            var sampleLocation = new Location { Latitude = -32.5799, Longitude = 115.8975 };
            var aarnetPerth = new Location { Latitude = -31.9554, Longitude = 115.8585 };
            Distance.Between(sampleLocation, aarnetPerth).ShouldBe(69, 1);
        }
    }
}
