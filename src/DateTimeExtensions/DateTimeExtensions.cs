using System;
using System.Collections.Generic;
using System.Text;

namespace DateTimeExtensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// To the unix timestamp.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        public static long ToUnixTimestamp(this DateTime dateTime)
        {
            return DateTimeOffset.Now.ToUnixTimeSeconds();
        }

        /// <summary>
        /// Unixes the time to time.
        /// </summary>
        /// <param name="timeStamp">The time stamp.</param>
        /// <returns></returns>
        public static DateTime UnixTimeToTime(this long timeStamp)
        {
            return DateTimeOffset.FromUnixTimeSeconds(timeStamp).UtcDateTime;
        }
    }
}
