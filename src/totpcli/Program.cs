using System;
using enki.totp;
using Microsoft.Extensions.Configuration;

namespace ConsoleApplication
{
    public class Program
    {
        static public IConfigurationRoot Configuration { get; set; }

        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
            //.SetBasePath (Directory.GetCurrentDirectory ())
            .AddJsonFile("config.json", optional: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args);
            Configuration = builder.Build();

            var timeInSeconds = Convert.ToInt32(Configuration["TimeInSeconds"]);
            var digits = Convert.ToInt32(Configuration["Digits"]);
            var totp = new Totp(Configuration["Key"], timeInSeconds, digits);

            //var cancellationToken = new CancellationToken();

            // TODO: Loop infinito, resolver pro cara não ter q matar o programa.
            var subtractTime = timeInSeconds;
            var exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine(string.Concat("Seu codigo atual: ", totp.getCodeString()));

                // Console.WriteLine(string.Format("\r\n Faltam {0} segundos para trocar o número.", subtractTime));
                // subtractTime--;
                // if(subtractTime < 0) subtractTime = timeInSeconds;

                // if(Console.ReadKey() == ConsoleKey.Escape) exit = true;
                Console.Write("\r\nFeche a janela para sair... ");

                System.Threading.Tasks.Task.Delay(1000).Wait();
            }
        }
    }
}