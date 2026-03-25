namespace enki.libs.workhours.interfaces
{
    public enum WeekDays
    {
        Monday = 1,
        Tuesday = 2,
        Wednesday = 3,
        Thursday = 4,
        Friday = 5,
        Saturday = 6,
        Sunday = 7,
    }

    public interface IWorkingHour
    {
        WeekDays StartDay { get; set; }
        int StartHour { get; set; }
        int StartMinute { get; set; }
        WeekDays EndDay { get; set; }
        int EndHour { get; set; }
        int EndMinute { get; set; }
    }
}