using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaJo.Models
{
    class DoughNotificationsModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int NotificationGuid {get; set;}
        public string NotificationType { get; set; }
        public string NotificationDate { get; set; }
    }
}
