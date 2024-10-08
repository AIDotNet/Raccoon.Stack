namespace Raccoon.Stack.Core.Time;

public static class DateTimeExtensions
{
    public static DateTime FromUnixTimestamp(this long unixTime)
    {
        var sTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Utc, TimeZoneInfo.Local);
        return sTime.AddMilliseconds(unixTime);
    }

    public static long ToUnixTimestamp(this DateTime time)
    {
        var sTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Utc, TimeZoneInfo.Local);
        return (long)(time - sTime).TotalMilliseconds;
    }

    public static string ToFormatString(this DateTime dt)
    {
        return dt.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public static DateTime ToMonthFirstDay(this DateTime now)
    {
        return new DateTime(now.Year, now.Month, 1);
    }

    public static DateTime ToMonthLastDay(this DateTime now)
    {
        return new DateTime(now.Year, now.Month + 1, 1).AddDays(-1);
    }

    /// <summary>
    ///     根据参数时间来输出显示格式
    /// </summary>
    public static string ToFriendlyString(this DateTime dt)
    {
        var result = string.Empty;
        var now = DateTime.Now;
        var tsEnd = new TimeSpan(now.Ticks);
        var tsStart = new TimeSpan(dt.Ticks);
        var ts = tsEnd.Subtract(tsStart).Duration();
        if (dt == DateTime.MinValue) return string.Empty;

        if (ts.TotalSeconds < 60)
            result = "刚刚";
        else if (ts.TotalMinutes < 60)
            result = (int)ts.TotalMinutes + "分钟前";
        else if (ts.TotalHours < 24)
            result = (int)ts.TotalHours + "小时前";
        else if (now.Year == dt.Year)
            result = dt.ToString("MM/dd HH:mm");
        else
            result = dt.ToString("yyyy/MM/dd HH:mm");

        return result;
    }
}