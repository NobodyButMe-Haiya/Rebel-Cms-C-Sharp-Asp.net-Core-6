using System;
namespace RebelCmsTemplate.Util
{
    public class CustomDateTimeConvert
    {
        public static DateOnly? ConvertToDate(DateTime? dateString)
        {

            if (dateString != null)
            {
                if (dateString.HasValue)
                {

                    return new DateOnly(dateString.Value.Year, dateString.Value.Month, dateString.Value.Day);
                }
            }
            return null;
        }
        public static TimeOnly? ConvertToTime(DateTime? timeString)
        {
            if (timeString != null)
            {
                if (timeString.HasValue)
                {
                    return new TimeOnly(timeString.Value.Hour, timeString.Value.Minute, timeString.Value.Second);
                }
            }
            return null;
        }
    }
}

