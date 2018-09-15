using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TravelerBot.MVC.Data.Models;

namespace TravelerBot.MVC.Data.Repositories.Interfaces
{
    public interface ISearchRepository
    {
        void Add(SearchOptions options);

        void Update(SearchOptions options);

        SearchOptions Get(Guid optionId);

        void Delete(Guid optionsId);

        void SaveChanges();
    }
}