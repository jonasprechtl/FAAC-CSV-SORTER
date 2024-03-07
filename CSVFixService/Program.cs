using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Config;
using CSVFixService;

static class Program
{
    static void Main(string[] args)
    {
        Config.CoreConfig.readConfig();
        var test = Config.CoreConfig.isNextRunDue();

        Config.Credentials.setCredential("FAAC", "FAACPWD");
        Console.Write(Config.Credentials.readCredential());
        Config.Credentials.changePassword("NEWPWD");
        Console.Write(Config.Credentials.readCredential());
        Config.Credentials.deleteCredentials();
        Console.Write(Config.Credentials.readCredential());

        var txt = File.ReadAllLines(@"\\51.12.57.16\drive\Minceraft.txt")
            
        
        if (args.Contains("--console")) {
            Console.WriteLine("Hello World!");
        } else{
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
                services.AddHostedService<MyWindowsService>(); // MyWindowsService ist ein Beispiel für einen Windows-Service, den Sie implementieren müssen
            });
}

public class MyWindowsService : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Hier die Logik zum Starten des Services einfügen
        Console.WriteLine("Service gestartet.");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Hier die Logik zum Stoppen des Services einfügen
        Console.WriteLine("Service gestoppt.");
        return Task.CompletedTask;
    }
}
