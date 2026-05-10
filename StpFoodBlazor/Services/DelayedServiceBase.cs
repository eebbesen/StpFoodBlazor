using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace StpFoodBlazor.Services
{
    public abstract class DelayedServiceBase(ILogger logger, IHostEnvironment environment)
    {
        protected async Task ApplyDelayAsync()
        {
            logger.LogError("{Env} environment detected, simulating delay.", environment.EnvironmentName);
            await Task.Delay(1000);
        }
    }
}
