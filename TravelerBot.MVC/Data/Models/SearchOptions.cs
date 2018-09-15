using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelerBot.MVC.Data.Models
{
    public class SearchOptions
    {
        public Guid SearchOptionsId { get; set; }

        public string Filter { get; set; }
    }
}