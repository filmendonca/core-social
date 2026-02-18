using AutoMapper;
using DataLayer.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using CommService = BusinessLayer.DomainServices.CommentService;

namespace Tests.BusinessLayer.DomainServices.CommentService
{
    public class DeleteAsyncTests
    {
        private readonly Mock<ICommentRepository> _commentRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;

        private readonly CommService _sut;

        public DeleteAsyncTests()
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
        public async Task WithValidId_SoftDeleteComment()
        {
            //Arrange
            var id = 1;

            //Act
            await _sut.DeleteAsync(id);

            //Assert
            _commentRepositoryMock
                .Verify(c => c.SoftDeleteAsync(id), Times.Once);
            _unitOfWorkMock
                .Verify(u => u.SaveAsync(), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task WithInvalidId_ThrowException(int id)
        {
            //Act
            Func<Task> act = () => _sut.DeleteAsync(id);

            //Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }
    }
}
