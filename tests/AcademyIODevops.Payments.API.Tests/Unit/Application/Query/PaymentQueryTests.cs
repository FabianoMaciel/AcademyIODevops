using AcademyIODevops.Payments.API.Application.Query;
using AcademyIODevops.Payments.API.Business;
using FluentAssertions;
using Moq;

namespace AcademyIODevops.Payments.API.Tests.Unit.Application.Query
{
    public class PaymentQueryTests
    {
        private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
        private readonly PaymentQuery _query;

        public PaymentQueryTests()
        {
            _paymentRepositoryMock = new Mock<IPaymentRepository>();
            _query = new PaymentQuery(_paymentRepositoryMock.Object);
        }

        [Fact]
        public async Task PaymentExists_ShouldReturnTrue_WhenPaymentExists()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            _paymentRepositoryMock
                .Setup(x => x.PaymentExists(studentId, courseId))
                .ReturnsAsync(true);

            // Act
            var result = await _query.PaymentExists(studentId, courseId);

            // Assert
            result.Should().BeTrue();

            _paymentRepositoryMock.Verify(
                x => x.PaymentExists(studentId, courseId),
                Times.Once
            );
        }

        [Fact]
        public async Task PaymentExists_ShouldReturnFalse_WhenPaymentDoesNotExist()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            _paymentRepositoryMock
                .Setup(x => x.PaymentExists(studentId, courseId))
                .ReturnsAsync(false);

            // Act
            var result = await _query.PaymentExists(studentId, courseId);

            // Assert
            result.Should().BeFalse();

            _paymentRepositoryMock.Verify(
                x => x.PaymentExists(studentId, courseId),
                Times.Once
            );
        }

        [Theory]
        [InlineData("550e8400-e29b-41d4-a716-446655440000", "6ba7b810-9dad-11d1-80b4-00c04fd430c8")]
        [InlineData("f47ac10b-58cc-4372-a567-0e02b2c3d479", "6ba7b811-9dad-11d1-80b4-00c04fd430c8")]
        [InlineData("a1b2c3d4-e5f6-4a5b-8c7d-9e0f1a2b3c4d", "b2c3d4e5-f6a7-4b5c-8d7e-9f0a1b2c3d4e")]
        public async Task PaymentExists_ShouldHandleDifferentGuids_Correctly(string studentIdStr, string courseIdStr)
        {
            // Arrange
            var studentId = Guid.Parse(studentIdStr);
            var courseId = Guid.Parse(courseIdStr);

            _paymentRepositoryMock
                .Setup(x => x.PaymentExists(studentId, courseId))
                .ReturnsAsync(true);

            // Act
            var result = await _query.PaymentExists(studentId, courseId);

            // Assert
            result.Should().BeTrue();

            _paymentRepositoryMock.Verify(
                x => x.PaymentExists(studentId, courseId),
                Times.Once
            );
        }

        [Fact]
        public async Task PaymentExists_ShouldCallRepository_OnlyOnce()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            _paymentRepositoryMock
                .Setup(x => x.PaymentExists(studentId, courseId))
                .ReturnsAsync(true);

            // Act
            await _query.PaymentExists(studentId, courseId);

            // Assert
            _paymentRepositoryMock.Verify(
                x => x.PaymentExists(It.IsAny<Guid>(), It.IsAny<Guid>()),
                Times.Once,
                "Repository should be called exactly once"
            );
        }
    }
}
