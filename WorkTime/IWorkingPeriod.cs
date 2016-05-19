namespace enki.libs.workhours.domain
{
    public enum PeriodType
    {
        Hour,
        Minute,
        Second
    }

    public interface IWorkingPeriod
    {
        PeriodType Type { get; }
        int dayOfWeek { get; set; }
		short startPeriod { get; set; }
        short endPeriod { get; set; }
    }
}