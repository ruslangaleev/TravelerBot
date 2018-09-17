using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TravelerBot.MVC.Data.Models;
using TravelerBot.MVC.Models;

namespace TravelerBot.Api.Data.Repositories
{
    public interface ITripRepository
    {
        void Add(Trip trip);

        void Update(Trip trip);

        void Delete(Guid tripId);

        void SaveChanges();

        IEnumerable<Trip> Get(string from, string to, DateTime? when);

        // Убрать второй параметр
        Trip GetTripByUserStateId(Guid userStateId, bool isPublished = false);

        IEnumerable<Trip> GetTripsByUserStateId(Guid userStateId);

        Trip GetTrip(Guid TripId);
    }
}
