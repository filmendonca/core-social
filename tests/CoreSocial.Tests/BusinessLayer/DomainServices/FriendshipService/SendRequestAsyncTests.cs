using Xunit;
using Moq;
using BusinessLayer.DomainServices;
using DataLayer.Interfaces;
using System;
using System.Threading.Tasks;
using DataLayer.Models;
using AutoMapper;
using FriendService = BusinessLayer.DomainServices.FriendshipService;

namespace Tests.BusinessLayer.DomainServices.FriendshipService
{
    public class SendRequestAsyncTests
    {
        private readonly Mock<IFriendshipRepository> _friendshipRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IFileStorage> _fileStorageMock;

        private readonly FriendService _sut;

        public SendRequestAsyncTests()
        {
            _friendshipRepositoryMock = new Mock<IFriendshipRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _fileStorageMock = new Mock<IFileStorage>();

            _sut = new FriendService(
                _friendshipRepositoryMock.Object,
                _mapperMock.Object,
                _unitOfWorkMock.Object,
                _fileStorageMock.Object
            );
        }

        [Fact]
        public async Task ShouldCreateFriendship_WhenNoneExists()
        {
            //Arrange
            var user1 = "user1";
            var user2 = "user2";

            _friendshipRepositoryMock
                .Setup(r => r.GetFriendshipBetweenUsersAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((Friendship)null);

            //Act
            await _sut.SendRequestAsync(user1, user2);

            //Assert
            _friendshipRepositoryMock
                .Verify(r => r.AddAsync(It.IsAny<Friendship>()), Times.Once);

            _unitOfWorkMock
                .Verify(r => r.SaveAsync(), Times.Once);
        }
    }
}
