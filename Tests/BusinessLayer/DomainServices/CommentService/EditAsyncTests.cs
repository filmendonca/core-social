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
    public class EditAsyncTests
    {
        private readonly Mock<ICommentRepository> _commentRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;

        private readonly CommService _sut;

        public EditAsyncTests()
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
        public async Task WithValidDTO_EditCommentAndSaveChanges()
        {
            //Arrange
            var dto = new CommentEditDTO
            {
                Id = 1,
                PostId = 1,
                UserId = "user1",
                Content = "new content",
                IsEdited = true,
                DateUpdated = DateTime.UtcNow
            };

            var existingComment = new Comment
            {
                Id = dto.Id,
                Content = "existing content",
                IsEdited = false,
                DateUpdated = new DateTime(2025, 11, 23)
            };

            //optional params should also be included
            _commentRepositoryMock
                //setup -> chamar o método propriamente dito, incluindo nele todos os params que tenham o tipo indicado
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Comment, bool>>>(), It.IsAny<List<string>>()))
                //returns -> retornar uma var (fixa) do tipo indicado
                .ReturnsAsync(existingComment);

            _mapperMock
                .Setup(m => m.Map(dto, existingComment))
                //"Change" fields
                .Callback(() =>
                {
                    existingComment.Content = dto.Content;
                    existingComment.IsEdited = dto.IsEdited;
                    existingComment.DateUpdated = dto.DateUpdated;
                });


            //Act
            await _sut.EditAsync(dto);

            //Assert

            //check if existing comment changed at all (optional)
            //var checking (state)
            existingComment.Content.Should().Be(dto.Content);
            existingComment.IsEdited.Should().BeTrue();
            existingComment.DateUpdated.Should().Be(dto.DateUpdated);

            //method call checking
            _commentRepositoryMock
                .Verify(r => r.UpdateAsync(existingComment), Times.Once);
            //optional
            _mapperMock
                .Verify(r => r.Map(dto, existingComment), Times.Once);
            //end optional

            _unitOfWorkMock
                .Verify(r => r.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task WithNullDTO_ThrowException()
        {
            //Act
            Func<Task> act = () => _sut.EditAsync(null);

            //Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }
    }
}
