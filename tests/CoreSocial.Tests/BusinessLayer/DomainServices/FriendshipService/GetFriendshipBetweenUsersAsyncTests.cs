using Xunit;
using Moq;
using BusinessLayer.DomainServices;
using DataLayer.Interfaces;
using System;
using System.Threading.Tasks;
using DataLayer.Models;
using AutoMapper;
using FriendService = BusinessLayer.DomainServices.FriendshipService;
using FluentAssertions;
using BusinessLayer.DTOs.Friendship;
using DataLayer.Enums;

namespace Tests.BusinessLayer.DomainServices.FriendshipService
{
    public class GetFriendshipBetweenUsersAsyncTests
    {
        private readonly Mock<IFriendshipRepository> _friendshipRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IFileStorage> _fileStorageMock;

        private readonly FriendService _sut;

        public GetFriendshipBetweenUsersAsyncTests()
        {
            _friendshipRepositoryMock = new Mock<IFriendshipRepository>();
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _fileStorageMock = new Mock<IFileStorage>();

            _sut = new FriendService(
                _friendshipRepositoryMock.Object,
                _mapperMock.Object,
                _unitOfWorkMock.Object,
                _fileStorageMock.Object
            );
        }

        [Theory]
        [InlineData(null, "user2")]
        [InlineData("user1", null)]
        [InlineData("", "user2")]
        [InlineData("user1", "")]
        [InlineData("   ", "user2")]
        [InlineData("user1", "   ")]
        [InlineData("user1", "user1")] // same user
        public async Task WithInvalidUserIds_ReturnNull(string userId1, string userId2)
        {
            //Act
            var result = await _sut.GetFriendshipBetweenUsersAsync(userId1, userId2);

            //Assert
            result.Should().BeNull();

            _friendshipRepositoryMock
                .Verify(r => r.GetFriendshipBetweenUsersAsync(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);

            _mapperMock
                .Verify(m => m.Map<FriendshipStateDTO>(It.IsAny<Friendship>()),
                Times.Never);
        }

        [Fact]
        public async Task WhenFriendshipDoesNotExist_ReturnNull()
        {
            // Arrange
            var userId1 = "user1";
            var userId2 = "user2";

            _friendshipRepositoryMock
                .Setup(r => r.GetFriendshipBetweenUsersAsync(userId1, userId2))
                .ReturnsAsync((Friendship)null);

            //Act
            var result = await _sut.GetFriendshipBetweenUsersAsync(userId1, userId2);

            //Assert
            result.Should().BeNull();

            _mapperMock.Verify(m => m.Map<FriendshipStateDTO>(It.IsAny<Friendship>()),
                Times.Never);
        }

        [Fact]
        public async Task WhenFriendshipExists_ReturnsMappedDTO()
        {
            //Arrange
            var userId1 = "user1";
            var userId2 = "user2";

            var friendship = new Friendship
            {
                Id = 1,
                Status = FriendshipStatus.Accepted,
                RequesterId = userId1,
                RecipientId = userId2
            };

            var friendshipDTO = new FriendshipStateDTO
            {
                FriendshipId = friendship.Id,
                Status = friendship.Status,
                RequesterId = friendship.RequesterId,
                RecipientId = friendship.RecipientId
            };

            _friendshipRepositoryMock
                .Setup(r => r.GetFriendshipBetweenUsersAsync(userId1, userId2))
                .ReturnsAsync(friendship);

            _mapperMock
                .Setup(m => m.Map<FriendshipStateDTO>(friendship))
                .Returns(friendshipDTO);

            //Act
            var result = await _sut.GetFriendshipBetweenUsersAsync(userId1, userId2);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(friendshipDTO);

            _friendshipRepositoryMock.Verify(
                r => r.GetFriendshipBetweenUsersAsync(userId1, userId2),
                Times.Once);

            _mapperMock.Verify(
                m => m.Map<FriendshipStateDTO>(friendship),
                Times.Once);
        }
    }
}
