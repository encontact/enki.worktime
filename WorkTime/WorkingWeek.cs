using System;
using System.Collections.Generic;

namespace enki.libs.workhours.domain
{
    /// <summary>
    /// Interface para um Calendário de trabalho semanal, armazena as horas de início e término de trabalho para cada dia da semana.
    /// </summary>
    public interface WorkingWeek
    {
        /// <summary>
        /// dia da semana, definido em DateTimeConstants
        /// </summary>
        /// <param name="dayOfWeek">hora de início de trabalho.</param>
        /// <returns>Número referente ao horário de inicio da semana de trabalho.</returns>
        List<WorkingPeriod> getPeriods(int dayOfWeek);

        /// <summary>
        /// Recupera o tempo útil total na semana.
        /// </summary>
        /// <returns>Objeto de tempo referente ao tempo útil total da semana.</returns>
        TimeSpan GetWeekTime();
    }
}