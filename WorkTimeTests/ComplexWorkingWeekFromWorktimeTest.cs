using enki.libs.workhours;
using enki.libs.workhours.interfaces;
using NodaTime;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace WorkTimeTests
{
    public class ComplexWorkingWeekFromWorktimeTest
    {
        private class TestWorkingHour : IWorkingHour
        {
            public WeekDays StartDay { get; set; }
            public int StartHour { get; set; }
            public int StartMinute { get; set; }
            public WeekDays EndDay { get; set; }
            public int EndHour { get; set; }
            public int EndMinute { get; set; }
        }

        [Fact]
        public void GetWorkingWeekFromWorktime_WithNullInput_Returns24x7Week()
        {
            var result = ComplexWorkingWeek.GetWorkingWeekFromWorktime(null);

            Assert.NotNull(result);
            var complexWeek = result as ComplexWorkingWeek;
            Assert.NotNull(complexWeek);
            
            var periods = complexWeek.getPeriods();
            Assert.Equal(7, periods.Count);
            
            foreach (var period in periods)
            {
                Assert.Equal(0, period.startPeriod);
                Assert.Equal(1440, period.endPeriod);
            }
        }

        [Fact]
        public void GetWorkingWeekFromWorktime_WithEmptyList_Returns24x7Week()
        {
            var emptyList = new List<IWorkingHour>();
            var result = ComplexWorkingWeek.GetWorkingWeekFromWorktime(emptyList);

            Assert.NotNull(result);
            var complexWeek = result as ComplexWorkingWeek;
            Assert.NotNull(complexWeek);
            
            var periods = complexWeek.getPeriods();
            Assert.Equal(7, periods.Count);
        }

        [Fact]
        public void GetWorkingWeekFromWorktime_WithSingleDaySinglePeriod_ConvertsCorrectly()
        {
            var workTimes = new List<IWorkingHour>
            {
                new TestWorkingHour
                {
                    StartDay = WeekDays.Monday,
                    StartHour = 8,
                    StartMinute = 0,
                    EndDay = WeekDays.Monday,
                    EndHour = 17,
                    EndMinute = 0
                }
            };

            var result = ComplexWorkingWeek.GetWorkingWeekFromWorktime(workTimes);
            var complexWeek = result as ComplexWorkingWeek;
            
            Assert.NotNull(complexWeek);
            var periods = complexWeek.getPeriods();
            Assert.Single(periods);
            
            var period = periods.First();
            Assert.Equal(1, period.dayOfWeek);
            Assert.Equal(480, period.startPeriod); // 8 * 60
            Assert.Equal(1020, period.endPeriod); // 17 * 60
        }

        [Fact]
        public void GetWorkingWeekFromWorktime_WithMultipleDaysPeriod_ConvertsCorrectly()
        {
            var workTimes = new List<IWorkingHour>
            {
                new TestWorkingHour
                {
                    StartDay = WeekDays.Monday,
                    StartHour = 9,
                    StartMinute = 0,
                    EndDay = WeekDays.Friday,
                    EndHour = 18,
                    EndMinute = 0
                }
            };

            var result = ComplexWorkingWeek.GetWorkingWeekFromWorktime(workTimes);
            var complexWeek = result as ComplexWorkingWeek;
            
            Assert.NotNull(complexWeek);
            var periods = complexWeek.getPeriods();
            Assert.Equal(5, periods.Count);
            
            var monday = periods.First(p => p.dayOfWeek == (int)IsoDayOfWeek.Monday);
            Assert.Equal(540, monday.startPeriod); // 9 * 60
            Assert.Equal(1439, monday.endPeriod);
            
            var tuesday = periods.First(p => p.dayOfWeek == (int)IsoDayOfWeek.Tuesday);
            Assert.Equal(0, tuesday.startPeriod);
            Assert.Equal(1439, tuesday.endPeriod);
            
            var friday = periods.First(p => p.dayOfWeek == (int)IsoDayOfWeek.Friday);
            Assert.Equal(0, friday.startPeriod);
            Assert.Equal(1080, friday.endPeriod); // 18 * 60
        }

        [Fact]
        public void GetWorkingWeekFromWorktime_WithMultipleNonOverlappingPeriods_ConvertsAll()
        {
            var workTimes = new List<IWorkingHour>
            {
                new TestWorkingHour
                {
                    StartDay = WeekDays.Monday,
                    StartHour = 8,
                    StartMinute = 0,
                    EndDay = WeekDays.Monday,
                    EndHour = 12,
                    EndMinute = 0
                },
                new TestWorkingHour
                {
                    StartDay = WeekDays.Monday,
                    StartHour = 13,
                    StartMinute = 0,
                    EndDay = WeekDays.Monday,
                    EndHour = 17,
                    EndMinute = 0
                }
            };

            var result = ComplexWorkingWeek.GetWorkingWeekFromWorktime(workTimes);
            var complexWeek = result as ComplexWorkingWeek;
            
            Assert.NotNull(complexWeek);
            var periods = complexWeek.getPeriods();
            Assert.Equal(2, periods.Count);
            
            var firstPeriod = periods[0];
            Assert.Equal(480, firstPeriod.startPeriod); // 8 * 60
            Assert.Equal(720, firstPeriod.endPeriod); // 12 * 60
            
            var secondPeriod = periods[1];
            Assert.Equal(780, secondPeriod.startPeriod); // 13 * 60
            Assert.Equal(1020, secondPeriod.endPeriod); // 17 * 60
        }

        [Fact]
        public void GetWorkingWeekFromWorktime_WithDifferentDaysAndTimes_ConvertsCorrectly()
        {
            var workTimes = new List<IWorkingHour>
            {
                new TestWorkingHour
                {
                    StartDay = WeekDays.Monday,
                    StartHour = 9,
                    StartMinute = 30,
                    EndDay = WeekDays.Monday,
                    EndHour = 17,
                    EndMinute = 30
                },
                new TestWorkingHour
                {
                    StartDay = WeekDays.Wednesday,
                    StartHour = 10,
                    StartMinute = 15,
                    EndDay = WeekDays.Wednesday,
                    EndHour = 16,
                    EndMinute = 45
                }
            };

            var result = ComplexWorkingWeek.GetWorkingWeekFromWorktime(workTimes);
            var complexWeek = result as ComplexWorkingWeek;
            
            Assert.NotNull(complexWeek);
            var periods = complexWeek.getPeriods();
            Assert.Equal(2, periods.Count);
            
            var mondayPeriod = periods.First(p => p.dayOfWeek == (int)IsoDayOfWeek.Monday);
            Assert.Equal(570, mondayPeriod.startPeriod); // 9 * 60 + 30
            Assert.Equal(1050, mondayPeriod.endPeriod); // 17 * 60 + 30
            
            var wednesdayPeriod = periods.First(p => p.dayOfWeek == (int)IsoDayOfWeek.Wednesday);
            Assert.Equal(615, wednesdayPeriod.startPeriod); // 10 * 60 + 15
            Assert.Equal(1005, wednesdayPeriod.endPeriod); // 16 * 60 + 45
        }

        [Fact]
        public void GetWorkingWeekFromWorktime_WithFullWeekCoverage_ConvertsAllDays()
        {
            var workTimes = new List<IWorkingHour>
            {
                new TestWorkingHour
                {
                    StartDay = WeekDays.Monday,
                    StartHour = 8,
                    StartMinute = 0,
                    EndDay = WeekDays.Friday,
                    EndHour = 17,
                    EndMinute = 0
                },
                new TestWorkingHour
                {
                    StartDay = WeekDays.Saturday,
                    StartHour = 9,
                    StartMinute = 0,
                    EndDay = WeekDays.Saturday,
                    EndHour = 13,
                    EndMinute = 0
                }
            };

            var result = ComplexWorkingWeek.GetWorkingWeekFromWorktime(workTimes);
            var complexWeek = result as ComplexWorkingWeek;
            
            Assert.NotNull(complexWeek);
            var periods = complexWeek.getPeriods();
            Assert.Equal(6, periods.Count); // Monday to Saturday
            
            var saturdayPeriod = periods.First(p => p.dayOfWeek == (int)IsoDayOfWeek.Saturday);
            Assert.Equal(540, saturdayPeriod.startPeriod); // 9 * 60
            Assert.Equal(780, saturdayPeriod.endPeriod); // 13 * 60
        }

        [Fact]
        public void GetWorkingWeekFromWorktime_WithMidnightTimes_HandlesCorrectly()
        {
            var workTimes = new List<IWorkingHour>
            {
                new TestWorkingHour
                {
                    StartDay = WeekDays.Monday,
                    StartHour = 0,
                    StartMinute = 0,
                    EndDay = WeekDays.Monday,
                    EndHour = 23,
                    EndMinute = 59
                }
            };

            var result = ComplexWorkingWeek.GetWorkingWeekFromWorktime(workTimes);
            var complexWeek = result as ComplexWorkingWeek;
            
            Assert.NotNull(complexWeek);
            var periods = complexWeek.getPeriods();
            Assert.Single(periods);
            
            var period = periods.First();
            Assert.Equal(0, period.startPeriod);
            Assert.Equal(1439, period.endPeriod); // 23 * 60 + 59
        }

        [Fact]
        public void GetWorkingWeekFromWorktime_WithOverlappingPeriodsSameDay_CreatesMultiplePeriods()
        {
            var workTimes = new List<IWorkingHour>
            {
                new TestWorkingHour
                {
                    StartDay = WeekDays.Tuesday,
                    StartHour = 6,
                    StartMinute = 0,
                    EndDay = WeekDays.Tuesday,
                    EndHour = 14,
                    EndMinute = 0
                },
                new TestWorkingHour
                {
                    StartDay = WeekDays.Tuesday,
                    StartHour = 12,
                    StartMinute = 0,
                    EndDay = WeekDays.Tuesday,
                    EndHour = 20,
                    EndMinute = 0
                }
            };

            var result = ComplexWorkingWeek.GetWorkingWeekFromWorktime(workTimes);
            var complexWeek = result as ComplexWorkingWeek;
            
            Assert.NotNull(complexWeek);
            var periods = complexWeek.getPeriods();
            Assert.Equal(2, periods.Count);
            Assert.All(periods, p => Assert.Equal((int)IsoDayOfWeek.Tuesday, p.dayOfWeek));
        }

        [Fact]
        public void GetWorkingWeekFromWorktime_WithWeekendToWeekdayPeriod_SpansCorrectly()
        {
            var workTimes = new List<IWorkingHour>
            {
                new TestWorkingHour
                {
                    StartDay = WeekDays.Saturday,
                    StartHour = 20,
                    StartMinute = 0,
                    EndDay = WeekDays.Monday,
                    EndHour = 8,
                    EndMinute = 0
                }
            };

            var result = ComplexWorkingWeek.GetWorkingWeekFromWorktime(workTimes);
            var complexWeek = result as ComplexWorkingWeek;
            
            Assert.NotNull(complexWeek);
            var periods = complexWeek.getPeriods();
            Assert.Equal(3, periods.Count); // Saturday, Sunday, Monday
            
            var saturday = periods.First(p => p.dayOfWeek == (int)IsoDayOfWeek.Saturday);
            Assert.Equal(1200, saturday.startPeriod); // 20 * 60
            Assert.Equal(1439, saturday.endPeriod);
            
            var sunday = periods.First(p => p.dayOfWeek == (int)IsoDayOfWeek.Sunday);
            Assert.Equal(0, sunday.startPeriod);
            Assert.Equal(1439, sunday.endPeriod);
            
            var monday = periods.First(p => p.dayOfWeek == (int)IsoDayOfWeek.Monday);
            Assert.Equal(0, monday.startPeriod);
            Assert.Equal(480, monday.endPeriod); // 8 * 60
        }

        [Fact]
        public void GetWorkingWeekFromWorktime_WithReverseOrderSameDay_HandlesWeekSpan()
        {
            var workTimes = new List<IWorkingHour>
            {
                new TestWorkingHour
                {
                    StartDay = WeekDays.Wednesday,
                    StartHour = 22,
                    StartMinute = 0,
                    EndDay = WeekDays.Wednesday,
                    EndHour = 6,
                    EndMinute = 0
                }
            };

            var result = ComplexWorkingWeek.GetWorkingWeekFromWorktime(workTimes);
            var complexWeek = result as ComplexWorkingWeek;
            
            Assert.NotNull(complexWeek);
            var periods = complexWeek.getPeriods();
            Assert.Equal(8, periods.Count); // Full week wrap around
        }

        [Fact]
        public void GetWorkingWeekFromWorktime_WithAllSevenDays_CreatesCompleteWeek()
        {
            var workTimes = new List<IWorkingHour>
            {
                new TestWorkingHour { StartDay = WeekDays.Monday, StartHour = 9, StartMinute = 0, EndDay = WeekDays.Monday, EndHour = 17, EndMinute = 0 },
                new TestWorkingHour { StartDay = WeekDays.Tuesday, StartHour = 9, StartMinute = 0, EndDay = WeekDays.Tuesday, EndHour = 17, EndMinute = 0 },
                new TestWorkingHour { StartDay = WeekDays.Wednesday, StartHour = 9, StartMinute = 0, EndDay = WeekDays.Wednesday, EndHour = 17, EndMinute = 0 },
                new TestWorkingHour { StartDay = WeekDays.Thursday, StartHour = 9, StartMinute = 0, EndDay = WeekDays.Thursday, EndHour = 17, EndMinute = 0 },
                new TestWorkingHour { StartDay = WeekDays.Friday, StartHour = 9, StartMinute = 0, EndDay = WeekDays.Friday, EndHour = 17, EndMinute = 0 },
                new TestWorkingHour { StartDay = WeekDays.Saturday, StartHour = 10, StartMinute = 0, EndDay = WeekDays.Saturday, EndHour = 14, EndMinute = 0 },
                new TestWorkingHour { StartDay = WeekDays.Sunday, StartHour = 10, StartMinute = 0, EndDay = WeekDays.Sunday, EndHour = 14, EndMinute = 0 }
            };

            var result = ComplexWorkingWeek.GetWorkingWeekFromWorktime(workTimes);
            var complexWeek = result as ComplexWorkingWeek;
            
            Assert.NotNull(complexWeek);
            var periods = complexWeek.getPeriods();
            Assert.Equal(7, periods.Count);
            
            foreach (int day in Enumerable.Range(1, 7))
            {
                Assert.Contains(periods, p => p.dayOfWeek == day);
            }
        }

        [Fact]
        public void GetWorkingWeekFromWorktime_WithMinuteAccuracy_PreservesMinutes()
        {
            var workTimes = new List<IWorkingHour>
            {
                new TestWorkingHour
                {
                    StartDay = WeekDays.Thursday,
                    StartHour = 8,
                    StartMinute = 15,
                    EndDay = WeekDays.Thursday,
                    EndHour = 16,
                    EndMinute = 45
                }
            };

            var result = ComplexWorkingWeek.GetWorkingWeekFromWorktime(workTimes);
            var complexWeek = result as ComplexWorkingWeek;
            
            Assert.NotNull(complexWeek);
            var periods = complexWeek.getPeriods();
            Assert.Single(periods);
            
            var period = periods.First();
            Assert.Equal(495, period.startPeriod); // 8 * 60 + 15
            Assert.Equal(1005, period.endPeriod); // 16 * 60 + 45
        }

        [Fact]
        public void GetWorkingWeekFromWorktime_WithOverlappingAdjacentPeriods_ConsolidatesCorrectly()
        {
            // Input data from the specified table
            var workTimes = new List<IWorkingHour>
            {
                new TestWorkingHour { StartDay = WeekDays.Monday, StartHour = 11, StartMinute = 0, EndDay = WeekDays.Tuesday, EndHour = 0, EndMinute = 0 },
                new TestWorkingHour { StartDay = WeekDays.Tuesday, StartHour = 11, StartMinute = 0, EndDay = WeekDays.Wednesday, EndHour = 0, EndMinute = 0 },
                new TestWorkingHour { StartDay = WeekDays.Wednesday, StartHour = 11, StartMinute = 0, EndDay = WeekDays.Thursday, EndHour = 0, EndMinute = 0 },
                new TestWorkingHour { StartDay = WeekDays.Thursday, StartHour = 11, StartMinute = 0, EndDay = WeekDays.Friday, EndHour = 0, EndMinute = 0 },
                new TestWorkingHour { StartDay = WeekDays.Friday, StartHour = 11, StartMinute = 0, EndDay = WeekDays.Saturday, EndHour = 0, EndMinute = 0 },
                new TestWorkingHour { StartDay = WeekDays.Saturday, StartHour = 11, StartMinute = 0, EndDay = WeekDays.Saturday, EndHour = 15, EndMinute = 0 },
                new TestWorkingHour { StartDay = WeekDays.Sunday, StartHour = 11, StartMinute = 0, EndDay = WeekDays.Sunday, EndHour = 15, EndMinute = 0 }
            };

            var result = ComplexWorkingWeek.GetWorkingWeekFromWorktime(workTimes);
            var complexWeek = result as ComplexWorkingWeek;
            
            Assert.NotNull(complexWeek);
            var periods = complexWeek.getPeriods();
            
            // CRITICAL: Verify NO period has start == end
            var invalidPeriods = periods.Where(p => p.startPeriod == p.endPeriod).ToList();
            Assert.Empty(invalidPeriods); // No invalid periods should exist
            
            // Verify no duplicate periods for the same day with same start and end times
            var groupedByDay = periods.GroupBy(p => p.dayOfWeek);
            foreach (var dayGroup in groupedByDay)
            {
                var uniquePeriods = dayGroup.Select(p => new { p.startPeriod, p.endPeriod }).Distinct();
                Assert.Equal(dayGroup.Count(), uniquePeriods.Count());
            }
            
            // Expected result after filtering invalid periods:
            // Monday: 11:00 to 23:59 (end of day)
            var mondayPeriods = periods.Where(p => p.dayOfWeek == (int)IsoDayOfWeek.Monday).ToList();
            Assert.Single(mondayPeriods);
            Assert.Equal(660, mondayPeriods[0].startPeriod); // 11 * 60
            Assert.Equal(1439, mondayPeriods[0].endPeriod); // End of day
            
            // Tuesday: Only 11:00 to 23:59 (the 00:00-00:00 period should be filtered out)
            var tuesdayPeriods = periods.Where(p => p.dayOfWeek == (int)IsoDayOfWeek.Tuesday).ToList();
            Assert.Single(tuesdayPeriods);
            Assert.Equal(660, tuesdayPeriods[0].startPeriod); // 11 * 60
            Assert.Equal(1439, tuesdayPeriods[0].endPeriod); // End of day
            
            // Wednesday: Only 11:00 to 23:59
            var wednesdayPeriods = periods.Where(p => p.dayOfWeek == (int)IsoDayOfWeek.Wednesday).ToList();
            Assert.Single(wednesdayPeriods);
            Assert.Equal(660, wednesdayPeriods[0].startPeriod);
            Assert.Equal(1439, wednesdayPeriods[0].endPeriod);
            
            // Thursday: Only 11:00 to 23:59
            var thursdayPeriods = periods.Where(p => p.dayOfWeek == (int)IsoDayOfWeek.Thursday).ToList();
            Assert.Single(thursdayPeriods);
            Assert.Equal(660, thursdayPeriods[0].startPeriod);
            Assert.Equal(1439, thursdayPeriods[0].endPeriod);
            
            // Friday: Only 11:00 to 23:59
            var fridayPeriods = periods.Where(p => p.dayOfWeek == (int)IsoDayOfWeek.Friday).ToList();
            Assert.Single(fridayPeriods);
            Assert.Equal(660, fridayPeriods[0].startPeriod);
            Assert.Equal(1439, fridayPeriods[0].endPeriod);
            
            // Saturday: Only 11:00-15:00 (the 00:00-00:00 period should be filtered out)
            var saturdayPeriods = periods.Where(p => p.dayOfWeek == (int)IsoDayOfWeek.Saturday).ToList();
            Assert.Single(saturdayPeriods);
            Assert.Equal(660, saturdayPeriods[0].startPeriod); // 11 * 60
            Assert.Equal(900, saturdayPeriods[0].endPeriod); // 15 * 60
            
            // Sunday: Single period 11:00-15:00
            var sundayPeriods = periods.Where(p => p.dayOfWeek == (int)IsoDayOfWeek.Sunday).ToList();
            Assert.Single(sundayPeriods);
            Assert.Equal(660, sundayPeriods[0].startPeriod); // 11 * 60
            Assert.Equal(900, sundayPeriods[0].endPeriod); // 15 * 60
            
            // Verify total: 7 periods (one per day, all valid)
            Assert.Equal(7, periods.Count);
        }
    }
}
