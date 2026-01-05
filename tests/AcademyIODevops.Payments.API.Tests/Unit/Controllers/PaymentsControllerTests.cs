using AcademyIODevops.Payments.API.Business;
using AcademyIODevops.Payments.API.Controllers;
using AcademyIODevops.Payments.API.Data;
using AcademyIODevops.Payments.API.Tests.Builders;
using AcademyIODevops.WebAPI.Core.User;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace AcademyIODevops.Payments.API.Tests.Unit.Controllers;

public class PaymentsControllerTests : IDisposable
{
    private readonly PaymentsContext _context;
    private readonly Mock<IAspNetUser> _aspNetUserMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly PaymentsController _controller;

    public PaymentsControllerTests()
    {
        var options = new DbContextOptionsBuilder<PaymentsContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _mediatorMock = new Mock<IMediator>();
        _context = new PaymentsContext(options, _mediatorMock.Object);
        _aspNetUserMock = new Mock<IAspNetUser>();
        _controller = new PaymentsController(_context, _aspNetUserMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithListOfPayments()
    {
        // Arrange
        var payments = PaymentBuilder.BuildMany(3);
        _context.Set<Payment>().AddRange(payments);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedPayments = okResult.Value.Should().BeAssignableTo<IEnumerable<Payment>>().Subject;
        returnedPayments.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoPayments()
    {
        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedPayments = okResult.Value.Should().BeAssignableTo<IEnumerable<Payment>>().Subject;
        returnedPayments.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_ShouldNotReturnDeletedPayments()
    {
        // Arrange
        var activePayment = new PaymentBuilder().Build();
        var deletedPayment = new PaymentBuilder().Build();
        deletedPayment.Deleted = true;

        _context.Set<Payment>().AddRange(activePayment, deletedPayment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedPayments = okResult.Value.Should().BeAssignableTo<IEnumerable<Payment>>().Subject;
        returnedPayments.Should().HaveCount(1);
        returnedPayments.Should().NotContain(p => p.Id == deletedPayment.Id);
    }

    [Fact]
    public async Task GetById_ShouldReturnOkWithPayment_WhenPaymentExists()
    {
        // Arrange
        var payment = new PaymentBuilder().Build();
        _context.Set<Payment>().Add(payment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetById(payment.Id);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedPayment = okResult.Value.Should().BeOfType<Payment>().Subject;
        returnedPayment.Id.Should().Be(payment.Id);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenPaymentDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _controller.GetById(nonExistentId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenPaymentIsDeleted()
    {
        // Arrange
        var deletedPayment = new PaymentBuilder().Build();
        deletedPayment.Deleted = true;
        _context.Set<Payment>().Add(deletedPayment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetById(deletedPayment.Id);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedAtAction_WithNewPayment()
    {
        // Arrange
        var payment = new PaymentBuilder().Build();

        // Act
        var result = await _controller.Create(payment);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(_controller.GetById));
        var returnedPayment = createdResult.Value.Should().BeOfType<Payment>().Subject;
        returnedPayment.Id.Should().Be(payment.Id);

        var paymentInDb = await _context.Set<Payment>().FindAsync(payment.Id);
        paymentInDb.Should().NotBeNull();
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenPaymentExists()
    {
        // Arrange
        var payment = new PaymentBuilder().Build();
        _context.Set<Payment>().Add(payment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Delete(payment.Id);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        // Verify soft delete - query ignoring query filters to see if entity is marked as deleted
        var paymentInDb = await _context.Set<Payment>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == payment.Id);

        paymentInDb.Should().NotBeNull();
        paymentInDb!.Deleted.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenPaymentDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _controller.Delete(nonExistentId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenPaymentIsAlreadyDeleted()
    {
        // Arrange
        var deletedPayment = new PaymentBuilder().Build();
        deletedPayment.Deleted = true;
        _context.Set<Payment>().Add(deletedPayment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Delete(deletedPayment.Id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task PaymentExists_ShouldReturnTrue_WhenPaymentExists()
    {
        // Arrange
        var payment = new PaymentBuilder().Build();
        _context.Set<Payment>().Add(payment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.PaymentExists(payment.CourseId, payment.StudentId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var exists = okResult.Value.Should().BeOfType<bool>().Subject;
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task PaymentExists_ShouldReturnFalse_WhenPaymentDoesNotExist()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var studentId = Guid.NewGuid();

        // Act
        var result = await _controller.PaymentExists(courseId, studentId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var exists = okResult.Value.Should().BeOfType<bool>().Subject;
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task PaymentExists_ShouldReturnFalse_WhenPaymentIsDeleted()
    {
        // Arrange
        var deletedPayment = new PaymentBuilder().Build();
        deletedPayment.Deleted = true;
        _context.Set<Payment>().Add(deletedPayment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.PaymentExists(deletedPayment.CourseId, deletedPayment.StudentId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var exists = okResult.Value.Should().BeOfType<bool>().Subject;
        exists.Should().BeFalse();
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
