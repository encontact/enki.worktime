using System.Collections.Generic;
using System;

using NodaTime;

using enki.libs.workhours.domain;
using WorkTime;

namespace enki.libs.workhours {

	/// <summary>
	/// Classe responsável pelo cálculo das hóras úteis.
	/// </summary>
	public class WorkingHoursTable {
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
			: this(workingWeek, LocalDateTime.Parse(start.ToString()), LocalDateTime.Parse(end.ToString())) {
		}

		/// <summary>
		/// Construtor datas padrões NodaTime(NodaTime.LocalDateTime).
		/// </summary>
		/// <param name="workingWeek">semana de trabalho</param>
		/// <param name="start">data inicial</param>
		/// <param name="end">data final</param>
		public WorkingHoursTable(WorkingWeek workingWeek, LocalDateTime start, LocalDateTime end)
			: this(workingWeek, new List<WorkingDaySlice>(), new SortedSet<WorkingDaySlice>(), start, end) {
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
				SortedSet<WorkingDaySlice> exceptions, LocalDateTime start, LocalDateTime end) {
			this.workingWeek = workingWeek;
			this.tableStart = start;
			this.tableEnd = end;
			this.mjdTableStart = DateUtils.toMJD(this.tableStart);
			this.exceptions = exceptions;
			this.exceptionsIt = exceptions.GetEnumerator();
			CreateRecurrentExceptionsBuckets(recurrentExceptions);
			this.fillTable();
		}

		private void CreateRecurrentExceptionsBuckets(List<WorkingDaySlice> recurrentExceptions) {
			foreach (var day in recurrentExceptions) {
				RecurrentExceptions.Add(day.getDate(), day.getDayStart(), day.getDayEnd());
			}
		}

		/// <summary>
		/// Estrutura que cria a tabela de cálculo de horas.
		/// </summary>
		private void fillTable() {
			this.workingMinutesSum = new long[Period.Between(this.tableStart, this.tableEnd, PeriodUnits.Days).Days + 2];
			this.workDay = new ComplexWorkingDay[this.workingMinutesSum.Length];
			if (this.exceptionsIt.MoveNext()) {
				this.nextException = this.exceptionsIt.Current;
			}
			// Inicia a regua.
			for (int i = 0; i < workDay.Length; i++) {
				this.workDay[i] = new ComplexWorkingDay();
			}

			this.workingMinutesSum[0] = 0;
			this.fillTable(1);
		}

		/// <summary>
		/// Dado um dia devolve um número de 0 a 355 que representa o dia no ano, o número sempre será o mesmo, independentemente do ano ser ou não bisexto
		/// </summary>
		/// <param name="date">Data</param>
		/// <returns>Inteiro representando o dia no ano</returns>
		private int getLeapSafeDayOfYear(LocalDateTime date) {
			int day = date.DayOfYear - 1;
			if (!CalendarSystem.Iso.IsLeapYear(date.Year) && date.Month > 2) {
				day++;
			}
			return day;
		}

		/// <summary>
		/// Preenche a tabela a partir de um ponto determinado
		/// </summary>
		/// <param name="startIndex">StartIndex ponto a partir do qual a tabela deve ser preenchida</param>
		private void fillTable(int startIndex) {
			int i = startIndex;
			LocalDateTime loopStart = this.tableStart.PlusDays(startIndex - 1);
			for (LocalDateTime currentDate = loopStart; currentDate.CompareTo(this.tableEnd) <= 0; currentDate = currentDate.PlusDays(1)) {
				this.fillStartEndTime(i, currentDate);
				// Calcula os minutos totais para o dia informado.
				short minutesOfday = 0;
				foreach (SimpleWorkingDay day in this.workDay[i].getDayParts()) {
					minutesOfday += (short)(day.getDayEnd() - day.getDayStart());
				}
				this.workingMinutesSum[i] = this.workingMinutesSum[i - 1] + minutesOfday;
				i++;
			}
		}

		/// <summary>
		/// Expande a tabela, para permitir que ela tenha um tamanho que chegue até a data final determinada.
		/// </summary>
		/// <param name="end">Nova data final</param>
		private void expandTableEnd(LocalDateTime end) {
			if (end.CompareTo(this.tableEnd) <= 0) {
				return;
			}

			int oldSize = this.workingMinutesSum.Length;
			int newArraySize = (int)Period.Between(this.tableStart, end, PeriodUnits.Days).Days;
			Array.Resize(ref this.workingMinutesSum, newArraySize + 2);
			Array.Resize(ref this.workDay, this.workingMinutesSum.Length);
			this.tableEnd = end;
			this.fillTable(oldSize);
		}

		/// <summary>
		/// Recalcula a tabela para poder começar de uma data inicial mais no passado
		/// </summary>
		/// <param name="start">Nova data inicial</param>
		private void expandTableStart(LocalDateTime start) {
			if (start.CompareTo(this.tableStart) >= 0) {
				return;
			}

			this.tableStart = start;
			this.mjdTableStart = DateUtils.toMJD(this.tableStart);
			this.exceptionsIt = this.exceptions.GetEnumerator();
			this.fillTable();
		}

		/// <summary>
		/// Preenche os horarios de início e término do trabalho para um dia, levando em consideração as exceções.
		/// </summary>
		/// <param name="dayIndex">indice do dia no array</param>
		/// <param name="currentDate">dia a ser preenchido.</param>
		private void fillStartEndTime(int dayIndex, LocalDateTime currentDate) {
			while (this.nextException != null && this.nextException.getDate().CompareTo(currentDate) < 0) {
				this.exceptionsIt.MoveNext();
				this.nextException = this.exceptionsIt.Current;
			}
			int day = this.getLeapSafeDayOfYear(currentDate);
			if (this.nextException != null && this.nextException.getDate().Equals(currentDate)) {
				// exceções
				workDay[dayIndex] = workDay[dayIndex] == null ? new ComplexWorkingDay() : workDay[dayIndex];
				var periods = GetExceptionDaySlices(nextException.getDayStart(), nextException.getDayEnd(), workingWeek.getPeriods((int)currentDate.IsoDayOfWeek));
				foreach (var period in periods) {
					this.workDay[dayIndex].addDayPart(new SimpleWorkingDay(
						currentDate.ToDateTimeUnspecified(), period.Item1, period.Item2)
					);
				}
				this.nextException = this.exceptionsIt.MoveNext() ? this.exceptionsIt.Current : null;
			} else if (RecurrentExceptions.Has(currentDate)) {
				// exceções recorrentes
				workDay[dayIndex] = workDay[dayIndex] == null ? new ComplexWorkingDay() : workDay[dayIndex];
				var time = RecurrentExceptions.GetPeriod(currentDate);
				var periods = GetExceptionDaySlices(time.Item1, time.Item2, workingWeek.getPeriods((int)currentDate.IsoDayOfWeek));
				foreach (var period in periods) {
					this.workDay[dayIndex].addDayPart(new SimpleWorkingDay(
						currentDate.ToDateTimeUnspecified(), period.Item1, period.Item2)
					);
				}
			} else {
				// dias normais
				workDay[dayIndex] = workDay[dayIndex] == null ? new ComplexWorkingDay() : workDay[dayIndex];
				foreach (WorkingPeriod slice in this.workingWeek.getPeriods((int)currentDate.IsoDayOfWeek)) {
					this.workDay[dayIndex].addDayPart(new SimpleWorkingDay(
						currentDate.ToDateTimeUnspecified(),
						slice.startPeriod,
						slice.endPeriod
					));
				}
			}
		}

		public static List<Tuple<short, short>> GetExceptionDaySlices(short start, short end, List<WorkingPeriod> workingPeriods) {
			var ret = new List<Tuple<short, short>>();
			foreach (var workingPeriod in workingPeriods) {
				if (workingPeriod.startPeriod < start) {
					if (workingPeriod.endPeriod <= start) {
						ret.Add(new Tuple<short, short>(workingPeriod.startPeriod, workingPeriod.endPeriod));
					} else if (workingPeriod.endPeriod <= end) {
						ret.Add(new Tuple<short, short>(workingPeriod.startPeriod, start));
					} else {
						ret.Add(new Tuple<short, short>(workingPeriod.startPeriod, start));
						ret.Add(new Tuple<short, short>(end, workingPeriod.endPeriod));
					}
				} else if (workingPeriod.startPeriod >= end) {
					ret.Add(new Tuple<short, short>(workingPeriod.startPeriod, workingPeriod.endPeriod));
				} else if (workingPeriod.startPeriod < end) {
					if (workingPeriod.endPeriod > end) {
						ret.Add(new Tuple<short, short>(end, workingPeriod.endPeriod));
					}
				} else {
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
		public int getWorkingHoursBetween(DateTime start, DateTime end) {
			return (int)Math.Floor(((double)this.getWorkingMinutesBetween(start, end) / MINS_PER_HOUR));
		}

		/// <summary>
		/// Calcula o número de minutos úteis entre dois DateTime's.
		/// </summary>
		/// <param name="start">data inicial.</param>
		/// <param name="end">data final.</param>
		/// <returns>Minutos úteis entre os dois DateTime's.</returns>
		public long getWorkingMinutesBetween(DateTime start, DateTime end) {
			int endIndex = DateUtils.toMJD(end) - this.mjdTableStart + 1;
			int startIndex = DateUtils.toMJD(start) - DateUtils.toMJD(this.tableStart) + 1;

			// Extende a tabela para cima ou para baixo se necessario
			if (endIndex >= this.workDay.Length || startIndex >= this.workDay.Length) {
				this.expandTableEnd(end.CompareTo(start) > 0 ? LocalDateTime.Parse(end.ToString()) : LocalDateTime.Parse(start.ToString()));
			}
			if (startIndex < 1 || endIndex < 1) {
				this.expandTableStart(end.CompareTo(start) < 0 ? LocalDateTime.Parse(end.ToString()) : LocalDateTime.Parse(start.ToString()));
				endIndex = DateUtils.toMJD(end) - this.mjdTableStart + 1;
				startIndex = DateUtils.toMJD(start) - DateUtils.toMJD(this.tableStart) + 1;
			}

			// Recupera os minutos uteis do periodo informado.
			long minutes = this.getWorkingMinutesSum(endIndex) - this.getWorkingMinutesSum(startIndex);
			// Calcula os minutos uteis do primeiro dia do bloco
			DateTime firstDayEnd = (new DateTime(start.Year, start.Month, start.Day, 0, 0, 0).AddMinutes(this.workDay[startIndex].getMaxEndDayPart()));
			if (start.CompareTo(firstDayEnd) < 0) {
				minutes += this.workDay[startIndex].getMinutesStartIn(new LocalTime(start.Hour, start.Minute, start.Second));
			}
			// Calcula os minutos uteis do ultimo dia do bloco
			DateTime lastDayEnd = (new DateTime(end.Year, end.Month, end.Day, 0, 0, 0).AddMinutes(this.workDay[endIndex].getMaxEndDayPart()));
			if (end.CompareTo(lastDayEnd) < 0) {
				int tmp1 = this.workDay[endIndex].getMinutesStartIn(new LocalTime(end.Hour, end.Minute, end.Second));
				int tmp2 = this.workDay[endIndex].getMinutesOfDay();
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
		public DateTime addWorkingHours(DateTime original, int hours, int minutes) {
			// Verifica se deve expandir o inicio da tabela.
			if (original.CompareTo(DateTime.Parse(this.tableStart.ToString())) < 0) {
				this.expandTableStart(LocalDateTime.Parse(original.ToString()));
			}
			// Recupera os minutos totais a serem adicionados
			int totalMinutes = minutes + (hours * MINS_PER_HOUR);
			if (totalMinutes == 0) {
				return original;
			}
			// Recupera o dia do ano a ser trabalhado
			int day = DateUtils.toMJD(original) - this.mjdTableStart + 1;
			// Recupera a contagem de minutos iniciais para a data "original", antes da adiçao.
			long firstDayTotal = this.getWorkingMinutesSum(day - 1) + this.getWorkingMinutesBetween(
				new DateTime(original.Year, original.Month, original.Day, 0, 0, 0),
				original
			);
			// Se nao houver espaço no dia para adicionar o horario necessario, verifica no dia seguinte
			while ( (this.getWorkingMinutesSum(day) - firstDayTotal) < totalMinutes) {
				day++;
			}
			long delta = this.workingMinutesSum[day - 1] - firstDayTotal;

			// Recupera a data onde serao inseridos os minutos uteis
			LocalDateTime resultedDate = DateUtils.fromMJD(day + this.mjdTableStart - 1);
			// Calcula os minutos uteis a serem adicionados
			int minutesToAdd = (int)(totalMinutes - delta + this.workDay[day].getMinStartDayPart()); //this.workStart[day]);

			// Conta minutos dos intervalos entre os períodos e adiciona ao valor de minutos a serem adicionados.
			short endMinutesPreviousSlice = 0;
			foreach (SimpleWorkingDay daySlice in this.workDay[day].getDayParts()) {
				if (endMinutesPreviousSlice != 0 && minutesToAdd > endMinutesPreviousSlice && minutesToAdd > daySlice.getDayStart()) {
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
		public LocalDateTime addWorkingHours(LocalDateTime original, int hours, int minutes) {
			// Verifica se deve expandir o inicio da tabela.
			if (original.CompareTo(this.tableStart) < 0) {
				this.expandTableStart(original);
			}
			// Recupera os minutos totais a serem adicionados
			int totalMinutes = minutes + (hours * MINS_PER_HOUR);
			if (totalMinutes == 0) {
				return original;
			}
			// Recupera o dia do ano a ser trabalhado
			int day = DateUtils.toMJD(original) - this.mjdTableStart + 1;
			// Recupera a contagem de minutos iniciais para a data "original", antes da adiçao.
			long firstDayTotal = this.getWorkingMinutesSum(day - 1) + this.getWorkingMinutesBetween(
				new DateTime(original.Year, original.Month, original.Day, 0, 0, 0),
				DateTime.Parse(original.ToString())
			);
			// Se nao houver espaço no dia para adicionar o horario necessario, verifica no dia seguinte
			while (this.getWorkingMinutesSum(day) - firstDayTotal < totalMinutes) {
				day++;
			}
			// Total de minutos consumidos em outros dias anteriores a data atual
			long delta = this.workingMinutesSum[day - 1] - firstDayTotal;

			// Recupera a data onde serao inseridos os minutos uteis
			LocalDateTime resultedDate = DateUtils.fromMJD(day + this.mjdTableStart - 1);

			// Total de Minutos que sobraram para serem adicionados na data atual
			int minutesToAdd = (int)((totalMinutes - delta) + this.workDay[day].getMinStartDayPart());

			// Conta minutos dos intervalos entre os períodos e adiciona ao valor de minutos a serem adicionados.
			short endMinutesPreviousSlice = 0;
			foreach (SimpleWorkingDay daySlice in this.workDay[day].getDayParts()) {
				if (endMinutesPreviousSlice != 0 && minutesToAdd > endMinutesPreviousSlice && minutesToAdd > daySlice.getDayStart()) {
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
		private long getWorkingMinutesSum(int day) {
			if (day >= this.workingMinutesSum.Length) {
				LocalDateTime end = this.tableStart.PlusDays(day + DEFAULT_TABLE_EXPANSION);
				this.expandTableEnd(end);
			}

			return this.workingMinutesSum[day];
		}
	}
}