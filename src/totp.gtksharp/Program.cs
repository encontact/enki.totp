using System;
using totp.ui;
using Microsoft.Extensions.Configuration;
using Gtk;

namespace totp
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init();

            var app = new Application("org.totp.totp", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            var builder = new ConfigurationBuilder()
            .AddJsonFile("config/config.json", optional: false)
            .AddEnvironmentVariables()
            .AddCommandLine(args);
            var configuration = builder.Build();

            var win = new MainWindow(configuration);
            app.AddWindow(win);

            win.Show();
            Application.Run();
        }
    }
}
