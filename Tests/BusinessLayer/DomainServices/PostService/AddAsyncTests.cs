using Xunit;
using Moq;
using BusinessLayer.DomainServices;
using DataLayer.Interfaces;
using System;
using System.Threading.Tasks;
using DataLayer.Models;
using AutoMapper;
using PoService = BusinessLayer.DomainServices.PostService;
using FluentAssertions;
using BusinessLayer.DTOs.Post;

namespace Tests.BusinessLayer.DomainServices.PostService
{
    public class AddAsyncTests
    {
        private readonly Mock<IPostRepository> _postRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IFileStorage> _fileStorageMock;

        private readonly PoService _sut;

        public AddAsyncTests()
        {
            _postRepositoryMock = new Mock<IPostRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _fileStorageMock = new Mock<IFileStorage>();

            _sut = new PoService(
                _postRepositoryMock.Object,
                _mapperMock.Object,
                _unitOfWorkMock.Object,
                _fileStorageMock.Object
            );
        }

        [Fact]
        public async Task WithValidPost_AddPost()
        {
            //Arrange
            var post = new Post
            {
                UserId = "user1",
                Content = "test"
            };

            //Act
            await _sut.AddAsync(post);

            //Assert

            //check if post above was "added"
            _postRepositoryMock
                .Verify(p => p.AddAsync(It.Is<Post>(r =>
                    r.UserId == post.UserId &&
                    r.Content == post.Content)),
                Times.Once);
        }

        [Fact]
        public async Task WithInvalidPost_ThrowException()
        {
            Func<Task> act = () => _sut.AddAsync((Post)null);

            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task WithValidPostDTO_AddPost()
        {
            //Arrange
            var postDTO = new PostCreateDTO
            {
                UserId = "user1",
                Content = "test"
            };

            var post = new Post
            {
                UserId = postDTO.UserId,
                Content = postDTO.Content
            };

            _mapperMock
                .Setup(m => m.Map<Post>(postDTO))
                .Returns(post);

            //Act
            await _sut.AddAsync(postDTO);

            //Assert

            //optional
            _mapperMock
                .Verify(r => r.Map<Post>(postDTO), Times.Once);

            //check if post above was "added"
            _postRepositoryMock
                .Verify(p => p.AddAsync(It.Is<Post>(r =>
                    r.UserId == postDTO.UserId &&
                    r.Content == postDTO.Content)),
                Times.Once);

        }

        [Fact]
        public async Task WithInvalidPostDTO_ThrowException()
        {
            Func<Task> act = () => _sut.AddAsync((PostCreateDTO)null);

            await act.Should().ThrowAsync<ArgumentException>();
        }
    }
}
