using System;
namespace RebelCmsTemplate.Util
{
    public class CustomDateTimeConvert
    {
        public static DateOnly ConvertToDate(DateTime dateString)
        {
            return new DateOnly(dateString.Year, dateString.Month, dateString.Day);
        }
        public static TimeOnly ConvertToTime(TimeSpan timeString)
        {
            return new TimeOnly(timeString.Hours, timeString.Minutes);
        }
    }
}

