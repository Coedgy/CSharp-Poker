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
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CSharp_Poker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private bool loginAction = false;
        private bool loginCheckRunning = false;

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
                formContent.Add(new KeyValuePair<string, string>("password", PasswordInput.Password));

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
            loginAction = true;
            this.Close();
            if (!loginCheckRunning)
            {
                loginCheckRunning = true;
                if (await LoginAction())
                {
                    loginAction = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("lol"); //TODO: Proper error messages
                }
                
                loginCheckRunning = false;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            OpenUrl("https://niisto.fi");
        }

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}