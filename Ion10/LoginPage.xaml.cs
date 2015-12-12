// Copyright (c) 2015, Joshua Cotton
// All rights reserved.

using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;

namespace Ion10 {
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page {
        public LoginPage() {
            InitializeComponent();
        }

        private async void Login(object sender, RoutedEventArgs e) {
            var user = Username.Text;
            var pass = Password.Password;
            if(user == "" || pass == "") {
                await new MessageDialog("Please enter a username and password").ShowAsync();
                return;
            }
            App.HttpClient = new HttpClient(new IonHttpFilter(user, pass, new Uri("https://ion.tjhsst.edu")));
            if(!(await App.HttpClient.GetAsync(new Uri("/profile"))).IsSuccessStatusCode) {
                await new MessageDialog("Invalid username or password").ShowAsync();
                App.HttpClient.Dispose();
                return;
            }
            if(RememberMe.IsChecked.Value) {
                //TODO: save username/password
            }
            (Window.Current.Content as Frame).Navigate(typeof(MainPage));
        }
    }
}
