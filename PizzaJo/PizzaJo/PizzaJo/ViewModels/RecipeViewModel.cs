using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PizzaJo.ViewModels
{
    /// <summary>
    /// Recipe View Model for the Recipe Page
    /// </summary>
    public class RecipeViewModel : BaseViewModel
    {
        public OSAppTheme currentTheme = Application.Current.RequestedTheme;
        public ICommand CalcCommand { get; }

        /// <summary>
        /// Intilize Recipe View Model
        /// </summary>
        public RecipeViewModel()
        {
            try
            {
                Title = "Hydration Calculator";
                CalcCommand = new Command(Calc);

                IntilizeTextColors();

                MessagingCenter.Subscribe<AboutViewModel>(this, "DarkMode1", (sender) =>
                {
                    IntilizeTextColors();
                });
            }
            catch (Exception ex)
            {
                App.Current.MainPage.DisplayAlert("Error", ex.Message, "Close");
            }
        }

        /// <summary>
        /// Intilize text colors not done in styles for Recipe page
        /// </summary>
        private void IntilizeTextColors()
        {
            try
            {
                if (currentTheme == OSAppTheme.Dark)
                {
                    HydrationColor = Color.White;
                    DoughColor = Color.White;
                    PizzasColor = Color.White;
                }
                else
                {
                    HydrationColor = Color.Black;
                    DoughColor = Color.Black;
                    PizzasColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


        /// <summary>
        /// Calculates the required water and flour
        /// </summary>
        private void Calc()
        {
            try
            {
                if ((HydrationText > 0 || HydrationText != null) && (DoughText > 0 || DoughText != null) && (PizzasText > 0 || PizzasText != null))
                {
                    int tempdough = (int)(Salt == null ? DoughText * PizzasText : (DoughText * PizzasText) - Salt);
                    tempdough = (int)(Yeast == null ? tempdough : tempdough - Yeast);
                    double flourcalc = ((double)((tempdough) / (1 + (HydrationText * .01))));
                    double watercalc = (double)((tempdough) - flourcalc);
                    Flour = "Flour: " + Math.Round(flourcalc).ToString() + "g";
                    Water = "Water: " + Math.Round(watercalc).ToString() + "g";
                    Measurements = $"Below is your doughs flour and water measurements for {HydrationText}% hydration and {PizzasText} dough balls.";
                }
                else
                {
                    if (HydrationText <= 0 || HydrationText == null)
                    {
                        HydrationColor = Color.Red;
                    }

                    if (DoughText <= 0 || DoughText == null)
                    {
                        DoughColor = Color.Red;
                    }

                    if (PizzasText <= 0 || PizzasText == null)
                    {
                        PizzasColor = Color.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                App.Current.MainPage.DisplayAlert("Error", ex.Message, "Close");
            }
        }



        private int? hydration;
        public int? HydrationText
        {
            get => hydration;
            set
            {
                if (value == hydration)
                    return;
                hydration = value;
                if (currentTheme == OSAppTheme.Dark)
                    HydrationColor = Color.White;
                else
                    HydrationColor = Color.Black;

                OnPropertyChanged();
            }
        }

        private int? dough;
        public int? DoughText
        {
            get => dough;
            set
            {
                if (value == dough)
                    return;
                dough = value;
                if (currentTheme == OSAppTheme.Dark)
                    DoughColor = Color.White;
                else
                    DoughColor = Color.Black;
                OnPropertyChanged();
            }
        }

        private int? pizzas;
        public int? PizzasText
        {
            get => pizzas;
            set
            {
                if (value == pizzas)
                    return;
                pizzas = value;
                if (currentTheme == OSAppTheme.Dark)
                    PizzasColor = Color.White;
                else
                    PizzasColor = Color.Black;

                OnPropertyChanged();
            }
        }

        private string flour = "";
        public string Flour
        {
            get => flour;
            set
            {
                if (value == flour)
                    return;
                flour = value;
                OnPropertyChanged();
            }
        }

        private string measurements = "";
        public string Measurements
        {
            get => measurements;
            set
            {
                if (value == measurements)
                    return;
                measurements = value;
                OnPropertyChanged();
            }
        }

        private string water = "";
        public string Water
        {
            get => water;
            set
            {
                if (value == water)
                    return;
                water = value;
                OnPropertyChanged();
            }
        }

        private int? salt;
        public int? Salt
        {
            get => salt;
            set
            {
                if (value == salt)
                    return;
                salt = value;
                OnPropertyChanged();
            }
        }

        private int? yeast;

        public int? Yeast
        {
            get => yeast;
            set
            {
                if (value == yeast)
                    return;
                yeast = value;
                OnPropertyChanged();
            }
        }

        private Color hydrationcolor;
        public Color HydrationColor
        {
            get => hydrationcolor;
            set
            {
                hydrationcolor = value;
                OnPropertyChanged();
            }
        }

        private Color doughcolor;
        public Color DoughColor
        {
            get => doughcolor;
            set
            {
                doughcolor = value;
                OnPropertyChanged();
            }
        }

        private Color pizzascolor;
        public Color PizzasColor
        {
            get => pizzascolor;
            set
            {
                pizzascolor = value;
                OnPropertyChanged();
            }
        }


    }
}
