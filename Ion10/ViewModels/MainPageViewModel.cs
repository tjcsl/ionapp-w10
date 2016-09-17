using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.UI.Popups;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Ion10.Services;

namespace Ion10.ViewModels {
    public class MainPageViewModel : ViewModelBase {
        public MainPageViewModel() {
            if(Windows.ApplicationModel.DesignMode.DesignModeEnabled) {
                Value = "Designtime value";
            }
        }

        string _Value = "Gas";
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState) {
            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending) {
            if(suspending) {
                suspensionState[nameof(Value)] = Value;
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args) {
            args.Cancel = false;
            await Task.CompletedTask;
        }

        public void GotoSettings() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 0);

        public void GotoPrivacy() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 1);

        public void GotoAbout() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 2);

    }
}

