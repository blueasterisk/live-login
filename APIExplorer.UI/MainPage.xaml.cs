using System;
using System.Threading.Tasks;

using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

using Microsoft.Live;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace APIExplorer
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static readonly string[] scopes = new string[] { "wl.signin", "wl.basic", "Office.OneNote_Create" };
        private LiveAuthClient authClient;
        private LiveConnectClient liveClient;

        public MainPage()
        {
            this.InitializeComponent();
            this.tbResponse.Text = string.Empty;
        }

        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.InitializePage();
        }

        private async void InitializePage()
        {
            try
            {
                this.authClient = new LiveAuthClient();
                LiveLoginResult loginResult = await this.authClient.InitializeAsync(scopes);
                if (loginResult.Status == LiveConnectSessionStatus.Connected)
                {
                    if (this.authClient.CanLogout)
                    {
                        this.btnLogin.Content = "Sign Out";
                    }
                    else
                    {
                        this.btnLogin.Visibility = Visibility.Collapsed;
                    }

                    this.liveClient = new LiveConnectClient(loginResult.Session);
                    this.GetMe();
                }
            }
            catch (LiveAuthException authExp)
            {
                this.tbResponse.Text = authExp.ToString();
            }
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.btnLogin.Content.ToString() == "Sign In")
                {
                    LiveLoginResult loginResult = await this.authClient.LoginAsync(scopes);
                    if (loginResult.Status == LiveConnectSessionStatus.Connected)
                    {
                        if (this.authClient.CanLogout)
                        {
                            this.btnLogin.Content = "Sign Out";
                        }
                        else
                        {
                            this.btnLogin.Visibility = Visibility.Collapsed;
                        }

                        this.liveClient = new LiveConnectClient(loginResult.Session);
                        this.GetMe();
                    }
                }
                else
                {
                    this.authClient.Logout();
                    this.btnLogin.Content = "Sign In";
                }
            }
            catch (LiveAuthException authExp)
            {
                this.tbResponse.Text = authExp.ToString();
            }
        }

        private async void GetMe()
        {
            try
            {
                Task<LiveOperationResult> task = this.liveClient.GetAsync("me");

                var result = await task;
                dynamic profile = result.Result;
                this.tbWelcome.Text = "Welcome " + profile.first_name + " " + profile.last_name;
            }
            catch (LiveConnectException e)
            {
                this.tbResponse.Text = e.ToString();
            }
            catch (OperationCanceledException)
            {
                this.tbResponse.Text = "Operation cancelled.";
            }
        }

    }
}
