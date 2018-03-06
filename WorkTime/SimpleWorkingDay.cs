using NodaTime;
using System;

namespace enki.libs.workhours
{
    /// <summary>
    /// Configuração simples de um dia de trabalho
    /// </summary>
    public class SimpleWorkingDay : WorkingDaySlice, IComparable<WorkingDaySlice>
    {
        private static readonly LocalTime MIDNIGHT = new LocalTime(0, 0, 0);

        private readonly LocalDateTime date;

        private readonly short dayStart;

        private readonly short dayEnd;

        /// <summary>
        /// Initializes a new instance of the <see cref="enki.libs.workhours.SimpleWorkingDay"/> class.
        /// </summary>
        /// <param name='date'>
        /// Data a ser incluida no formato DateTime padrao.
        /// </param>
        /// <param name='dayStart'>
        /// Inicio do dia, contado em minutos a partir das 0h do dia.
        /// </param>
        /// <param name='dayEnd'>
        /// Final do dia, contato em minutos a partir das 0h do dia.
        /// </param>
        public SimpleWorkingDay(DateTime date, short dayStart, short dayEnd)
            : this(DateUtils.ToLocalDateTime(date), dayStart, dayEnd)
        {
        }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        /// <param name="date">Dia</param>
        /// <param name="dayStart">Hora de inicio, contada em mínutos a partir das 0h do dia</param>
        /// <param name="dayEnd">Hora de término, contara em minutos a partir da 0h do dia</param>
        public SimpleWorkingDay(LocalDateTime date, short dayStart, short dayEnd)
        {
            this.date = date;
            this.dayStart = dayStart;
            this.dayEnd = dayEnd;
        }

        /// <summary>
        /// Segundo construtor
        /// </summary>
        /// <param name="date">Dia</param>
        /// <param name="dayStart">Hora de inicio no formato NodaTome.LocalTime</param>
        /// <param name="dayEnd">Hora de termino no formato NodaTome.LocalTime</param>
        public SimpleWorkingDay(LocalDateTime date, LocalTime dayStart, LocalTime dayEnd)
            : this(date, (short)Period.Between(MIDNIGHT, dayStart, PeriodUnits.Minutes).Minutes, (short)Period.Between(MIDNIGHT, dayEnd, PeriodUnits.Minutes).Minutes)
        {
        }

        /// <summary>
        /// Recupera a data referente ao dia.
        /// </summary>
        /// <returns>Data referente ao dia no formato NodaTime.LocalDateTime</returns>
        public LocalDateTime getDate()
        {
            return this.date;
        }

        /// <summary>
        /// Recupera a data referente ao dia no formato DateTime do C# (Formato UTC/GMT).
        /// </summary>
        /// <returns>Data referente ao dia no formato DateTime</returns>
        public DateTime getDateUnspecified()
        {
            return this.date.ToDateTimeUnspecified();
        }

        /// <summary>
        /// Recupera o horário de início do período
        /// </summary>
        /// <returns>Horario de inicio do período contado em minutos a partir das 0h do dia</returns>
        public short getDayStart()
        {
            return this.dayStart;
        }

        /// <summary>
        /// Recupera o horário de término do período
        /// </summary>
        /// <returns>Horario de término do período contado em minutos a partir das 0h do dia</returns>
        public short getDayEnd()
        {
            return this.dayEnd;
        }

        /// <summary>
        /// Compara dois dias pela data.
        /// </summary>
        /// <param name="obj">O dia com o qual esse deve ser comparado.</param>
        /// <returns>-1 se menor que, 0 se igual a ou 1 se maior que</returns>
        public int CompareTo(WorkingDaySlice workingDay)
        {
            return this.getDate().CompareTo(workingDay.getDate());
        }
    }
}