using enki.libs.workhours;
using NodaTime;
using System;
using Xunit;

namespace WorkTimeTests
{
    public class ComplexWorkingWeekTest
    {
        [Fact]
        public void SetWorkDayTest()
        {
            var startLocalTime = new LocalTime(8, 0);
            var endLocalTime = new LocalTime(18, 0);
            var workingWeek = new ComplexWorkingWeek();
            var invalidDayOfWeek = (int)IsoDayOfWeek.None;

            Assert.Throws<ArgumentException>("dayOfWeek", () => workingWeek.setWorkDay(invalidDayOfWeek, startLocalTime, endLocalTime));
        }
    }
}