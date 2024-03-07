using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Config;
using CSVFixService;
using System.Security.Cryptography;

static class Program
{
    static void Main(string[] args)
    {

        //Read Config on program Start
        CoreConfig.readConfig();
        ModifyCSV.FixCSV();

        if (args.Contains("--console"))
        {
            Console.WriteLine("Hello World!");
        }
        else
        {
            CreateHostBuilder(args).Build().Run();
        }
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
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Hier die Logik zum Starten des Services einfügen
        Console.WriteLine("Service gestartet.");
        Task.Run(() => RunMainServiceLogic(), cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Hier die Logik zum Stoppen des Services einfügen
        Console.WriteLine("Service gestoppt.");
        return Task.CompletedTask;
    }

    private void RunMainServiceLogic()
    {
        CoreConfig.readConfig();

        while (true)
        {
            if (Config.CoreConfig.isNextRunDue())
            {
                Config.CoreConfig.registerRun();
                CSVFixService.ModifyCSV.FixCSV();
            }
            Config.CoreConfig.readConfig();

            Thread.Sleep(TimeSpan.FromMinutes(1));
        }
    }
}
