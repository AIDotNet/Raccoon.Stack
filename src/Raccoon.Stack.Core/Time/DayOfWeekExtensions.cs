namespace Raccoon.Stack.Core.Time;

public static class DayOfWeekExtensions
{
    public static bool IsWeekend(this DayOfWeek dayOfWeek)
    {
        return dayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
    }

    public static bool IsWeekday(this DayOfWeek dayOfWeek)
    {
        return !dayOfWeek.IsWeekend();
    }
    
    public static string GetWeekName(DayOfWeek dow)
    {
        switch (dow)
        {
            case DayOfWeek.Sunday:
                return "星期天";
            case DayOfWeek.Monday:
                return "星期一";
            case DayOfWeek.Tuesday:
                return "星期二";
            case DayOfWeek.Wednesday:
                return "星期三";
            case DayOfWeek.Thursday:
                return "星期四";
            case DayOfWeek.Friday:
                return "星期五";
            case DayOfWeek.Saturday:
            default:
                return "星期六";
        }
    }
}