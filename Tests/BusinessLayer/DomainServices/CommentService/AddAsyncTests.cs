using AutoMapper;
using BusinessLayer.DomainServices;
using BusinessLayer.DTOs.Comment;
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
    public class AddAsyncTests
    {
        private readonly Mock<ICommentRepository> _commentRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;

        private readonly CommService _sut;

        public AddAsyncTests()
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


        ////AddAsync -> (Prod) Method name
        ////WithValidDto -> Possible context
        ////AddsCommentAndSavesChanges -> Intended outcome
        //Old name: AddAsync_WithValidDTO_AddCommentAndSaveChanges()
        [Fact]
        public async Task WithValidDTO_AddCommentAndSaveChanges()
        {
            //Arrange
            var dto = new CommentCreateDTO
            {
                Content = "test content",
                PostId = 1,
                UserId = "user1"
            };

            var mappedComment = new Comment
            {
                Content = dto.Content,
                PostId = dto.PostId,
                UserId = dto.UserId
            };

            _mapperMock
                .Setup(m => m.Map<Comment>(dto))
                .Returns(mappedComment);

            //Act
            var result = await _sut.AddAsync(dto);

            //Assert
            result.Should().BeTrue();

            _mapperMock.Verify(m => m.Map<Comment>(dto), Times.Once);
            _commentRepositoryMock.Verify(r => r.AddAsync(mappedComment), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task WithNullDto_ThrowException()
        {
            //Act
            Func<Task> act = () => _sut.AddAsync(null);

            //Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }
    }
}
