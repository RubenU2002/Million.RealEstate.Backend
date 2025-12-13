using FluentAssertions;
using Moq;
using Million.Application.Common.Interfaces;
using Million.Application.Properties.Commands;
using Million.Domain.Entities;

namespace Million.Tests.Properties.Commands;

[TestFixture]
public class DeletePropertyHandlerTests
{
    private Mock<IPropertyRepository> _propertyRepositoryMock;
    private Mock<ICurrentUserService> _currentUserServiceMock;
    private DeletePropertyHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _propertyRepositoryMock = new Mock<IPropertyRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _handler = new DeletePropertyHandler(
            _propertyRepositoryMock.Object,
            _currentUserServiceMock.Object);
    }

    [Test]
    public async Task Handle_WithValidCommand_ShouldDeletePropertySuccessfully()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var propertyId = Guid.NewGuid();
        var property = Property.Create("Test Property", "Test Description", "Test Address", 100000, 2020, ownerId);
        
        var command = new DeletePropertyCommand(propertyId);

        _currentUserServiceMock
            .Setup(x => x.OwnerIdFromToken)
            .Returns(ownerId);

        _propertyRepositoryMock
            .Setup(x => x.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        _propertyRepositoryMock
            .Setup(x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _propertyRepositoryMock.Verify(
            x => x.DeleteAsync(propertyId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenPropertyNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var propertyId = Guid.NewGuid();
        var command = new DeletePropertyCommand(propertyId);

        _currentUserServiceMock
            .Setup(x => x.OwnerIdFromToken)
            .Returns(ownerId);

        _propertyRepositoryMock
            .Setup(x => x.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Property?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain($"Property with ID {propertyId} not found");
    }

    [Test]
    public async Task Handle_WhenOwnerNotFoundInToken_ShouldReturnUnauthorized()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var command = new DeletePropertyCommand(propertyId);

        _currentUserServiceMock
            .Setup(x => x.OwnerIdFromToken)
            .Returns((Guid?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("Owner ID not found in token");
    }

    [Test]
    public async Task Handle_WhenUserDoesNotOwnProperty_ShouldReturnForbidden()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var differentOwnerId = Guid.NewGuid();
        var propertyId = Guid.NewGuid();
        var property = Property.Create("Test Property", "Test Description", "Test Address", 100000, 2020, differentOwnerId);
        
        var command = new DeletePropertyCommand(propertyId);

        _currentUserServiceMock
            .Setup(x => x.OwnerIdFromToken)
            .Returns(ownerId);

        _propertyRepositoryMock
            .Setup(x => x.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("You can only delete your own properties");
    }

    [Test]
    public async Task Handle_WhenDeleteFails_ShouldReturnFailureResult()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var propertyId = Guid.NewGuid();
        var property = Property.Create("Test Property", "Test Description", "Test Address", 100000, 2020, ownerId);
        
        var command = new DeletePropertyCommand(propertyId);

        _currentUserServiceMock
            .Setup(x => x.OwnerIdFromToken)
            .Returns(ownerId);

        _propertyRepositoryMock
            .Setup(x => x.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        _propertyRepositoryMock
            .Setup(x => x.DeleteAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("Failed to delete property");
    }

    [Test]
    public async Task Handle_WhenRepositoryThrowsException_ShouldReturnFailureResult()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var propertyId = Guid.NewGuid();
        var property = Property.Create("Test Property", "Test Description", "Test Address", 100000, 2020, ownerId);
        
        var command = new DeletePropertyCommand(propertyId);

        _currentUserServiceMock
            .Setup(x => x.OwnerIdFromToken)
            .Returns(ownerId);

        _propertyRepositoryMock
            .Setup(x => x.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        _propertyRepositoryMock
            .Setup(x => x.DeleteAsync(propertyId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("Error deleting property");
    }
}
