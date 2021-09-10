using PizzaJo.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PizzaJo.ViewModels
{
    /// <summary>
    /// View model for the About Page
    /// </summary>
    public class AboutViewModel : BaseViewModel
    {
        public INavigation Navigation { get; set; }
        public ICommand CruxCommand { get; }
        public ICommand SendEmailCommand { get; }
        public ICommand PersonalCommand { get; }

        /// <summary>
        /// About View Model Intilization
        /// </summary>
        public AboutViewModel()
        {
            try
            {
                CruxCommand = new Command(Crux);
                SendEmailCommand = new Command(SendEmail);
                PersonalCommand = new Command(Personal);

                var isdark = Preferences.Get("darkmode", "true");

                if (isdark == "true")
                    DarkMode = true;
                else if (isdark == "false")
                    DarkMode = false;
                
            }
            catch (Exception ex)
            {
                App.Current.MainPage.DisplayAlert("Error", ex.Message, "Close");
            }
        }

        /// <summary>
        /// Redirect to Crux website
        /// </summary>
        public void Crux()
        {
            try
            {
                var uri = new Uri("https://cruxresolutions.com/");
                Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                App.Current.MainPage.DisplayAlert("Error", ex.Message, "Close");
            }
        }

        /// <summary>
        /// Redirect to personl jonthomaswoods website
        /// </summary>
        public void Personal()
        {
            try
            {
                var uri = new Uri("https://jonthomaswoods.com/");
                Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                App.Current.MainPage.DisplayAlert("Error", ex.Message, "Close");
            }
        }

        /// <summary>
        ///User email for error or suggestion 
        /// </summary>
        public void SendEmail()
        {
            try
            {
                string user = Name;
                string message = Message;

                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(message))
                {
                    DisablePage = true;

                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                    mail.From = new MailAddress("support@cruxresolutions.com");
                    mail.To.Add("support@cruxresolutions.com");
                    mail.Subject = "Pizzaiolo Error";
                    mail.Body = user + ": " + message;
                    SmtpServer.Port = 587;
                    SmtpServer.Host = "smtp.gmail.com";
                    SmtpServer.EnableSsl = true;
                    SmtpServer.UseDefaultCredentials = false;
                    SmtpServer.Credentials = new System.Net.NetworkCredential("support@cruxresolutions.com", "CodeCode2020!");

                    SmtpServer.Send(mail);
                    mail.Dispose();
                    SmtpServer.Dispose();

                    DisablePage = false;
                    App.Current.MainPage.DisplayAlert("Success", "Email sent to app team, please be patient for a response.", "Okay");
                }
                else
                {
                    App.Current.MainPage.DisplayAlert("Incomplete Form", "Please enter your email and a message in order to email the app support team.", "Cancel");
                }
            }
            catch (Exception ex)
            {
                App.Current.MainPage.DisplayAlert("Error", ex.Message, "Close");
            }
        }

        
        private string name = "";
        public string Name
        {
            get => name;
            set
            {
                if (value == name)
                    return;
                name = value;
                OnPropertyChanged();
            }
        }


        private string message = "";
        public string Message
        {
            get => message;
            set
            {
                if (value == message)
                    return;
                message = value;
                OnPropertyChanged();
            }
        }

        private bool darkmode = false;
        public bool DarkMode
        {
            get => darkmode;
            set
            {
                if (DarkMode)
                {
                    Application.Current.UserAppTheme = OSAppTheme.Dark;
                    Preferences.Set("darkmode", "true");
                    MessagingCenter.Send(this, "DarkMode1");
                    MessagingCenter.Send(this, "DarkMode2");
                }
                else
                {
                    Application.Current.UserAppTheme = OSAppTheme.Light;
                    Preferences.Set("darkmode", "false");
                    MessagingCenter.Send(this, "DarkMode1");
                    MessagingCenter.Send(this, "DarkMode2");
                }

                darkmode = value;
                OnPropertyChanged();
            }
        }

        private bool disablepage = false;
        public bool DisablePage
        {
            get => disablepage;
            set
            {
                disablepage = value;
                OnPropertyChanged();
            }
        }

    }
}
