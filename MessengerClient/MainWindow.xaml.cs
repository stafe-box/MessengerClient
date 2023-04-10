using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
//using static System.Net.Mime.MediaTypeNames;
using System.Drawing;

namespace MessengerClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string? _usernameBuffer;
        private Messanger _messanger = new Messanger();
        public MainWindow()
        {
            InitializeComponent();
            Recepient.ItemsSource = CurrentUser.Users;
            CurrentUser.Users.Add("@all");
        }

        private void ColorChooserBnt_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog CPD = new ColorPickerDialog();
            CPD.colorChanged += ColorChanged;
            CPD.ShowDialog();
        }

        private void ColorChanged()
        {
            SendBtn.BorderBrush = new SolidColorBrush(CurrentUser.UserColor);
        }

        private void UNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (UNameBox.Text.Length > 3 & UNameBox.Text.ToLower() != "all")
            {
                _usernameBuffer = UNameBox.Text;
                UNameBox.BorderBrush = new SolidColorBrush(Colors.Black);
                ApplyBnt.IsEnabled = true;
            }
            else
            {
                UNameBox.BorderBrush = new SolidColorBrush(Colors.Red);
                ApplyBnt.IsEnabled = false;
            }
        }

        private void ApplyBnt_Click(object sender, RoutedEventArgs e)
        {
            if (!(IPBox.Text.Length > 7 & IPBox.Text.Length < 15))
            {
                RaiceIPDailog();
            }
            else
            {
                Task.Run(() =>
                {
                    if (_usernameBuffer != null)
                    {
                        Message message = new Message(true,
                            _usernameBuffer,
                            "Server",
                            "Register",
                            CurrentUser.UserColor);
                        string json = message.Serialize();
                        _messanger.Send(IPBox.Text, json);
                    }
                });
                _messanger.RegSucsess += RegistrationSucsess;
                _messanger.SendExp += SendindFailed;
                _messanger.WorkingException += GotExceptionWhileWork;
                _messanger.RegFailed += RegistrationDenied; ;
                _messanger.ServError += ErrorOnServer;

                ApplyBnt.IsEnabled = false;
                UNameBox.IsEnabled = false;
                IPBox.IsEnabled = false;
                ColorChooserBnt.IsEnabled = false;
            }
        }

        private void ErrorOnServer()
        {
            throw new NotImplementedException();
        }

        private void RegistrationDenied()
        {
            throw new NotImplementedException();
        }

        private void GotExceptionWhileWork(string e)
        {
            throw new NotImplementedException();
        }

        private void SendindFailed(string e)
        {
            throw new NotImplementedException();
        }

        private void RegistrationSucsess()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                MessageSendBox.IsEnabled = true;
                SendBtn.IsEnabled = true;

                TextRange tr = new TextRange(MessageRecivedBox.Document.ContentEnd, MessageRecivedBox.Document.ContentEnd);
                tr.Text = "Вы Зарегестрированны!" + "\n";
                try
                {
                    tr.ApplyPropertyValue(TextElement.ForegroundProperty,
                        new SolidColorBrush(CurrentUser.UserColor));
                }
                catch (FormatException) { }
            }));

        }
        private void RaiceIPDailog()
        {
            MessageBoxImage mbi = MessageBoxImage.Error;
            MessageBoxButton mbb = MessageBoxButton.OK;
            MessageBox.Show("Check IP adress", "IP adress error", mbb, mbi);
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
