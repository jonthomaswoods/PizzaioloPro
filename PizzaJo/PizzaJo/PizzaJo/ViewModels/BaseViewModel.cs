using PizzaJo.Models;
using PizzaJo.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;



namespace PizzaJo.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public ICommand AboutNavCommand { get; }

        public BaseViewModel()
        {
            try
            {
                AboutNavCommand = new Command(async () => await AboutNav());
            }
            catch (Exception ex)
            {
                App.Current.MainPage.DisplayAlert("Error", ex.Message, "Close");
            }
        }

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        public async Task AboutNav()
        {
            try
            {
                await Application.Current.MainPage.Navigation.PushAsync(new AboutPage());
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Close");
            }
        }
        
        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        
    }
}
