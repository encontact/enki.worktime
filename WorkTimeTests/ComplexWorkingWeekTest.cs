using enki.libs.workhours;
using NodaTime;
using System;
using System.Linq;
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
            var validDayOfWeek = (int)IsoDayOfWeek.Monday;

            workingWeek.setWorkDay(validDayOfWeek, startLocalTime, endLocalTime);

            Assert.True(workingWeek.getPeriods(validDayOfWeek).Any());
        }

        [Fact]
        public void SmallestInvalidSetWorkDayTest()
        {
            var startLocalTime = new LocalTime(8, 0);
            var endLocalTime = new LocalTime(18, 0);
            var workingWeek = new ComplexWorkingWeek();
            var invalidDayOfWeek = (int)IsoDayOfWeek.None;

            Assert.Throws<ArgumentException>("dayOfWeek", () => workingWeek.setWorkDay(invalidDayOfWeek, startLocalTime, endLocalTime));
        }

        [Fact]
        public void LongestInvalidSetWorkDayTest()
        {
            var startLocalTime = new LocalTime(8, 0);
            var endLocalTime = new LocalTime(18, 0);
            var workingWeek = new ComplexWorkingWeek();
            var invalidDayOfWeek = (int)IsoDayOfWeek.Sunday + 1;

            Assert.Throws<ArgumentException>("dayOfWeek", () => workingWeek.setWorkDay(invalidDayOfWeek, startLocalTime, endLocalTime));
        }

        [Fact]
        public void MultiDaysPeriodTest()
        {
            var startDay = (int)IsoDayOfWeek.Monday;
            var startLocalTime = new LocalTime(8, 0);

            var endDay = (int)IsoDayOfWeek.Friday;
            var endLocalTime = new LocalTime(18, 0);

            var workingWeek = new ComplexWorkingWeek();

            workingWeek.setWorkPeriod(startDay, endDay, startLocalTime, endLocalTime);

            Assert.True(workingWeek.getPeriods(startDay).Any());
            Assert.True(workingWeek.getPeriods(endDay).Any());
            Assert.Equal(5, workingWeek.getPeriods().Count);
        }

        [Fact]
        public void SingleDayPeriodTest()
        {
            var startDay = (int)IsoDayOfWeek.Monday;
            var startLocalTime = new LocalTime(8, 0);

            var endDay = (int)IsoDayOfWeek.Monday;
            var endLocalTime = new LocalTime(18, 0);

            var workingWeek = new ComplexWorkingWeek();

            workingWeek.setWorkPeriod(startDay, endDay, startLocalTime, endLocalTime);

            Assert.True(workingWeek.getPeriods(startDay).Any());
            Assert.True(workingWeek.getPeriods(endDay).Any());
            Assert.Single(workingWeek.getPeriods());
        }
    }
}