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

using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI.Xaml;
using Ion10.Views;
using Template10.Common;
using Template10.Services.SettingsService;
using Template10.Utils;

namespace Ion10.Services.SettingsServices {
    public class SettingsService {
        ISettingsHelper _helper;
        private Uri baseUri;
        private int initialized;
        private Uri oauthCallbackUri;
        private string oauthId;
        private string oauthSecret;

        private SettingsService() {
            _helper = new SettingsHelper();
        }

        public static SettingsService Instance { get; } = new SettingsService();

        public Uri BaseUri => baseUri;
        public string OAuthId => oauthId;
        public string OAuthSecret => oauthSecret;
        public Uri OAuthCallbackUri => oauthCallbackUri;

        public string OAuthToken {
            get {
                var vault = new PasswordVault();
                var token = vault.RetrieveAll().Where(t => t.Resource == "OAuthToken").ToImmutableList();
                if(token.Count == 0) {
                    return null;
                }
                return vault.Retrieve(token[0].Resource, token[0].UserName).Password;
            }
            set {
                var vault = new PasswordVault();
                var tokens = vault.RetrieveAll().Where(t => t.Resource == "OAuthToken").ToImmutableList();
                if(tokens.Count != 0) {
                    foreach(var token in tokens) {
                        vault.Remove(token);
                    }
                }
                vault.Add(new PasswordCredential("OAuthToken", "IonUser", value));
            }
        }

        public bool UseShellBackButton {
            get { return _helper.Read(nameof(UseShellBackButton), true); }
            set {
                _helper.Write(nameof(UseShellBackButton), value);
                BootStrapper.Current.NavigationService.Dispatcher.Dispatch(() => {
                    BootStrapper.Current.ShowShellBackButton = value;
                    BootStrapper.Current.UpdateShellBackButton();
                    BootStrapper.Current.NavigationService.Refresh();
                });
            }
        }

        public ApplicationTheme AppTheme {
            get {
                var theme = ApplicationTheme.Light;
                var value = _helper.Read(nameof(AppTheme), theme.ToString());
                return Enum.TryParse(value, out theme) ? theme : ApplicationTheme.Dark;
            }
            set {
                _helper.Write(nameof(AppTheme), value.ToString());
                (Window.Current.Content as FrameworkElement).RequestedTheme = value.ToElementTheme();
                Shell.HamburgerMenu.RefreshStyles(value);
            }
        }

        public TimeSpan CacheMaxDuration {
            get { return _helper.Read(nameof(CacheMaxDuration), TimeSpan.FromDays(2)); }
            set {
                _helper.Write(nameof(CacheMaxDuration), value);
                BootStrapper.Current.CacheMaxDuration = value;
            }
        }

        public async Task InitializeAsync() {
            if(Interlocked.Exchange(ref initialized, 1) == 1) {
                return;
            }
            var configFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/config.txt"));
            using(var stream = await configFile.OpenStreamForReadAsync()) {
                using(var reader = new StreamReader(stream)) {
                    baseUri = new Uri(reader.ReadLine());
                    oauthId = reader.ReadLine();
                    oauthSecret = reader.ReadLine();
                    oauthCallbackUri = new Uri(reader.ReadLine());
                }
            }
        }
    }
}
