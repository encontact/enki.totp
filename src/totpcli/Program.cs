using System;
using enki.totp;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ConsoleApplication
{
    public class Program
    {
        static public IConfigurationRoot Configuration { get; set; }

        public static void Main()
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("config.json");
            Configuration = builder.Build();

            var timeInSeconds = Convert.ToInt32(Configuration["TimeInSeconds"]);
            var digits = Convert.ToInt32(Configuration["Digits"]);
            var totp = new Totp(Configuration["Key"], timeInSeconds, digits);

            //var cancellationToken = new CancellationToken();

            // TODO: Loop infinito, resolver pro cara não ter q matar o programa.
            var subtractTime = timeInSeconds;
            var exit = false;
            while (!exit) {
                Console.Clear();
                Console.WriteLine(string.Concat("Seu codigo atual: ", totp.getCodeString()));
                
                // Console.WriteLine(string.Format("\r\n Faltam {0} segundos para trocar o número.", subtractTime));
                // subtractTime--;
                // if(subtractTime < 0) subtractTime = timeInSeconds;
                
                // if(Console.ReadKey() == ConsoleKey.Escape) exit = true;
                Console.Write("\r\nFeche a janela para sair... ");

                System.Threading.Tasks.Task.Delay(1000).Wait();
            }
            
            // // while (Console.ReadKey().Key != ConsoleKey.Enter) {
            // //     Console.Clear();
            // //     Console.WriteLine(string.Concat("Seu codigo atual: ", totp.getCodeString()));
            // //     Console.Write("Precione <Enter> para sair... ");
            // // }
            // //ConsoleKeyInfo cki;
            // int cki;
            // // Prevent example from ending if CTL+C is pressed.
            // Console.TreatControlCAsInput = true;
            // do 
            // {
            //     Console.Clear();
            //     Console.WriteLine(string.Concat("Seu codigo atual: ", totp.getCodeString()));
            //     Console.WriteLine("Press the q key to quit: \n");
            //     // Console.Write(" --- You pressed ");
            //     // if((cki.Modifiers & ConsoleModifiers.Alt) != 0) Console.Write("ALT+");
            //     // if((cki.Modifiers & ConsoleModifiers.Shift) != 0) Console.Write("SHIFT+");
            //     // if((cki.Modifiers & ConsoleModifiers.Control) != 0) Console.Write("CTL+");
            //     // Console.WriteLine(cki.Key.ToString());
            //     cki = Console.Read();
            // //} while (cki.Key != ConsoleKey.Escape);
            // } while (cki != 'q');
        }
    }
}
