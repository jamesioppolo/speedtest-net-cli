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
            var vicPark = new Location { Latitude = -32.5799, Longitude = 115.8975 };
            var aarnet = new Location { Latitude = -31.9554, Longitude = 115.8585 };
            Distance.Between(vicPark, aarnet).ShouldBe(69, 1);
        }
    }
}
