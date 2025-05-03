namespace StpFoodBlazor.Services
{
    public interface ITimeService
    {
        public string GetDayOfWeek()
        {
            return DateTime.Now.DayOfWeek.ToString();
        }

        public DateTime GetCurrentDate()
        {
            return DateTime.Now.Date;
        }
    }
}
