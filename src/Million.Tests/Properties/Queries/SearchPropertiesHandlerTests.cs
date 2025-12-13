using FluentAssertions;
using Moq;
using Million.Application.Common.Interfaces;
using Million.Application.Common.Models;
using Million.Application.Properties.DTOs;
using Million.Application.Properties.Queries;
using Million.Domain.Entities;

namespace Million.Tests.Properties.Queries;

[TestFixture]
public class SearchPropertiesHandlerTests
{
    private Mock<IPropertyRepository> _propertyRepositoryMock;
    private Mock<IOwnerRepository> _ownerRepositoryMock;
    private Mock<IPropertyImageRepository> _propertyImageRepositoryMock;
    private SearchPropertiesHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _propertyRepositoryMock = new Mock<IPropertyRepository>();
        _ownerRepositoryMock = new Mock<IOwnerRepository>();
        _propertyImageRepositoryMock = new Mock<IPropertyImageRepository>();
        _handler = new SearchPropertiesHandler(
            _propertyRepositoryMock.Object,
            _ownerRepositoryMock.Object,
            _propertyImageRepositoryMock.Object);
    }

    [Test]
    public async Task Handle_WithValidRequest_ShouldReturnPaginatedProperties()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        
        var owner = new Owner(ownerId, "Test Owner", "Test Address", null, DateTime.Now);
        var property = Property.Create("Test Property", "Test Description", "Test Address", 100000, 2020, ownerId);
        
        var properties = new List<Property> { property };
        var images = new List<PropertyImage>();

        var query = new SearchPropertiesQuery(PageNumber: 1, PageSize: 10);

        _propertyRepositoryMock
            .Setup(x => x.SearchPagedAsync(It.IsAny<PaginationParameters>(), null, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((properties, 1));

        _ownerRepositoryMock
            .Setup(x => x.GetByIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(owner);

        _propertyImageRepositoryMock
            .Setup(x => x.GetByPropertyIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(images);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(1);
        result.Value.TotalCount.Should().Be(1);
        result.Value.PageNumber.Should().Be(1);
        result.Value.PageSize.Should().Be(10);
        
        var propertyDto = result.Value.Items.First();
        propertyDto.Name.Should().Be("Test Property");
        propertyDto.OwnerName.Should().Be("Test Owner");
    }

    [Test]
    public async Task Handle_WithFilters_ShouldPassFiltersToRepository()
    {
        // Arrange
        var query = new SearchPropertiesQuery("Test Name", "Test Address", 50000, 200000, 1, 5);

        _propertyRepositoryMock
            .Setup(x => x.SearchPagedAsync(It.IsAny<PaginationParameters>(), "Test Name", "Test Address", 50000, 200000, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Property>(), 0));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        _propertyRepositoryMock.Verify(
            x => x.SearchPagedAsync(
                It.Is<PaginationParameters>(p => p.PageNumber == 1 && p.PageSize == 5),
                "Test Name",
                "Test Address",
                50000,
                200000,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenRepositoryThrowsException_ShouldReturnFailureResult()
    {
        // Arrange
        var query = new SearchPropertiesQuery(PageNumber: 1, PageSize: 10);

        _propertyRepositoryMock
            .Setup(x => x.SearchPagedAsync(It.IsAny<PaginationParameters>(), null, null, null, null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("Error searching properties");
    }
}
