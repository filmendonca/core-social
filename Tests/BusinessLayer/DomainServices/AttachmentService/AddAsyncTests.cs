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
    public class AddAsyncTests
    {
        private readonly Mock<IAttachmentRepository> _attachmentRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        private readonly AttachService _sut;

        public AddAsyncTests()
        {
            _attachmentRepositoryMock = new Mock<IAttachmentRepository>();
            _mapperMock = new Mock<IMapper>();

            _sut = new AttachService(
                _attachmentRepositoryMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task WithValidAttachment_AddAttachment()
        {
            //Arrange
            var attachment = new Attachment
            {
                UserId = "user1",
                FileName = "test"
            };

            //Act
            await _sut.AddAsync(attachment);

            //Assert

            _attachmentRepositoryMock
                .Verify(p => p.AddAsync(It.Is<Attachment>(r =>
                    r.UserId == attachment.UserId &&
                    r.FileName == attachment.FileName)),
                Times.Once);
        }

        [Fact]
        public async Task WithInvalidAttachment_ThrowException()
        {
            Func<Task> act = () => _sut.AddAsync((Attachment)null);
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task WithValidAttachmentDTO_AddAttachment()
        {
            //Arrange
            var attachmentDTO = new AttachmentCreateDTO
            {
                UserId = "user1",
                FileName = "test",
            };

            var attachment = new Attachment
            {
                UserId = attachmentDTO.UserId,
                FileName = attachmentDTO.FileName
            };

            _mapperMock
                .Setup(m => m.Map<Attachment>(attachmentDTO))
                .Returns(attachment);

            //Act
            await _sut.AddAsync(attachmentDTO);

            //Assert

            //optional
            _mapperMock
                .Verify(r => r.Map<Attachment>(attachmentDTO), Times.Once);

            _attachmentRepositoryMock
                .Verify(p => p.AddAsync(It.Is<Attachment>(r =>
                    r.UserId == attachmentDTO.UserId &&
                    r.FileName == attachmentDTO.FileName)),
                Times.Once);

        }

        [Fact]
        public async Task WithInvalidAttachmentDTO_ThrowException()
        {
            Func<Task> act = () => _sut.AddAsync((AttachmentCreateDTO)null);
            await act.Should().ThrowAsync<ArgumentException>();
        }
    }
}
