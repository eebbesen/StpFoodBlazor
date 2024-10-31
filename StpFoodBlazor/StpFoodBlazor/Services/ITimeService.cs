namespace StpFoodBlazor.Services
{
    public interface ITimeService
    {
        public string GetDayOfWeek()
        {
            return DateTime.Now.DayOfWeek.ToString();
        }
    }
}
