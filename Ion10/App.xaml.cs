// The MIT License (MIT) 
// 
// Copyright (c) 2016 Ion Native App Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Ion10.Services;
using Ion10.Services.SettingsServices;
using Ion10.Views;
using Template10.Common;
using Template10.Controls;

namespace Ion10 {
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki
    [Bindable]
    sealed partial class App : BootStrapper {
        public App() {
            InitializeComponent();
            SplashFactory = e => new Splash(e);

            #region App settings

            var _settings = SettingsService.Instance;
            RequestedTheme = _settings.AppTheme;
            CacheMaxDuration = _settings.CacheMaxDuration;
            ShowShellBackButton = _settings.UseShellBackButton;

            #endregion
        }

        public static OAuthSession OAuthSession { get; private set; }

        public override async Task OnInitializeAsync(IActivatedEventArgs args) {
            await SettingsService.Instance.InitializeAsync();
            if(Window.Current.Content as ModalDialog == null) {
                // create a new frame 
                var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);

                // create modal root
                Window.Current.Content = new ModalDialog {
                    DisableBackButtonWhenModal = true,
                    Content = new Shell(nav),
                    ModalContent = new Busy()
                };
            }
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args) {
            var settings = SettingsService.Instance;
            OAuthSession = new OAuthSession(
                settings.BaseUri,
                settings.OAuthId,
                settings.OAuthSecret,
                settings.OAuthCallbackUri);
            var refreshToken = settings.OAuthToken;
            if(refreshToken == null) {
                var code = await OAuthSession.GetOAuthCodeAsync();
                refreshToken = (await OAuthSession.GetOAuthTokenAsync(code)).RefreshToken;
            }
            var token = await OAuthSession.RefreshOAuthTokenAsync(refreshToken);
            settings.OAuthToken = token.RefreshToken;
            SessionState["token"] = token;
            NavigationService.Navigate(typeof(MainPage));
        }
    }
}
