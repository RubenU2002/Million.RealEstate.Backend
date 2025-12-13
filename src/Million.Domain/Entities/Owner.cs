namespace Million.Domain.Entities;

public class Owner
{
    private Owner() 
    { 
        Name = string.Empty;
        Address = string.Empty;
        Photo = string.Empty;
    }

    public Owner(Guid id, string name, string address,
                 string? photo, DateTime birthday)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address is required", nameof(address));

        Id = id;
        Name = name;
        Address = address;
        Photo = photo ?? string.Empty;
        Birthday = birthday;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Address { get; private set; }
    public string Photo { get; private set; }
    public DateTime Birthday { get; private set; }

    public static Owner Create(string name, string address, string? photo, DateTime birthday)
    {
        return new Owner(Guid.NewGuid(), name, address, photo, birthday);
    }

    public void ChangeAddress(string newAddress)
    {
        if (string.IsNullOrWhiteSpace(newAddress))
            throw new ArgumentException("Address is required", nameof(newAddress));
        Address = newAddress;
    }
}