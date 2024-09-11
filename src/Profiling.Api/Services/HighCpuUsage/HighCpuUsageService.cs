namespace Profiling.Api.Services.HighCpuUsage;

public class HighCpuUsageService : IHighCpuUsageService
{
    private readonly ILogger<HighCpuUsageService> _logger;

    public HighCpuUsageService(ILogger<HighCpuUsageService> logger)
    {
        _logger = logger;
    }

    public void Run()
    {
        _logger.LogInformation("Start Calculating Prime Number");
        long _ = CalculatePrimeNumber(1_000_000);
        _logger.LogInformation("End Calculating Prime Number");
    }
    private static long CalculatePrimeNumber(int n)
    {
        var count = 0;
        long a = 2;
        while (count < n)
        {
            long b = 2;
            var prime = 1;
            while (b * b <= a)
            {
                if (a % b == 0)
                {
                    prime = 0;
                    break;
                }
                b++;
            }
            if (prime > 0)
            {
                count++;
            }
            a++;
        }
        return --a;
    }
}