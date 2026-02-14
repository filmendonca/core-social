using BusinessLayer.DTOs.Friendship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.DomainServices.Interfaces
{
    public interface IFriendshipService
    {
        Task<IEnumerable<FriendshipStateDTO>> GetIncomingRequestsAsync(string userId);
        Task<FriendshipStateDTO?> GetFriendshipBetweenUsersAsync(string userId1, string userId2);
        Task<IEnumerable<FriendDTO>> GetFriendsAsync(string userId);
        Task SendRequestAsync(string requesterId, string recipientId);
        Task AcceptRequestAsync(int friendshipId, string currentUserId);
        Task DeclineRequestAsync(int friendshipId, string currentUserId);
        Task RemoveFriendAsync(string userId1, string userId2);
    }
}
