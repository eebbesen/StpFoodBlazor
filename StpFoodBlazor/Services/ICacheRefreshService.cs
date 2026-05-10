namespace StpFoodBlazor.Services
{
    public interface ICacheRefreshService
    {
        Task RefreshAsync(string[] keys);
    }
}
