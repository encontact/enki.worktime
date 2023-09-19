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

            Assert.Equal(5, workingWeek.getPeriods().Count);

            var monday = workingWeek.getPeriods(startDay).First();
            var tuesday = workingWeek.getPeriods((int)IsoDayOfWeek.Tuesday).First();
            var wednesday = workingWeek.getPeriods((int)IsoDayOfWeek.Wednesday).First();
            var thursday = workingWeek.getPeriods((int)IsoDayOfWeek.Thursday).First();
            var friday = workingWeek.getPeriods(endDay).First();

            Assert.NotNull(monday);
            Assert.Equal(480, monday.startPeriod);
            Assert.Equal(1439, monday.endPeriod);

            Assert.NotNull(tuesday);
            Assert.Equal(0, tuesday.startPeriod);
            Assert.Equal(1439, tuesday.endPeriod);

            Assert.NotNull(wednesday);
            Assert.Equal(0, wednesday.startPeriod);
            Assert.Equal(1439, wednesday.endPeriod);

            Assert.NotNull(thursday);
            Assert.Equal(0, thursday.startPeriod);
            Assert.Equal(1439, thursday.endPeriod);

            Assert.NotNull(friday);
            Assert.Equal(0, friday.startPeriod);
            Assert.Equal(1080, friday.endPeriod);
        }

        [Fact]
        public void NextDayMethodTest()
        {
            var sunday = IsoDayOfWeek.Sunday;
            var monday = ComplexWorkingWeek.NextDayOfWeek(sunday);
            var tuesday = ComplexWorkingWeek.NextDayOfWeek(monday);
            var wednesday = ComplexWorkingWeek.NextDayOfWeek(tuesday);
            var thursday = ComplexWorkingWeek.NextDayOfWeek(wednesday);
            var friday = ComplexWorkingWeek.NextDayOfWeek(thursday);
            var saturday = ComplexWorkingWeek.NextDayOfWeek(friday);
            var anotherSunday = ComplexWorkingWeek.NextDayOfWeek(saturday);
            var anotheMonday = ComplexWorkingWeek.NextDayOfWeek(anotherSunday);

            Assert.Equal(IsoDayOfWeek.Sunday, sunday);
            Assert.Equal(IsoDayOfWeek.Monday, monday);
            Assert.Equal(IsoDayOfWeek.Tuesday, tuesday);
            Assert.Equal(IsoDayOfWeek.Wednesday, wednesday);
            Assert.Equal(IsoDayOfWeek.Thursday, thursday);
            Assert.Equal(IsoDayOfWeek.Friday, friday);
            Assert.Equal(IsoDayOfWeek.Saturday, saturday);
            Assert.Equal(IsoDayOfWeek.Sunday, anotherSunday);
            Assert.Equal(IsoDayOfWeek.Monday, anotheMonday);
        }

        [Fact]
        public void ReverseOrderPeriodTest()
        {
            var startDay = (int)IsoDayOfWeek.Thursday;
            var startLocalTime = new LocalTime(8, 0);

            var endDay = (int)IsoDayOfWeek.Tuesday;
            var endLocalTime = new LocalTime(18, 0);

            var workingWeek = new ComplexWorkingWeek();

            workingWeek.setWorkPeriod(startDay, endDay, startLocalTime, endLocalTime);

            Assert.Equal(6, workingWeek.getPeriods().Count);

            var thursday = workingWeek.getPeriods(startDay).First();
            var friday = workingWeek.getPeriods((int)IsoDayOfWeek.Friday).First();
            var saturday = workingWeek.getPeriods((int)IsoDayOfWeek.Saturday).First();
            var sunday = workingWeek.getPeriods((int)IsoDayOfWeek.Sunday).First();
            var monday = workingWeek.getPeriods((int)IsoDayOfWeek.Monday).First();
            var tuesday = workingWeek.getPeriods((int)IsoDayOfWeek.Tuesday).First();

            Assert.NotNull(thursday);
            Assert.Equal(480, thursday.startPeriod);
            Assert.Equal(1439, thursday.endPeriod);

            Assert.NotNull(friday);
            Assert.Equal(0, friday.startPeriod);
            Assert.Equal(1439, friday.endPeriod);

            Assert.NotNull(saturday);
            Assert.Equal(0, saturday.startPeriod);
            Assert.Equal(1439, saturday.endPeriod);

            Assert.NotNull(sunday);
            Assert.Equal(0, sunday.startPeriod);
            Assert.Equal(1439, sunday.endPeriod);

            Assert.NotNull(monday);
            Assert.Equal(0, monday.startPeriod);
            Assert.Equal(1439, monday.endPeriod);

            Assert.NotNull(tuesday);
            Assert.Equal(0, tuesday.startPeriod);
            Assert.Equal(1080, tuesday.endPeriod);
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