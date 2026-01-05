using AcademyIODevops.Core.Data;
using AcademyIODevops.Core.DomainObjects.DTOs;
using AcademyIODevops.Core.Messages.IntegrationCommands;
using AcademyIODevops.Core.Messages.Notifications;
using AcademyIODevops.Courses.API.Application.Commands;
using AcademyIODevops.Courses.API.Application.Handlers;
using AcademyIODevops.Courses.API.Models;
using AcademyIODevops.Courses.API.Tests.Builders;
using FluentAssertions;
using MediatR;
using Moq;

namespace AcademyIODevops.Courses.API.Tests.Unit.Application.Handlers
{
    public class ValidatePaymentCourseCommandHandlerTests
    {
        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly Mock<ILessonRepository> _lessonRepositoryMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CourseCommandHandler _handler;

        public ValidatePaymentCourseCommandHandlerTests()
        {
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _lessonRepositoryMock = new Mock<ILessonRepository>();
            _mediatorMock = new Mock<IMediator>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _courseRepositoryMock
                .Setup(x => x.UnitOfWork)
                .Returns(_unitOfWorkMock.Object);

            _handler = new CourseCommandHandler(
                _courseRepositoryMock.Object,
                _lessonRepositoryMock.Object,
                _mediatorMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnTrue_WhenCourseExistsAndPaymentIsProcessed()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var studentId = Guid.NewGuid();
            var course = new CourseBuilder()
                .WithId(courseId)
                .WithPrice(199.99)
                .Build();

            var command = new ValidatePaymentCourseCommand(
                courseId: courseId,
                studentId: studentId,
                cardName: "JOHN DOE",
                cardNumber: "4532015112830366",
                cardExpirationDate: "12/2025",
                cardCVV: "123"
            );

            _courseRepositoryMock
                .Setup(x => x.GetById(courseId))
                .ReturnsAsync(course);

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<MakePaymentCourseCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            _courseRepositoryMock.Verify(
                x => x.GetById(courseId),
                Times.Once
            );

            _mediatorMock.Verify(
                x => x.Send(It.Is<MakePaymentCourseCommand>(cmd =>
                    cmd.PaymentCourse.CourseId == courseId &&
                    cmd.PaymentCourse.StudentId == studentId &&
                    cmd.PaymentCourse.Total == course.Price
                ), It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenCourseDoesNotExist()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var command = new ValidatePaymentCourseCommand(
                courseId: courseId,
                studentId: Guid.NewGuid(),
                cardName: "JOHN DOE",
                cardNumber: "4532015112830366",
                cardExpirationDate: "12/2025",
                cardCVV: "123"
            );

            _courseRepositoryMock
                .Setup(x => x.GetById(courseId))
                .ReturnsAsync((Course?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _courseRepositoryMock.Verify(
                x => x.GetById(courseId),
                Times.Once
            );

            _mediatorMock.Verify(
                x => x.Publish(
                    It.Is<DomainNotification>(n => n.Value == "Curso não encontrado."),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );

            _mediatorMock.Verify(
                x => x.Send(It.IsAny<MakePaymentCourseCommand>(), It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new ValidatePaymentCourseCommand(
                courseId: Guid.Empty,
                studentId: Guid.NewGuid(),
                cardName: "JOHN DOE",
                cardNumber: "4532015112830366",
                cardExpirationDate: "12/2025",
                cardCVV: "123"
            );

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _courseRepositoryMock.Verify(
                x => x.GetById(It.IsAny<Guid>()),
                Times.Never
            );

            _mediatorMock.Verify(
                x => x.Send(It.IsAny<MakePaymentCourseCommand>(), It.IsAny<CancellationToken>()),
                Times.Never
            );
        }


        [Fact]
        public async Task Handle_ShouldReturnPaymentResult_WhenPaymentIsProcessed()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new CourseBuilder()
                .WithId(courseId)
                .WithPrice(149.99)
                .Build();

            var command = new ValidatePaymentCourseCommand(
                courseId: courseId,
                studentId: Guid.NewGuid(),
                cardName: "TEST USER",
                cardNumber: "4532015112830366",
                cardExpirationDate: "12/2025",
                cardCVV: "789"
            );

            _courseRepositoryMock
                .Setup(x => x.GetById(courseId))
                .ReturnsAsync(course);

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<MakePaymentCourseCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _mediatorMock.Verify(
                x => x.Send(It.IsAny<MakePaymentCourseCommand>(), It.IsAny<CancellationToken>()),
                Times.Once
            );
        }
    }
}
