namespace SmartPark.Data;

public sealed class ParkingSyncHostedService(
    IServiceProvider services,
    ILogger<ParkingSyncHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await DbInitializer.SyncParkingAsync(services, logger, stoppingToken);
            await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
        }
    }
}
