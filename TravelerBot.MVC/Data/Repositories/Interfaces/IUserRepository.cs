using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TravelerBot.MVC.Data.Models;

namespace TravelerBot.MVC.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        UserState GetUserState(int accountId);

        void Update(UserState userState);

        void AddUserState(UserState userState);
    }
}