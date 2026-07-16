using NodaTime;
using System;
using System.Collections.Generic;

namespace enki.libs.workhours
{
    /// <summary>
    /// Representa um dia de trabalho complexo, e contem uma lista de partes de dia (WorkingDaySlice)
    /// </summary>
    public class ComplexWorkingDay
    {
        /// <summary>
        /// Representa a lista de partes que formam este dia util
        /// </summary>
        private List<SimpleWorkingDay> dayParts = new List<SimpleWorkingDay>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkTime.WorkingDay"/> class.
        /// </summary>
        public ComplexWorkingDay()
        {
        }

        /// <summary>
        /// Recupera a quantidade de horas uteis contidas neste dia de trabalho.
        /// </summary>
        /// <returns>
        /// The day hours.
        /// </returns>
        public short getDayHours()
        {
            try
            {
                return (short)(this.getMinutesOfDay() / 60);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Adiciona uma fatia de horario ao dia
        /// </summary>
        /// <param name='daySlice'>
        /// Pedaço do dia
        /// </param>
        public void addDayPart(SimpleWorkingDay daySlice)
        {
            if (this.validateToAdd(daySlice) == true)
            {
                this.dayParts.Add(daySlice);
                this.sortDayPartsByStartTime();
            }
            else
            {
                throw new Exception("Invalid slice.", new Exception("The slice with a slice informed crosses existing list."));
            }
        }

        /// <summary>
        /// Valida se o periodo a ser inserido é permitido.
        /// </summary>
        /// <param name="daySlice"></param>
        /// <returns></returns>
        private bool validateToAdd(SimpleWorkingDay daySlice)
        {
            foreach (SimpleWorkingDay slice in this.dayParts)
            {
                if (daySlice.getDayStart() < slice.getDayEnd() && daySlice.getDayStart() > slice.getDayStart()) return false;
                if (daySlice.getDayEnd() < slice.getDayEnd() && daySlice.getDayEnd() > slice.getDayStart()) return false;
            }
            return true;
        }

        /// <summary>
        /// Recupera a lista de partes que formam este dia.
        /// </summary>
        /// <returns>
        /// The day parts.
        /// </returns>
        public List<SimpleWorkingDay> getDayParts()
        {
            return this.dayParts;
        }

        /// <summary>
        /// Recupera a quantidade de minutos entre 0h e o inicio da primeira parte do dia.
        /// </summary>
        /// <returns>
        /// Quantidade de minutos na primeira parte util do dia.
        /// </returns>
        public short getMinStartDayPart()
        {
            if (this.dayParts.Count == 0)
            {
                return 0;
            }
            short minValue = short.MaxValue;
            foreach (SimpleWorkingDay item in this.dayParts)
            {
                short value = item.getDayStart();
                if (value < minValue)
                {
                    minValue = value;
                }
            }
            return minValue;
        }

        /// <summary>
        /// Recupera a quantidade de minutos entre 0h e o final da ultima parte do dia.
        /// </summary>
        /// <returns>
        /// Quantidade de minutos na ultima parte util do dia.
        /// </returns>
        public short getMaxEndDayPart()
        {
            try
            {
                if (this.dayParts.Count == 0)
                {
                    return 0;
                }
                short maxValue = short.MinValue;
                foreach (SimpleWorkingDay item in this.dayParts)
                {
                    short value = item.getDayEnd();
                    if (value > maxValue)
                    {
                        maxValue = value;
                    }
                }
                return maxValue;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Recupera o numero total de minutos uteis do dia.
        /// </summary>
        /// <returns>
        /// Minutos uteis do dia.
        /// </returns>
        public short getMinutesOfDay()
        {
            try
            {
                short minutes = 0;
                foreach (SimpleWorkingDay slice in this.dayParts)
                {
                    minutes += (short)(slice.getDayEnd() - slice.getDayStart());
                }

                return minutes;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Recupera o numero total de minutos uteis do dia a partir de determinado horario.
        /// </summary>
        /// <returns>
        /// Minutos uteis do dia contados a partir da hora inicial informada.
        /// </returns>
        public short getMinutesStartIn(LocalTime startTime)
        {
            try
            {
                //Recupera a quantidade de minutos entre as 00:00 do dia e a hora inicial a ser validada.
                // NOTA: O arredondamento dos segundos está ocorrendo sempre para baixo.
                int startMinutes = (int)Period.Between(
                    new LocalDateTime(2000, 01, 01, 0, 0, 0),
                    new LocalDateTime(2000, 01, 01, startTime.Hour, startTime.Minute, startTime.Second),
                    PeriodUnits.Minutes
                ).Minutes;

                // Efetua a contagem das horas uteis
                short minutes = 0;
                foreach (SimpleWorkingDay slice in this.dayParts)
                {
                    if (startMinutes < slice.getDayEnd())
                    {
                        if (startMinutes > slice.getDayStart())
                        {
                            minutes += (short)(slice.getDayEnd() - startMinutes);
                        }
                        else
                        {
                            minutes += (short)(slice.getDayEnd() - slice.getDayStart());
                        }
                    }
                }

                return minutes;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Verifica se um determinado horario esta dentro de alguma das partes uteis do dia.
        /// A comparacao é feita na granularidade de minutos, sendo o inicio da fatia inclusivo
        /// e o final exclusivo. Ex: fatia das 08:00 as 12:00 contem 08:00:30 mas nao contem 12:00:30.
        /// </summary>
        /// <returns>
        /// Verdadeiro se o horario informado pertence a algum periodo util do dia.
        /// </returns>
        public bool isWithinWorkingPeriod(LocalTime time)
        {
            //Recupera a quantidade de minutos entre as 00:00 do dia e a hora a ser validada.
            int timeMinutes = (int)Period.Between(
                new LocalDateTime(2000, 01, 01, 0, 0, 0),
                new LocalDateTime(2000, 01, 01, time.Hour, time.Minute, time.Second),
                PeriodUnits.Minutes
            ).Minutes;

            foreach (SimpleWorkingDay slice in this.dayParts)
            {
                if (timeMinutes >= slice.getDayStart() && timeMinutes < slice.getDayEnd())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Recupera o numero total de minutos uteis do dia ate um determinado horario.
        /// </summary>
        /// <returns>
        /// Minutos uteis do dia contados ate a hora final informada.
        /// </returns>
        public short getMinutesEndIn(LocalTime endTime)
        {
            try
            {
                //Recupera a quantidade de minutos entre as 00:00 do dia e a hora final a ser validada.
                int endMinutes = (int)Period.Between(
                    new LocalDateTime(2000, 01, 01, 0, 0, 0),
                    new LocalDateTime(2000, 01, 01, endTime.Hour, endTime.Minute, endTime.Second),
                    PeriodUnits.Minutes
                ).Minutes;

                // Efetua a contagem das horas uteis
                short minutes = 0;
                foreach (SimpleWorkingDay slice in this.dayParts)
                {
                    if (endMinutes > slice.getDayStart())
                    {
                        if (endMinutes < slice.getDayEnd())
                        {
                            minutes += (short)(endMinutes - slice.getDayStart());
                        }
                        else
                        {
                            minutes += (short)(slice.getDayEnd() - slice.getDayStart());
                        }
                    }
                }

                return minutes;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Ordena a lista de horarios pelo horario de inicio
        /// </summary>
        private void sortDayPartsByStartTime()
        {
            try
            {
                this.dayParts.Sort(
                    delegate (SimpleWorkingDay wd1, SimpleWorkingDay wd2)
                    {
                        return wd1.getDayStart().CompareTo(wd2.getDayStart());
                    });
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}