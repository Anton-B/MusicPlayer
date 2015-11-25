namespace MusicPlayerAPI
{
    public static class TimeFormatter
    {
        public static string Format(int time)
        {
            return ((time < 10) ? "0" : "").ToString() + time.ToString();
        }
    }
}
