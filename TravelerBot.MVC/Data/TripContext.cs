using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TravelerBot.MVC.Models;

namespace TravelerBot.MVC.Data
{
    public class TripContext : DbContext
    {
        public DbSet<Trip> Trips { get; set; }
    }
}