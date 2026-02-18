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
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Tests.BusinessLayer.DomainServices.PostService
{
    public class EditAsyncTests
    {
        private readonly Mock<IPostRepository> _postRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IFileStorage> _fileStorageMock;

        private readonly PoService _sut;

        public EditAsyncTests()
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

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task WithValidDTO_UpdateComment(bool saveChanges)
        {
            //Arrange
            var postDTO = new PostEditDTO
            {
                Id = 1,
                UserId = "user1",
                Content = "new content",
                IsEdited = true,
                DateUpdated = DateTime.UtcNow
            };
            
            var existingPost = new Post
            {
                Id = postDTO.Id,
                UserId = postDTO.UserId,
                Content = "old content",
                IsEdited = false,
                DateUpdated = new DateTime(2025, 11, 28)
            };

            _postRepositoryMock
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Post, bool>>>(), It.IsAny<List<string>>()))
                .ReturnsAsync(existingPost);

            _mapperMock
                .Setup(m => m.Map(postDTO, existingPost))
                .Callback(() =>
               {
                   existingPost.Content = postDTO.Content;
                   existingPost.IsEdited = postDTO.IsEdited;
                   existingPost.DateUpdated = postDTO.DateUpdated;
               });

            //Act
            await _sut.EditAsync(postDTO, saveChanges);

            //Assert

            existingPost.Content.Should().Be(postDTO.Content);
            existingPost.IsEdited.Should().BeTrue();
            existingPost.DateUpdated.Should().Be(postDTO.DateUpdated);

            //optional
            _mapperMock
                .Verify(m => m.Map(postDTO, existingPost), Times.Once);

            _postRepositoryMock
                .Verify(r => r.UpdateAsync(existingPost), Times.Once);

            if (saveChanges)
                _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
            else
                _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task WithInvalidDTO_ThrowException()
        {
            Func<Task> act = () => _sut.EditAsync(null);
            await act.Should().ThrowAsync<ArgumentException>();
        }
    }
}
