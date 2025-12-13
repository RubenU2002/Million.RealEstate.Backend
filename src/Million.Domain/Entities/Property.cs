namespace Million.Domain.Entities;

public class Property
{
    private Property() 
    { 
        Name = string.Empty;
        Address = string.Empty;
        CodeInternal = string.Empty;
        Description = string.Empty;
    }

    public Property(Guid id, Guid ownerId, string name, string address,
                    decimal price, string codeInternal, int year, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address is required", nameof(address));
        if (string.IsNullOrWhiteSpace(codeInternal))
            throw new ArgumentException("CodeInternal is required", nameof(codeInternal));
        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative");

        Id = id;
        OwnerId = ownerId;
        Name = name;
        Address = address;
        Price = price;
        CodeInternal = codeInternal;
        Year = year;
        Description = description ?? string.Empty;
        Created = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid OwnerId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Address { get; private set; }
    public decimal Price { get; private set; }
    public string CodeInternal { get; private set; }
    public int Year { get; private set; }
    public DateTime Created { get; private set; }

    public IList<PropertyImage> Images { get; private init; } = new List<PropertyImage>();
    public IList<PropertyTrace> Traces { get; private init; } = new List<PropertyTrace>();

    public static Property Create(string name, string description, string address, 
                                 decimal price, int year, Guid ownerId)
    {
        var codeInternal = $"PROP-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        return new Property(Guid.NewGuid(), ownerId, name, address, price, codeInternal, year, description);
    }

    public void ChangePrice(decimal newPrice)
    {
        if (newPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(newPrice), "Price cannot be negative");
        Price = newPrice;
    }

    public void UpdateDetails(string name, string description, string address, 
                             decimal price, int year)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address is required", nameof(address));
        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative");

        Name = name;
        Description = description ?? string.Empty;
        Address = address;
        Price = price;
        Year = year;
    }
}