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
    public class EditAsyncTests
    {
        private readonly Mock<IAttachmentRepository> _attachmentRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        private readonly AttachService _sut;

        public EditAsyncTests()
        {
            _attachmentRepositoryMock = new Mock<IAttachmentRepository>();
            _mapperMock = new Mock<IMapper>();

            _sut = new AttachService(
                _attachmentRepositoryMock.Object,
                _mapperMock.Object
            );
        }
    }
}
