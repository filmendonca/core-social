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
    public class DeleteAsyncTests
    {
        private readonly Mock<IPostRepository> _postRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IFileStorage> _fileStorageMock;

        private readonly PoService _sut;

        public DeleteAsyncTests()
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
        public async Task WithValidId_SoftDeleteComment()
        {
            //Arrange
            var id = 1;

            //Act
            await _sut.DeleteAsync(id);

            //Assert
            _postRepositoryMock
                .Verify(c => c.SoftDeleteAsync(id), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task WithInvalidId_ThrowException(int id)
        {
            Func<Task> act = () => _sut.DeleteAsync(id);
            await act.Should().ThrowAsync<ArgumentException>();
        }
    }
}
