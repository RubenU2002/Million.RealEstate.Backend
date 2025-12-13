using FluentAssertions;
using Moq;
using Million.Application.Common.Interfaces;
using Million.Application.Properties.Commands;
using Million.Domain.Entities;

namespace Million.Tests.Properties.Commands;

[TestFixture]
public class CreatePropertyHandlerTests
{
    private Mock<IPropertyRepository> _propertyRepositoryMock;
    private Mock<IOwnerRepository> _ownerRepositoryMock;
    private Mock<IPropertyImageRepository> _propertyImageRepositoryMock;
    private Mock<ICurrentUserService> _currentUserServiceMock;
    private CreatePropertyHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _propertyRepositoryMock = new Mock<IPropertyRepository>();
        _ownerRepositoryMock = new Mock<IOwnerRepository>();
        _propertyImageRepositoryMock = new Mock<IPropertyImageRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _handler = new CreatePropertyHandler(
            _propertyRepositoryMock.Object,
            _ownerRepositoryMock.Object,
            _propertyImageRepositoryMock.Object,
            _currentUserServiceMock.Object);
    }

    [Test]
    public async Task Handle_WithValidCommand_ShouldCreatePropertySuccessfully()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var propertyId = Guid.NewGuid();
        var command = new CreatePropertyCommand(
            "Test Property",
            "Test Description", 
            "Test Address",
            100000,
            2023,
            new List<string>());

        _currentUserServiceMock
            .Setup(x => x.OwnerIdFromToken)
            .Returns(ownerId);

        _ownerRepositoryMock
            .Setup(x => x.ExistsAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _propertyRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Property>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(propertyId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(propertyId);

        _propertyRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Property>(p => 
                p.Name == "Test Property" && 
                p.Description == "Test Description" &&
                p.Address == "Test Address" &&
                p.Price == 100000 &&
                p.Year == 2023 &&
                p.OwnerId == ownerId), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Test]
    public async Task Handle_WithImages_ShouldCreatePropertyAndImages()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var propertyId = Guid.NewGuid();
        var images = new List<string> { "image1.jpg", "image2.jpg" };
        var command = new CreatePropertyCommand(
            "Test Property",
            "Test Description",
            "Test Address",
            100000,
            2023,
            images);

        _currentUserServiceMock
            .Setup(x => x.OwnerIdFromToken)
            .Returns(ownerId);

        _ownerRepositoryMock
            .Setup(x => x.ExistsAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _propertyRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Property>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(propertyId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        _propertyImageRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<PropertyImage>(), It.IsAny<CancellationToken>()), 
            Times.Exactly(2));
    }

    [Test]
    public async Task Handle_WhenOwnerNotFoundInToken_ShouldReturnUnauthorized()
    {
        // Arrange
        var command = new CreatePropertyCommand(
            "Test Property",
            "Test Description",
            "Test Address",
            100000,
            2023,
            new List<string>());

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
    public async Task Handle_WhenOwnerDoesNotExist_ShouldReturnBadRequest()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var command = new CreatePropertyCommand(
            "Test Property",
            "Test Description",
            "Test Address",
            100000,
            2023,
            new List<string>());

        _currentUserServiceMock
            .Setup(x => x.OwnerIdFromToken)
            .Returns(ownerId);

        _ownerRepositoryMock
            .Setup(x => x.ExistsAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain($"Owner with ID {ownerId} does not exist");
    }

    [Test]
    public async Task Handle_WhenRepositoryThrowsException_ShouldReturnFailureResult()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var command = new CreatePropertyCommand(
            "Test Property",
            "Test Description",
            "Test Address",
            100000,
            2023,
            new List<string>());

        _currentUserServiceMock
            .Setup(x => x.OwnerIdFromToken)
            .Returns(ownerId);

        _ownerRepositoryMock
            .Setup(x => x.ExistsAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _propertyRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Property>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("Error creating property");
    }
}
