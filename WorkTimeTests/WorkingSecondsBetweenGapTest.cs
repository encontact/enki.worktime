using enki.libs.workhours;
using enki.libs.workhours.domain;
using NodaTime;
using System;
using System.Collections.Generic;
using Xunit;

namespace enki.tests.libs.date
{
    /// <summary>
    /// Testes do cálculo de segundos úteis quando o dia possui mais de um período de trabalho
    /// (gap/intervalo entre períodos, ex: pausa de almoço ou expediente noturno que cruza o dia).
    ///
    /// Cenário de bug (PROD): eventos que iniciam dentro do gap retornavam segundos NEGATIVOS
    /// (ex: -57), pois o ajuste de segundos de getWorkingSecondsBetween considera apenas o
    /// envelope do dia (getMinStartDayPart..getMaxEndDayPart) e não os períodos úteis reais.
    /// </summary>
    public class WorkingSecondsBetweenGapTest
    {
        /// <summary>
        /// Semana seg-sex com dois períodos por dia: 08:00-12:00 e 14:00-18:00 (gap de almoço).
        /// </summary>
        private static WorkingHoursTable CreateTableWithLunchGap()
        {
            var week = new ComplexWorkingWeek();
            for (var day = (int)IsoDayOfWeek.Monday; day <= (int)IsoDayOfWeek.Friday; day++)
            {
                week.setWorkPeriod(day, day, new LocalTime(8, 0), new LocalTime(12, 0));
                week.setWorkPeriod(day, day, new LocalTime(14, 0), new LocalTime(18, 0));
            }

            return new WorkingHoursTable(
                week, new List<WorkingDaySlice>(), new SortedSet<WorkingDaySlice>(),
                new LocalDateTime(2026, 7, 13, 0, 0, 0), new LocalDateTime(2026, 7, 14, 0, 0, 0)
            );
        }

        /// <summary>
        /// Configuração real que reproduziu o bug em produção (caixa 2303):
        /// períodos que cruzam o dia (23:00 -> 10:59 do dia seguinte), gerando em cada dia útil
        /// os períodos [00:00-10:59] e [23:00-23:59] com um gap de ~12h no meio do envelope.
        /// </summary>
        private static WorkingHoursTable CreateTableWithOvernightPeriods()
        {
            var week = new ComplexWorkingWeek();
            week.setWorkPeriod((int)IsoDayOfWeek.Monday, (int)IsoDayOfWeek.Tuesday, new LocalTime(23, 0), new LocalTime(10, 59));
            week.setWorkPeriod((int)IsoDayOfWeek.Tuesday, (int)IsoDayOfWeek.Wednesday, new LocalTime(23, 0), new LocalTime(10, 59));
            week.setWorkPeriod((int)IsoDayOfWeek.Wednesday, (int)IsoDayOfWeek.Thursday, new LocalTime(23, 0), new LocalTime(10, 59));
            week.setWorkPeriod((int)IsoDayOfWeek.Thursday, (int)IsoDayOfWeek.Friday, new LocalTime(23, 0), new LocalTime(10, 59));
            week.setWorkPeriod((int)IsoDayOfWeek.Friday, (int)IsoDayOfWeek.Monday, new LocalTime(23, 0), new LocalTime(11, 0));

            return new WorkingHoursTable(
                week, new List<WorkingDaySlice>(), new SortedSet<WorkingDaySlice>(),
                new LocalDateTime(2026, 6, 15, 0, 0, 0), new LocalDateTime(2026, 6, 16, 0, 0, 0)
            );
        }

        [Fact]
        public void GetWorkingSecondsBetween_EventoRealDaCaixa2303DentroDoGap_DeveSerZero()
        {
            var table = CreateTableWithOvernightPeriods();

            // Evento real 103683746: 16/06/2026 (terça) 11:04:59 -> 11:06:02.
            // O evento inteiro está dentro do gap 10:59-23:00, portanto o tempo útil é 0.
            // Bug atual: retorna -57 (0 minutos úteis + 2s do fim - 59s do início).
            var seconds = table.getWorkingSecondsBetween(
                new DateTime(2026, 6, 16, 11, 4, 59),
                new DateTime(2026, 6, 16, 11, 6, 2)
            );

            Assert.Equal(0, seconds);
        }

        [Fact]
        public void GetWorkingSecondsBetween_InicioEFimDentroDoGapDeAlmoco_DeveSerZero()
        {
            var table = CreateTableWithLunchGap();

            // 14/07/2026 é terça. Evento inteiro dentro do gap 12:00-14:00: tempo útil real = 0.
            // Bug atual: retorna -59 (subtrai os segundos do início por estar dentro do envelope do dia).
            var seconds = table.getWorkingSecondsBetween(
                new DateTime(2026, 7, 14, 12, 30, 59),
                new DateTime(2026, 7, 14, 12, 45, 0)
            );

            Assert.Equal(0, seconds);
        }

        [Fact]
        public void GetWorkingSecondsBetween_InicioNoGapCruzandoMinuto_DeveSerZero()
        {
            var table = CreateTableWithLunchGap();

            // Evento de 1 segundo dentro do gap, cruzando a fronteira do minuto (:59 -> :00).
            // Tempo útil real = 0. Bug atual: retorna -59.
            var seconds = table.getWorkingSecondsBetween(
                new DateTime(2026, 7, 14, 12, 30, 59),
                new DateTime(2026, 7, 14, 12, 31, 0)
            );

            Assert.Equal(0, seconds);
        }

        [Fact]
        public void GetWorkingSecondsBetween_InicioNoGapEFimNoPeriodoSeguinte_ContaApenasTempoUtil()
        {
            var table = CreateTableWithLunchGap();

            // Início no gap (12:30:59) e fim 30s após o retorno do expediente (14:00:30).
            // Tempo útil real = 30s (14:00:00 -> 14:00:30). Bug atual: retorna -29.
            var seconds = table.getWorkingSecondsBetween(
                new DateTime(2026, 7, 14, 12, 30, 59),
                new DateTime(2026, 7, 14, 14, 0, 30)
            );

            Assert.Equal(30, seconds);
        }

        [Fact]
        public void GetWorkingSecondsBetween_FimDentroDoGap_NaoDeveSomarSegundosDoFim()
        {
            var table = CreateTableWithLunchGap();

            // Início em período útil (11:30:00) e fim dentro do gap (12:30:45).
            // Tempo útil real = 30min (11:30:00 -> 12:00:00) = 1800s.
            // Bug atual: retorna 1845 (soma os 45s do fim por estar dentro do envelope do dia).
            var seconds = table.getWorkingSecondsBetween(
                new DateTime(2026, 7, 14, 11, 30, 0),
                new DateTime(2026, 7, 14, 12, 30, 45)
            );

            Assert.Equal(1800, seconds);
        }
    }
}
