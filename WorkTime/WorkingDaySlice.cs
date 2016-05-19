using NodaTime;
using System;

namespace enki.libs.workhours
{
    /// <summary>
    /// Interface padrão que representa um dia de trabalho
    /// </summary>
    public interface WorkingDaySlice : IComparable<WorkingDaySlice>
    {
        /// <summary>
        /// Dia a ser contabilizado.
        /// </summary>
        /// <returns></returns>
        LocalDateTime getDate();

        /// <summary>
        /// Inicio do dia de trabalho em minutos a partir da 0h
        /// </summary>
        /// <returns></returns>
        short getDayStart();

        /// <summary>
        /// Termino do dia de trabalho em minutos a partir da 0h
        /// </summary>
        /// <returns></returns>
        short getDayEnd();
    }
}