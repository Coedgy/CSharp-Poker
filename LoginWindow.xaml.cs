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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace CSharp_Poker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private bool loginAction = false;

        static HttpClient client = new HttpClient();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (loginAction)
            {
                MainScreen screen = new MainScreen();
                screen.Show();
            }
        }

        private async Task<bool> LoginAction()
        {
            try
            {
                List<KeyValuePair<string, string>> formContent = new List<KeyValuePair<string, string>>();
                formContent.Add(new KeyValuePair<string, string>("username", UsernameInput.Text));
                formContent.Add(new KeyValuePair<string, string>("password", PasswordInput.Text));

                var content = new FormUrlEncodedContent(formContent);

                var message = await client.PostAsync(Program.BaseURL + "/users/login", content);

                message.Dispose();

                if (message.StatusCode == HttpStatusCode.Accepted)
                {
                    client.CancelPendingRequests();
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            client.CancelPendingRequests();
            return false;
        }

        private async void SubmitButtonClick(object sender, RoutedEventArgs e)
        {
            if (await LoginAction())
            {
                loginAction = true;
                this.Close();
            }

            MessageBox.Show("lol"); //TODO: Proper error messages
        }
    }
}