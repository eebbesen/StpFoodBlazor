using System.Reflection;

namespace StpFoodBlazor.Helpers
{
    public static class Helper
    {
        public static string GetVersion()
        {
            var attribute = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            return attribute?.InformationalVersion ?? string.Empty;
        }
    }
}
