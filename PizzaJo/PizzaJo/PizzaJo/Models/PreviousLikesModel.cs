using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaJo.Models
{
    class PreviousLikesModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Key { get; set; }
    }
}
