using Windows.UI.Xaml;
using System.Threading.Tasks;
using Ion10.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using Template10.Controls;
using Template10.Common;
using System;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml.Data;
using Windows.Web.Http;
using Ion10.Services;

namespace Ion10 {
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    [Bindable]
    sealed partial class App : Template10.Common.BootStrapper {
        public App() {
            InitializeComponent();
            SplashFactory = (e) => new Views.Splash(e);

            #region App settings

            var _settings = SettingsService.Instance;
            RequestedTheme = _settings.AppTheme;
            CacheMaxDuration = _settings.CacheMaxDuration;
            ShowShellBackButton = _settings.UseShellBackButton;

            #endregion
        }

        public static HttpClient HttpClient { get; set; } = new HttpClient();

        public override async Task OnInitializeAsync(IActivatedEventArgs args) {
            await SettingsService.Instance.InitializeAsync();
            if(Window.Current.Content as ModalDialog == null) {
                // create a new frame 
                var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);

                // create modal root
                Window.Current.Content = new ModalDialog {
                    DisableBackButtonWhenModal = true,
                    Content = new Views.Shell(nav),
                    ModalContent = new Views.Busy(),
                };
            }
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args) {
            var settings = SettingsService.Instance;
            var refreshToken = settings.OAuthToken;
            if(refreshToken == null) { 
                var code = await OAuthService.Instance.GetOAuthCodeAsync(
                    settings.BaseUri,
                    settings.OAuthId,
                    settings.OAuthCallbackUri);
                refreshToken = (await OAuthService.Instance.GetOAuthTokenAsync(
                    settings.BaseUri,
                    settings.OAuthId,
                    settings.OAuthSecret,
                    code,
                    settings.OAuthCallbackUri)).RefreshToken;
            }
            var token = await OAuthService.Instance.RefreshOAuthTokenAsync(
                settings.BaseUri,
                settings.OAuthId,
                settings.OAuthSecret,
                refreshToken,
                settings.OAuthCallbackUri);
            settings.OAuthToken = token.RefreshToken;
            NavigationService.Navigate(typeof(Views.MainPage));
        }
    }
}
