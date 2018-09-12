using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TravelerBot.Api.Data.Repositories;
using TravelerBot.MVC.Models;

namespace TravelerBot.MVC.Data.Repositories
{
    public class TripRepository : ITripRepository
    {
        TripContext tripContext = new TripContext();

        public Trip Get(int accountId)
        {
            return tripContext.Trips.FirstOrDefault(t => t.AccountId == accountId);
        }

        public void Add(Trip trip)
        {
            tripContext.Trips.Add(trip);
            tripContext.SaveChanges();
        }

        public void Update(Trip trip)
        {
            tripContext.Entry(trip).State = System.Data.Entity.EntityState.Modified;
            tripContext.SaveChanges();
        }

        //protected override void Dispose(bool disposing)
        //{
        //    tripContext.Dispose();
        //    base.Dispose(disposing);
        //}
    }
}