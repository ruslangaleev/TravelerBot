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

        //Trip Get(int accountVkontakteId);

        IEnumerable<Trip> Get(int accountVkontakteId);

        Trip Get();

        void Delete(Guid tripId);

        IEnumerable<Trip> Get(string from, string to, DateTime? when);
    }
}
