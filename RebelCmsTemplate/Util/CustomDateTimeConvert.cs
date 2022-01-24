namespace RebelCmsTemplate.Util;

public static class CustomDateTimeConvert
{
    public static DateOnly? ConvertToDate(DateTime? dateString)
    {
        if (dateString is { })
        {
            return new DateOnly(dateString.Value.Year, dateString.Value.Month, dateString.Value.Day);
        }

        return null;
    }

    public static TimeOnly? ConvertToTime(DateTime? timeString)
    {
        if (timeString is { })
        {
            return new TimeOnly(timeString.Value.Hour, timeString.Value.Minute, timeString.Value.Second);
        }

        return null;
    }
}