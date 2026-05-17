namespace DQHieu.Framework
{
    using UnityEngine;

    public static class TimeFormatter
    {
        public static string ToMMSS(this float totalSeconds)
        {
            // Tránh số âm và làm tròn xuống
            int timeInSeconds = Mathf.Max(0, Mathf.FloorToInt(totalSeconds));

            int minutes = timeInSeconds / 60;
            int seconds = timeInSeconds % 60;

            return $"{minutes:D2}:{seconds:D2}";
        }

        public static string ToMMSS(this int totalSeconds)
        {
            int timeInSeconds = Mathf.Max(0, totalSeconds);

            int minutes = timeInSeconds / 60;
            int seconds = timeInSeconds % 60;

            return $"{minutes:D2}:{seconds:D2}";
        }
    }

}