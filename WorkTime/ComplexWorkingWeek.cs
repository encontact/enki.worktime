using System.Collections.Generic;
using NodaTime;
using enki.libs.workhours.domain;
using System;

namespace enki.libs.workhours
{
    /// <summary>
    /// Calendário de trabalho semanal, armazena as horas de início e término de trabalho para cada dia da semana.
    /// </summary>
    public class ComplexWorkingWeek : WorkingWeek
    {

        private static readonly LocalTime HOURS_235959 = new LocalTime(23, 59, 59, 999);

        private static readonly LocalTime HOURS_18 = new LocalTime(18, 0, 0);

        private static readonly LocalTime HOURS_16 = new LocalTime(16, 0, 0);

        private static readonly LocalTime HOURS_13 = new LocalTime(13, 0, 0);

        private static readonly LocalTime HOURS_12 = new LocalTime(12, 0, 0);

        private static readonly LocalTime HOURS_9 = new LocalTime(9, 0, 0);

        private static readonly LocalTime HOURS_8 = new LocalTime(8, 0, 0);

        private static readonly LocalTime MIDNIGHT = new LocalTime(0, 0, 0);

        private List<WorkingPeriod> dayPeriods { get; set; }


        /// <summary>
        /// Recupera a lista de periodos de um dia da semana especifico
        /// </summary>
        /// <param name="dayOfWeek">Dia da semana.</param>
        /// <returns>Número referente ao horário de inicio da semana de trabalho.</returns>
        public List<WorkingPeriod> getPeriods(int dayOfWeek)
        {
            return dayPeriods.FindAll(period => period.dayOfWeek == dayOfWeek);
        }

        /// <summary>
        /// Recupera o tempo útil na semana.
        /// </summary>
        /// <returns>Tempo útil na semana.</returns>
        public TimeSpan GetWeekTime()
        {
            var time = new TimeSpan();

            foreach(var period in dayPeriods)
            {
                var diff = period.endPeriod - period.startPeriod;

                switch (period.Type)
                {
                    case PeriodType.Hour:
                        time = time.Add(TimeSpan.FromHours(diff));
                        break;
                    case PeriodType.Minute:
                        time = time.Add(TimeSpan.FromMinutes(diff));
                        break;
                    case PeriodType.Second:
                        time = time.Add(TimeSpan.FromSeconds(diff));
                        break;
                }
            }

            return time;
        }

        /// <summary>
        /// Define os horários de inicio e término do trabalho para um periodo do dia da semana informado.
        /// </summary>
        /// <param name="dayOfWeek">Dia da semana, definido em "DateTimeConstants"</param>
        /// <param name="start">Horario de inicio em NodaTime.LocalTime</param>
        /// <param name="end">Horario de término em NodaTime.LocalTime</param>
        public void setWorkDay(int dayOfWeek, LocalTime start, LocalTime end)
        {
            if (dayOfWeek == (int)IsoDayOfWeek.None)
                throw new ArgumentException($"The value {dayOfWeek} is not a valid day of week.");

            dayPeriods = dayPeriods == null ? new List<WorkingPeriod>() : dayPeriods;
            dayPeriods.Add(new WorkingPeriod(dayOfWeek,
                                             (short)Period.Between(MIDNIGHT, start, PeriodUnits.Minutes).Minutes,
                                             (short)Period.Between(MIDNIGHT, end, PeriodUnits.Minutes).Minutes));
        }

        /// <summary>
        /// Recupera uma semana padrão de 7 dias com oito horas de trabalho por dia em 1 periodo por dia.
        /// Trabalho no período das 08 às 16
        /// </summary>
        /// <returns>Semana pré-montada</returns>
        public static WorkingWeek getWeek8x7()
        {
            ComplexWorkingWeek workingWeek = new ComplexWorkingWeek();
            workingWeek.setWorkDay((int)IsoDayOfWeek.Monday, HOURS_8, HOURS_16);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Tuesday, HOURS_8, HOURS_16);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Wednesday, HOURS_8, HOURS_16);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Thursday, HOURS_8, HOURS_16);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Friday, HOURS_8, HOURS_16);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Saturday, HOURS_8, HOURS_16);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Sunday, HOURS_8, HOURS_16);
            return workingWeek;
        }

        /// <summary>
        /// Recupera uma semana padrão de 7 dias com vinte e quatro horas de trabalho por dia em 1 periodo por dia.
        /// Trabalho no período das 00:00 às 23:59
        /// </summary>
        /// <returns>Semana pré-montada</returns>
        public static WorkingWeek getWeek24x7()
        {
            ComplexWorkingWeek workingWeek = new ComplexWorkingWeek();
            workingWeek.setWorkDay((int)IsoDayOfWeek.Monday, MIDNIGHT, HOURS_235959);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Tuesday, MIDNIGHT, HOURS_235959);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Wednesday, MIDNIGHT, HOURS_235959);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Thursday, MIDNIGHT, HOURS_235959);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Friday, MIDNIGHT, HOURS_235959);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Saturday, MIDNIGHT, HOURS_235959);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Sunday, MIDNIGHT, HOURS_235959);
            return workingWeek;
        }

        /// <summary>
        /// Recupera uma semana padrão de 5 dias com oito horas de trabalho por dia em 1 periodo por dia.
        /// Trabalho no período das 08 às 16
        /// </summary>
        /// <returns>Semana pré-montada</returns>
        public static WorkingWeek getWeek8x5()
        {
            ComplexWorkingWeek workingWeek = new ComplexWorkingWeek();
            workingWeek.setWorkDay((int)IsoDayOfWeek.Monday, HOURS_8, HOURS_16);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Tuesday, HOURS_8, HOURS_16);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Wednesday, HOURS_8, HOURS_16);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Thursday, HOURS_8, HOURS_16);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Friday, HOURS_8, HOURS_16);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Saturday, MIDNIGHT, MIDNIGHT);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Sunday, MIDNIGHT, MIDNIGHT);
            return workingWeek;
        }

        /// <summary>
        /// Recupera uma semana de trabalho com dois periodos por dia, com 8 horas de trabalho por
        /// dia separados nos periodos: 9 as 12 e 13 as 18
        /// </summary>
        /// <returns>
        /// Semana 8x7 em 2 periodos.
        /// </returns>
        public static WorkingWeek getWeek8x7In2Periods()
        {
            ComplexWorkingWeek workingWeek = new ComplexWorkingWeek();
            workingWeek.setWorkDay((int)IsoDayOfWeek.Monday, HOURS_9, HOURS_12);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Monday, HOURS_13, HOURS_18);

            workingWeek.setWorkDay((int)IsoDayOfWeek.Tuesday, HOURS_9, HOURS_12);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Tuesday, HOURS_13, HOURS_18);

            workingWeek.setWorkDay((int)IsoDayOfWeek.Wednesday, HOURS_9, HOURS_12);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Wednesday, HOURS_13, HOURS_18);

            workingWeek.setWorkDay((int)IsoDayOfWeek.Thursday, HOURS_9, HOURS_12);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Thursday, HOURS_13, HOURS_18);

            workingWeek.setWorkDay((int)IsoDayOfWeek.Friday, HOURS_9, HOURS_12);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Friday, HOURS_13, HOURS_18);

            workingWeek.setWorkDay((int)IsoDayOfWeek.Saturday, HOURS_9, HOURS_12);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Saturday, HOURS_13, HOURS_18);

            workingWeek.setWorkDay((int)IsoDayOfWeek.Sunday, HOURS_9, HOURS_12);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Sunday, HOURS_13, HOURS_18);

            return workingWeek;
        }

        /// <summary>
        /// Recupera uma semana de trabalho com dois periodos por dia, com 8 horas de trabalho por
        /// dia separados nos periodos: 9 as 12 e 13 as 18
        /// </summary>
        /// <returns>
        /// Semana 8x7 em 2 periodos.
        /// </returns>
        public static WorkingWeek GetWeek8X5In2Periods()
        {
            ComplexWorkingWeek workingWeek = new ComplexWorkingWeek();
            workingWeek.setWorkDay((int)IsoDayOfWeek.Monday, HOURS_9, HOURS_12);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Monday, HOURS_13, HOURS_18);

            workingWeek.setWorkDay((int)IsoDayOfWeek.Tuesday, HOURS_9, HOURS_12);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Tuesday, HOURS_13, HOURS_18);

            workingWeek.setWorkDay((int)IsoDayOfWeek.Wednesday, HOURS_9, HOURS_12);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Wednesday, HOURS_13, HOURS_18);

            workingWeek.setWorkDay((int)IsoDayOfWeek.Thursday, HOURS_9, HOURS_12);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Thursday, HOURS_13, HOURS_18);

            workingWeek.setWorkDay((int)IsoDayOfWeek.Friday, HOURS_9, HOURS_12);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Friday, HOURS_13, HOURS_18);

            workingWeek.setWorkDay((int)IsoDayOfWeek.Saturday, MIDNIGHT, MIDNIGHT);

            workingWeek.setWorkDay((int)IsoDayOfWeek.Sunday, MIDNIGHT, MIDNIGHT);

            return workingWeek;
        }

        /// <summary>
        /// Recupera uma semana de trabalho com dois periodos por dia, com 8 horas de trabalho por
        /// dia separados nos periodos: 9 as 12 e 13 as 18 e 
        /// Final de semana Sabado e Domingo das 09 as 12
        /// </summary>
        /// <returns>
        /// Semana 8x7 em 2 periodos + final semana 1 periodo.
        /// </returns>
        public static WorkingWeek GetWeek8X5In2PeriodsAndHalfWeekend()
        {
            ComplexWorkingWeek workingWeek = new ComplexWorkingWeek();
            workingWeek.setWorkDay((int)IsoDayOfWeek.Monday, HOURS_9, HOURS_12);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Monday, HOURS_13, HOURS_18);

            workingWeek.setWorkDay((int)IsoDayOfWeek.Tuesday, HOURS_9, HOURS_12);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Tuesday, HOURS_13, HOURS_18);

            workingWeek.setWorkDay((int)IsoDayOfWeek.Wednesday, HOURS_9, HOURS_12);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Wednesday, HOURS_13, HOURS_18);

            workingWeek.setWorkDay((int)IsoDayOfWeek.Thursday, HOURS_9, HOURS_12);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Thursday, HOURS_13, HOURS_18);

            workingWeek.setWorkDay((int)IsoDayOfWeek.Friday, HOURS_9, HOURS_12);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Friday, HOURS_13, HOURS_18);

            workingWeek.setWorkDay((int)IsoDayOfWeek.Saturday, HOURS_9, HOURS_12);

            workingWeek.setWorkDay((int)IsoDayOfWeek.Sunday, HOURS_9, HOURS_12);

            return workingWeek;
        }
    }
}