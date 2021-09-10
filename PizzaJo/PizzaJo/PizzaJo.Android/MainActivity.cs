using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Content;
using System.IO;
using Plugin.LocalNotifications;

namespace PizzaJo.Droid
{
    [Activity(Label = "Pizzaiolo Pro", Icon = "@mipmap/icon", Theme = "@style/MyTheme.Splash", LaunchMode = LaunchMode.SingleTop ,MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            LocalNotificationsImplementation.NotificationIconId = Resource.Drawable.pizzaicon;

            base.SetTheme(Resource.Style.MainTheme);
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            var dbName = "PizzaiolaPro.db3";
            string folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            string completePath = Path.Combine(folderPath, dbName);

            LoadApplication(new App(completePath));

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
        }

    }
}