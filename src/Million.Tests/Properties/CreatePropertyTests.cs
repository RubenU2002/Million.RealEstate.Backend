using FluentAssertions;
using Million.Application.Common.Interfaces;
using Million.Application.Properties.Commands;
using Million.Domain.Entities;
using Moq;
using NUnit.Framework;

namespace Million.Tests.Properties;

public class CreatePropertyTests
{
    private Mock<IPropertyRepository> _propertyRepositoryMock;
    private Mock<IOwnerRepository> _ownerRepositoryMock;
    private Mock<IPropertyImageRepository> _propertyImageRepositoryMock;
    private Mock<ICurrentUserService> _currentUserServiceMock;
    private CreatePropertyHandler _handler;

    [SetUp]
    public void Setup()
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
    public async Task Handle_ShouldReturnUnauthorized_WhenOwnerIdIsNotInToken()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.OwnerIdFromToken).Returns((Guid?)null);
        var command = new CreatePropertyCommand("Name", "Desc", "Address", 1000m, 2023, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("Owner ID not found");
    }

    [Test]
    public async Task Handle_ShouldCreateProperty_WhenValidRequest()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.OwnerIdFromToken).Returns(ownerId);
        _ownerRepositoryMock.Setup(x => x.ExistsAsync(ownerId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        
        var command = new CreatePropertyCommand("Luxury House", "Beautiful house", "123 St", 500000m, 2024, null);

        _propertyRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Property>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        
        _propertyRepositoryMock.Verify(x => x.AddAsync(It.Is<Property>(p => 
            p.Name == command.Name && 
            p.Price == command.Price &&
            p.OwnerId == ownerId
        ), It.IsAny<CancellationToken>()), Times.Once);
    }
}
