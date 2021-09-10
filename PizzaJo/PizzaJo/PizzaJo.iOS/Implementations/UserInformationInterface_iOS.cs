using Foundation;
using PizzaJo.Interfaces;
using PizzaJo.iOS.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(UserInformationInterface_iOS))]
namespace PizzaJo.iOS.Implementations
{
    public class UserInformationInterface_iOS : IUserInformation
    {
        public string GetUserName()
        {
            return UIDevice.CurrentDevice.IdentifierForVendor.ToString();
        }
    }
}