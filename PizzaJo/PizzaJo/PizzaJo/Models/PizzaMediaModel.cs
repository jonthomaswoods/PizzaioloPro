using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace PizzaJo.Models
{
    public class PizzaMediaModel
    {
        [JsonIgnore]
        public ImageSource PizzaImage { get; set; }
        public byte[] PizzaImageJson { get; set; }
        public string Description { get; set; }
        public string PhotoDate { get; set; }
        public int Likes { get; set; }
        public string DeviceID { get; set; }

        [JsonIgnore]
        public string FireKey { get; set; }
    }
}
