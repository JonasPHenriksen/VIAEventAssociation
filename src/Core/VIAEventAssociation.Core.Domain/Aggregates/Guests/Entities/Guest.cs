using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Tools.OperationResult;

public class Guest
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

    //TODO change the create method to take value objects with operation result to gather all the errors.
    
    public static OperationResult<Guest> Create(string email, string firstName, string lastName, string profilePictureUrl)
    {
        var errors = new List<Error>();

        var emailResult = Email.Create(email);
        var firstNameResult = Name.Create(firstName);
        var lastNameResult = Name.Create(lastName);

        if (!Uri.TryCreate(profilePictureUrl, UriKind.Absolute, out var uri))
            errors.Add(new Error("InvalidUrl", "Profile picture URL must be a valid absolute URL."));

        if (!emailResult.IsSuccess) errors.AddRange(emailResult.Errors);
        if (!firstNameResult.IsSuccess) errors.AddRange(firstNameResult.Errors);
        if (!lastNameResult.IsSuccess) errors.AddRange(lastNameResult.Errors);

        if (errors.Any())
            return OperationResult<Guest>.Failure(errors);

        return OperationResult<Guest>.Success(new Guest(emailResult.Value, firstNameResult.Value, lastNameResult.Value, uri));
    }
}