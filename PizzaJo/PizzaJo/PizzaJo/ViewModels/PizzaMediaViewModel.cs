using Firebase.Database;
using Firebase.Database.Query;
using Google.Apis.Auth.OAuth2;
using PizzaJo.Config;
using PizzaJo.Interfaces;
using PizzaJo.Models;
using Plugin.Media;
using Plugin.Media.Abstractions;
using SQLite;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PizzaJo.ViewModels
{
    /// <summary>
    /// View model for the Pizza Media Page
    /// </summary>
    public class PizzaMediaViewModel : BaseViewModel
    {

        public ICommand RefreshList { get; set; }
        public ICommand SelectImage { get; set; }
        public ICommand UploadImage { get; set; }
        public ICommand LoadMore { get; set; }
        public ICommand LikeCommand { get; set; }

        private byte[] SelectedImage;


        /// <summary>
        /// Initilize the Pizza Media page
        /// </summary>
        public PizzaMediaViewModel()
        {
            try
            {
                var current = Connectivity.NetworkAccess;

                if (current == NetworkAccess.Internet)
                {
                    NoInternetPage = false;
                    InternetPage = true;
                    RefreshList = new Command(PerformRefresh);
                    SelectImage = new Command(ImageSelect);
                    UploadImage = new Command(ImageUpload);
                    LoadMore = new Command(LoadMoreImages);
                    LikeCommand = new Command<string>((fireid) => LikeImage(fireid));

                    GetNewPizzas();
                }
                else
                {
                    NoInternetPage = true;
                    InternetPage = false;
                }
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Select an image from internal storage
        /// </summary>
        private async void ImageSelect()
        {
            try
            {
                await CrossMedia.Current.Initialize();
                if (CrossMedia.Current.IsPickPhotoSupported)
                {
                    var mediaOptions = new PickMediaOptions()
                    {
                        PhotoSize = PhotoSize.Full
                    };

                    var selectedImage = await CrossMedia.Current.PickPhotoAsync(mediaOptions);

                    if (selectedImage != null)
                    {


                        SelectedImageSource = ImageSource.FromStream(() => selectedImage.GetStream());

                        var stream1 = new MemoryStream();
                        selectedImage.GetStream().CopyTo(stream1);
                        SelectedImage = stream1.ToArray();
                            

                        ImageShown = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Post a PizzaMediaModel to Firebase DB
        /// </summary>
        private async void ImageUpload()
        {
            try
            {
                if (!string.IsNullOrEmpty(PizzaDescription) && SelectedImage != null)
                {
                    var device_id = DependencyService.Get<IUserInformation>().GetUserName();

                    DisablePage = true;
                    FirebaseClient fc = new FirebaseClient(ConfigValues.FirebaseClient, 
                        new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(ConfigValues.FirebaseSecret) });
                    var result = await fc
                     .Child("PizzaTable")
                     .PostAsync(new PizzaMediaModel() { PizzaImageJson = SelectedImage, PhotoDate = DateTime.Now.ToString(), Description = PizzaDescription, Likes = 0, DeviceID = device_id });
                    ImageShown = false;
                    GetNewPizzas();

                    PizzaDescription = null;

                    DisablePage = false;
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Refresh the listview of the pizza media
        /// </summary>
        private void PerformRefresh()
        {
            try
            {
                IsRefreshing = true;
                GetNewPizzas();
                IsRefreshing = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        //Start count for the amount of items in the list view
        public int listcount = 5;

        /// <summary>
        /// Gets the PizzaMediaModel items from the Firebase DB
        /// </summary>
        private async void GetNewPizzas()
        {
            try
            {
                DisablePage = true;
                PizzaItems = new ObservableCollection<PizzaMediaModel>();

                FirebaseClient fc = new FirebaseClient(ConfigValues.FirebaseClient,
                                    new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(ConfigValues.FirebaseSecret) });

                var GetPizza = (await fc
                  .Child("PizzaTable")
                  .OnceAsync<PizzaMediaModel>()).Select(item => new PizzaMediaModel
                  {
                      PizzaImageJson = item.Object.PizzaImageJson,
                      Description = item.Object.Description,
                      PhotoDate = item.Object.PhotoDate,
                      Likes = item.Object.Likes,
                      FireKey = item.Key
                  }).OrderByDescending(d => d.PhotoDate).ToList();

                int count = 0;
                foreach (var pizza in GetPizza)
                {
                    pizza.PizzaImage = ImageSource.FromStream(() => new MemoryStream(pizza.PizzaImageJson));

                    PizzaItems.Add(pizza);
                    count++;
                    if (count >= listcount)
                        break;
                }
                
                DisablePage = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

        /// <summary>
        /// Put a new 'Like' to an existing PizzaMediaModel in the Firebase DB
        /// </summary>
        /// <param name="fireid"> The Firebase item Key</param>
        private async void LikeImage(string fireid)
        {
            try
            {
                if (!string.IsNullOrEmpty(fireid))
                {
                    using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                    {
                        conn.CreateTable<PreviousLikesModel>();
                        var liked = conn.Query<PreviousLikesModel>("Select * FROM PreviousLikesModel Where Key = ?", fireid).FirstOrDefault();

                       var pizzamodel = PizzaItems.Where(k => k.FireKey == fireid).FirstOrDefault();
                        

                        if (liked == null)
                        {
                            if (pizzamodel != null)
                            {
                                DisablePage = true;
                                pizzamodel.Likes++;
                                FirebaseClient fc = new FirebaseClient(ConfigValues.FirebaseClient,
                                    new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(ConfigValues.FirebaseSecret) });

                                await fc.Child("PizzaTable").Child(pizzamodel.FireKey).PutAsync(new PizzaMediaModel() { PizzaImageJson = pizzamodel.PizzaImageJson, PhotoDate = pizzamodel.PhotoDate, Description = pizzamodel.Description, Likes = pizzamodel.Likes });

                                PizzaItems[PizzaItems.IndexOf(PizzaItems.Where(k => k.FireKey == pizzamodel.FireKey).FirstOrDefault())] = pizzamodel;

                                var insertliked = new PreviousLikesModel();
                                insertliked.Key = pizzamodel.FireKey;
                                conn.Insert(insertliked);

                                DisablePage = false;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// Adds 5 to the list view count
        /// </summary>
        private void LoadMoreImages()
        {
            try
            {
                listcount += 5;
                GetNewPizzas();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


        private ObservableCollection<PizzaMediaModel> pizzaitems;
        public ObservableCollection<PizzaMediaModel> PizzaItems
        {
            get { return pizzaitems; }
            set
            {
                pizzaitems = value;
                OnPropertyChanged();
            }
        }

        private bool isRefreshing = false;
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set
            {
                isRefreshing = value;
                OnPropertyChanged();
            }
        }

        private ImageSource selectedimagesource;

        public ImageSource SelectedImageSource
        {
            get => selectedimagesource;
            set
            {
                selectedimagesource = value;
                OnPropertyChanged();
            }
        }

        private bool imageshown = false;
        public bool ImageShown
        {
            get => imageshown;
            set
            {
                imageshown = value;
                OnPropertyChanged();
            }
        }

        private string pizzadescription;
        public string PizzaDescription
        {
            get => pizzadescription;
            set
            {
                pizzadescription = value;
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

        private bool internetpage = false;
        public bool InternetPage
        {
            get => internetpage;
            set
            {
                internetpage = value;
                OnPropertyChanged();
            }
        }

        private bool nointernetpage = false;
        public bool NoInternetPage
        {
            get => nointernetpage;
            set
            {
                nointernetpage = value;
                OnPropertyChanged();
            }
        }

        private PizzaMediaModel selectedlike;
        public PizzaMediaModel SelectedLike
        {
            get { return selectedlike; }
            set
            {
                if (selectedlike != value)
                {
                    selectedlike = value;

                    OnPropertyChanged();

                }
            }
        }

    }
}
