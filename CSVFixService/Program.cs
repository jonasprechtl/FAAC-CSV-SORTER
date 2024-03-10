using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Config;
using FileOperations;
static class Program
{
    static void Main(string[] args)
    {
        //Read Config on program Start
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
        // Hier die Logik zum Starten des Services einfügen
        Console.WriteLine("Service gestartet.");
        _executingTask = Task.Run(() => RunMainServiceLogic(_cts.Token));
        return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        // Hier die Logik zum Stoppen des Services einfügen
        Console.WriteLine("Service gestoppt.");
        if (_executingTask == null)
        {
            return;
        }
        _cts.Cancel();
        await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));
    }

    private async Task RunMainServiceLogic(CancellationToken cancellationToken)
    {

        while (!cancellationToken.IsCancellationRequested)
        {

            try
            {
                CoreConfig.readConfig();

                if (CoreConfig.isNextRunDue())
                {
                    //FixCSV() will throw, if the file is empty. 
                    //FixCSV() will throw, if auth is enabled but no credentials are set
                    //This can happen, when the config is not yet set (eg. first run)
                    CSVFix.FixCSV();
                    CoreConfig.registerRun();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
        }
    }
}
