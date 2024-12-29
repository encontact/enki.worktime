using enki.libs.workhours;
using enki.libs.workhours.domain;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace enki.tests.libs.date
{
    public class WorkingHoursUtilsTest
    {

        [Fact]
        public void testWorkingMinutesBetween()
        {
            WorkingWeek workingWeek = ComplexWorkingWeek.getWeek8x7();

            WorkingHoursTable tabela = new WorkingHoursTable(workingWeek, new LocalDateTime(2000, 1, 3, 0, 0, 0), new LocalDateTime(2000, 1, 4, 0, 0, 0));

            // testes em um único dia
            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 1, 0, 0, 0, 0), new DateTime(2000, 1, 1, 0, 0, 0, 0)));
            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 1, 9, 0, 0, 0), new DateTime(2000, 1, 1, 9, 0, 0, 0)));
            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 1, 20, 0, 0, 0), new DateTime(2000, 1, 1, 20, 0, 0, 0)));

            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 1, 0, 0, 0, 0), new DateTime(2000, 1, 1, 1, 0, 0, 0)));
            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 1, 22, 0, 0, 0), new DateTime(2000, 1, 1, 23, 0, 0, 0)));

            Assert.Equal(8 * 60, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 1, 0, 0, 0, 0), new DateTime(2000, 1, 1, 23, 0, 0, 0)));
            Assert.Equal(60, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 1, 8, 0, 0, 0), new DateTime(2000, 1, 1, 9, 0, 0, 0)));

            // testes usando somente um dia sem horas úteis
            tabela = new WorkingHoursTable(ComplexWorkingWeek.getWeek8x5(), new LocalDateTime(2000, 1, 3, 0, 0, 0), new LocalDateTime(2000, 1, 4, 0, 0, 0));
            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 8, 0, 0, 0, 0), new DateTime(2000, 1, 8, 0, 0, 0, 0)));
            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 8, 9, 0, 0, 0), new DateTime(2000, 1, 8, 9, 0, 0, 0)));
            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 8, 20, 0, 0, 0), new DateTime(2000, 1, 8, 20, 0, 0, 0)));

            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 8, 0, 0, 0, 0), new DateTime(2000, 1, 8, 1, 0, 0, 0)));
            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 8, 22, 0, 0, 0), new DateTime(2000, 1, 8, 23, 0, 0, 0)));

            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 8, 0, 0, 0, 0), new DateTime(2000, 1, 8, 23, 0, 0, 0)));
            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 8, 8, 0, 0, 0), new DateTime(2000, 1, 8, 9, 0, 0, 0)));

            // testes usando dois dias com segundo dia sem nenhuma hora útil
            Assert.Equal(8 * 60, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 7, 0, 0, 0, 0), new DateTime(2000, 1, 8, 23, 0, 0, 0)));
            Assert.Equal(60, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 7, 15, 0, 0, 0), new DateTime(2000, 1, 8, 23, 0, 0, 0)));
            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 7, 23, 0, 0, 0), new DateTime(2000, 1, 8, 23, 0, 0, 0)));

            // testes usando dois dias com primeiro dia sem nenhuma hora útil
            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 9, 0, 0, 0, 0), new DateTime(2000, 1, 10, 0, 0, 0, 0)));
            Assert.Equal(60, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 9, 0, 0, 0, 0), new DateTime(2000, 1, 10, 9, 0, 0, 0)));
            Assert.Equal(8 * 60, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 9, 0, 0, 0, 0), new DateTime(2000, 1, 10, 23, 0, 0, 0)));

            // testes atravessando um readonlyde semana
            Assert.Equal(120, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 7, 15, 0, 0, 0), new DateTime(2000, 1, 10, 9, 0, 0, 0)));
            Assert.Equal(60, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 7, 15, 0, 0, 0), new DateTime(2000, 1, 10, 7, 0, 0, 0)));
            Assert.Equal(60, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 7, 15, 0, 0, 0), new DateTime(2000, 1, 10, 8, 0, 0, 0)));
            Assert.Equal(61, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 7, 15, 0, 0, 0), new DateTime(2000, 1, 10, 8, 1, 0, 0)));
            Assert.Equal(60, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 7, 16, 0, 0, 0), new DateTime(2000, 1, 10, 9, 0, 0, 0)));
            Assert.Equal(60, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 7, 17, 0, 0, 0), new DateTime(2000, 1, 10, 9, 0, 0, 0)));
            Assert.Equal(61, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 7, 15, 59, 0, 0), new DateTime(2000, 1, 10, 9, 0, 0, 0)));

            // teste com período grande
            tabela = new WorkingHoursTable(workingWeek, new LocalDateTime(1978, 7, 14, 0, 0, 0), new LocalDateTime(2009, 9, 15, 0, 0, 0));
            Assert.Equal(11373 * 8 * 60, tabela.getWorkingMinutesBetween(new DateTime(1978, 7, 29, 0, 0, 0, 0), new DateTime(2009, 9, 17, 0, 0,
                    0, 0)));
        }

        [Fact]
        public void testWorkingHoursBetween()
        {
            WorkingWeek workingWeek = ComplexWorkingWeek.getWeek8x7();

            WorkingHoursTable tabela = new WorkingHoursTable(workingWeek, new LocalDateTime(2000, 1, 3, 0, 0, 0), new LocalDateTime(2000, 1, 4, 0, 0, 0));

            // testes em um único dia
            Assert.Equal(0, tabela.getWorkingHoursBetween(new DateTime(2000, 1, 1, 0, 0, 0, 0), new DateTime(2000, 1, 1, 0, 0, 0, 0)));
            Assert.Equal(0, tabela.getWorkingHoursBetween(new DateTime(2000, 1, 1, 9, 0, 0, 0), new DateTime(2000, 1, 1, 9, 0, 0, 0)));
            Assert.Equal(0, tabela.getWorkingHoursBetween(new DateTime(2000, 1, 1, 20, 0, 0, 0), new DateTime(2000, 1, 1, 20, 0, 0, 0)));

            Assert.Equal(0, tabela.getWorkingHoursBetween(new DateTime(2000, 1, 1, 0, 0, 0, 0), new DateTime(2000, 1, 1, 1, 0, 0, 0)));
            Assert.Equal(0, tabela.getWorkingHoursBetween(new DateTime(2000, 1, 1, 22, 0, 0, 0), new DateTime(2000, 1, 1, 23, 0, 0, 0)));

            Assert.Equal(8, tabela.getWorkingHoursBetween(new DateTime(2000, 1, 1, 0, 0, 0, 0), new DateTime(2000, 1, 1, 23, 0, 0, 0)));
            Assert.Equal(1, tabela.getWorkingHoursBetween(new DateTime(2000, 1, 1, 8, 0, 0, 0), new DateTime(2000, 1, 1, 9, 0, 0, 0)));

            // testes usando somente um dia sem horas úteis
            tabela = new WorkingHoursTable(ComplexWorkingWeek.getWeek8x5(), new LocalDateTime(2000, 1, 3, 0, 0, 0), new LocalDateTime(2000, 1, 4, 0, 0, 0));
            Assert.Equal(0, tabela.getWorkingHoursBetween(new DateTime(2000, 1, 8, 0, 0, 0, 0), new DateTime(2000, 1, 8, 0, 0, 0, 0)));
            Assert.Equal(0, tabela.getWorkingHoursBetween(new DateTime(2000, 1, 8, 9, 0, 0, 0), new DateTime(2000, 1, 8, 9, 0, 0, 0)));
            Assert.Equal(0, tabela.getWorkingHoursBetween(new DateTime(2000, 1, 8, 20, 0, 0, 0), new DateTime(2000, 1, 8, 20, 0, 0, 0)));

            Assert.Equal(0, tabela.getWorkingHoursBetween(new DateTime(2000, 1, 8, 0, 0, 0, 0), new DateTime(2000, 1, 8, 1, 0, 0, 0)));
            Assert.Equal(0, tabela.getWorkingHoursBetween(new DateTime(2000, 1, 8, 22, 0, 0, 0), new DateTime(2000, 1, 8, 23, 0, 0, 0)));

            Assert.Equal(0, tabela.getWorkingHoursBetween(new DateTime(2000, 1, 8, 0, 0, 0, 0), new DateTime(2000, 1, 8, 23, 0, 0, 0)));
            Assert.Equal(0, tabela.getWorkingHoursBetween(new DateTime(2000, 1, 8, 8, 0, 0, 0), new DateTime(2000, 1, 8, 9, 0, 0, 0)));

            // testes usando dois dias com segundo dia sem nenhuma hora útil
            Assert.Equal(8, tabela.getWorkingHoursBetween(new DateTime(2000, 1, 7, 0, 0, 0, 0), new DateTime(2000, 1, 8, 23, 0, 0, 0)));
            Assert.Equal(1, tabela.getWorkingHoursBetween(new DateTime(2000, 1, 7, 15, 0, 0, 0), new DateTime(2000, 1, 8, 23, 0, 0, 0)));
            Assert.Equal(0, tabela.getWorkingHoursBetween(new DateTime(2000, 1, 7, 23, 0, 0, 0), new DateTime(2000, 1, 8, 23, 0, 0, 0)));

            // testes usando dois dias com primeiro dia sem nenhuma hora útil
            Assert.Equal(0, tabela.getWorkingHoursBetween(new DateTime(2000, 1, 9, 0, 0, 0, 0), new DateTime(2000, 1, 10, 0, 0, 0, 0)));
            Assert.Equal(1, tabela.getWorkingHoursBetween(new DateTime(2000, 1, 9, 0, 0, 0, 0), new DateTime(2000, 1, 10, 9, 0, 0, 0)));
            Assert.Equal(8, tabela.getWorkingHoursBetween(new DateTime(2000, 1, 9, 0, 0, 0, 0), new DateTime(2000, 1, 10, 23, 0, 0, 0)));

            // testes atravessando um readonlyde semana
            Assert.Equal(2, tabela.getWorkingHoursBetween(new DateTime(2000, 1, 7, 15, 0, 0, 0), new DateTime(2000, 1, 10, 9, 0, 0, 0)));
            Assert.Equal(1, tabela.getWorkingHoursBetween(new DateTime(2000, 1, 7, 15, 0, 0, 0), new DateTime(2000, 1, 10, 7, 0, 0, 0)));
            Assert.Equal(1, tabela.getWorkingHoursBetween(new DateTime(2000, 1, 7, 15, 0, 0, 0), new DateTime(2000, 1, 10, 8, 0, 0, 0)));
            Assert.Equal(1, tabela.getWorkingHoursBetween(new DateTime(2000, 1, 7, 15, 0, 0, 0), new DateTime(2000, 1, 10, 8, 1, 0, 0)));
            Assert.Equal(1, tabela.getWorkingHoursBetween(new DateTime(2000, 1, 7, 16, 0, 0, 0), new DateTime(2000, 1, 10, 9, 0, 0, 0)));
            Assert.Equal(1, tabela.getWorkingHoursBetween(new DateTime(2000, 1, 7, 17, 0, 0, 0), new DateTime(2000, 1, 10, 9, 0, 0, 0)));
            Assert.Equal(1, tabela.getWorkingHoursBetween(new DateTime(2000, 1, 7, 15, 59, 0, 0), new DateTime(2000, 1, 10, 9, 0, 0, 0)));

            // teste com período grande
            tabela = new WorkingHoursTable(workingWeek, new LocalDateTime(1978, 7, 15, 0, 0, 0), new LocalDateTime(1978, 7, 16, 0, 0, 0));
            Assert.Equal(11373 * 8, tabela.getWorkingHoursBetween(new DateTime(1978, 7, 29, 0, 0, 0, 0), new DateTime(2009, 9, 17, 0, 0, 0, 0)));
        }

        [Fact]
        public void testWorkingHoursBetweenWithExceptions()
        {
            SortedSet<WorkingDaySlice> exceptions = new SortedSet<WorkingDaySlice>();
            // Registra feriado de trabalho das 00:00 até o 12:00 do dia 03/01/2000
            exceptions.Add(new SimpleWorkingDay(new LocalDateTime(2000, 1, 3, 0, 0, 0), new LocalTime(0, 0, 0), new LocalTime(12, 0, 0)));
            // Registra feriado no dia 06/01/2000 das 08:00 as 16:00 (ou seja, o dia período todo do dia não trabalhado)
            exceptions.Add(new SimpleWorkingDay(new LocalDateTime(2000, 1, 6, 0, 0, 0), new LocalTime(8, 0, 0), new LocalTime(16, 0, 0)));

            WorkingHoursTable tabela = new WorkingHoursTable(ComplexWorkingWeek.getWeek8x5(), new List<WorkingDaySlice>(), exceptions, new LocalDateTime(2000, 1, 3, 0, 0, 0), new LocalDateTime(2000, 1, 4, 0, 0, 0));
            Assert.Equal(28, tabela.getWorkingHoursBetween(new DateTime(2000, 1, 1, 0, 0, 0, 0), new DateTime(2000, 1, 10, 0, 0, 0, 0)));
        }

        [Fact]
        public void testRecurrentExceptions()
        {
            List<WorkingDaySlice> recurrentExceptions = new List<WorkingDaySlice>();
            SortedSet<WorkingDaySlice> exceptions = new SortedSet<WorkingDaySlice>();

            DateTime min = new DateTime(2000, 1, 1, 0, 0, 0, 0);
            DateTime max = new DateTime(2010, 12, 31, 23, 59, 59, 999);
            long total = 1377600;

            // Verifica o período total
            WorkingHoursTable tabela = new WorkingHoursTable(ComplexWorkingWeek.getWeek8x5(), new LocalDateTime(2000, 1, 1, 0, 0, 0), new LocalDateTime(2010, 12, 31, 0, 0, 0));
            Assert.Equal(total, tabela.getWorkingMinutesBetween(min, max));

            // Verifica o periodo total com um feriado
            recurrentExceptions.Add(new SimpleWorkingDay(new LocalDateTime(2010, 7, 3, 0, 0, 0), (short)480, (short)960));
            tabela = new WorkingHoursTable(ComplexWorkingWeek.getWeek8x5(), recurrentExceptions, exceptions, new LocalDateTime(2000, 1, 1, 0, 0, 0), new LocalDateTime(2010, 12, 31, 0, 0, 0));
            var HorasDoFeriadoNosAnos = 8 * 8 * 60; // 8 horas dia vezes 8 dias no período que são dias comerciais vezes 60 minutos por dia.
            Assert.Equal(total - HorasDoFeriadoNosAnos, tabela.getWorkingMinutesBetween(min, max));

            // Verifica o periodo com o feriado em outra data
            recurrentExceptions.Clear();
            recurrentExceptions.Add(new SimpleWorkingDay(new LocalDateTime(2010, 1, 3, 0, 0, 0), (short)0, (short)1440));
            tabela = new WorkingHoursTable(ComplexWorkingWeek.getWeek8x5(), recurrentExceptions, exceptions, new LocalDateTime(2000, 1, 1, 0, 0, 0), new LocalDateTime(2010, 12, 31, 0, 0, 0));
            Assert.Equal(total - HorasDoFeriadoNosAnos, tabela.getWorkingMinutesBetween(min, max));

            // Teste considerando que exceções normais devem se sobrepor às recorrentes
            exceptions.Add(new SimpleWorkingDay(new LocalDateTime(2000, 1, 3, 0, 0, 0), new LocalTime(0, 0, 0), new LocalTime(7, 0, 0)));
            tabela = new WorkingHoursTable(ComplexWorkingWeek.getWeek8x5(), recurrentExceptions, exceptions, new LocalDateTime(2000, 1, 1, 0, 0, 0), new LocalDateTime(2010, 12, 31, 0, 0, 0));
            var HorasDoFeriadoNosAnosMenosUm = 7 * 8 * 60;
            Assert.Equal(total - HorasDoFeriadoNosAnosMenosUm, tabela.getWorkingMinutesBetween(min, max));
        }

        [Fact]
        public void testOutOfBounds()
        {
            WorkingHoursTable tabela = new WorkingHoursTable(ComplexWorkingWeek.getWeek8x5(), new LocalDateTime(2000, 1, 3, 0, 0, 0), new LocalDateTime(2000, 1, 4, 0, 0, 0));
            tabela.getWorkingHoursBetween(new DateTime(1999, 12, 31, 23, 59, 59, 999), new DateTime(2000, 1, 10, 0, 0, 0, 0));
            tabela.getWorkingHoursBetween(new DateTime(2001, 1, 1, 0, 0, 0, 0), new DateTime(2000, 1, 10, 0, 0, 0, 0));
            tabela.getWorkingHoursBetween(new DateTime(2000, 1, 10, 0, 0, 0, 0), new DateTime(1999, 12, 31, 23, 59, 59, 999));
            tabela.getWorkingHoursBetween(new DateTime(2000, 1, 10, 0, 0, 0, 0), new DateTime(2001, 1, 1, 0, 0, 0, 0));
        }

        [Fact]
        public void testAddWorkingHoursDateTime()
        {
            WorkingHoursTable tabela = new WorkingHoursTable(ComplexWorkingWeek.getWeek8x5(), new LocalDateTime(1999, 1, 3, 0, 0, 0), new LocalDateTime(2000, 1, 4, 0, 0, 0));

            Assert.Equal(new DateTime(2000, 1, 3, 16, 0, 0, 0), tabela.addWorkingHours(new DateTime(2000, 1, 1, 0, 0, 0, 0), 8, 0));
            Assert.Equal(new DateTime(2000, 1, 1, 0, 0, 0, 0), tabela.addWorkingHours(new DateTime(2000, 1, 1, 0, 0, 0, 0), 0, 0));
            Assert.Equal(new DateTime(2000, 1, 7, 16, 0, 0, 0), tabela.addWorkingHours(new DateTime(2000, 1, 1, 0, 0, 0, 0), 40, 0));
            Assert.Equal(new DateTime(2000, 1, 10, 9, 0, 0, 0), tabela.addWorkingHours(new DateTime(2000, 1, 1, 0, 0, 0, 0), 41, 0));
            Assert.Equal(new DateTime(2000, 1, 14, 16, 0, 0, 0), tabela.addWorkingHours(new DateTime(2000, 1, 1, 0, 0, 0, 0), 80, 0));
            Assert.Equal(new DateTime(2009, 11, 19, 9, 0, 0, 0), tabela.addWorkingHours(new DateTime(2009, 11, 18, 17, 7, 43, 860), 1, 0));
            Assert.Equal(new DateTime(2009, 11, 19, 8, 7, 0, 0), tabela.addWorkingHours(new DateTime(2009, 11, 18, 15, 7, 43, 860), 1, 0));
            Assert.Equal(new DateTime(2009, 11, 19, 8, 8, 0, 0), tabela.addWorkingHours(new DateTime(2009, 11, 18, 15, 8, 0, 0), 1, 0));
            Assert.Equal(new DateTime(2000, 1, 3, 0, 0, 0, 0), tabela.addWorkingHours(new DateTime(2000, 1, 3, 0, 0, 0, 0), 0, 0));

            SortedSet<WorkingDaySlice> exceptions = new SortedSet<WorkingDaySlice>();
            // Registra feriado no período das 00:00 até as 12:00, trabalhando o restante do expediente normalmente
            exceptions.Add(new SimpleWorkingDay(new LocalDateTime(2000, 1, 3, 0, 0, 0), new LocalTime(0, 0, 0), new LocalTime(12, 0, 0)));
            // Registra feriado para todo o dia 06/01
            exceptions.Add(new SimpleWorkingDay(new LocalDateTime(2000, 1, 6, 0, 0, 0), new LocalTime(0, 0, 0), new LocalTime(23, 59, 0)));

            tabela = new WorkingHoursTable(ComplexWorkingWeek.getWeek8x5(), new List<WorkingDaySlice>(), exceptions, new LocalDateTime(2000, 1, 3, 0, 0, 0), new LocalDateTime(2000, 1, 4, 0, 0, 0));
            Assert.Equal(new DateTime(2000, 1, 10, 8, 1, 0, 0), tabela.addWorkingHours(new DateTime(2000, 1, 1, 0, 0, 0, 0), 28, 1));

            DateTime now = new DateTime(2009, 9, 22, 0, 0, 0, 0);
            WorkingWeek week = ComplexWorkingWeek.getWeek8x5();
            WorkingHoursTable hoursTable = new WorkingHoursTable(week, now.AddDays(1), now.AddDays(2));
            DateTime addWorkingHours = hoursTable.addWorkingHours(now, 0, 60);
            Assert.Equal(new DateTime(2009, 9, 22, 9, 0, 0, 0), addWorkingHours);
            DateTime addWorkingHoursV2 = hoursTable.addWorkingHours(now, 1, 0);
            Assert.Equal(new DateTime(2009, 9, 22, 9, 0, 0, 0), addWorkingHoursV2);
        }

        [Fact]
        public void testAddWorkingHoursLocalDateTime()
        {
            WorkingHoursTable tabela = new WorkingHoursTable(ComplexWorkingWeek.getWeek8x5(), new LocalDateTime(1999, 1, 3, 0, 0, 0), new LocalDateTime(2000, 1, 4, 0, 0, 0));

            Assert.Equal(new LocalDateTime(2000, 1, 3, 16, 0, 0, 0), tabela.addWorkingHours(new LocalDateTime(2000, 1, 1, 0, 0, 0, 0), 8, 0));
            Assert.Equal(new LocalDateTime(2000, 1, 1, 0, 0, 0, 0), tabela.addWorkingHours(new LocalDateTime(2000, 1, 1, 0, 0, 0, 0), 0, 0));
            Assert.Equal(new LocalDateTime(2000, 1, 7, 16, 0, 0, 0), tabela.addWorkingHours(new LocalDateTime(2000, 1, 1, 0, 0, 0, 0), 40, 0));
            Assert.Equal(new LocalDateTime(2000, 1, 10, 9, 0, 0, 0), tabela.addWorkingHours(new LocalDateTime(2000, 1, 1, 0, 0, 0, 0), 41, 0));
            Assert.Equal(new LocalDateTime(2000, 1, 14, 16, 0, 0, 0), tabela.addWorkingHours(new LocalDateTime(2000, 1, 1, 0, 0, 0, 0), 80, 0));
            Assert.Equal(new LocalDateTime(2009, 11, 19, 9, 0, 0, 0), tabela.addWorkingHours(new LocalDateTime(2009, 11, 18, 17, 7, 43, 860), 1, 0));
            Assert.Equal(new LocalDateTime(2009, 11, 19, 8, 7, 0, 0), tabela.addWorkingHours(new LocalDateTime(2009, 11, 18, 15, 7, 43, 860), 1, 0));
            Assert.Equal(new LocalDateTime(2009, 11, 19, 8, 8, 0, 0), tabela.addWorkingHours(new LocalDateTime(2009, 11, 18, 15, 8, 0, 0), 1, 0));
            Assert.Equal(new LocalDateTime(2000, 1, 3, 0, 0, 0, 0), tabela.addWorkingHours(new LocalDateTime(2000, 1, 3, 0, 0, 0, 0), 0, 0));

            SortedSet<WorkingDaySlice> exceptions = new SortedSet<WorkingDaySlice>();
            exceptions.Add(new SimpleWorkingDay(new LocalDateTime(2000, 1, 3, 0, 0, 0), new LocalTime(0, 0, 0), new LocalTime(12, 0, 0)));
            exceptions.Add(new SimpleWorkingDay(new LocalDateTime(2000, 1, 6, 0, 0, 0), new LocalTime(0, 0, 0), new LocalTime(23, 59, 0)));

            tabela = new WorkingHoursTable(ComplexWorkingWeek.getWeek8x5(), new List<WorkingDaySlice>(), exceptions, new LocalDateTime(2000, 1, 3, 0, 0, 0), new LocalDateTime(2000, 1, 4, 0, 0, 0));
            Assert.Equal(new LocalDateTime(2000, 1, 10, 8, 1, 0, 0), tabela.addWorkingHours(new LocalDateTime(2000, 1, 1, 0, 0, 0, 0), 28, 1));

            LocalDateTime now = new LocalDateTime(2009, 9, 22, 0, 0, 0, 0);
            WorkingWeek week = ComplexWorkingWeek.getWeek8x5();
            WorkingHoursTable hoursTable = new WorkingHoursTable(week, now.PlusDays(1), now.PlusDays(2));
            LocalDateTime addWorkingHours = hoursTable.addWorkingHours(now, 0, 60);
            Assert.Equal(new LocalDateTime(2009, 9, 22, 9, 0, 0, 0), addWorkingHours);
        }

        [Fact]
        public void testWorkingMinutesBetweenWithPeriods()
        {
            WorkingWeek workingWeek = ComplexWorkingWeek.getWeek8x7In2Periods();

            WorkingHoursTable tabela = new WorkingHoursTable(workingWeek, new LocalDateTime(2000, 1, 3, 0, 0, 0), new LocalDateTime(2000, 1, 4, 0, 0, 0));

            // testes em um único dia
            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 1, 0, 0, 0, 0), new DateTime(2000, 1, 1, 0, 0, 0, 0)));
            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 1, 9, 0, 0, 0), new DateTime(2000, 1, 1, 9, 0, 0, 0)));
            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 1, 20, 0, 0, 0), new DateTime(2000, 1, 1, 20, 0, 0, 0)));

            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 1, 0, 0, 0, 0), new DateTime(2000, 1, 1, 1, 0, 0, 0)));
            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 1, 22, 0, 0, 0), new DateTime(2000, 1, 1, 23, 0, 0, 0)));

            Assert.Equal(8 * 60, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 1, 0, 0, 0, 0), new DateTime(2000, 1, 1, 23, 0, 0, 0)));
            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 1, 8, 0, 0, 0), new DateTime(2000, 1, 1, 9, 0, 0, 0)));
            Assert.Equal(60, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 1, 9, 0, 0, 0), new DateTime(2000, 1, 1, 10, 0, 0, 0)));

            // testes usando somente um dia sem horas úteis
            tabela = new WorkingHoursTable(ComplexWorkingWeek.getWeek8x5(), new LocalDateTime(2000, 1, 3, 0, 0, 0), new LocalDateTime(2000, 1, 4, 0, 0, 0));
            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 8, 0, 0, 0, 0), new DateTime(2000, 1, 8, 0, 0, 0, 0)));
            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 8, 9, 0, 0, 0), new DateTime(2000, 1, 8, 9, 0, 0, 0)));
            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 8, 20, 0, 0, 0), new DateTime(2000, 1, 8, 20, 0, 0, 0)));

            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 8, 0, 0, 0, 0), new DateTime(2000, 1, 8, 1, 0, 0, 0)));
            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 8, 22, 0, 0, 0), new DateTime(2000, 1, 8, 23, 0, 0, 0)));

            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 8, 0, 0, 0, 0), new DateTime(2000, 1, 8, 23, 0, 0, 0)));
            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 8, 8, 0, 0, 0), new DateTime(2000, 1, 8, 9, 0, 0, 0)));

            // testes usando dois dias com segundo dia sem nenhuma hora útil
            Assert.Equal(8 * 60, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 7, 0, 0, 0, 0), new DateTime(2000, 1, 8, 23, 0, 0, 0)));
            Assert.Equal(60, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 7, 15, 0, 0, 0), new DateTime(2000, 1, 8, 23, 0, 0, 0)));
            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 7, 23, 0, 0, 0), new DateTime(2000, 1, 8, 23, 0, 0, 0)));

            // testes usando dois dias com primeiro dia sem nenhuma hora útil
            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 9, 0, 0, 0, 0), new DateTime(2000, 1, 10, 0, 0, 0, 0)));
            Assert.Equal(60, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 9, 0, 0, 0, 0), new DateTime(2000, 1, 10, 9, 0, 0, 0)));
            Assert.Equal(8 * 60, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 9, 0, 0, 0, 0), new DateTime(2000, 1, 10, 23, 0, 0, 0)));

            // testes atravessando um readonlyde semana
            Assert.Equal(120, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 7, 15, 0, 0, 0), new DateTime(2000, 1, 10, 9, 0, 0, 0)));
            Assert.Equal(60, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 7, 15, 0, 0, 0), new DateTime(2000, 1, 10, 7, 0, 0, 0)));
            Assert.Equal(60, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 7, 15, 0, 0, 0), new DateTime(2000, 1, 10, 8, 0, 0, 0)));
            Assert.Equal(61, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 7, 15, 0, 0, 0), new DateTime(2000, 1, 10, 8, 1, 0, 0)));
            Assert.Equal(60, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 7, 16, 0, 0, 0), new DateTime(2000, 1, 10, 9, 0, 0, 0)));
            Assert.Equal(60, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 7, 17, 0, 0, 0), new DateTime(2000, 1, 10, 9, 0, 0, 0)));
            Assert.Equal(61, tabela.getWorkingMinutesBetween(new DateTime(2000, 1, 7, 15, 59, 0, 0), new DateTime(2000, 1, 10, 9, 0, 0, 0)));

            // teste com período grande
            tabela = new WorkingHoursTable(workingWeek, new LocalDateTime(1978, 7, 14, 0, 0, 0), new LocalDateTime(2009, 9, 15, 0, 0, 0));
            Assert.Equal(11373 * 8 * 60, tabela.getWorkingMinutesBetween(new DateTime(1978, 7, 29, 0, 0, 0, 0), new DateTime(2009, 9, 17, 0, 0,
                    0, 0)));

            Assert.Equal(new LocalDateTime(2000, 1, 1, 14, 30, 0, 0), tabela.addWorkingHours(new LocalDateTime(2000, 1, 1, 11, 30, 0, 0), 2, 0));

        }

        [Fact]
        public void testWorkingHoursTableExceptions()
        {
            short h = 60;
            short h_0000 = 0;
            short h_0800 = (short)(h * 8);
            short h_0900 = (short)(h * 9);
            short h_1200 = (short)(h * 12);
            short h_1300 = (short)(h * 13);
            short h_1800 = (short)(h * 18);
            short h_2400 = (short)(h * 24);
            short workDay = h_0800;

            WorkingWeek workingWeek = ComplexWorkingWeek.getWeek8x7In2Periods();
            List<WorkingDaySlice> recurrentExceptions = new List<WorkingDaySlice> {
                new SimpleWorkingDay(new DateTime(2011,12,29),h_0000,h_2400),
                new SimpleWorkingDay(new DateTime(2012,12,30),h_0000,h_2400),
                new SimpleWorkingDay(new DateTime(2012,12,31),h_0000,h_2400),
                new SimpleWorkingDay(new DateTime(2013,1,1),h_0000,h_2400),
                new SimpleWorkingDay(new DateTime(2013,1,2),h_0000,h_2400),
            };
            WorkingHoursTable tabela = new WorkingHoursTable(workingWeek, recurrentExceptions,
                new SortedSet<WorkingDaySlice>(), new LocalDateTime(2012, 12, 28, 0, 0, 0), new LocalDateTime(2013, 1, 2, 0, 0, 0));
            Assert.Equal(workDay, tabela.getWorkingMinutesBetween(new DateTime(2012, 12, 28, 0, 0, 0, 0), new DateTime(2013, 1, 2, 23, 59, 59, 0)));


            recurrentExceptions = new List<WorkingDaySlice> {
                new SimpleWorkingDay(new DateTime(2011,12,28),h_0000,h_2400),
                new SimpleWorkingDay(new DateTime(2012,12,31),h_0000,h_2400),
                new SimpleWorkingDay(new DateTime(2013,1,1),h_0000,h_2400),
                new SimpleWorkingDay(new DateTime(2013,1,2),h_0000,h_2400),
            };
            tabela = new WorkingHoursTable(workingWeek, recurrentExceptions,
                new SortedSet<WorkingDaySlice>(), new LocalDateTime(2012, 12, 28, 0, 0, 0), new LocalDateTime(2013, 1, 3, 0, 0, 0));
            Assert.Equal(workDay * 3, tabela.getWorkingMinutesBetween(new DateTime(2012, 12, 28, 0, 0, 0, 0), new DateTime(2013, 1, 3, 23, 59, 59, 0)));

            recurrentExceptions = new List<WorkingDaySlice> {
                new SimpleWorkingDay(new DateTime(2012,12,28),h_0000,h_2400),
                new SimpleWorkingDay(new DateTime(2012,12,29),h_0000,h_2400),
                new SimpleWorkingDay(new DateTime(2012,12,30),h_0000,h_2400),
                new SimpleWorkingDay(new DateTime(2012,12,31),h_0000,h_2400),
                new SimpleWorkingDay(new DateTime(2013,1,1),h_0000,h_2400),
                new SimpleWorkingDay(new DateTime(2013,1,2),h_0000,h_2400),
            };
            tabela = new WorkingHoursTable(workingWeek, recurrentExceptions,
                new SortedSet<WorkingDaySlice>(), new LocalDateTime(2012, 12, 28, 0, 0, 0), new LocalDateTime(2013, 1, 2, 0, 0, 0));
            Assert.Equal(workDay * 0, tabela.getWorkingMinutesBetween(new DateTime(2012, 12, 28, 0, 0, 0, 0), new DateTime(2013, 1, 2, 23, 59, 59, 0)));

            recurrentExceptions = new List<WorkingDaySlice> {
                new SimpleWorkingDay(new DateTime(2012,12,28),h_0000,h_1200),
                new SimpleWorkingDay(new DateTime(2012,12,30),h_0000,h_2400),
                new SimpleWorkingDay(new DateTime(2012,12,31),h_0000,h_2400),
                new SimpleWorkingDay(new DateTime(2013,1,1),h_0000,h_2400),
                new SimpleWorkingDay(new DateTime(2013,1,2),h_0000,h_2400),
            };
            tabela = new WorkingHoursTable(workingWeek, recurrentExceptions,
                new SortedSet<WorkingDaySlice>(), new LocalDateTime(2012, 12, 28, 0, 0, 0), new LocalDateTime(2013, 1, 2, 0, 0, 0));
            Assert.Equal(workDay + (h * 5), tabela.getWorkingMinutesBetween(new DateTime(2012, 12, 28, 0, 0, 0, 0), new DateTime(2013, 1, 2, 23, 59, 59, 0)));

            // Como os feriados são recorrentes, todo o período é desconsiderado no cálculo.
            recurrentExceptions = new List<WorkingDaySlice> {
                new SimpleWorkingDay(new DateTime(2011,12,28),h_0000,h_2400),
                new SimpleWorkingDay(new DateTime(2011,12,29),h_0000,h_2400),
                new SimpleWorkingDay(new DateTime(2012,12,30),h_0000,h_2400),
                new SimpleWorkingDay(new DateTime(2012,12,31),h_0000,h_2400),
                new SimpleWorkingDay(new DateTime(2013,1,1),h_0000,h_2400),
                new SimpleWorkingDay(new DateTime(2013,1,2),h_0000,h_2400),
            };
            tabela = new WorkingHoursTable(workingWeek, recurrentExceptions,
                new SortedSet<WorkingDaySlice>(), new LocalDateTime(2012, 12, 28, 0, 0, 0), new LocalDateTime(2013, 1, 2, 0, 0, 0));
            Assert.Equal(0, tabela.getWorkingMinutesBetween(new DateTime(2012, 12, 28, 0, 0, 0, 0), new DateTime(2013, 1, 2, 23, 59, 59, 0)));
        }

        [Fact]
        public void testWorkingHoursTimeModification()
        {
            short h = 60;
            short h_0000 = 0;
            short h_0800 = (short)(h * 8);
            short h_0900 = (short)(h * 9);
            short h_1200 = (short)(h * 12);
            short h_1300 = (short)(h * 13);
            short h_1800 = (short)(h * 18);
            short h_2400 = (short)(h * 24);
            short workDay = h_0800;

            WorkingWeek workingWeek = ComplexWorkingWeek.GetWeek8X5In2Periods();
            SortedSet<WorkingDaySlice> exceptions = new SortedSet<WorkingDaySlice> {
                new SimpleWorkingDay(new DateTime(2012,12,7),h_0000,h_2400),
            };
            WorkingHoursTable tabela = new WorkingHoursTable(workingWeek, new List<WorkingDaySlice>(),
                exceptions, new LocalDateTime(2012, 12, 1, 0, 0, 0), new LocalDateTime(2012, 12, 10, 0, 0, 0));
            Assert.Equal(workDay, tabela.getWorkingMinutesBetween(new DateTime(2012, 12, 6, 0, 0, 0, 0), new DateTime(2012, 12, 8, 0, 0, 0, 0)));
        }

        [Fact]
        public void testWorkingMinutesBetweenWithExceptions()
        {
            SortedSet<WorkingDaySlice> exceptions = new SortedSet<WorkingDaySlice>();
            exceptions.Add(new SimpleWorkingDay(new LocalDateTime(2012, 12, 7, 0, 0, 0), new LocalTime(0, 0, 0), new LocalTime(23, 59, 59)));
            exceptions.Add(new SimpleWorkingDay(new LocalDateTime(2012, 12, 10, 0, 0, 0), new LocalTime(0, 0, 0), new LocalTime(23, 59, 59)));

            WorkingHoursTable tabela = new WorkingHoursTable(ComplexWorkingWeek.GetWeek8X5In2Periods(), new List<WorkingDaySlice>(), exceptions, new LocalDateTime(2012, 12, 1, 0, 0, 0), new LocalDateTime(2012, 12, 10, 23, 59, 59));
            Assert.Equal(215, tabela.getWorkingMinutesBetween(new DateTime(2012, 12, 6, 14, 25, 0, 0), new DateTime(2012, 12, 7, 11, 37, 0, 0)));
        }

        [Fact]
        public void testGetExceptionDaySlices()
        {
            short h = 60;
            short h_0000 = 0;
            short h_0800 = (short)(h * 8);
            short h_0900 = (short)(h * 9);
            short h_1000 = (short)(h * 10);
            short h_1200 = (short)(h * 12);
            short h_1300 = (short)(h * 13);
            short h_1400 = (short)(h * 14);
            short h_1800 = (short)(h * 18);
            short h_2400 = (short)(h * 24);
            short workDay = h_0800;
            var workingPeriods = new List<WorkingPeriod> {
                new WorkingPeriod(0,h_0900,h_1200),
                new WorkingPeriod(0,h_1300,h_1800)
            };

            short start = h_0000;
            short end = h_0000;
            // var ret = WorkingHoursTable.GetExceptionDaySlices(start, end, workingPeriods);
            var workingSlices = workingPeriods.Select(w => (w.startPeriod, w.endPeriod)).ToList();
            var sliceBlock = new List<(short, short)> { (start, end)};
            var ret = WorkingHoursTable.GetExceptionDaySlices(sliceBlock, workingSlices);
            Assert.Equal(2, ret.Count());
            var firstItem = ret.First();
            var secondItem = ret.Last();
            Assert.Equal(h_0900, firstItem.start);
            Assert.Equal(h_1200, firstItem.end);
            Assert.Equal(h_1300, secondItem.start);
            Assert.Equal(h_1800, secondItem.end);

            start = h_0900;
            end = h_1200;
            sliceBlock = new List<(short, short)> { (start, end)};
            ret = WorkingHoursTable.GetExceptionDaySlices(sliceBlock, workingSlices);
            Assert.Single(ret);
            firstItem = ret.First();
            Assert.Equal(h_1300, firstItem.start);
            Assert.Equal(h_1800, firstItem.end);

            start = h_1300;
            end = h_1800;
            sliceBlock = new List<(short, short)> { (start, end)};
            ret = WorkingHoursTable.GetExceptionDaySlices(sliceBlock, workingSlices);
            Assert.Single(ret);
            firstItem = ret.First();
            Assert.Equal(h_0900, firstItem.start);
            Assert.Equal(h_1200, firstItem.end);

            start = h_1000;
            end = h_1400;
            sliceBlock = new List<(short, short)> { (start, end)};
            ret = WorkingHoursTable.GetExceptionDaySlices(sliceBlock, workingSlices);
            Assert.Equal(2, ret.Count());
            firstItem = ret.First();
            secondItem = ret.Last();
            Assert.Equal(h_0900, firstItem.start);
            Assert.Equal(h_1000, firstItem.end);
            Assert.Equal(h_1400, secondItem.start);
            Assert.Equal(h_1800, secondItem.end);

        }

        [Fact]
        public void testPeriodWithOutWorkWeek()
        {
            // Semana sem nenhum momento útil.
            LocalTime MIDNIGHT = new LocalTime(0, 0, 0);
            var workingWeek = new ComplexWorkingWeek();
            workingWeek.setWorkDay((int)IsoDayOfWeek.Monday, MIDNIGHT, MIDNIGHT);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Tuesday, MIDNIGHT, MIDNIGHT);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Wednesday, MIDNIGHT, MIDNIGHT);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Thursday, MIDNIGHT, MIDNIGHT);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Friday, MIDNIGHT, MIDNIGHT);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Saturday, MIDNIGHT, MIDNIGHT);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Sunday, MIDNIGHT, MIDNIGHT);

            var workingTable = new WorkingHoursTable(
                workingWeek,
                new List<WorkingDaySlice>(),
                new SortedSet<WorkingDaySlice>(),
                new LocalDateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0),
                new LocalDateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59)
            );

            var date = new DateTime(2016, 02, 01, 22, 56, 23);
            var hours = 16666;
            var minutes = 39;
            var result = workingTable.addWorkingHours(date, hours, minutes);
            Assert.Equal(date, result);
        }

        [Fact]
        public void testPeriodWithExceptionInAllDays()
        {
            // Semana sem nenhum momento útil.
            LocalTime MIDNIGHT = new LocalTime(0, 0, 0);
            LocalTime EIGHT = new LocalTime(8, 0, 0);
            var workingWeek = new ComplexWorkingWeek();
            workingWeek.setWorkDay((int)IsoDayOfWeek.Monday, MIDNIGHT, EIGHT);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Tuesday, MIDNIGHT, EIGHT);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Wednesday, MIDNIGHT, EIGHT);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Thursday, MIDNIGHT, EIGHT);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Friday, MIDNIGHT, EIGHT);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Saturday, MIDNIGHT, MIDNIGHT);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Sunday, MIDNIGHT, MIDNIGHT);

            var startPeriod = new DateTime(2016, 01, 01, 0, 0, 0);
            var endPeriod = new DateTime(2016, 01, 07, 23, 59, 59);

            // Define todo o período como feriado.
            var exceptions = new SortedSet<WorkingDaySlice>();
            var daysOnPeriod = Convert.ToInt32((endPeriod - startPeriod).TotalDays);
            short firstMinuteOfDay = 0;
            short lastMinuteOfDay = 1439;
            for (int day = 1; day <= daysOnPeriod; day++)
            {
                var workDaySlice = new SimpleWorkingDay(new DateTime(2016, 01, day), firstMinuteOfDay, lastMinuteOfDay);
                exceptions.Add(workDaySlice);
            }

            var workingTable = new WorkingHoursTable(
                workingWeek,
                new List<WorkingDaySlice>(),
                exceptions,
                new LocalDateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0),
                new LocalDateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59)
            );

            Assert.Equal(0, workingTable.getWorkingHoursBetween(startPeriod, endPeriod));
        }

        [Fact]
        public void testAddWorkingHoursTwoPeriodBeforeLunchTime()
        {
            var workingTable = new WorkingHoursTable(
                ComplexWorkingWeek.getWeek8x7In2Periods(),
                new List<WorkingDaySlice>(),
                new SortedSet<WorkingDaySlice>(),
                new LocalDateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0),
                new LocalDateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59)
            );

            var date = new DateTime(2017, 06, 06, 11, 30, 00);
            var hours = 1;
            var minutes = 0;
            var result = workingTable.addWorkingHours(date, hours, minutes);

            var expectedResult = new DateTime(2017, 06, 06, 13, 30, 00);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void testAddWorkingHoursTwoPeriodIgnoreLunchTime()
        {
            var workingTable = new WorkingHoursTable(
                ComplexWorkingWeek.getWeek8x7In2Periods(),
                new List<WorkingDaySlice>(),
                new SortedSet<WorkingDaySlice>(),
                new LocalDateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0),
                new LocalDateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59)
            );

            var date = new DateTime(2017, 06, 06, 12, 15, 00);
            var hours = 1;
            var minutes = 0;
            var result = workingTable.addWorkingHours(date, hours, minutes);

            var expectedResult = new DateTime(2017, 06, 06, 14, 00, 00);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void testAddWorkingHoursTwoPeriodIgnoreWeekend()
        {
            var workingTable = new WorkingHoursTable(
                ComplexWorkingWeek.GetWeek8X5In2PeriodsAndHalfWeekend(),
                new List<WorkingDaySlice>(),
                new SortedSet<WorkingDaySlice>(),
                new LocalDateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0),
                new LocalDateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59)
            );

            var hours = 16;
            var minutes = 0;

            // Termina exatamente ao meio-dia e não deve extender o período.
            var date = new DateTime(2017, 06, 02, 10, 0, 0);
            var result = workingTable.addWorkingHours(date, hours, minutes);
            var expectedResult = new DateTime(2017, 06, 05, 12, 00, 00);
            Assert.Equal(expectedResult, result);

            // Inicia no meio do período de intervalo e todo intervalo deve ser descartado no calculo.
            var dateStartInInterval = new DateTime(2017, 06, 02, 12, 30, 0);
            var resultStartInInterval = workingTable.addWorkingHours(dateStartInInterval, hours, minutes);
            var expectedResultStartInInterval = new DateTime(2017, 06, 05, 15, 00, 00);
            Assert.Equal(expectedResultStartInInterval, resultStartInInterval);

            // Inicia no meio do período de intervalo e todo intervalo deve ser descartado no calculo.
            var dateStartWeekend = new DateTime(2017, 06, 04, 00, 00, 0);
            var resultStartWeekend = workingTable.addWorkingHours(dateStartWeekend, hours, minutes);
            var expectedResultWeekend = new DateTime(2017, 06, 06, 15, 00, 00);
            Assert.Equal(expectedResultWeekend, resultStartWeekend);
        }

        [Fact]
        public void testAddWorkingHoursTwoPeriodNightPeriod()
        {
            // Semana sem nenhum momento útil.
            LocalTime MIDNIGHT = new LocalTime(0, 0, 0);
            LocalTime NINE = new LocalTime(9, 0, 0);
            LocalTime EIGHTEEN = new LocalTime(18, 30, 0);
            LocalTime TWENT_THREE = new LocalTime(23, 59, 0);
            var workingWeek = new ComplexWorkingWeek();
            workingWeek.setWorkDay((int)IsoDayOfWeek.Monday, MIDNIGHT, NINE);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Monday, EIGHTEEN, TWENT_THREE);

            workingWeek.setWorkDay((int)IsoDayOfWeek.Tuesday, MIDNIGHT, NINE);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Tuesday, EIGHTEEN, TWENT_THREE);

            workingWeek.setWorkDay((int)IsoDayOfWeek.Wednesday, MIDNIGHT, NINE);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Wednesday, EIGHTEEN, TWENT_THREE);

            workingWeek.setWorkDay((int)IsoDayOfWeek.Thursday, MIDNIGHT, NINE);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Thursday, EIGHTEEN, TWENT_THREE);

            workingWeek.setWorkDay((int)IsoDayOfWeek.Friday, MIDNIGHT, NINE);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Friday, EIGHTEEN, TWENT_THREE);

            workingWeek.setWorkDay((int)IsoDayOfWeek.Saturday, MIDNIGHT, TWENT_THREE);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Sunday, MIDNIGHT, TWENT_THREE);

            var workingTable = new WorkingHoursTable(
                workingWeek,
                new List<WorkingDaySlice>(),
                new SortedSet<WorkingDaySlice>(),
                new LocalDateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0),
                new LocalDateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59)
            );

            var date = new DateTime(2017, 06, 06, 10, 10, 00);
            var hours = 1;
            var minutes = 0;
            var result = workingTable.addWorkingHours(date, hours, minutes);
            var expectedResult = new DateTime(2017, 06, 06, 19, 30, 00);
            Assert.Equal(expectedResult, result);

            var dateWeekend = new DateTime(2017, 06, 04, 10, 10, 00);
            var hoursWeekend = 1;
            var minutesWeekend = 0;
            var resultWeekend = workingTable.addWorkingHours(dateWeekend, hoursWeekend, minutesWeekend);
            var expectedResultWeekend = new DateTime(2017, 06, 04, 11, 10, 00);
            Assert.Equal(expectedResultWeekend, resultWeekend);
        }

        [Fact]
        public void testAddWorkingHoursSingleRecurrentHoliday()
        {
            LocalTime MIDNIGHT = new LocalTime(0, 0, 0);
            LocalTime NINE = new LocalTime(9, 0, 0);
            LocalTime EIGHTEEN = new LocalTime(18, 00, 0);
            LocalTime TWENT_THREE = new LocalTime(23, 59, 0);
            var workingWeek = new ComplexWorkingWeek();
            workingWeek.setWorkDay((int)IsoDayOfWeek.Monday, MIDNIGHT, TWENT_THREE);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Tuesday, MIDNIGHT, TWENT_THREE);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Wednesday, MIDNIGHT, TWENT_THREE);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Thursday, MIDNIGHT, TWENT_THREE);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Friday, MIDNIGHT, TWENT_THREE);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Saturday, MIDNIGHT, MIDNIGHT);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Sunday, NINE, EIGHTEEN);

            var exceptions = new List<WorkingDaySlice>();
            var workDaySlice = new SimpleWorkingDay(new DateTime(2017, 06, 18), (short)(0 + (0 * 60)), (short)(59 + (23 * 60)));
            exceptions.Add(workDaySlice);

            var workingTable = new WorkingHoursTable(
                workingWeek,
                exceptions,
                new SortedSet<WorkingDaySlice>(),
                new LocalDateTime(2017, 06, 18, 15, 00),
                new LocalDateTime(2017, 06, 18, 16, 59, 59)
            );

            var date = new DateTime(2017, 06, 18, 15, 44, 08);
            var hours = 0;
            var minutes = 25;
            var result = workingTable.addWorkingHours(date, hours, minutes);
            var expectedResult = new DateTime(2017, 06, 19, 00, 25, 00);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void testAddWorkingHoursSingleDayHoliday()
        {
            LocalTime MIDNIGHT = new LocalTime(0, 0, 0);
            LocalTime NINE = new LocalTime(9, 0, 0);
            LocalTime EIGHTEEN = new LocalTime(18, 00, 0);
            LocalTime TWENT_THREE = new LocalTime(23, 59, 0);
            var workingWeek = new ComplexWorkingWeek();
            workingWeek.setWorkDay((int)IsoDayOfWeek.Monday, MIDNIGHT, TWENT_THREE);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Tuesday, MIDNIGHT, TWENT_THREE);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Wednesday, MIDNIGHT, TWENT_THREE);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Thursday, MIDNIGHT, TWENT_THREE);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Friday, MIDNIGHT, TWENT_THREE);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Saturday, MIDNIGHT, MIDNIGHT);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Sunday, NINE, EIGHTEEN);
            
            var exceptions = new SortedSet<WorkingDaySlice>();
            var workDaySlice = new SimpleWorkingDay(new DateTime(2017, 06, 18), (short)(0 + (0 * 60)), (short)(59 + (23 * 60)));
            exceptions.Add(workDaySlice);

            var workingTable = new WorkingHoursTable(
                workingWeek,
                new List<WorkingDaySlice>(),
                exceptions,
                new LocalDateTime(2017, 06, 18, 15, 00),
                new LocalDateTime(2017, 06, 18, 16, 59, 59)
            );

            var date = new DateTime(2017, 06, 18, 15, 44, 08);
            var hours = 0;
            var minutes = 25;
            var result = workingTable.addWorkingHours(date, hours, minutes);
            var expectedResult = new DateTime(2017, 06, 19, 00, 25, 00);
            Assert.Equal(expectedResult, result);
        }
    
        /// <summary>
        /// Efetua um teste colocando 2 registros de feriado que geram conflito de cruzamento de horario
        /// </summary>
        [Fact]
        public void testAddWorkingHoursSingleConflictedHoliday()
        {
            LocalTime MIDNIGHT = new LocalTime(0, 0, 0);
            LocalTime MIDDAY = new LocalTime(12, 0, 0);
            LocalTime NINE = new LocalTime(9, 0, 0);
            LocalTime EIGHTEEN = new LocalTime(18, 00, 0);
            LocalTime THIRTEEN = new LocalTime(13, 00, 0);
            LocalTime TWENT_THREE = new LocalTime(23, 59, 0);
            var workingWeek = new ComplexWorkingWeek();
            workingWeek.setWorkDay((int)IsoDayOfWeek.Monday, MIDNIGHT, TWENT_THREE);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Tuesday, MIDNIGHT, TWENT_THREE);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Wednesday, MIDNIGHT, TWENT_THREE);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Thursday, MIDNIGHT, TWENT_THREE);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Friday, MIDNIGHT, TWENT_THREE);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Saturday, MIDNIGHT, MIDNIGHT);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Sunday, NINE, MIDDAY);
            workingWeek.setWorkDay((int)IsoDayOfWeek.Sunday, THIRTEEN, EIGHTEEN);
            
            var exceptions = new SortedSet<WorkingDaySlice>();
            var zeroHourInMinutes = (short)(0 + (0 * 60));
            var thirteenHourInMinutes = (short)(0 + (13 * 60));
            var elevenHourInMinutes = (short)(0 + (11 * 60));
            var lastMinuteInDay = (short)(59 + (23 * 60));

            // Feriado 1 no primeiro período...
            var workDaySlice = new SimpleWorkingDay(new DateTime(2024, 12, 29), zeroHourInMinutes, thirteenHourInMinutes, false);
            Assert.True(exceptions.Add(workDaySlice), "O primeiro feriado não foi adicionado na lista de feriados");
            // Feriado conflitando com parte do segundo período...
            var secondWorkDaySlice = new SimpleWorkingDay(new DateTime(2024, 12, 29), elevenHourInMinutes, lastMinuteInDay, false);
            Assert.True(exceptions.Add(secondWorkDaySlice), "O segundo feriado não foi adicionado na lista de feriados");

            var workingTable = new WorkingHoursTable(
                workingWeek,
                new List<WorkingDaySlice>(),
                exceptions,
                new LocalDateTime(2024, 12, 29, 15, 00),
                new LocalDateTime(2024, 12, 29, 16, 59, 59)
            );

            var date = new DateTime(2024, 12, 29, 15, 44, 08);
            var hours = 0;
            var minutes = 25;
            var result = workingTable.addWorkingHours(date, hours, minutes);
            var expectedResult = new DateTime(2024, 12, 30, 00, 25, 00);
            Assert.Equal(expectedResult, result);
        }
    }
}
