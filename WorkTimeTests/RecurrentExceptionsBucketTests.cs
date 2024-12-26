using System;
using System.Collections.Generic;
using Xunit;

namespace WorkTime.Tests
{
    public class RecurrentExceptionsBucketTests
    {
        [Fact]
        public void AddUniqueByDayTest()
        {
            var date2000 = new NodaTime.LocalDateTime(2000, 01, 01, 0, 0);
            var date2001 = new NodaTime.LocalDateTime(2000, 01, 01, 0, 0);

            var recurrentBucket = new RecurrentExceptionsBucket(true);
            try
            {
                recurrentBucket.Add(date2000, 480, 960);
            }
            catch
            {
                Assert.True(false, "Fail to add recurrent exception.");
            }

            try
            {
                recurrentBucket.Add(date2001, 0, 960);
                Assert.True(false, "Fail when accept invalid period.");
            }
            catch (Exception e)
            {
                Assert.Equal("There is already an item to the date indicated in the list.", e.Message);
            }
        }

        // [Fact]
        // public void AddSolvingSimpleConflitTest()
        // {
        //     var date2000 = new NodaTime.LocalDateTime(2000, 01, 01, 0, 0);
        //     var date2001 = new NodaTime.LocalDateTime(2000, 01, 01, 0, 0);

        //     var recurrentBucket = new RecurrentExceptionsBucket();
        //     try
        //     {
        //         recurrentBucket.Add(date2000, 480, 960);
        //         recurrentBucket.Add(date2001, 0, 960);
        //         Assert.True(true, "Accepted conflited period.");
                
        //         var period = recurrentBucket.GetPeriod(date2000);
        //         Assert.Equal(0, period.Item1);
        //         Assert.Equal(960, period.Item2);
        //     }
        //     catch (Exception e)
        //     {
        //         // Assert.Equal("There is already an item to the date indicated in the list.", e.Message);
        //         Assert.Fail(e.Message);
        //     }
        // }

        // [Fact]
        // public void AddSolvingTwoPeriodConflitTest()
        // {
        //     var date2000 = new NodaTime.LocalDateTime(2000, 01, 01, 0, 0);
        //     var date2001 = new NodaTime.LocalDateTime(2000, 01, 01, 0, 0);

        //     var recurrentBucket = new RecurrentExceptionsBucket();
        //     try
        //     {
        //         recurrentBucket.Add(date2000, 480, 960);
        //         recurrentBucket.Add(date2001, 1000, 1100);
        //         Assert.True(true, "Accepted conflited period.");
                
        //         var period = recurrentBucket.GetPeriod(date2000);
        //         Assert.Equal(480, period.Item1);
        //         Assert.Equal(960, period.Item2);
                
        //         // var periods = recurrentBucket.GetPeriods(date2000);
        //         // Assert.Equal(480, period.Item1);
        //         // Assert.Equal(960, period.Item2);
        //     }
        //     catch (Exception e)
        //     {
        //         // Assert.Equal("There is already an item to the date indicated in the list.", e.Message);
        //         Assert.Fail(e.Message);
        //     }
        // }

        [Fact]
        public void HasTest()
        {
            var date2000 = new NodaTime.LocalDateTime(2000, 01, 01, 0, 0);
            var date2001 = new NodaTime.LocalDateTime(2000, 01, 01, 0, 0);

            var recurrentBucket = new RecurrentExceptionsBucket();
            recurrentBucket.Add(date2000, 480, 960);

            Assert.True(recurrentBucket.Has(date2000));
            Assert.True(recurrentBucket.Has(date2001));
            Assert.False(recurrentBucket.Has(new NodaTime.LocalDateTime(2005, 10, 05, 0, 0)));
        }

        [Fact]
        public void GetPeriodTest()
        {
            var date2000 = new NodaTime.LocalDateTime(2000, 01, 01, 0, 0);
            var date2001 = new NodaTime.LocalDateTime(2000, 01, 01, 0, 0);

            var recurrentBucket = new RecurrentExceptionsBucket();
            recurrentBucket.Add(date2000, 480, 960);

            Assert.Equal(new Tuple<short, short>(480, 960), recurrentBucket.GetPeriod(date2000));
            Assert.Equal(new Tuple<short, short>(480, 960), recurrentBucket.GetPeriod(date2001));
            try
            {
                recurrentBucket.GetPeriod(new NodaTime.LocalDateTime(2005, 10, 05, 0, 0));
                Assert.True(false, "Fail to get period.");
            }
            catch (KeyNotFoundException)
            {
                Assert.True(true);
            }
            catch (Exception e)
            {
                Assert.True(false, e.Message);
            }
        }
    }
}