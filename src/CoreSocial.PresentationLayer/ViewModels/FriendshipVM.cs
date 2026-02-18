using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresentationLayer.ViewModels
{
    public class FriendshipVM
    {

    }

    public class FriendVM
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string AvatarUrl { get; set; }
        public int Reputation { get; set; }
    }

    public class IncomingFriendRequestViewModel
    {
        public int FriendshipId { get; set; }
        public string SenderId { get; set; }
        public string SenderUserName { get; set; }
        public int? SenderAvatarId { get; set; }
    }

    public class OutgoingFriendRequestViewModel
    {
        public int FriendshipId { get; set; }
        public string RecipientId { get; set; }
        public string RecipientUserName { get; set; }
        public int? RecipientAvatarId { get; set; }
    }

    public class FriendshipStatusVM
    {
        public bool IsFriend { get; set; }
        public bool IsPending { get; set; }
        public bool IsIncomingRequest { get; set; }
        public bool IsOutgoingRequest { get; set; }

        public int? FriendshipId { get; set; }
        public string RecipientId { get; set; }
        public string RequesterId { get; set; }
        public string RequesterUsername { get; set; }
    }
}
