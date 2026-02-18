using Xunit;
using Moq;
using BusinessLayer.DomainServices;
using DataLayer.Interfaces;
using System;
using System.Threading.Tasks;
using DataLayer.Models;
using AutoMapper;
using AttachService = BusinessLayer.DomainServices.AttachmentService;
using FluentAssertions;
using BusinessLayer.DTOs.Attachment;

namespace Tests.BusinessLayer.DomainServices.AttachmentService
{
    public class DeleteAsyncTests
    {
        private readonly Mock<IAttachmentRepository> _attachmentRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        private readonly AttachService _sut;

        public DeleteAsyncTests()
        {
            _attachmentRepositoryMock = new Mock<IAttachmentRepository>();
            _mapperMock = new Mock<IMapper>();

            _sut = new AttachService(
                _attachmentRepositoryMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task WithValidId_SoftDeleteAttachment()
        {
            //Arrange
            var id = 1;

            //Act
            await _sut.DeleteAsync(id);

            //Assert
            _attachmentRepositoryMock
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
