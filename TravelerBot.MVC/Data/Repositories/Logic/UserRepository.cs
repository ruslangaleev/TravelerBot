using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TravelerBot.Api.Data.Repositories;
using TravelerBot.MVC.Data.Models;
using TravelerBot.MVC.Data.Repositories.Interfaces;

namespace TravelerBot.MVC.Data.Repositories.Logic
{
    public class UserRepository : IUserRepository
    {
        TripContext tripContext = new TripContext();

        public void AddUserState(UserState userState)
        {
            tripContext.UserStates.Add(userState);
            tripContext.SaveChanges();
        }

        public UserState GetUserState(int accountId)
        {
            return tripContext.UserStates.FirstOrDefault(t => t.AccountId == accountId);
        }

        public void Update(UserState userState)
        {
            tripContext.Entry(userState).State = System.Data.Entity.EntityState.Modified;
            tripContext.SaveChanges();
        }
    }
}