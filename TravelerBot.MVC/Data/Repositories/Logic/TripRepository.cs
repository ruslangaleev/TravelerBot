using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TravelerBot.Api.Data.Repositories;
using TravelerBot.MVC.Data.Models;
using TravelerBot.MVC.Models;

namespace TravelerBot.MVC.Data.Repositories
{
    public class TripRepository : ITripRepository
    {
        TripContext tripContext = new TripContext();

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

        public void Delete(Guid tripId)
        {
            var trip = tripContext.Trips.Find(tripId);
            if (trip != null)
            {
                tripContext.Trips.Remove(trip);
            }
        }

        public Trip Get()
        {
            return tripContext.Trips.FirstOrDefault(t => t.DateTime >= DateTime.Now && t.IsPublished);
        }

        public IEnumerable<Trip> Get(string from, string to, DateTime? whenn)
        {
            IQueryable<Trip> query = tripContext.Trips;
            if (!string.IsNullOrEmpty(from))
            {
                query = query.Where(t => t.Whence == from);
            }
            if (!string.IsNullOrEmpty(to))
            {
                query = query.Where(t => t.Where == to);
            }
            if (whenn != null)
            {
                var when = (DateTime)whenn;
                var dateStart = new DateTime(when.Year, when.Month, when.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                var dateEnd = new DateTime(when.Year, when.Month, when.Day, 23, 59, 59);

                query = query.Where(t => t.DateTime >= dateStart && t.DateTime <= dateEnd);
            }
            else
            {
                var when = DateTime.Now;
                query = query.Where(t => t.DateTime >= when);
            }
            return query.Include(t => t.UserState).ToList();
        }

        public Trip Get(Guid tripId)
        {
            return tripContext.Trips.Find(tripId);
        }

        public void SaveChanges()
        {
            tripContext.SaveChanges();
        }

        public Trip GetTripByUserStateId(Guid userStateId)
        {
            return tripContext.Trips.FirstOrDefault(t => t.UserStateId == userStateId && t.IsPublished == false);
        }

        public IEnumerable<Trip> GetTripsByUserStateId(Guid userStateId)
        {
            return tripContext.Trips.Where(t => t.UserStateId == userStateId && t.IsPublished == true);
        }
    }
}