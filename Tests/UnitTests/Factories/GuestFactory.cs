namespace UnitTests.Factories;

using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Tools.OperationResult;

public class GuestFactory
{
    private Guest _guest;
    private string _email;
    private string _firstName;
    private string _lastName;
    private string _profilePictureUrl;

    public GuestFactory()
    {
        _guest = Guest.Create(new Email("330943@via.dk"),new Name("Jonas"),new Name("Henriksen"), new Uri("https://www.example.com/path/to/resource")).Value;
    }
    public static GuestFactory Init()
    {
        return new GuestFactory();
    }
    public GuestFactory WithEmail(Email email)
    {
        _guest.Email = email;
        return this;
    }

    public GuestFactory WithFirstName(Name firstName)
    {
        _guest.FirstName = firstName;
        return this;
    }

    public GuestFactory WithLastName(Name lastName)
    {
        _guest.LastName = lastName;
        return this;
    }
    
    public GuestFactory WithProfilePicture(Uri imageUrl)
    {
        _guest.ProfilePictureUrl = imageUrl;
        return this;
    }

    public OperationResult<Guest> Build()
    {
        return _guest;
    }
}