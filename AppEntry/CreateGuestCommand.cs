using System;
using System.Collections.Generic;
using System.Linq;
using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Tools.OperationResult;

public class CreateGuestCommand
{
    public Email Email { get; }
    public Name FirstName { get; }
    public Name LastName { get; }
    public Uri ProfilePictureUrl { get; }

    private CreateGuestCommand(Email email, Name firstName, Name lastName, Uri profilePictureUrl)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        ProfilePictureUrl = profilePictureUrl;
    }

    public static OperationResult<CreateGuestCommand> Create(string email, string firstName, string lastName, string profilePictureUrl)
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
            return OperationResult<CreateGuestCommand>.Failure(errors);

        return OperationResult<CreateGuestCommand>.Success(new CreateGuestCommand(emailResult.Value, firstNameResult.Value, lastNameResult.Value, uri));
    }
}