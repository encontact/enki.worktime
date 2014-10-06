namespace enki.libs.workhours.domain
{
    /// <summary>
    /// Representa um periodo util de trabalho num dia de semana.
    /// </summary>
    public class WorkingPeriod
    {
		/// <summary>
		/// Gets or sets the day of week.
		/// </summary>
		/// <value>
		/// The day of week.
		/// </value>
		public int dayOfWeek {get; set;}
        /// <summary>
        /// Gets or sets the period start.
        /// </summary>
        /// <value>
        /// Inicio do periodo, contado em minutos a partir das 0h do dia.
        /// </value>
		public short startPeriod {get; set; }
		/// <summary>
		/// Gets or sets the period end.
		/// </summary>
		/// <value>
		/// Final do periodo, contado em minutos a partir das 0h do dia.
		/// </value>
		public short endPeriod { get; set; }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="enki.libs.workhours.domain.WorkingPeriod"/> class.
		/// </summary>
		public WorkingPeriod ()
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="enki.libs.workhours.domain.WorkingPeriod"/> class.
		/// </summary>
		/// <param name='dayOfWeek'>
		/// Dia da semana no padrao definido em "DateTimeConstants" do NodaTime.
		/// </param>
		/// <param name='startPeriod'>
		/// Inicio do periodo, contado em minutos a partir das 0h do dia.
		/// </param>
		/// <param name='endPeriod'>
		/// Final do periodo, contado em minutos a partir das 0h do dia.
		/// </param>
		public WorkingPeriod (int dayOfWeek, short startPeriod, short endPeriod)
		{
			this.dayOfWeek = dayOfWeek;
			this.startPeriod = startPeriod;
			this.endPeriod = endPeriod;
		}
	}
}
