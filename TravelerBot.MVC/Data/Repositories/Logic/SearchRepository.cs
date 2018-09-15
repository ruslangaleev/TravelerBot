using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TravelerBot.MVC.Data.Models;
using TravelerBot.MVC.Data.Repositories.Interfaces;

namespace TravelerBot.MVC.Data.Repositories.Logic
{
    public class SearchRepository : ISearchRepository
    {
        TripContext tripContext = new TripContext();

        public void Add(SearchOptions options)
        {
            tripContext.SearchOptions.Add(options);
        }

        public void Delete(Guid optionsId)
        {
            var trip = tripContext.SearchOptions.Find(optionsId);
            if (trip != null)
            {
                tripContext.SearchOptions.Remove(trip);
            }
        }

        public SearchOptions Get(Guid optionId)
        {
            return tripContext.SearchOptions.Find(optionId);
        }

        public void SaveChanges()
        {
            tripContext.SaveChanges();
        }

        public void Update(SearchOptions options)
        {
            tripContext.Entry(options).State = System.Data.Entity.EntityState.Modified;
        }
    }
}