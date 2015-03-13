using System.Collections.Generic;

using NodaTime;

using enki.libs.workhours.domain;

namespace enki.libs.workhours {
	/// <summary>
	/// Calendário de trabalho semanal, armazena as horas de início e término de trabalho para cada dia da semana.
	/// </summary>
	public class ComplexWorkingWeek : WorkingWeek {

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
		public List<WorkingPeriod> getPeriods(int dayOfWeek) {
			return this.dayPeriods.FindAll(period => period.dayOfWeek == dayOfWeek);
		}

		/// <summary>
		/// Define os horários de inicio e término do trabalho para um periodo do dia da semana informado.
		/// </summary>
		/// <param name="dayOfWeek">Dia da semana, definido em "DateTimeConstants"</param>
		/// <param name="start">Horario de inicio em NodaTime.LocalTime</param>
		/// <param name="end">Horario de término em NodaTime.LocalTime</param>
		public void setWorkDay(int dayOfWeek, LocalTime start, LocalTime end) {
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
		public static WorkingWeek getWeek8x7() {
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
		public static WorkingWeek getWeek24x7() {
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
		public static WorkingWeek getWeek8x5() {
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
		public static WorkingWeek getWeek8x7In2Periodos() {
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
		public static WorkingWeek GetWeek8X5In2Periodos() {
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
	}
}