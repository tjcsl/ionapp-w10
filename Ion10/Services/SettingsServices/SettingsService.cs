using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Storage;
using Template10.Common;
using Template10.Utils;
using Windows.UI.Xaml;

namespace Ion10.Services.SettingsServices {
    public class SettingsService {
        public static SettingsService Instance { get; } = new SettingsService();
        Template10.Services.SettingsService.ISettingsHelper _helper;
        private int initialized;
        private Uri baseUri;
        private string oauthId;
        private string oauthSecret;
        private Uri oauthCallbackUri;

        private SettingsService() {
            _helper = new Template10.Services.SettingsService.SettingsHelper();
        }

        public async Task InitializeAsync() {
            if(Interlocked.Exchange(ref initialized, 1) == 1) {
                return;
            }
            var configFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/config.txt"));
            using(var stream = await configFile.OpenStreamForReadAsync())
            using(var reader = new StreamReader(stream)) {
                baseUri = new Uri(reader.ReadLine());
                oauthId = reader.ReadLine();
                oauthSecret = reader.ReadLine();
                oauthCallbackUri = new Uri(reader.ReadLine());
            }
        }

        public Uri BaseUri => baseUri;
        public string OAuthId => oauthId;
        public string OAuthSecret => oauthSecret;
        public Uri OAuthCallbackUri => oauthCallbackUri;

        public string OAuthToken {
            get {
                var vault = new PasswordVault();
                var token = vault.FindAllByResource("OAuthToken");
                if(token.Count == 0) {
                    return null;
                }
                return token[0].Password;
            }
            set {
                var vault = new PasswordVault();
                var tokens = vault.FindAllByResource("OAuthToken");
                if(tokens.Count != 0) {
                    foreach(var token in tokens) {
                        vault.Remove(token);
                    }
                }
                vault.Add(new PasswordCredential("OAuthToken", "IonUser", value));
            }
        }

        public bool UseShellBackButton {
            get { return _helper.Read<bool>(nameof(UseShellBackButton), true); }
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
                var value = _helper.Read<string>(nameof(AppTheme), theme.ToString());
                return Enum.TryParse<ApplicationTheme>(value, out theme) ? theme : ApplicationTheme.Dark;
            }
            set {
                _helper.Write(nameof(AppTheme), value.ToString());
                (Window.Current.Content as FrameworkElement).RequestedTheme = value.ToElementTheme();
                Views.Shell.HamburgerMenu.RefreshStyles(value);
            }
        }

        public TimeSpan CacheMaxDuration {
            get { return _helper.Read<TimeSpan>(nameof(CacheMaxDuration), TimeSpan.FromDays(2)); }
            set {
                _helper.Write(nameof(CacheMaxDuration), value);
                BootStrapper.Current.CacheMaxDuration = value;
            }
        }
    }
}

