using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PizzaJo.Droid.Implementations;
using PizzaJo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

[assembly: Dependency(typeof(UserInformationInterface))]
namespace PizzaJo.Droid.Implementations
{
    public class UserInformationInterface : IUserInformation
    {
        public string GetUserName()
        {
            return Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
        }
    }
}