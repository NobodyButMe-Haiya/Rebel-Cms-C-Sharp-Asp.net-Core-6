using System;
namespace RebelCmsTemplate.Util
{
    public class CustomDateTimeConvert
    {
        public static DateOnly? ConvertToDate(string? dateString)
        {
            // we using old style much accurate
            if (dateString?.Length == 10)
            {
                var dateArray = dateString?.Split("-");
                if (dateArray != null)
                    if (dateArray.Length > 0)
                        return new DateOnly(Convert.ToInt32(dateArray[0].ToString()), Convert.ToInt32(dateArray[1].ToString()), Convert.ToInt32(dateArray[2].ToString()));
            }
            else if (dateString?.Length == 8)
            {
                var year = dateString.Substring(0, 4);
                var month = dateString.Substring(5, 6);
                var day = dateString.Substring(7, 8);
                return new DateOnly(Convert.ToInt32(year),Convert.ToInt32(month), Convert.ToInt32(day));
            }
            return null;
        }
        public static TimeOnly? ConvertToTime(string? timeString)
        {
            var timeTest = timeString?.Split(":");
            if(timeString != null)
            {
                if (timeString.Length > 0)
                {
                    return new TimeOnly(Convert.ToInt32(timeString[0]), Convert.ToInt32(timeString[1]));
                }
            }

            return null;
        }
    }
}

