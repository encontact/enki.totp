using System;
using enki.totp;
using Microsoft.Extensions.Configuration;
// using Mono.Terminal;
using Terminal.Gui;

namespace totp.uiterm
{
    public class MainWindow : Terminal.Gui.Toplevel
    {
        private Totp token;
        private Window win;
        private Label tokenValue = null;
        private Label tokenSeconds = null;

        public MainWindow()
        {
            var builder = new ConfigurationBuilder()
            .AddJsonFile("config/config.json", optional: false)
            .AddEnvironmentVariables();
            var config = builder.Build();

            var timeInSeconds = Convert.ToInt32(config["TimeInSeconds"]);
            var digits = Convert.ToInt32(config["Digits"]);
            var totpKey = config["Key"];
            token = new Totp("JZQSAZLONNUSA43PNVXXGIDUN5SG64ZAMNQXEYLTEBWXK2LUN4QGM33EMFZQ", timeInSeconds, digits);

            Application.Init();
            var top = Application.Top;

            // Creates the top-level window to show
            win = new Window("enContact - Token")
            {
                X = 0,
                Y = 0, // Leave one row for the toplevel menu

                // By using Dim.Fill(), it will automatically resize without manual intervention
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            top.Add(win);

            var tokenBlock = new Window("Token")
            {
                X = 3,
                Y = 3,
                Width = 20,
                Height = 5,
            };
            tokenBlock.Add(GetTokenLabel(token.getCodeString()));

            var timeBlock = new Window("Tempo restante")
            {
                X = 3,
                Y = 10,
                Width = 60,
                Height = 5,
            };
            timeBlock.Add(GetTokenSeconds(token.RemainingSeconds()));

            var copyBtn = new Button(3, 20, "Copiar", true)
            {
                CanFocus = true
            };
            copyBtn.Clicked += () =>
            {
                TextCopy.ClipboardService.SetText(tokenValue.Text.ToString());
            };
            var exitBtn = new Button(25, 20, "Sair")
            {
                CanFocus = true
            };
            exitBtn.Clicked += () => {
                    top.Running = false;
                    Application.RequestStop();
                    Application.Shutdown();
            };

            var copyInfoLabel = new Label("Pressione ALT+C para o botão copiar.") { X = 3, Y = 22 };
            var exitInfoLabel = new Label("Pressione ALT+S para o botão sair.") { X = 3, Y = 23 };

            win.Add(tokenBlock, timeBlock, copyBtn, exitBtn, copyInfoLabel, exitInfoLabel);

            Application.MainLoop.AddTimeout (TimeSpan.FromMilliseconds (1000), timer);
            Application.Run();
        }

        bool timer (MainLoop caller)
        {
            try
            {
                GetTokenSeconds(token.RemainingSeconds());
                GetTokenLabel(token.getCodeString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
	        return true;
        }

        private Label GetTokenLabel(string value)
        {
            var tokenX = 1;
            var tokenY = 1;
            var tokenWidth = 10;
            var tokenHeight = 2;

            if (tokenValue == null)
            {
                tokenValue = new Label(value) { X = tokenX, Y = tokenY, Width = tokenWidth, Height = tokenHeight };
            }
            else
            {
                tokenValue.Text = value;
            }
            return tokenValue;
        }

        private Label GetTokenSeconds(int value)
        {
            var tokenX = 1;
            var tokenY = 1;
            var tokenWidth = 50;
            var tokenHeight = 2;

            var message = $"Faltam {value} segundos para trocar o número.";
            if (tokenSeconds == null)
            {
                tokenSeconds = new Label(message) { X = tokenX, Y = tokenY, Width = tokenWidth, Height = tokenHeight };
            }
            else
            {
                tokenSeconds.Text = message;
            }
            return tokenSeconds;
        }

        public static bool Quit() => true;
    }
}
