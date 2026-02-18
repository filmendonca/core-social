using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    public interface IFriendshipRepository : IGenericRepository<Friendship>
    {
        Task<Friendship?> GetFriendshipBetweenUsersAsync(string userId1, string userId2);
        Task<IEnumerable<Friendship>> GetAcceptedByUserIdAsync(string userId);
        Task<IEnumerable<Friendship>> GetIncomingRequestsAsync(string userId);
        Task<IEnumerable<Friendship>> GetFriendsAsync(string userId);
    }
}
