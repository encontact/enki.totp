using System;
using enki.totp;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var Key = "GEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQ";
            if(args.Length > 2 && args[0] == "-k") {
                Key = args[1];
            }

            var totp = new Totp(Key, 30, 8);
            Console.WriteLine(string.Concat("Seu codigo atual: ", totp.getCodeString()));
        }
    }
}
