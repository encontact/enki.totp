using System;
using System.Timers;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using enki.totp;
using Microsoft.Extensions.Configuration;

namespace totp.ui
{
    public class MainWindow : Window
    {
        private Totp token;
        private static System.Timers.Timer aTimer;
        [UI] private Label _label1 = null;
        [UI] private Label _label2 = null;
        [UI] private Button _button1 = null;

        public MainWindow(IConfigurationRoot config) : this(config, new Builder("MainWindow.glade")) { }

        private MainWindow(IConfigurationRoot config, Builder builder) : base(builder.GetObject("MainWindow").Handle)
        {
            builder.Autoconnect(this);
            
            DeleteEvent += Window_DeleteEvent;
            _button1.Clicked += Button1_Clicked;

            var timeInSeconds = Convert.ToInt32(config["TimeInSeconds"]);
            var digits = Convert.ToInt32(config["Digits"]);
            var totpKey = config["Key"];
            token = new Totp("JZQSAZLONNUSA43PNVXXGIDUN5SG64ZAMNQXEYLTEBWXK2LUN4QGM33EMFZQ", timeInSeconds, digits);

            aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += RefreshTokenEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        private void Button1_Clicked(object sender, EventArgs a)
            => TextCopy.ClipboardService.SetText(_label1.Text);

        private void RefreshTokenEvent(Object source, ElapsedEventArgs e)
        {
            try
            {
                _label1.Text = "<big><b>" + token.getCodeString() + "</b></big>" ;
                _label1.UseMarkup = true;
                _label2.Text = $"Faltam <b>{token.RemainingSeconds()}</b> segundos para trocar o número.";
                _label2.UseMarkup = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
