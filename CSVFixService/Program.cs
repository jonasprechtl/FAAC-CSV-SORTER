using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Config;
using FileOperations;
using Log;

static class Program
{
    static void Main(string[] args)
    {
        Logger.Log("Startup Initiated", LogLevel.Info);
        CreateHostBuilder(args).Build().Run();
    }

    // Diese Methode konfiguriert den IHostBuilder für den Betrieb als Windows Service
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseWindowsService() // Stellt sicher, dass die Anwendung als Windows-Service läuft
            .ConfigureServices((hostContext, services) =>
            {
                // Fügen Sie hier Dienste hinzu, die als Teil Ihres Services laufen sollen
                services.AddHostedService<WindowsService>(); // Windows Service
            });
}

public class WindowsService : IHostedService
{
    private Task _executingTask;
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Logger.Log("Service received start signal", LogLevel.Info);
        _executingTask = Task.Run(() => RunMainServiceLogic(_cts.Token));
        return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        
        Logger.Log("Service received stop signal", LogLevel.Info);
        Console.WriteLine("Service gestoppt.");
        if (_executingTask == null)
        {
            Logger.Log("Service was not running when stop signal was received", LogLevel.Warning);
            return;
        }
        _cts.Cancel();
        await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));
    }

    private async Task RunMainServiceLogic(CancellationToken cancellationToken)
    {
        Logger.Log("Starting Service Logic", LogLevel.Info);
        while (!cancellationToken.IsCancellationRequested)
        {
            Logger.Log("New Service Loop Iteration", LogLevel.Verbose);

            try
            {
                Logger.Log("Reading Config", LogLevel.Verbose);
                CoreConfig.readConfig();
                Logger.Log("Config Read Successfully", LogLevel.Verbose);

                Logger.Log("Checking if next run is due", LogLevel.Verbose);
                if (CoreConfig.isNextRunDue())
                {
                    Logger.Log("Next run is due, initiating next CSV Fix Run", LogLevel.Info);
                    //FixCSV() will throw, if the file is empty. 
                    //FixCSV() will throw, if auth is enabled but no credentials are set
                    //This can happen, when the config is not yet set (eg. first run)
                    Logger.Log("Running CSV Fix", LogLevel.Verbose);
                    CSVFix.FixCSV();
                    Logger.Log("CSV Fix Run Successfully", LogLevel.Info);

                    Logger.Log("Registering Run", LogLevel.Verbose);    
                    CoreConfig.registerRun();
                    Logger.Log("Run Registered Successfully", LogLevel.Verbose);
                } else {
                    Logger.Log("Next run is not due, skipping CSV Fix Run", LogLevel.Verbose);
                }
            }
            catch (Exception e)
            {
                 Logger.Log($"An error occurred while executing the main Service Loop", LogLevel.Error);
                 Logger.Log("Exception: " + e.Message, LogLevel.Error);
            }

            Logger.Log("Sleeping and waiting for next iteration", LogLevel.Verbose);
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        }
    }
}
