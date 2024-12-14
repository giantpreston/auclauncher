using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace YourNamespace
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string ClientId = "1317587040333991989";
        private const string ClientSecret = "1im0V3hxZ8t9eKj2m3AC6iKAlQz-wwTv";
        private const string RedirectUri = "http://localhost:5000/callback";
        private const string AuthorizationUrl = "https://discord.com/oauth2/authorize";
        private const string TokenUrl = "https://discord.com/api/oauth2/token";
        private const string UserInfoUrl = "https://discord.com/api/users/@me";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CustomTitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ToggleMaximizeRestore();
            }
            else
            {
                DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleMaximizeRestore();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartDiscordLogin();
        }

        private void ToggleMaximizeRestore()
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        /// <summary>
        /// Initiates the Discord login process by redirecting the user to the OAuth2 URL.
        /// </summary>
        private async void StartDiscordLogin()
        {
            string authUrl = $"{AuthorizationUrl}?client_id={ClientId}" +
                             $"&redirect_uri={Uri.EscapeDataString(RedirectUri)}" +
                             "&response_type=code&scope=identify";

            // Open the Discord OAuth2 login page in the user's default browser
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = authUrl,
                UseShellExecute = true
            });

            // Wait for the authorization code
            string authorizationCode = await StartHttpListenerForCode();

            if (!string.IsNullOrEmpty(authorizationCode))
            {
                await ExchangeCodeForToken(authorizationCode);
            }
        }

        /// <summary>
        /// Starts an HTTP listener to capture the authorization code from the redirect URI.
        /// </summary>
        private async System.Threading.Tasks.Task<string> StartHttpListenerForCode()
        {
            var listener = new HttpListener();
            listener.Prefixes.Add($"{RedirectUri}/");
            listener.Start();

            // Wait for the response
            var context = await listener.GetContextAsync();
            var code = System.Web.HttpUtility.ParseQueryString(context.Request.Url.Query).Get("code");

            // Send a response to the browser
            var response = context.Response;
            string responseString = "<html><body>Discord login successful! You may close this tab and return to the launcher.</body></html>";
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();

            listener.Stop();
            return code;
        }

        /// <summary>
        /// Exchanges the authorization code for an access token.
        /// </summary>
        private async System.Threading.Tasks.Task ExchangeCodeForToken(string code)
        {
            var client = new HttpClient();
            var values = new Dictionary<string, string>
            {
                { "client_id", ClientId },
                { "client_secret", ClientSecret },
                { "code", code },
                { "grant_type", "authorization_code" },
                { "redirect_uri", RedirectUri },
            };

            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync(TokenUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());
                string accessToken = jsonResponse["access_token"].ToString();

                // Use the access token to fetch user data
                await GetUserInfo(accessToken);
            }
            else
            {
                MessageBox.Show("Failed to retrieve access token.");
            }
        }

        /// <summary>
        /// Fetches user information using the access token.
        /// </summary>
        private async System.Threading.Tasks.Task GetUserInfo(string accessToken)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var response = await client.GetAsync(UserInfoUrl);
            
            if (response.IsSuccessStatusCode)
            {
                var userInfo = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());
                string username = userInfo["username"].ToString();
                string discriminator = userInfo["discriminator"].ToString();
                string avatar = userInfo["avatar"].ToString();

                LoginButton.Visibility = Visibility.Collapsed;
                paragraph1.Text = "Welcome, " + username;
                paragraph2.Text = "the launcher will initiate shortly..";
            }
            else
            {
                MessageBox.Show("Failed to retrieve user info.");
            }
        }
    }
}
