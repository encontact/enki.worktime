using NodaTime;
using System;

namespace enki.libs.workhours
{
    /// <summary>
    /// Classe utilitaria para apoio ao cálculo de horas úteis
    /// </summary>
    public class DateUtils
    {
        private static readonly double M_0_25 = 0.25;

        private static readonly double M_0_5 = 0.5;

        private static readonly int M_10 = 10;

        private static readonly int M_100 = 100;

        private static readonly double M_122_1 = 122.1;

        private static readonly int M_15 = 15;

        private static readonly int M_1524 = 1524;

        private static readonly int M_153 = 153;

        private static readonly int M_1582 = 1582;

        private static readonly int M_1867216 = 1867216;

        private static readonly int M_2439870 = 2439870;

        private static readonly int M_3 = 3;

        private static readonly double M_30_6001 = 30.6001;

        private static readonly int M_31 = 31;

        private static readonly int M_32045 = 32045;

        private static readonly double M_36524_25 = 36524.25;

        private static readonly int M_4 = 4;

        private static readonly int M_400 = 400;

        private static readonly int M_4715 = 4715;

        private static readonly int M_4800 = 4800;

        private static readonly int M_5 = 5;

        private static readonly double M_6680_0 = 6680.0;

        private static readonly int DAYS_PER_YEAR = 365;

        private static readonly int HALF_A_DAY = 12;

        private static readonly int HOURS_PER_DAY = 24;

        private static readonly double JULIAN_YEAR_LEN = 365.25;

        private static readonly int MINUTES_PER_DAY = 1440;

        private static readonly double MODIFIED_JULIAN_DATE_OFFSET = 2400000.5;

        private static readonly int MONTH_OFFSET = 14;

        private static readonly int MONTHS_PER_YEAR = 12;

        private static readonly int SECONDS_PER_DAY = 86400;

        private static readonly int JGREG = M_15 + M_31 * (M_10 + MONTHS_PER_YEAR * M_1582);

        /// <summary>
        /// Construtor privado da classe
        /// </summary>
        private DateUtils()
        {
        }

        /// <summary>
        /// Converte um dia Juliano para uma data normal.
        /// </summary>
        /// <param name="julianDate">Dia Juliano</param>
        /// <returns>LocalDateTime referente a data convertida</returns>
        public static LocalDateTime fromMJD(int julianDate)
        {
            int ja = (int)(julianDate + MODIFIED_JULIAN_DATE_OFFSET + M_0_5);
            if (ja >= JGREG)
            {
                int jalpha = (int)((ja - M_1867216 - M_0_25) / M_36524_25);
                ja = ja + 1 + jalpha - jalpha / M_4;
            }

            int jb = ja + M_1524;
            int jc = (int)(M_6680_0 + (jb - M_2439870 - M_122_1) / JULIAN_YEAR_LEN);
            int jd = DAYS_PER_YEAR * jc + jc / M_4;
            int je = (int)((jb - jd) / M_30_6001);
            int day = jb - jd - (int)(M_30_6001 * je);
            int month = je - 1;
            if (month > MONTHS_PER_YEAR)
            {
                month = month - MONTHS_PER_YEAR;
            }
            int year = jc - M_4715;
            if (month > 2)
            {
                year--;
            }
            if (year <= 0)
            {
                year--;
            }

            return new LocalDateTime(year, month, day, 0, 0, 0);
        }

        /// <summary>
        /// Calcula o MJD, Modified Julian Date. Ver <a href="http://en.wikipedia.org/wiki/Modified_Julian_Date#Alternatives">http://en.wikipedia.org/wiki/Modified_Julian_Date#Alternatives</a>
        /// </summary>
        /// <param name="year">Ano</param>
        /// <param name="month">Mês</param>
        /// <param name="day">Dia</param>
        /// <param name="hour">Hora</param>
        /// <param name="minute">Minuto</param>
        /// <param name="second">Segundo</param>
        /// <returns>Data convertida em formato Juliano (Midified Julian Date)</returns>
        private static int toMJD(int year, int month, int day, int hour, int minute, int second)
        {
            int a = (MONTH_OFFSET - month) / MONTHS_PER_YEAR;
            int y = year + M_4800 - a;
            int m = month + MONTHS_PER_YEAR * a - M_3;

            int jdn = day + (M_153 * m + 2) / M_5 + DAYS_PER_YEAR * y + y / M_4 - y / M_100 + y / M_400 - M_32045;
            int jd = jdn + (hour - HALF_A_DAY) / HOURS_PER_DAY + minute / MINUTES_PER_DAY + second / SECONDS_PER_DAY;

            int result = (int)(jd - MODIFIED_JULIAN_DATE_OFFSET);

            return result;
        }

        /// <summary>
        /// Calcula o MJD, Modified Julian Date. Ver <a href="http://en.wikipedia.org/wiki/Modified_Julian_Date#Alternatives">http://en.wikipedia.org/wiki/Modified_Julian_Date#Alternatives</a>
        /// </summary>
        /// <param name="date">Data no formato NodaTime.LocalDateTime</param>
        /// <returns>Data convertida em formato Juliano (Midified Julian Date)</returns>
        public static int toMJD(LocalDateTime date)
        {
            int day = date.Day;
            int month = date.Month;
            int year = date.Year;

            return toMJD(year, month, day, 0, 0, 0);
        }

        /// <summary>
        /// Calcula o MJD, Modified Julian Date. Ver <a href="http://en.wikipedia.org/wiki/Modified_Julian_Date#Alternatives">
        /// http://en.wikipedia.org/wiki/Modified_Julian_Date#Alternatives</a>
        /// </summary>
        /// <param name="date">Data</param>
        /// <returns>Midified Julian Date</returns>
        public static int toMJD(DateTime date)
        {
            int day = date.Day;
            int month = date.Month;
            int year = date.Year;

            return toMJD(year, month, day, 0, 0, 0);
        }
    }
}