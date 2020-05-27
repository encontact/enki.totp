using System;
using System.Timers;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using enki.totp;

namespace totp.ui
{
    public class MainWindow : Window
    {
        private Totp token;
        private static System.Timers.Timer aTimer;
        [UI] private Label _label1 = null;
        [UI] private Label _label2 = null;
        [UI] private Button _button1 = null;

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetObject("MainWindow").Handle)
        {
            builder.Autoconnect(this);
            
            DeleteEvent += Window_DeleteEvent;
            _button1.Clicked += Button1_Clicked;

            var timeInSeconds = 5;
            var digits = 8;
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
                _label2.Text = $"Faltam {token.RemainingSeconds()} segundos para trocar o número.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
