namespace Million.Domain.Entities;

public class PropertyTrace
{
    private PropertyTrace() 
    { 
        Name = string.Empty;
    }

    public PropertyTrace(Guid id, Guid propertyId, DateTime dateSale,
                         string name, decimal value, decimal tax)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be negative");
        if (tax < 0)
            throw new ArgumentOutOfRangeException(nameof(tax), "Tax cannot be negative");

        Id = id;
        PropertyId = propertyId;
        DateSale = dateSale;
        Name = name;
        Value = value;
        Tax = tax;
    }

    public Guid Id { get; private set; }
    public Guid PropertyId { get; private set; }
    public DateTime DateSale { get; private set; }
    public string Name { get; private set; }
    public decimal Value { get; private set; }
    public decimal Tax { get; private set; }

    public static PropertyTrace Create(DateTime dateSale, string name, decimal value, decimal tax, Guid propertyId)
    {
        return new PropertyTrace(Guid.NewGuid(), propertyId, dateSale, name, value, tax);
    }
}