using System.Text.RegularExpressions;
using VIAEventAssociation.Core.Tools.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.Guests;

    public class Guest
    {
        public GuestId Id { get; }
        public string Email { get; }
        public string FirstName { get; } //TODO Make ValueClasses
        public string LastName { get; }
        public Uri ProfilePictureUrl { get; }

        private Guest(GuestId id, string email, string firstName, string lastName, Uri profilePictureUrl)
        {
            Id = id;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            ProfilePictureUrl = profilePictureUrl;
        }

        public static OperationResult<Guest> Create(string email, string firstName, string lastName, string profilePictureUrl)
        {
            // Validate email
            var emailResult = ValidateEmail(email);
            if (!emailResult.IsSuccess)
                return OperationResult<Guest>.Failure(emailResult.Errors);

            // Validate first name
            var firstNameResult = ValidateName(firstName, "First name");
            if (!firstNameResult.IsSuccess)
                return OperationResult<Guest>.Failure(firstNameResult.Errors);

            // Validate last name
            var lastNameResult = ValidateName(lastName, "Last name");
            if (!lastNameResult.IsSuccess)
                return OperationResult<Guest>.Failure(lastNameResult.Errors);

            // Validate profile picture URL
            var urlResult = ValidateProfilePictureUrl(profilePictureUrl);
            if (!urlResult.IsSuccess)
                return OperationResult<Guest>.Failure(urlResult.Errors);

            // Format names and email
            firstName = FormatName(firstName);
            lastName = FormatName(lastName);
            email = email.ToLower();

            // Create the guest
            return OperationResult<Guest>.Success(new Guest(
                GuestId.New(),
                email,
                firstName,
                lastName,
                new Uri(profilePictureUrl)
            ));
        }

        private static OperationResult<Unit> ValidateEmail(string email)
        {
            // Check if email ends with "@via.dk"
            if (!email.EndsWith("@via.dk"))
                return OperationResult<Unit>.Failure("InvalidEmail", "Only people with a VIA email can register.");

            // Check email format
            var emailPattern = @"^[a-zA-Z0-9]{3,6}@via\.dk$";
            if (!Regex.IsMatch(email, emailPattern))
                return OperationResult<Unit>.Failure("InvalidEmail", "Email must be in the format <text1>@via.dk, where <text1> is 3-6 letters or digits.");

            return OperationResult<Unit>.Success();
        }

        private static OperationResult<Unit> ValidateName(string name, string fieldName)
        {
            // Check length
            if (name.Length < 2 || name.Length > 25)
                return OperationResult<Unit>.Failure("InvalidName", $"{fieldName} must be between 2 and 25 characters.");

            // Check if name contains only letters
            if (!Regex.IsMatch(name, @"^[a-zA-Z]+$"))
                return OperationResult<Unit>.Failure("InvalidName", $"{fieldName} can only contain letters.");

            return OperationResult<Unit>.Success();
        }

        private static OperationResult<Unit> ValidateProfilePictureUrl(string url)
        {
            // Check if URL is valid
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult) || uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps)
                return OperationResult<Unit>.Failure("InvalidUrl", "Profile picture URL must be a valid HTTP or HTTPS URL.");

            return OperationResult<Unit>.Success();
        }

        private static string FormatName(string name)
        {
            // Capitalize the first letter and make the rest lowercase
            return char.ToUpper(name[0]) + name.Substring(1).ToLower();
        }
    }

    public record GuestId(Guid Value)
    {
        public static GuestId New() => new(Guid.NewGuid());
    }