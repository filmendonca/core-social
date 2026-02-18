using DataLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs.Friendship
{
    public class FriendshipDTO
    {

    }

    public class FriendDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Bio { get; set; }
        public int Reputation { get; set; }
        public int? AvatarId { get; set; }

        public string RequesterId { get; set; }
        public string RequesterName { get; set; }
        public string RequesterAvatarUrl { get; set; }
        public string RecipientId { get; set; }
        public string RecipientName { get; set; }
        public string RecipientAvatarUrl { get; set; }

        public string AvatarUrl { get; set; }
        public string FilePath { get; set; }
    }

    public class FriendshipStateDTO
    {
        public int? FriendshipId { get; set; }

        public FriendshipStatus Status { get; set; }

        public string RequesterId { get; set; }
        public string RecipientId { get; set; }
        public string RequesterName { get; set; }
        public string RecipientName { get; set; }
    }

    public class FriendRequestDTO
    {
        public int FriendshipId { get; set; }
        public string SenderId { get; set; }
        public string SenderUserName { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class SendFriendRequestDTO
    {
        public string RecipientId { get; set; }
    }

    public class IncomingFriendRequestDTO
    {
        public int FriendshipId { get; set; }
        public string SenderId { get; set; }
        public string SenderUserName { get; set; }
        public int? SenderAvatarId { get; set; }
        public DateTime RequestedAt { get; set; }
    }

    public class OutgoingFriendRequestDTO
    {
        public int FriendshipId { get; set; }
        public string RecipientId { get; set; }
        public string RecipientUserName { get; set; }
        public int? RecipientAvatarId { get; set; }
        public DateTime RequestedAt { get; set; }
    }

    public class RespondToFriendRequestDTO
    {
        public int FriendshipId { get; set; }
    }

    public class RemoveFriendDTO
    {
        public string FriendUserId { get; set; }
    }

    public class FriendshipStatusDTO
    {
        public bool IsFriend { get; set; }
        public bool IsPending { get; set; }
        public bool IsIncomingRequest { get; set; }
        public bool IsOutgoingRequest { get; set; }
        public int? FriendshipId { get; set; }
    }

}
