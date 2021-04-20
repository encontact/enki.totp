using enki.totp;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Spectre.Console;

namespace ConsoleApplication
{
    public class Program
    {
        static public IConfigurationRoot Configuration { get; set; }

        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
            .AddJsonFile("config.json", optional: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args);
            Configuration = builder.Build();

            var timeInSeconds = Convert.ToInt32(Configuration["TimeInSeconds"]);
            var digits = Convert.ToInt32(Configuration["Digits"]);
            var totp = new Totp(Configuration["Key"], timeInSeconds, digits);

            var exit = false;
            while (!exit)
            {
                // https://spectreconsole.net/live/progress
                Console.Clear();

                var rule = new Rule("[red]Token de acesso[/]");
                AnsiConsole.Render(rule);

                // Create a table
                var table = new Table();

                // Add some columns
                table.AddColumn("Seu código atual");
                table.AddColumn("Segundos para trocar o número");

                // Add some rows
                table.AddRow(
                    new Markup($"[blue]{totp.getCodeString()}[/]").Centered(), 
                    new Markup($"[green]{totp.RemainingSeconds()}[/]").Centered()
                );

                // Render the table to the console
                AnsiConsole.Render(table);

                Console.Write("");
                new Markup($"[silver]\r\nFeche a janela para sair...[/]");
                Task.Delay(1000).Wait();
            }
        }
    }
}