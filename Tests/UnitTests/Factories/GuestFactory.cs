namespace UnitTests.Factories;

using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Tools.OperationResult;

public class GuestFactory
{
    private string _email;
    private string _firstName;
    private string _lastName;
    private string _profilePictureUrl;

    public static GuestFactory Init() => new GuestFactory();
    public GuestFactory WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public GuestFactory WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public GuestFactory WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }
    
    public GuestFactory WithProfilePicture(string imageUrl)
    {
        _profilePictureUrl = imageUrl;
        return this;
    }

    public OperationResult<Guest> Build()
    {
        return Guest.Create(_email, _firstName, _lastName, _profilePictureUrl);
    }

    // Method to easily create a guest with default values
    public static Guest CreateGuest()
    {
        return new GuestFactory()
            .WithEmail("user123@via.dk")
            .WithFirstName("John")
            .WithLastName("Doe")
            .WithProfilePicture("https://www.example.com/path/to/resource")
            .Build().Value;
    }
}