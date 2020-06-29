using System;
using Terminal.Gui;
using Microsoft.Extensions.Configuration;

namespace totp.uiterm
{
    public class Program
    {
        /// <summary>
        /// Projeto efetuado utilizando o projeto:
        /// https://github.com/migueldeicaza/gui.cs
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Application.Run<MainWindow>();
        }
    }
}
