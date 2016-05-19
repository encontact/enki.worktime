using enki.libs.workhours;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using System;

namespace enki.tests.libs.date
{
    public class DateUtilsTest
    {
        private int[] MJD =
                { 7527, 21320, 90166, 58651, 92987, 77267, 30485, 24416, 93588, 41670, 84093, 17165, 85118, 62129, 16703, 22317, 35561, 21083,
                        32414, 16949, 87038, 59538, 90480, 64542, 61394, 97793, 70940, 1980, 7128, 99359, 40473, 24413, 54116, 87890, 60791,
                        82099, 86309, 74298, 4947, 42719, 78960, 33879, 49946, 40208, 19660, 38352, 83873, 40042, 13218, 52968, 33931, 65378,
                        20369, 93566, 44214, 34707, 61728, 76283, 73416, 28888, 59562, 98860, 64552, 93652, 75176, 82609, 75069, 98011, 72639,
                        71850, 97056, 96222, 90756, 21265, 50032, 77701, 76950, 15475, 49777, 28443, 93736, 67404, 58088, 78592, 88170, 23266,
                        71555, 12728, 68421, 59213, 17736, 80842, 67831, 49034, 31374, 63124, 46988, 25006, 25134, 60934 };

        private LocalDateTime[] DATES =
                { new LocalDateTime(1879, 6, 27, 0, 0, 0), new LocalDateTime(1917, 4, 2, 0, 0, 0), new LocalDateTime(2105, 9, 29, 0, 0, 0), new LocalDateTime(2019, 6, 17, 0, 0, 0),
                        new LocalDateTime(2113, 6, 20, 0, 0, 0), new LocalDateTime(2070, 6, 5, 0, 0, 0), new LocalDateTime(1942, 5, 6, 0, 0, 0), new LocalDateTime(1925, 9, 23, 0, 0, 0),
                        new LocalDateTime(2115, 2, 11, 0, 0, 0), new LocalDateTime(1972, 12, 19, 0, 0, 0), new LocalDateTime(2089, 2, 11, 0, 0, 0), new LocalDateTime(1905, 11, 16, 0, 0, 0),
                        new LocalDateTime(2091, 12, 3, 0, 0, 0), new LocalDateTime(2028, 12, 24, 0, 0, 0), new LocalDateTime(1904, 8, 11, 0, 0, 0), new LocalDateTime(1919, 12, 25, 0, 0, 0),
                        new LocalDateTime(1956, 3, 29, 0, 0, 0), new LocalDateTime(1916, 8, 8, 0, 0, 0), new LocalDateTime(1947, 8, 17, 0, 0, 0), new LocalDateTime(1905, 4, 14, 0, 0, 0),
                        new LocalDateTime(2097, 3, 6, 0, 0, 0), new LocalDateTime(2021, 11, 20, 0, 0, 0), new LocalDateTime(2106, 8, 9, 0, 0, 0), new LocalDateTime(2035, 8, 3, 0, 0, 0),
                        new LocalDateTime(2026, 12, 20, 0, 0, 0), new LocalDateTime(2126, 8, 17, 0, 0, 0), new LocalDateTime(2053, 2, 7, 0, 0, 0), new LocalDateTime(1864, 4, 19, 0, 0, 0),
                        new LocalDateTime(1878, 5, 24, 0, 0, 0), new LocalDateTime(2130, 11, 30, 0, 0, 0), new LocalDateTime(1969, 9, 9, 0, 0, 0), new LocalDateTime(1925, 9, 20, 0, 0, 0),
                        new LocalDateTime(2007, 1, 16, 0, 0, 0), new LocalDateTime(2099, 7, 6, 0, 0, 0), new LocalDateTime(2025, 4, 26, 0, 0, 0), new LocalDateTime(2083, 8, 28, 0, 0, 0),
                        new LocalDateTime(2095, 3, 8, 0, 0, 0), new LocalDateTime(2062, 4, 19, 0, 0, 0), new LocalDateTime(1872, 6, 3, 0, 0, 0), new LocalDateTime(1975, 11, 3, 0, 0, 0),
                        new LocalDateTime(2075, 1, 23, 0, 0, 0), new LocalDateTime(1951, 8, 21, 0, 0, 0), new LocalDateTime(1995, 8, 17, 0, 0, 0), new LocalDateTime(1968, 12, 18, 0, 0, 0),
                        new LocalDateTime(1912, 9, 15, 0, 0, 0), new LocalDateTime(1963, 11, 19, 0, 0, 0), new LocalDateTime(2088, 7, 6, 0, 0, 0), new LocalDateTime(1968, 7, 5, 0, 0, 0),
                        new LocalDateTime(1895, 1, 25, 0, 0, 0), new LocalDateTime(2003, 11, 25, 0, 0, 0), new LocalDateTime(1951, 10, 12, 0, 0, 0), new LocalDateTime(2037, 11, 16, 0, 0, 0),
                        new LocalDateTime(1914, 8, 25, 0, 0, 0), new LocalDateTime(2115, 1, 20, 0, 0, 0), new LocalDateTime(1979, 12, 7, 0, 0, 0), new LocalDateTime(1953, 11, 26, 0, 0, 0),
                        new LocalDateTime(2027, 11, 19, 0, 0, 0), new LocalDateTime(2067, 9, 25, 0, 0, 0), new LocalDateTime(2059, 11, 19, 0, 0, 0), new LocalDateTime(1937, 12, 21, 0, 0, 0),
                        new LocalDateTime(2021, 12, 14, 0, 0, 0), new LocalDateTime(2129, 7, 19, 0, 0, 0), new LocalDateTime(2035, 8, 13, 0, 0, 0), new LocalDateTime(2115, 4, 16, 0, 0, 0),
                        new LocalDateTime(2064, 9, 13, 0, 0, 0), new LocalDateTime(2085, 1, 19, 0, 0, 0), new LocalDateTime(2064, 5, 29, 0, 0, 0), new LocalDateTime(2127, 3, 23, 0, 0, 0),
                        new LocalDateTime(2057, 10, 3, 0, 0, 0), new LocalDateTime(2055, 8, 6, 0, 0, 0), new LocalDateTime(2124, 8, 10, 0, 0, 0), new LocalDateTime(2122, 4, 29, 0, 0, 0),
                        new LocalDateTime(2107, 5, 12, 0, 0, 0), new LocalDateTime(1917, 2, 6, 0, 0, 0), new LocalDateTime(1995, 11, 11, 0, 0, 0), new LocalDateTime(2071, 8, 13, 0, 0, 0),
                        new LocalDateTime(2069, 7, 23, 0, 0, 0), new LocalDateTime(1901, 4, 1, 0, 0, 0), new LocalDateTime(1995, 3, 1, 0, 0, 0), new LocalDateTime(1936, 10, 2, 0, 0, 0),
                        new LocalDateTime(2115, 7, 9, 0, 0, 0), new LocalDateTime(2043, 6, 4, 0, 0, 0), new LocalDateTime(2017, 12, 1, 0, 0, 0), new LocalDateTime(2074, 1, 20, 0, 0, 0),
                        new LocalDateTime(2100, 4, 12, 0, 0, 0), new LocalDateTime(1922, 7, 31, 0, 0, 0), new LocalDateTime(2054, 10, 15, 0, 0, 0), new LocalDateTime(1893, 9, 22, 0, 0, 0),
                        new LocalDateTime(2046, 3, 17, 0, 0, 0), new LocalDateTime(2020, 12, 30, 0, 0, 0), new LocalDateTime(1907, 6, 10, 0, 0, 0), new LocalDateTime(2080, 3, 19, 0, 0, 0),
                        new LocalDateTime(2044, 8, 4, 0, 0, 0), new LocalDateTime(1993, 2, 16, 0, 0, 0), new LocalDateTime(1944, 10, 11, 0, 0, 0), new LocalDateTime(2031, 9, 15, 0, 0, 0),
                        new LocalDateTime(1987, 7, 12, 0, 0, 0), new LocalDateTime(1927, 5, 6, 0, 0, 0), new LocalDateTime(1927, 9, 11, 0, 0, 0), new LocalDateTime(2025, 9, 16, 0, 0, 0) };

        [TestMethod]
        public void testConversionToMJD()
        {
            for (int i = 0; i < this.MJD.Length; i++)
            {
                Assert.AreEqual(this.MJD[i], DateUtils.toMJD(this.DATES[i]));
            }
        }

        [TestMethod]
        public void testConversionFromMJD()
        {
            for (int i = 0; i < this.MJD.Length; i++)
            {
                Assert.AreEqual(this.DATES[i], DateUtils.fromMJD(this.MJD[i]));
            }
        }

        [TestMethod]
        public void testMJD()
        {
            for (int i = 0; i < 100000; i += 73)
            {
                Assert.AreEqual(i, DateUtils.toMJD(DateUtils.fromMJD(i)));
            }
        }

        [TestMethod]
        public void testPeriod()
        {
            LocalTime MIDNIGHT = new LocalTime(0, 0, 0);
            LocalTime EIGHT_MORNING = new LocalTime(8, 0, 0);

            long minutes = Period.Between(MIDNIGHT, EIGHT_MORNING, PeriodUnits.Minutes).Minutes;

            Assert.AreEqual(480, minutes);
        }

        [TestMethod]
        public void testLocalDateTimeParse()
        {
            LocalDateTime correctValue = new LocalDateTime(2001, 01, 01, 15, 12, CalendarSystem.Iso);
            LocalDateTime wrongValue = LocalDateTime.Parse(new DateTime(2001, 1, 1, 15, 12, 00).ToString());
            Assert.AreEqual(correctValue, wrongValue);
        }
    }
}