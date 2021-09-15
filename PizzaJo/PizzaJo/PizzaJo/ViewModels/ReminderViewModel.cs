using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using SQLite;
using PizzaJo.Models;
using Plugin.LocalNotification;
using Plugin.LocalNotifications;

namespace PizzaJo.ViewModels
{
    /// <summary>
    /// Reminder View Model for Reminder Page
    /// </summary>
    class ReminderViewModel : BaseViewModel
    {
        public ICommand SetReminder { get; }

        /// <summary>
        /// Reminder View Model Intilization
        /// </summary>
        public ReminderViewModel()
        {
            try
            {
                SetReminder = new Command(ExecuteReminder);

                IntilizeTextColors();

                MessagingCenter.Subscribe<AboutViewModel>(this, "DarkMode", (sender) =>
                {
                    IntilizeTextColors();
                });

                using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                {
                    conn.CreateTable<DoughNotificationsModel>();

                    var notificationsquery = conn.Query<DoughNotificationsModel>("Select * FROM DoughNotificationsModel");

                    var templist = new List<DoughNotificationsModel>();
                    if (notificationsquery.Count > 0)
                    {
                        foreach (var notification in notificationsquery)
                        {
                            if (Convert.ToDateTime(notification.NotificationDate) < DateTime.Now)
                            {
                                conn.Delete(notification);
                                templist.Add(notification);
                            }
                        }

                        if (templist.Count != 0)
                        {
                            foreach (var removetemp in templist)
                            {
                                notificationsquery.Remove(removetemp);
                            }
                        }

                        if (notificationsquery.Count != 0)
                            DoughNotifications = notificationsquery;
                        else
                            NoReminders();

                    }
                    else
                    {
                        NoReminders();
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Intilize text colors not done in styles for Reminders page
        /// </summary>
        private void IntilizeTextColors()
        {
            try
            {
                if (Application.Current.RequestedTheme == OSAppTheme.Dark)
                {
                    ReminderColor = Color.FromHex("#e2f1f8");
                    ReminderSecondColor = Color.FromHex("#e2f1f8");
                }
                else
                {
                    ReminderColor = Color.FromHex("#373737");
                    ReminderSecondColor = Color.FromHex("#373737");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Reminder Command for all 3 reminders
        /// </summary>
        private async void ExecuteReminder()
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                {
                    conn.CreateTable<DoughNotificationsModel>();

                    if (InitialStack)
                    {
                        if (StartTime != null && StartDate != null && (ReminderTime > 1 || ReminderTime != null))
                        {
                            if (InitialStack)
                            {
                                StartDate = StartDate.Subtract(StartDate.TimeOfDay).Add(StartTime).AddHours((int)ReminderTime);

                                if (StartDate > DateTime.Now)
                                {
                                    int notGUID = BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0);

                                    if (Device.RuntimePlatform == Device.iOS)
                                    {
                                        string nottitle = "Dough Reminder!";
                                        string notmessage = "Your doughs first fermentation is done. Let's make some dough balls!";
                                        CrossLocalNotifications.Current.Show(nottitle, notmessage, notGUID, StartDate.AddHours((int)ReminderTime));
                                    }
                                    else if (Device.RuntimePlatform == Device.Android)
                                    {
                                        var notification = new NotificationRequest
                                        {
                                            NotificationId = notGUID,
                                            Title = "Dough Reminder!",
                                            Description = "Your doughs first fermentation is done. Let's make some dough balls!",
                                            ReturningData = "",
                                            Android = { IconSmallName = { ResourceName = "pizza" }, IconLargeName = { ResourceName = "icon" } },
                                            Schedule = { NotifyTime = StartDate.AddHours((int)ReminderTime) }
                                        };
                                        await NotificationCenter.Current.Show(notification);
                                    }

                                    var notificationmodel = new DoughNotificationsModel();
                                    notificationmodel.NotificationType = "Initial Fermentation";
                                    notificationmodel.NotificationDate = StartDate.ToString();

                                    conn.Insert(notificationmodel);
                                    var notificationsquery = conn.Query<DoughNotificationsModel>("Select * FROM DoughNotificationsModel");

                                    if (notificationsquery.Count > 0)
                                        DoughNotifications = notificationsquery;

                                }
                                else
                                {
                                    await Application.Current.MainPage.DisplayAlert("Invalid Date", "Your date/time must be in the future.", "Okay");

                                }
                            }
                        }
                        else
                        {
                            if (ReminderTime < 1 || ReminderTime != null)
                            {
                                ReminderColor = Color.Red;
                                await Task.Delay(300);
                                ReminderColor = Color.Black;
                                await Task.Delay(300);
                                ReminderColor = Color.Red;
                                await Task.Delay(300);
                                ReminderColor = Color.Black;
                            }
                        }
                    }

                    if (SecondStack)
                    {
                        if (SecondStartDate != null && SecondStartTime != null && (ReminderSecondTime > 1 || ReminderSecondTime != null))
                        {
                            SecondStartDate = SecondStartDate.Subtract(SecondStartDate.TimeOfDay).Add(SecondStartTime).AddHours((int)ReminderSecondTime);

                            if (SecondStartDate > DateTime.Now)
                            {
                                if (!InitialStack || (InitialStack && SecondStartDate >= StartDate))
                                {
                                    int notGUID = BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0);

                                    if (Device.RuntimePlatform == Device.iOS)
                                    {
                                        string nottitle = "Dough Reminder!";
                                        string notmessage = "Your doughs second fermentation is done. Pizza time is close!";
                                        CrossLocalNotifications.Current.Show(nottitle, notmessage, notGUID, SecondStartDate);
                                    }
                                    else if (Device.RuntimePlatform == Device.Android)
                                    {
                                        var notification = new NotificationRequest
                                        {
                                            NotificationId = notGUID,
                                            Title = "Dough Reminder!",
                                            Description = "Your doughs second fermentation is done. Pizza time is close!",
                                            ReturningData = "",
                                            Android = { IconSmallName = { ResourceName = "pizza" }, IconLargeName = { ResourceName = "icon" } },
                                            Schedule = { NotifyTime = SecondStartDate }
                                        };
                                        await NotificationCenter.Current.Show(notification);
                                    }

                                    var secondnotificationmodel = new DoughNotificationsModel();
                                    secondnotificationmodel.NotificationType = "Second Fermentation";
                                    secondnotificationmodel.NotificationDate = SecondStartDate.AddHours((int)ReminderSecondTime).ToString();

                                    conn.Insert(secondnotificationmodel);
                                    var notificationsquery = conn.Query<DoughNotificationsModel>("Select * FROM DoughNotificationsModel");

                                    if (notificationsquery.Count > 0)
                                        DoughNotifications = notificationsquery;
                                }
                                else
                                {
                                    await Application.Current.MainPage.DisplayAlert("Invalid Date", "Your second date/time must be after the inital date/time.", "Okay");
                                }
                            }
                            else
                            {
                                await Application.Current.MainPage.DisplayAlert("Invalid Date", "Your date/time must be in the future.", "Okay");
                            }
                        }
                        else
                        {
                            if (ReminderSecondTime < 1 || ReminderSecondTime != null)
                            {
                                ReminderSecondColor = Color.Red;
                                await Task.Delay(300);
                                ReminderSecondColor = Color.Black;
                                await Task.Delay(300);
                                ReminderSecondColor = Color.Red;
                                await Task.Delay(300);
                                ReminderSecondColor = Color.Black;
                            }
                        }
                    }


                    if (FinishedStack)
                    {
                        if (FinishedStartDate != null && FinishedStartTime != null)
                        {
                            FinishedStartDate = FinishedStartDate.Subtract(FinishedStartDate.TimeOfDay).Add(FinishedStartTime);

                            if (FinishedStartDate > DateTime.Now)
                            {
                                if (!SecondStack || (SecondStack && FinishedStartDate >= SecondStartDate))
                                {
                                    int notGUID = BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0);

                                    if (Device.RuntimePlatform == Device.iOS)
                                    {
                                        string nottitle = "Pizza Time!";
                                        string notmessage = "Your are ready to make some pizza!";
                                        CrossLocalNotifications.Current.Show(nottitle, notmessage, notGUID, FinishedStartDate);
                                    }
                                    else if (Device.RuntimePlatform == Device.Android)
                                    {
                                        var notification = new NotificationRequest
                                        {
                                            NotificationId = notGUID,
                                            Title = "Pizza Time!",
                                            Description = "Your are ready to make some pizza!",
                                            Android = { IconSmallName = { ResourceName = "pizza" }, IconLargeName = { ResourceName = "icon" } },
                                            Schedule = { NotifyTime = FinishedStartDate }
                                        };
                                        var res = await NotificationCenter.Current.Show(notification);
                                    }

                                    var finalnotificationmodel = new DoughNotificationsModel();
                                    finalnotificationmodel.NotificationGuid = notGUID;
                                    finalnotificationmodel.NotificationType = "Final Fermentation";
                                    finalnotificationmodel.NotificationDate = FinishedStartDate.ToString();

                                    conn.Insert(finalnotificationmodel);
                                    var notificationsquery = conn.Query<DoughNotificationsModel>("Select * FROM DoughNotificationsModel");

                                    DoughNotifications.Clear();
                                    if (notificationsquery.Count > 0)
                                        DoughNotifications = notificationsquery;
                                }
                                else
                                {
                                    await Application.Current.MainPage.DisplayAlert("Invalid Date", "Your finished date/time must be after the second date/time.", "Okay");
                                }
                            }
                            else
                            {
                                await Application.Current.MainPage.DisplayAlert("Invalid Date", "Your date/time must be in the future.", "Okay");

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
        /// Delete a selected reminder 
        /// </summary>
        /// <param name="notificationsModel"> The Selected notification in the list view</param>
        private async void DeleteReminder(DoughNotificationsModel notificationsModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(notificationsModel.NotificationDate))
                {
                    var res = await Application.Current.MainPage.DisplayAlert("Delete Reminder", $"Delete reminder at {notificationsModel.NotificationDate}?", "Yes", "No");
                    if (res)
                    {
                        using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
                        {
                            conn.CreateTable<DoughNotificationsModel>();
                            conn.Delete(notificationsModel);

                            var notificationsquery = conn.Query<DoughNotificationsModel>("Select * FROM DoughNotificationsModel");

                            DoughNotifications.Clear();
                            if (notificationsquery.Count > 0)
                                DoughNotifications = notificationsquery;
                            else
                                NoReminders();
                        }


                        if (Device.RuntimePlatform == Device.iOS)
                        {
                            CrossLocalNotifications.Current.Cancel(notificationsModel.NotificationGuid);
                        }
                        else if (Device.RuntimePlatform == Device.Android)
                        {
                            NotificationCenter.Current.Cancel(notificationsModel.NotificationGuid);
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
        /// Shown if there are no current reminders
        /// </summary>
        private void NoReminders()
        {
            try
            {
                var emptynot = new List<DoughNotificationsModel>();
                var emptymodel = new DoughNotificationsModel();
                emptymodel.NotificationType = "No Reminders";
                emptynot.Add(emptymodel);
                DoughNotifications = emptynot;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }



        private DoughNotificationsModel selectedNotification;
        public DoughNotificationsModel SelectedNotification
        {
            get { return selectedNotification; }
            set
            {
                if (selectedNotification != value)
                {
                    selectedNotification = value;
                    OnPropertyChanged();

                    DeleteReminder(selectedNotification);
                }
            }
        }

        private Color remindercolor;
        public Color ReminderColor
        {
            get => remindercolor;
            set
            {
                remindercolor = value;
                OnPropertyChanged();
            }
        }

        private Color remindersecondcolor;
        public Color ReminderSecondColor
        {
            get => remindersecondcolor;
            set
            {
                remindersecondcolor = value;
                OnPropertyChanged();
            }
        }

        private DateTime startdate = DateTime.Now;
        public DateTime StartDate
        {
            get => startdate;
            set
            {
                startdate = value;
                OnPropertyChanged();
            }
        }

        private TimeSpan starttime = DateTime.Now.TimeOfDay;
        public TimeSpan StartTime
        {
            get => starttime;
            set
            {
                starttime = value;
                OnPropertyChanged();
            }
        }

        private DateTime secondstartdate = DateTime.Now;
        public DateTime SecondStartDate
        {
            get => secondstartdate;
            set
            {
                secondstartdate = value;
                OnPropertyChanged();
            }
        }

        private TimeSpan secondstarttime = DateTime.Now.TimeOfDay;
        public TimeSpan SecondStartTime
        {
            get => secondstarttime;
            set
            {
                secondstarttime = value;
                OnPropertyChanged();
            }
        }

        private DateTime finishedstartdate = DateTime.Now;
        public DateTime FinishedStartDate
        {
            get => finishedstartdate;
            set
            {
                finishedstartdate = value;
                OnPropertyChanged();
            }
        }

        private TimeSpan finishedstarttime = DateTime.Now.TimeOfDay;
        public TimeSpan FinishedStartTime
        {
            get => finishedstarttime;
            set
            {
                finishedstarttime = value;
                OnPropertyChanged();
            }
        }

        private int? reminderTime;
        public int? ReminderTime
        {
            get => reminderTime;
            set
            {
                if (value == reminderTime)
                    return;

                if (Application.Current.RequestedTheme == OSAppTheme.Dark)
                    ReminderColor = Color.FromHex("#e2f1f8");
                else
                    ReminderColor = Color.FromHex("#373737");

                reminderTime = value;
                OnPropertyChanged();
            }
        }

        private int? remindersecondTime;
        public int? ReminderSecondTime
        {
            get => remindersecondTime;
            set
            {
                if (value == remindersecondTime)
                    return;
                if (Application.Current.RequestedTheme == OSAppTheme.Dark)
                    ReminderSecondColor = Color.FromHex("#e2f1f8");
                else
                    ReminderSecondColor = Color.FromHex("#373737");

                remindersecondTime = value;
                OnPropertyChanged();
            }
        }

        private bool initialstack = true;
        public bool InitialStack
        {
            get => initialstack;
            set
            {
                if (value == initialstack)
                    return;
                initialstack = value;
                OnPropertyChanged();
            }
        }

        private bool secondstack = false;
        public bool SecondStack
        {
            get => secondstack;
            set
            {
                if (value == secondstack)
                    return;
                secondstack = value;
                OnPropertyChanged();
            }
        }
        private bool finishedstack = false;
        public bool FinishedStack
        {
            get => finishedstack;
            set
            {
                if (value == finishedstack)
                    return;
                finishedstack = value;
                OnPropertyChanged();
            }
        }

        private List<DoughNotificationsModel> doughNotifications;
        public List<DoughNotificationsModel> DoughNotifications
        {
            get => doughNotifications;
            set { doughNotifications = value; OnPropertyChanged(); }
        }
    }
}
