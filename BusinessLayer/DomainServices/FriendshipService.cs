using Ardalis.GuardClauses;
using AutoMapper;
using BusinessLayer.DomainServices.Interfaces;
using BusinessLayer.DTOs.Friendship;
using DataLayer.Enums;
using DataLayer.Interfaces;
using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Enums;

namespace BusinessLayer.DomainServices
{
    public class FriendshipService : IFriendshipService
    {
        #region Dependency Injection

        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorage _fileStorage;

        public FriendshipService(IFriendshipRepository friendshipRepository, IMapper mapper, IUnitOfWork unitOfWork, IFileStorage fileStorage)
        {
            _friendshipRepository = friendshipRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
        }

        #endregion

        //public async Task<IEnumerable<FriendshipStateDTO>> GetFriendsAsync(string userId)
        //{
        //    var friendships = await _friendshipRepository.GetFriendsAsync(userId);
        //    return _mapper.Map<IEnumerable<FriendshipStateDTO>>(friendships);
        //}

        public async Task<IEnumerable<FriendshipStateDTO>> GetIncomingRequestsAsync(string userId)
        {
            var friendships = await _friendshipRepository.GetIncomingRequestsAsync(userId);
            return _mapper.Map<IEnumerable<FriendshipStateDTO>>(friendships);
        }

        //UserId1 -> the user that accesses the other user's profile
        //UserId2 -> the one whose profile is being accessed
        public async Task<FriendshipStateDTO?> GetFriendshipBetweenUsersAsync(string userId1, string userId2)
        {
            //Prevent null values and both ids being equal
            if (string.IsNullOrWhiteSpace(userId1) || string.IsNullOrWhiteSpace(userId2) || userId1 == userId2)
                return null;

            var friendship = await _friendshipRepository.GetFriendshipBetweenUsersAsync(userId1, userId2);
            if (friendship == null)
                return null;

            return _mapper.Map<FriendshipStateDTO>(friendship);
        }

        public async Task<IEnumerable<FriendDTO>> GetFriendsAsync(string userId)
        {
            var friendships = await _friendshipRepository.GetAcceptedByUserIdAsync(userId);

            var friendsDTO = new List<FriendDTO>();

            friendsDTO = _mapper.Map<List<FriendDTO>>(
                friendships,
                opt => opt.Items["UserId"] = userId);

            var imgPath = string.Empty;

            foreach (var dto in friendsDTO)
            {
                switch (dto.FilePath)
                {
                    case "img\\uploads":
                        imgPath = _fileStorage.GetDirectory(FileStorageDirectory.Upload);
                        break;
                    //Set default image
                    case null:
                        imgPath = _fileStorage.GetDirectory(FileStorageDirectory.Default);
                        dto.AvatarUrl = "profile_pic.jpg";
                        break;
                    default:
                        throw new Exception("Something went wrong");
                }

                dto.AvatarUrl = $"{imgPath}\\{dto.AvatarUrl}";
            }

            return friendsDTO;
        }


        public async Task SendRequestAsync(string requesterId, string recipientId)
        {
            if (requesterId == recipientId)
                throw new Exception("You cannot befriend yourself.");

            var existingFriendship = await _friendshipRepository.GetFriendshipBetweenUsersAsync(requesterId, recipientId);
            if (existingFriendship != null)
                throw new Exception("A friendship already exists or is pending.");

            var friendship = new Friendship
            {
                RequesterId = requesterId,
                RecipientId = recipientId,
                CreatedBy = DataCreation.User,
                UpdatedBy =DataCreation.User,
                Status = FriendshipStatus.Pending
            };

            await _friendshipRepository.AddAsync(friendship);
            await _unitOfWork.SaveAsync();
        }

        public async Task AcceptRequestAsync(int friendshipId, string currentUserId)
        {
            //var friendship = await _friendshipRepository.GetByIdAsync(friendshipId);

            var friendship = await _friendshipRepository.GetAsync(f => f.Id == friendshipId);
            Guard.Against.Null(friendship, nameof(friendship), "Friend request not found.");

            //if (friendship == null)
            //    throw new NotFoundException("Friend request not found.");

            if (friendship.RecipientId != currentUserId)
                throw new Exception("You cannot accept this request.");

            if (friendship.Status != FriendshipStatus.Pending)
                throw new Exception("Friend request is not pending.");

            friendship.Status = FriendshipStatus.Accepted;
            await _friendshipRepository.UpdateAsync(friendship);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeclineRequestAsync(int friendshipId, string currentUserId)
        {
            var friendship = await _friendshipRepository.GetAsync(f => f.Id == friendshipId);
            Guard.Against.Null(friendship, nameof(friendship), "Friend request not found.");

            if (friendship.RecipientId != currentUserId)
                throw new Exception("You cannot decline this request.");

            friendship.Status = FriendshipStatus.Declined;
            await _friendshipRepository.UpdateAsync(friendship);
            await _unitOfWork.SaveAsync();
        }

        public async Task RemoveFriendAsync(string userId1, string userId2)
        {
            var existingFriendship = await _friendshipRepository.GetFriendshipBetweenUsersAsync(userId1, userId2);

            if (existingFriendship == null || existingFriendship.Status != FriendshipStatus.Accepted)
                throw new Exception("The users are not friends.");

            await _friendshipRepository.DeleteAsync(existingFriendship.Id);
            await _unitOfWork.SaveAsync();
        }
    }
}
