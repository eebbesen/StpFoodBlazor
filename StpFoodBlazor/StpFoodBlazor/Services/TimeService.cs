namespace StpFoodBlazor.Services
{
    public class TimeService : ITimeService
    {
        public string GetDayOfWeek()
        {
            return DateTime.Now.DayOfWeek.ToString();
        }
    }
}
