using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Common.Bases;
using VIAEventAssociation.Core.Tools.OperationResult;

public class Guest : AggregateRoot
{
    internal Email Email { get; set; }
    internal Name FirstName { get; set; }
    internal Name LastName { get; set; }
    internal Uri ProfilePictureUrl { get; set; }
    
    internal GuestId GuestId { get; }

    private Guest(Email email, Name firstName, Name lastName, Uri profilePictureUrl)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        ProfilePictureUrl = profilePictureUrl;
        GuestId = new GuestId(Guid.NewGuid());
    }
    
    public static OperationResult<Guest> Create(Email email, Name firstName, Name lastName, Uri profilePictureUrl)
    {
        return OperationResult<Guest>.Success(new Guest(email, firstName, lastName, profilePictureUrl));
    }
}