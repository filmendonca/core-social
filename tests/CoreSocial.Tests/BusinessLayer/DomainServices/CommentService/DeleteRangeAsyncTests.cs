using AutoMapper;
using DataLayer.Interfaces;
using DataLayer.Models;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using CommService = BusinessLayer.DomainServices.CommentService;

namespace Tests.BusinessLayer.DomainServices.CommentService
{
    public class DeleteRangeAsyncTests
    {
        private readonly Mock<ICommentRepository> _commentRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;

        private readonly CommService _sut;

        public DeleteRangeAsyncTests()
        {
            _commentRepositoryMock = new Mock<ICommentRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();

            _sut = new CommService(
                _commentRepositoryMock.Object,
                _mapperMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task WithValidPostId_SoftDeleteAllComments()
        {
            // Arrange
            var postId = 1;

            var comments = new List<Comment>
            {
                new Comment { Id = 1, PostId = postId },
                new Comment { Id = 2, PostId = postId }
            };

            _commentRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Comment, bool>>>(), null, null))
                .ReturnsAsync(comments);

            // Act
            await _sut.DeleteRangeAsync(postId);

            // Assert
            foreach (var comment in comments)
            {
                _commentRepositoryMock
                    .Verify(r => r.SoftDeleteAsync(comment.Id), Times.Once);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task WithInvalidPostId_ThrowException(int postId)
        {
            //Act
            Func<Task> act = () => _sut.DeleteRangeAsync(postId);

            //Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task WhenCommentsAreNull_ThrowException()
        {
            //Arrange
            var postId = 1;

            _commentRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Comment, bool>>>(), null, null))
                .ReturnsAsync((IEnumerable<Comment>)null);

            //Act
            Func<Task> act = () => _sut.DeleteRangeAsync(postId);

            //Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }
    }
}
