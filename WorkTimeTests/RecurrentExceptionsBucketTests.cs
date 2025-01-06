using System;
using System.Collections.Generic;
using System.Linq;
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
                Assert.Fail("Fail to add recurrent exception.");
            }

            try
            {
                recurrentBucket.Add(date2001, 0, 960);
                Assert.Fail("Fail when accept invalid period.");
            }
            catch (Exception e)
            {
                Assert.Equal("There is already an item to the date indicated in the list.", e.Message);
            }
        }

        /// <summary>
        /// Em situações onde há um conflito na quebra, a recuperação irá trazer em períodos separados,
        /// mesmo que conflitando, a classe que calcula a diferença de horas agora deve considerar essa
        /// situação e processar todos os pedaços para chegar ao valor final.
        /// </summary>
        [Fact]
        public void AddSolvingSimpleConflictTest()
        {
            var date2000 = new NodaTime.LocalDateTime(2000, 01, 01, 0, 0);
            var date2001 = new NodaTime.LocalDateTime(2000, 01, 01, 0, 0);

            var recurrentBucket = new RecurrentExceptionsBucket();
            try
            {
                recurrentBucket.Add(date2000, 480, 960);
                recurrentBucket.Add(date2001, 0, 960);
                var period = recurrentBucket.GetPeriods(date2000);
                Assert.Equal(2, period.Count());
                Assert.Equal(480, period.First().start);
                Assert.Equal(960, period.First().end);
                Assert.Equal(0, period.Last().start);
                Assert.Equal(960, period.Last().end);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

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

            Assert.Equal(new List<(short, short)> { (480, 960) }, recurrentBucket.GetPeriods(date2000));
            Assert.Equal(new List<(short, short)> { (480, 960) }, recurrentBucket.GetPeriods(date2001));
            try
            {
                recurrentBucket.GetPeriods(new NodaTime.LocalDateTime(2005, 10, 05, 0, 0));
                Assert.Fail("Fail to get period.");
            }
            catch (KeyNotFoundException)
            {
                Assert.True(true);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}