using enki.totp;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

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

            // TODO: Loop infinito, resolver pro cara não ter q matar o programa.
            //var subtractTime = timeInSeconds;
            var exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine(string.Concat("Seu codigo atual: ", totp.getCodeString()));
                Console.WriteLine($"Faltam {totp.RemainingSeconds()} segundos para trocar o número.");

                // Console.WriteLine(string.Format("\r\n Faltam {0} segundos para trocar o número.", subtractTime));
                // subtractTime--;
                // if(subtractTime < 0) subtractTime = timeInSeconds;

                // if(Console.ReadKey() == ConsoleKey.Escape) exit = true;
                Console.Write("\r\nFeche a janela para sair... ");

                Task.Delay(1000).Wait();
            }
        }
    }
}