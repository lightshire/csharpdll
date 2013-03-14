using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLibrary.helpers
{
    public class Convertion
    {
        public static DateTime FromUnixTimeStamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }
        public static double ToUnixTimeStamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan span = date - origin;
            return Math.Floor(span.TotalSeconds);
        }
        public static string ToMySQLDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }
    }
}
