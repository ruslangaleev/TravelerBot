using System.Threading.Tasks;
using TravelerBot.MVC.Models;

namespace TravelerBot.Api.Data.Repositories
{
    public interface ITripRepository
    {
        void Add(Trip trip);

        void Update(Trip trip);

        Trip Get(int accountVkontakteId);
    }
}
