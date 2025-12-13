namespace Million.Domain.Entities;

public class PropertyImage
{
    private PropertyImage() 
    { 
        File = string.Empty;
    }

    public PropertyImage(Guid id, Guid propertyId, string file, bool enabled)
    {
        if (string.IsNullOrWhiteSpace(file))
            throw new ArgumentException("File is required", nameof(file));

        Id = id;
        PropertyId = propertyId;
        File = file;
        Enabled = enabled;
    }

    public Guid Id { get; private set; }
    public Guid PropertyId { get; private set; }
    public string File { get; private set; }
    public bool Enabled { get; private set; }

    public static PropertyImage Create(Guid propertyId, string file, bool enabled = true)
    {
        return new PropertyImage(Guid.NewGuid(), propertyId, file, enabled);
    }

    public void Toggle() => Enabled = !Enabled;
}