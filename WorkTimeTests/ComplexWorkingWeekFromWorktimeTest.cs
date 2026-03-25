using enki.libs.workhours;
using enki.libs.workhours.interfaces;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace WorkTimeTests
{
    public class ComplexWorkingWeekFromWorktimeTest
    {
        private class WorkingHour : IWorkingHour
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
                new WorkingHour
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
                new WorkingHour
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
                new WorkingHour
                {
                    StartDay = WeekDays.Monday,
                    StartHour = 8,
                    StartMinute = 0,
                    EndDay = WeekDays.Monday,
                    EndHour = 12,
                    EndMinute = 0
                },
                new WorkingHour
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
                new WorkingHour
                {
                    StartDay = WeekDays.Monday,
                    StartHour = 9,
                    StartMinute = 30,
                    EndDay = WeekDays.Monday,
                    EndHour = 17,
                    EndMinute = 30
                },
                new WorkingHour
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
                new WorkingHour
                {
                    StartDay = WeekDays.Monday,
                    StartHour = 8,
                    StartMinute = 0,
                    EndDay = WeekDays.Friday,
                    EndHour = 17,
                    EndMinute = 0
                },
                new WorkingHour
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
                new WorkingHour
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
                new WorkingHour
                {
                    StartDay = WeekDays.Tuesday,
                    StartHour = 6,
                    StartMinute = 0,
                    EndDay = WeekDays.Tuesday,
                    EndHour = 14,
                    EndMinute = 0
                },
                new WorkingHour
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
                new WorkingHour
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
                new WorkingHour
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
                new WorkingHour { StartDay = WeekDays.Monday, StartHour = 9, StartMinute = 0, EndDay = WeekDays.Monday, EndHour = 17, EndMinute = 0 },
                new WorkingHour { StartDay = WeekDays.Tuesday, StartHour = 9, StartMinute = 0, EndDay = WeekDays.Tuesday, EndHour = 17, EndMinute = 0 },
                new WorkingHour { StartDay = WeekDays.Wednesday, StartHour = 9, StartMinute = 0, EndDay = WeekDays.Wednesday, EndHour = 17, EndMinute = 0 },
                new WorkingHour { StartDay = WeekDays.Thursday, StartHour = 9, StartMinute = 0, EndDay = WeekDays.Thursday, EndHour = 17, EndMinute = 0 },
                new WorkingHour { StartDay = WeekDays.Friday, StartHour = 9, StartMinute = 0, EndDay = WeekDays.Friday, EndHour = 17, EndMinute = 0 },
                new WorkingHour { StartDay = WeekDays.Saturday, StartHour = 10, StartMinute = 0, EndDay = WeekDays.Saturday, EndHour = 14, EndMinute = 0 },
                new WorkingHour { StartDay = WeekDays.Sunday, StartHour = 10, StartMinute = 0, EndDay = WeekDays.Sunday, EndHour = 14, EndMinute = 0 }
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
                new WorkingHour
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
                new WorkingHour { StartDay = WeekDays.Monday, StartHour = 11, StartMinute = 0, EndDay = WeekDays.Tuesday, EndHour = 0, EndMinute = 0 },
                new WorkingHour { StartDay = WeekDays.Tuesday, StartHour = 11, StartMinute = 0, EndDay = WeekDays.Wednesday, EndHour = 0, EndMinute = 0 },
                new WorkingHour { StartDay = WeekDays.Wednesday, StartHour = 11, StartMinute = 0, EndDay = WeekDays.Thursday, EndHour = 0, EndMinute = 0 },
                new WorkingHour { StartDay = WeekDays.Thursday, StartHour = 11, StartMinute = 0, EndDay = WeekDays.Friday, EndHour = 0, EndMinute = 0 },
                new WorkingHour { StartDay = WeekDays.Friday, StartHour = 11, StartMinute = 0, EndDay = WeekDays.Saturday, EndHour = 0, EndMinute = 0 },
                new WorkingHour { StartDay = WeekDays.Saturday, StartHour = 11, StartMinute = 0, EndDay = WeekDays.Saturday, EndHour = 15, EndMinute = 0 },
                new WorkingHour { StartDay = WeekDays.Sunday, StartHour = 11, StartMinute = 0, EndDay = WeekDays.Sunday, EndHour = 15, EndMinute = 0 }
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

        [Fact]
        public void AddWorkingHours_WithMarchBugDate_ValidatesCorrectWorkingWeek()
        {
            var workTimes = new List<IWorkingHour>
            {
                new WorkingHour { StartDay = WeekDays.Monday, StartHour = 10, StartMinute = 0, EndDay = WeekDays.Tuesday, EndHour = 2, EndMinute = 59 },
                new WorkingHour { StartDay = WeekDays.Tuesday, StartHour = 10, StartMinute = 0, EndDay = WeekDays.Wednesday, EndHour = 8, EndMinute = 12 },
                new WorkingHour { StartDay = WeekDays.Wednesday, StartHour = 10, StartMinute = 0, EndDay = WeekDays.Thursday, EndHour = 8, EndMinute = 12 },
                new WorkingHour { StartDay = WeekDays.Thursday, StartHour = 10, StartMinute = 0, EndDay = WeekDays.Friday, EndHour = 8, EndMinute = 12 },
                new WorkingHour { StartDay = WeekDays.Friday, StartHour = 10, StartMinute = 0, EndDay = WeekDays.Saturday, EndHour = 8, EndMinute = 12 },
                new WorkingHour { StartDay = WeekDays.Saturday, StartHour = 11, StartMinute = 0, EndDay = WeekDays.Sunday, EndHour = 0, EndMinute = 0 },
                new WorkingHour { StartDay = WeekDays.Sunday, StartHour = 11, StartMinute = 0, EndDay = WeekDays.Monday, EndHour = 0, EndMinute = 0 },
                new WorkingHour { StartDay = WeekDays.Tuesday, StartHour = 3, StartMinute = 0, EndDay = WeekDays.Tuesday, EndHour = 8, EndMinute = 12 }
            };

            var workingWeek = ComplexWorkingWeek.GetWorkingWeekFromWorktime(workTimes);

            var startPeriod = DateTime.UtcNow;
            var endPeriod = DateTime.UtcNow;
            var startDate = new LocalDateTime(startPeriod.Year, startPeriod.Month, 23, 20, 0, 0);
            var endDate = new LocalDateTime(endPeriod.Year, endPeriod.Month, 23, 20, 59, 59);

            var workingTable = new WorkingHoursTable(
                workingWeek,
                new List<WorkingDaySlice>(),
                new SortedSet<WorkingDaySlice>(),
                startDate,
                endDate
            );

            var date = new DateTime(2026, 3, 23, 20, 15, 0);

            var hours = 0;
            var minutes = 40;
            var result = workingTable.addWorkingHours(date, hours, minutes);
            var expectedResult = new DateTime(2026, 3, 23, 20, 55, 0);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void AddWorkingHours_WithTwoPeriodsInDay_AddsHoursAcrossPeriods()
        {
            // Two non-overlapping periods: Monday 8:00-12:00 and Monday 14:00-18:00
            var workTimes = new List<IWorkingHour>
            {
                new WorkingHour { StartDay = WeekDays.Monday, StartHour = 8, StartMinute = 0, EndDay = WeekDays.Monday, EndHour = 12, EndMinute = 0 },
                new WorkingHour { StartDay = WeekDays.Monday, StartHour = 14, StartMinute = 0, EndDay = WeekDays.Monday, EndHour = 18, EndMinute = 0 }
            };

            var workingWeek = ComplexWorkingWeek.GetWorkingWeekFromWorktime(workTimes);

            var startPeriod = DateTime.UtcNow;
            var endPeriod = DateTime.UtcNow;
            var startDate = new LocalDateTime(startPeriod.Year, startPeriod.Month, 23, 8, 0, 0);
            var endDate = new LocalDateTime(endPeriod.Year, endPeriod.Month, 23, 18, 0, 0);

            var workingTable = new WorkingHoursTable(
                workingWeek,
                new List<WorkingDaySlice>(),
                new SortedSet<WorkingDaySlice>(),
                startDate,
                endDate
            );

            // Starting at 11:00 AM on Monday (March 23, 2026), add 3 hours
            // Should skip lunch break (12:00-14:00) and end at 16:00
            var date = new DateTime(2026, 3, 23, 11, 0, 0);
            var hours = 3;
            var minutes = 0;
            var result = workingTable.addWorkingHours(date, hours, minutes);
            var expectedResult = new DateTime(2026, 3, 23, 16, 0, 0);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void AddWorkingHours_WithNightShift_CalculatesCorrectly()
        {
            var workTimes = new List<IWorkingHour>
            {
                new WorkingHour { StartDay = WeekDays.Tuesday, StartHour = 20, StartMinute = 0, EndDay = WeekDays.Wednesday, EndHour = 4, EndMinute = 0 }
            };

            var workingWeek = ComplexWorkingWeek.GetWorkingWeekFromWorktime(workTimes);

            var startPeriod = DateTime.UtcNow;
            var endPeriod = DateTime.UtcNow;
            var startDate = new LocalDateTime(startPeriod.Year, startPeriod.Month, 24, 20, 0, 0);
            var endDate = new LocalDateTime(endPeriod.Year, endPeriod.Month, 25, 4, 0, 0);

            var workingTable = new WorkingHoursTable(
                workingWeek,
                new List<WorkingDaySlice>(),
                new SortedSet<WorkingDaySlice>(),
                startDate,
                endDate
            );

            // Starting at 22:30 on Tuesday (March 24, 2026), add 5 hours 15 minutes
            var date = new DateTime(2026, 3, 24, 22, 30, 0);
            var hours = 5;
            var minutes = 15;
            var result = workingTable.addWorkingHours(date, hours, minutes);
            var expectedResult = new DateTime(2026, 3, 25, 3, 46, 0);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void AddWorkingHours_WithMultiplePeriodsInDay_AddsMinutesCorrectly()
        {
            // Three periods on Wednesday: 6:00-10:00, 11:00-15:00, 16:00-20:00
            var workTimes = new List<IWorkingHour>
            {
                new WorkingHour { StartDay = WeekDays.Wednesday, StartHour = 6, StartMinute = 0, EndDay = WeekDays.Wednesday, EndHour = 10, EndMinute = 0 },
                new WorkingHour { StartDay = WeekDays.Wednesday, StartHour = 11, StartMinute = 0, EndDay = WeekDays.Wednesday, EndHour = 15, EndMinute = 0 },
                new WorkingHour { StartDay = WeekDays.Wednesday, StartHour = 16, StartMinute = 0, EndDay = WeekDays.Wednesday, EndHour = 20, EndMinute = 0 }
            };

            var workingWeek = ComplexWorkingWeek.GetWorkingWeekFromWorktime(workTimes);

            var startPeriod = DateTime.UtcNow;
            var endPeriod = DateTime.UtcNow;
            var startDate = new LocalDateTime(startPeriod.Year, startPeriod.Month, 25, 6, 0, 0);
            var endDate = new LocalDateTime(endPeriod.Year, endPeriod.Month, 25, 20, 0, 0);

            var workingTable = new WorkingHoursTable(
                workingWeek,
                new List<WorkingDaySlice>(),
                new SortedSet<WorkingDaySlice>(),
                startDate,
                endDate
            );

            // Starting at 9:30 AM on Wednesday (March 25, 2026), add 6 hours 45 minutes
            // Should skip breaks and end at 18:15
            var date = new DateTime(2026, 3, 25, 9, 30, 0);
            var hours = 6;
            var minutes = 45;
            var result = workingTable.addWorkingHours(date, hours, minutes);
            var expectedResult = new DateTime(2026, 3, 25, 18, 15, 0);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void AddWorkingHours_WithExtendedShift_SpansMultipleDays()
        {
            // Long shift: Thursday 18:00 to Friday 06:00
            var workTimes = new List<IWorkingHour>
            {
                new WorkingHour { StartDay = WeekDays.Thursday, StartHour = 18, StartMinute = 0, EndDay = WeekDays.Friday, EndHour = 6, EndMinute = 0 }
            };

            var workingWeek = ComplexWorkingWeek.GetWorkingWeekFromWorktime(workTimes);

            var startPeriod = DateTime.UtcNow;
            var endPeriod = DateTime.UtcNow;
            var startDate = new LocalDateTime(startPeriod.Year, startPeriod.Month, 26, 18, 0, 0);
            var endDate = new LocalDateTime(endPeriod.Year, endPeriod.Month, 27, 6, 0, 0);

            var workingTable = new WorkingHoursTable(
                workingWeek,
                new List<WorkingDaySlice>(),
                new SortedSet<WorkingDaySlice>(),
                startDate,
                endDate
            );

            // Starting at 19:00 on Thursday (March 26, 2026), add 8 hours 30 minutes
            var date = new DateTime(2026, 3, 26, 19, 0, 0);
            var hours = 8;
            var minutes = 30;
            var result = workingTable.addWorkingHours(date, hours, minutes);
            var expectedResult = new DateTime(2026, 3, 27, 3, 31, 0);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void AddWorkingHours_WithSplitShift_HandlesGapsCorrectly()
        {
            // Split shift on Friday: 7:00-11:30 and 13:30-18:00
            var workTimes = new List<IWorkingHour>
            {
                new WorkingHour { StartDay = WeekDays.Friday, StartHour = 7, StartMinute = 0, EndDay = WeekDays.Friday, EndHour = 11, EndMinute = 30 },
                new WorkingHour { StartDay = WeekDays.Friday, StartHour = 13, StartMinute = 30, EndDay = WeekDays.Friday, EndHour = 18, EndMinute = 0 }
            };

            var workingWeek = ComplexWorkingWeek.GetWorkingWeekFromWorktime(workTimes);

            var startPeriod = DateTime.UtcNow;
            var endPeriod = DateTime.UtcNow;
            var startDate = new LocalDateTime(startPeriod.Year, startPeriod.Month, 27, 7, 0, 0);
            var endDate = new LocalDateTime(endPeriod.Year, endPeriod.Month, 27, 18, 0, 0);

            var workingTable = new WorkingHoursTable(
                workingWeek,
                new List<WorkingDaySlice>(),
                new SortedSet<WorkingDaySlice>(),
                startDate,
                endDate
            );

            // Starting at 10:45 on Friday (March 27, 2026), add 2 hours 15 minutes
            // Should skip lunch break (11:30-13:30) and end at 15:00
            var date = new DateTime(2026, 3, 27, 10, 45, 0);
            var hours = 2;
            var minutes = 15;
            var result = workingTable.addWorkingHours(date, hours, minutes);
            var expectedResult = new DateTime(2026, 3, 27, 15, 0, 0);
            Assert.Equal(expectedResult, result);
        }
    }
}
