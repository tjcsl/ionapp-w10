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
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Ion10.Services.SettingsServices;
using Ion10.Views;
using Template10.Mvvm;

namespace Ion10.ViewModels {
    public class SettingsPageViewModel : ViewModelBase {
        public SettingsPartViewModel SettingsPartViewModel { get; } = new SettingsPartViewModel();
        public AboutPartViewModel AboutPartViewModel { get; } = new AboutPartViewModel();
    }

    public class SettingsPartViewModel : ViewModelBase {
        private string _BusyText = "Please wait...";
        SettingsService _settings;

        DelegateCommand _ShowBusyCommand;

        public SettingsPartViewModel() {
            if(DesignMode.DesignModeEnabled) {
                // designtime
            } else {
                _settings = SettingsService.Instance;
            }
        }

        public bool UseShellBackButton {
            get { return _settings.UseShellBackButton; }
            set {
                _settings.UseShellBackButton = value;
                RaisePropertyChanged();
            }
        }

        public bool UseLightThemeButton {
            get { return _settings.AppTheme.Equals(ApplicationTheme.Light); }
            set {
                _settings.AppTheme = value ? ApplicationTheme.Light : ApplicationTheme.Dark;
                RaisePropertyChanged();
            }
        }

        public string BusyText {
            get { return _BusyText; }
            set {
                Set(ref _BusyText, value);
                _ShowBusyCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand ShowBusyCommand
            => _ShowBusyCommand ?? (_ShowBusyCommand = new DelegateCommand(async () => {
                   Busy.SetBusy(true, _BusyText);
                   await Task.Delay(5000);
                   Busy.SetBusy(false);
               }, () => !string.IsNullOrEmpty(BusyText)));
    }

    public class AboutPartViewModel : ViewModelBase {
        public static Uri Logo => Package.Current.Logo;

        public static string DisplayName => Package.Current.DisplayName;

        public static string Publisher => Package.Current.PublisherDisplayName;

        public static string Version {
            get {
                var v = Package.Current.Id.Version;
                return $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
            }
        }

        public static Uri RateMe => new Uri("http://aka.ms/template10");
    }
}
