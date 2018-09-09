using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelerBot.Api.Data.Models;

namespace TravelerBot.Api.Data.Repositories
{
    public interface ITripRepository
    {
        Task Add(Trip trip);

        Task Update(Trip trip);


    }
}
