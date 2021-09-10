using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using PizzaJo.Views;
using System;
using System.IO;
using System.Reflection;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PizzaJo
{
    public partial class App : Application
    {
        public static string FilePath;
        public App(string filepath)
        {
            InitializeComponent();

            FilePath = filepath;

            var isdark = Preferences.Get("darkmode", "true");

            if (isdark == "true")
                Current.UserAppTheme = OSAppTheme.Dark;
            else
                Current.UserAppTheme = OSAppTheme.Light;

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
