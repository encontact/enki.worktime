using enki.libs.workhours.domain;
using NodaTime;
using System;
using System.Collections.Generic;
using WorkTime;

namespace enki.libs.workhours
{
    /// <summary>
    /// Classe responsável pelo cálculo das hóras úteis.
    /// </summary>
    public class WorkingHoursTable
    {
        private static readonly int DEFAULT_TABLE_EXPANSION = 180;

        private static readonly int MINS_PER_HOUR = 60;

        private static readonly LocalTime MIDNIGHT = new LocalTime(0, 0, 0);

        // Para cada dia do ano, contem as horas uteis somadas do inicio da tabela ate o dia especifico.
        private long[] workingMinutesSum;

        // Inicio e fim de horario de cada dia do periodo
        private ComplexWorkingDay[] workDay;

        private RecurrentExceptionsBucket RecurrentExceptions = new RecurrentExceptionsBucket();

        private LocalDateTime tableStart;

        private LocalDateTime tableEnd;

        private int mjdTableStart;

        private WorkingDaySlice nextException = null;

        private IEnumerator<WorkingDaySlice> exceptionsIt;

        private readonly SortedSet<WorkingDaySlice> exceptions;

        private readonly WorkingWeek workingWeek;

        /// <summary>
        /// Construtor datas padrões C# (DateTime)
        /// </summary>
        /// <param name="workingWeek">semana de trabalho</param>
        /// <param name="start">data inicial</param>
        /// <param name="end">data final</param>
        public WorkingHoursTable(WorkingWeek workingWeek, DateTime start, DateTime end)
            : this(workingWeek, LocalDateTime.Parse(start.ToString()), LocalDateTime.Parse(end.ToString()))
        {
        }

        /// <summary>
        /// Construtor datas padrões NodaTime(NodaTime.LocalDateTime).
        /// </summary>
        /// <param name="workingWeek">semana de trabalho</param>
        /// <param name="start">data inicial</param>
        /// <param name="end">data final</param>
        public WorkingHoursTable(WorkingWeek workingWeek, LocalDateTime start, LocalDateTime end)
            : this(workingWeek, new List<WorkingDaySlice>(), new SortedSet<WorkingDaySlice>(), start, end)
        {
        }

        /// <summary>
        /// Construtor padrão completo.
        /// </summary>
        /// <param name="workingWeek">semana de trabalho</param>
        /// <param name="recurrentExceptions">exceções anualmente recorrentes</param>
        /// <param name="exceptions">lista de dias com horários diferentes do padrão de trabalho semanal, ex: feriados</param>
        /// <param name="start">data inicial</param>
        /// <param name="end">data final</param>
        public WorkingHoursTable(WorkingWeek workingWeek, List<WorkingDaySlice> recurrentExceptions,
                SortedSet<WorkingDaySlice> exceptions, LocalDateTime start, LocalDateTime end)
        {
            this.workingWeek = workingWeek;
            tableStart = start;
            tableEnd = end;
            mjdTableStart = DateUtils.toMJD(tableStart);
            this.exceptions = exceptions;
            exceptionsIt = exceptions.GetEnumerator();
            CreateRecurrentExceptionsBuckets(recurrentExceptions);
            fillTable();
        }

        private void CreateRecurrentExceptionsBuckets(List<WorkingDaySlice> recurrentExceptions)
        {
            foreach (var day in recurrentExceptions)
            {
                RecurrentExceptions.Add(day.getDate(), day.getDayStart(), day.getDayEnd());
            }
        }

        /// <summary>
        /// Estrutura que cria a tabela de cálculo de horas.
        /// </summary>
        private void fillTable()
        {
            workingMinutesSum = new long[Period.Between(tableStart, tableEnd, PeriodUnits.Days).Days + 2];
            workDay = new ComplexWorkingDay[workingMinutesSum.Length];
            if (exceptionsIt.MoveNext())
            {
                nextException = exceptionsIt.Current;
            }
            // Inicia a regua.
            for (int i = 0; i < workDay.Length; i++)
            {
                workDay[i] = new ComplexWorkingDay();
            }

            workingMinutesSum[0] = 0;
            fillTable(1);
        }

        /// <summary>
        /// Dado um dia devolve um número de 0 a 355 que representa o dia no ano, o número sempre será o mesmo, independentemente do ano ser ou não bisexto
        /// </summary>
        /// <param name="date">Data</param>
        /// <returns>Inteiro representando o dia no ano</returns>
        private int getLeapSafeDayOfYear(LocalDateTime date)
        {
            int day = date.DayOfYear - 1;
            if (!CalendarSystem.Iso.IsLeapYear(date.Year) && date.Month > 2)
            {
                day++;
            }
            return day;
        }

        /// <summary>
        /// Preenche a tabela a partir de um ponto determinado
        /// </summary>
        /// <param name="startIndex">StartIndex ponto a partir do qual a tabela deve ser preenchida</param>
        private void fillTable(int startIndex)
        {
            int i = startIndex;
            LocalDateTime loopStart = tableStart.PlusDays(startIndex - 1);
            for (LocalDateTime currentDate = loopStart; currentDate.CompareTo(tableEnd) <= 0; currentDate = currentDate.PlusDays(1))
            {
                fillStartEndTime(i, currentDate);
                // Calcula os minutos totais para o dia informado.
                short minutesOfday = 0;
                foreach (SimpleWorkingDay day in workDay[i].getDayParts())
                {
                    minutesOfday += (short)(day.getDayEnd() - day.getDayStart());
                }
                workingMinutesSum[i] = workingMinutesSum[i - 1] + minutesOfday;
                i++;
            }
        }

        /// <summary>
        /// Expande a tabela, para permitir que ela tenha um tamanho que chegue até a data final determinada.
        /// </summary>
        /// <param name="end">Nova data final</param>
        private void expandTableEnd(LocalDateTime end)
        {
            if (end.CompareTo(tableEnd) <= 0)
            {
                return;
            }

            int oldSize = workingMinutesSum.Length;
            int newArraySize = (int)Period.Between(tableStart, end, PeriodUnits.Days).Days;
            Array.Resize(ref workingMinutesSum, newArraySize + 2);
            Array.Resize(ref workDay, workingMinutesSum.Length);
            tableEnd = end;
            fillTable(oldSize);
        }

        /// <summary>
        /// Recalcula a tabela para poder começar de uma data inicial mais no passado
        /// </summary>
        /// <param name="start">Nova data inicial</param>
        private void expandTableStart(LocalDateTime start)
        {
            if (start.CompareTo(tableStart) >= 0)
            {
                return;
            }

            tableStart = start;
            mjdTableStart = DateUtils.toMJD(tableStart);
            exceptionsIt = exceptions.GetEnumerator();
            fillTable();
        }

        /// <summary>
        /// Preenche os horarios de início e término do trabalho para um dia, levando em consideração as exceções.
        /// </summary>
        /// <param name="dayIndex">indice do dia no array</param>
        /// <param name="currentDate">dia a ser preenchido.</param>
        private void fillStartEndTime(int dayIndex, LocalDateTime currentDate)
        {
            while (nextException != null && nextException.getDate().CompareTo(currentDate) < 0)
            {
                exceptionsIt.MoveNext();
                nextException = exceptionsIt.Current;
            }
            int day = getLeapSafeDayOfYear(currentDate);
            if (nextException != null && nextException.getDate().Equals(currentDate))
            {
                // exceções
                workDay[dayIndex] = workDay[dayIndex] == null ? new ComplexWorkingDay() : workDay[dayIndex];
                var periods = GetExceptionDaySlices(nextException.getDayStart(), nextException.getDayEnd(), workingWeek.getPeriods((int)currentDate.IsoDayOfWeek));
                foreach (var period in periods)
                {
                    workDay[dayIndex].addDayPart(new SimpleWorkingDay(
                        currentDate.ToDateTimeUnspecified(), period.Item1, period.Item2)
                    );
                }
                nextException = exceptionsIt.MoveNext() ? exceptionsIt.Current : null;
            }
            else if (RecurrentExceptions.Has(currentDate))
            {
                // exceções recorrentes
                workDay[dayIndex] = workDay[dayIndex] == null ? new ComplexWorkingDay() : workDay[dayIndex];
                var time = RecurrentExceptions.GetPeriod(currentDate);
                var periods = GetExceptionDaySlices(time.Item1, time.Item2, workingWeek.getPeriods((int)currentDate.IsoDayOfWeek));
                foreach (var period in periods)
                {
                    workDay[dayIndex].addDayPart(new SimpleWorkingDay(
                        currentDate.ToDateTimeUnspecified(), period.Item1, period.Item2)
                    );
                }
            }
            else
            {
                // dias normais
                workDay[dayIndex] = workDay[dayIndex] == null ? new ComplexWorkingDay() : workDay[dayIndex];
                foreach (WorkingPeriod slice in workingWeek.getPeriods((int)currentDate.IsoDayOfWeek))
                {
                    workDay[dayIndex].addDayPart(new SimpleWorkingDay(
                        currentDate.ToDateTimeUnspecified(),
                        slice.startPeriod,
                        slice.endPeriod
                    ));
                }
            }
        }

        /// <summary>
        /// Recupera o período útil a ser trabalhado considerando um dia de exceção.
        /// No caso, a regra para exceção diz que o período da exceção não é trabalhado, mas se o dia
        /// for útil, o restante do dia deve ser contabilizado.
        /// Exemplo: Exceção: 10/02/2000 das 00:00 as 12:00, se o dia for útil das 08:00 as 16:00,
        ///          ainda deve ser contato o tempo de trabalho das 12:00 as 16:00.
        /// </summary>
        /// <param name="start">Minuto de inicio do feriado</param>
        /// <param name="end">Minuto de fim do feriado</param>
        /// <param name="workingPeriods">Períodos de trabalho do dia.</param>
        /// <returns>Lista de períodos a serem considerados no tempo de trabalho.</returns>
        public static List<Tuple<short, short>> GetExceptionDaySlices(short start, short end, List<WorkingPeriod> workingPeriods)
        {
            var ret = new List<Tuple<short, short>>();
            foreach (var workingPeriod in workingPeriods)
            {
                if (workingPeriod.startPeriod < start)
                {
                    if (workingPeriod.endPeriod <= start)
                    {
                        ret.Add(new Tuple<short, short>(workingPeriod.startPeriod, workingPeriod.endPeriod));
                    }
                    else if (workingPeriod.endPeriod <= end)
                    {
                        ret.Add(new Tuple<short, short>(workingPeriod.startPeriod, start));
                    }
                    else
                    {
                        ret.Add(new Tuple<short, short>(workingPeriod.startPeriod, start));
                        ret.Add(new Tuple<short, short>(end, workingPeriod.endPeriod));
                    }
                }
                else if (workingPeriod.startPeriod >= end)
                {
                    ret.Add(new Tuple<short, short>(workingPeriod.startPeriod, workingPeriod.endPeriod));
                }
                else if (workingPeriod.startPeriod < end)
                {
                    if (workingPeriod.endPeriod > end)
                    {
                        ret.Add(new Tuple<short, short>(end, workingPeriod.endPeriod));
                    }
                }
                else
                {
                    ret.Add(new Tuple<short, short>(workingPeriod.startPeriod, workingPeriod.endPeriod));
                }
            }
            return ret;
        }

        /// <summary>
        /// Devolve a quantidade de horas úteis entre dois DateTime's arredondado p/ baixo (floor)
        /// </summary>
        /// <param name="start">inicio</param>
        /// <param name="end">fim</param>
        /// <returns>A quantidade de horas úteis entre dois DateTime's</returns>
        public int getWorkingHoursBetween(DateTime start, DateTime end)
        {
            return (int)Math.Floor(((double)getWorkingMinutesBetween(start, end) / MINS_PER_HOUR));
        }

        /// <summary>
        /// Calcula o número de minutos úteis entre dois DateTime's.
        /// </summary>
        /// <param name="start">data inicial.</param>
        /// <param name="end">data final.</param>
        /// <returns>Minutos úteis entre os dois DateTime's.</returns>
        public long getWorkingMinutesBetween(DateTime start, DateTime end)
        {
            int endIndex = DateUtils.toMJD(end) - mjdTableStart + 1;
            int startIndex = DateUtils.toMJD(start) - DateUtils.toMJD(tableStart) + 1;

            // Extende a tabela para cima ou para baixo se necessario
            if (endIndex >= workDay.Length || startIndex >= workDay.Length)
            {
                expandTableEnd(end.CompareTo(start) > 0 ? LocalDateTime.Parse(end.ToString()) : LocalDateTime.Parse(start.ToString()));
            }
            if (startIndex < 1 || endIndex < 1)
            {
                expandTableStart(end.CompareTo(start) < 0 ? LocalDateTime.Parse(end.ToString()) : LocalDateTime.Parse(start.ToString()));
                endIndex = DateUtils.toMJD(end) - mjdTableStart + 1;
                startIndex = DateUtils.toMJD(start) - DateUtils.toMJD(tableStart) + 1;
            }

            // Recupera os minutos uteis do periodo informado.
            long minutes = getWorkingMinutesSum(endIndex) - getWorkingMinutesSum(startIndex);
            // Calcula os minutos uteis do primeiro dia do bloco
            DateTime firstDayEnd = (new DateTime(start.Year, start.Month, start.Day, 0, 0, 0).AddMinutes(workDay[startIndex].getMaxEndDayPart()));
            if (start.CompareTo(firstDayEnd) < 0)
            {
                minutes += workDay[startIndex].getMinutesStartIn(new LocalTime(start.Hour, start.Minute, start.Second));
            }
            // Calcula os minutos uteis do ultimo dia do bloco
            DateTime lastDayEnd = (new DateTime(end.Year, end.Month, end.Day, 0, 0, 0).AddMinutes(workDay[endIndex].getMaxEndDayPart()));
            if (end.CompareTo(lastDayEnd) < 0)
            {
                int tmp1 = workDay[endIndex].getMinutesStartIn(new LocalTime(end.Hour, end.Minute, end.Second));
                int tmp2 = workDay[endIndex].getMinutesOfDay();
                minutes -= tmp1 < tmp2 ? tmp1 : tmp2;
            }

            return minutes;
        }

        /// <summary>
        /// Adiciona uma determinada quantidade de horas e minutos uteis a um DateTime padrão C#
        /// </summary>
        /// <param name="original">data original</param>
        /// <param name="hours">horas uteis</param>
        /// <param name="minutes">minutos uteis</param>
        /// <returns>nova data.</returns>
        public DateTime addWorkingHours(DateTime original, int hours, int minutes)
        {
            // Se a semana não tem nenhuma hora útil, ignora o cálculo.
            if (workingWeek.GetWeekTime().TotalMinutes == 0) return original;

            // Verifica se deve expandir o inicio da tabela.
            if (original.CompareTo(DateTime.Parse(tableStart.ToString())) < 0)
            {
                expandTableStart(LocalDateTime.Parse(original.ToString()));
            }
            // Recupera os minutos totais a serem adicionados
            int totalMinutes = minutes + (hours * MINS_PER_HOUR);
            if (totalMinutes == 0)
            {
                return original;
            }
            // Recupera o dia do ano a ser trabalhado
            int day = DateUtils.toMJD(original) - mjdTableStart + 1;
            // Recupera a contagem de minutos iniciais para a data "original", antes da adiçao.
            long firstDayTotal = getWorkingMinutesSum(day - 1) + getWorkingMinutesBetween(
                new DateTime(original.Year, original.Month, original.Day, 0, 0, 0),
                original
            );
            // Se nao houver espaço no dia para adicionar o horario necessario, verifica no dia seguinte
            while (getWorkingMinutesSum(day) - firstDayTotal < totalMinutes)
            {
                day++;
            }
            long delta = workingMinutesSum[day - 1] - firstDayTotal;

            // Recupera a data onde serao inseridos os minutos uteis
            LocalDateTime resultedDate = DateUtils.fromMJD(day + mjdTableStart - 1);
            // Calcula os minutos uteis a serem adicionados
            int minutesToAdd = (int)(totalMinutes - delta + workDay[day].getMinStartDayPart()); //this.workStart[day]);

            // Conta minutos dos intervalos entre os períodos e adiciona ao valor de minutos a serem adicionados.
            short endMinutesPreviousSlice = 0;
            foreach (SimpleWorkingDay daySlice in workDay[day].getDayParts())
            {
                if (endMinutesPreviousSlice != 0 && minutesToAdd > endMinutesPreviousSlice && minutesToAdd > daySlice.getDayStart())
                {
                    minutesToAdd += daySlice.getDayStart() - endMinutesPreviousSlice;
                }
                endMinutesPreviousSlice = daySlice.getDayEnd();
            }

            // Recupera a data com a soma corrida dos minutos necessários
            DateTime result = new DateTime(resultedDate.Year, resultedDate.Month, resultedDate.Day, MIDNIGHT.Hour, MIDNIGHT.Minute, MIDNIGHT.Second).AddMinutes(minutesToAdd);

            return result;
        }

        /// <summary>
        /// Adiciona uma determinada quantidade de horas e minutos uteis a um LocalDateTime do NodaTime
        /// </summary>
        /// <param name="original">data original</param>
        /// <param name="hours">horas uteis</param>
        /// <param name="minutes">minutos uteis</param>
        /// <returns>nova data.</returns>
        public LocalDateTime addWorkingHours(LocalDateTime original, int hours, int minutes)
        {
            // Verifica se deve expandir o inicio da tabela.
            if (original.CompareTo(tableStart) < 0)
            {
                expandTableStart(original);
            }
            // Recupera os minutos totais a serem adicionados
            int totalMinutes = minutes + (hours * MINS_PER_HOUR);
            if (totalMinutes == 0)
            {
                return original;
            }
            // Recupera o dia do ano a ser trabalhado
            int day = DateUtils.toMJD(original) - mjdTableStart + 1;
            // Recupera a contagem de minutos iniciais para a data "original", antes da adiçao.
            long firstDayTotal = getWorkingMinutesSum(day - 1) + getWorkingMinutesBetween(
                new DateTime(original.Year, original.Month, original.Day, 0, 0, 0),
                DateTime.Parse(original.ToString())
            );
            // Se nao houver espaço no dia para adicionar o horario necessario, verifica no dia seguinte
            while (getWorkingMinutesSum(day) - firstDayTotal < totalMinutes)
            {
                day++;
            }
            // Total de minutos consumidos em outros dias anteriores a data atual
            long delta = workingMinutesSum[day - 1] - firstDayTotal;

            // Recupera a data onde serao inseridos os minutos uteis
            LocalDateTime resultedDate = DateUtils.fromMJD(day + mjdTableStart - 1);

            // Total de Minutos que sobraram para serem adicionados na data atual
            int minutesToAdd = (int)((totalMinutes - delta) + workDay[day].getMinStartDayPart());

            // Conta minutos dos intervalos entre os períodos e adiciona ao valor de minutos a serem adicionados.
            short endMinutesPreviousSlice = 0;
            foreach (SimpleWorkingDay daySlice in workDay[day].getDayParts())
            {
                if (endMinutesPreviousSlice != 0 && minutesToAdd > endMinutesPreviousSlice && minutesToAdd > daySlice.getDayStart())
                {
                    minutesToAdd += daySlice.getDayStart() - endMinutesPreviousSlice;
                }
                endMinutesPreviousSlice = daySlice.getDayEnd();
            }

            // Recupera a data com a soma corrida dos minutos necessários
            LocalDateTime result = new LocalDateTime(resultedDate.Year, resultedDate.Month, resultedDate.Day, MIDNIGHT.Hour, MIDNIGHT.Minute, MIDNIGHT.Second).PlusMinutes(minutesToAdd);

            return result;
        }

        /// <summary>
        /// Pega a quantidade de horas úteis somada na tabela até um determinado dia, expande a tabela se for necessário
        /// </summary>
        /// <param name="day">Dia Juliano(MJD)</param>
        /// <returns>Quantidade de horas úteis</returns>
        private long getWorkingMinutesSum(int day)
        {
            if (day >= workingMinutesSum.Length)
            {
                LocalDateTime end = tableStart.PlusDays(day + DEFAULT_TABLE_EXPANSION);
                expandTableEnd(end);
            }

            return workingMinutesSum[day];
        }
    }
}