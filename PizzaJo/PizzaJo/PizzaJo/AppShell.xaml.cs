using PizzaJo.ViewModels;
using PizzaJo.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace PizzaJo
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(RecipePage), typeof(RecipePage));
            Routing.RegisterRoute(nameof(RemindersPage), typeof(RemindersPage));
            Routing.RegisterRoute(nameof(PizzaMediaPage), typeof(PizzaMediaPage));
            Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));
        }

    }
}
