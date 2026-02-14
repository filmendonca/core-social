using DataLayer.Context;
using DataLayer.Enums;
using DataLayer.Interfaces;
using DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
    public class FriendshipRepository : GenericRepository<Friendship>, IFriendshipRepository
    {
        public FriendshipRepository(AppDbContext context) : base(context) { }

        //Make sure that there can only be one row for each friendship (between two different users)
        public async Task<Friendship?> GetFriendshipBetweenUsersAsync(string userId1, string userId2)
        {
            return await _db.FirstOrDefaultAsync(f =>
                        (f.RequesterId == userId1 && f.RecipientId == userId2) ||
                        (f.RequesterId == userId2 && f.RecipientId == userId1));
        }

        public async Task<IEnumerable<Friendship>> GetAcceptedByUserIdAsync(string userId)
        {
            return await _db
                            .Where(f =>
                                f.Status == FriendshipStatus.Accepted &&
                                (f.RequesterId == userId || f.RecipientId == userId))
                            .Include(f => f.Requester)
                            .Include(f => f.Recipient)
                            .ToListAsync();
        }

        public async Task<IEnumerable<Friendship>> GetFriendsAsync(string userId)
        {
            return await _db
                .Where(f => (f.RequesterId == userId || f.RecipientId == userId) && f.Status == FriendshipStatus.Accepted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Friendship>> GetIncomingRequestsAsync(string userId)
        {
            return await _db
                            .Where(f =>
                                f.RecipientId == userId &&
                                f.Status == FriendshipStatus.Pending)
                            .ToListAsync();
        }

    }
}
