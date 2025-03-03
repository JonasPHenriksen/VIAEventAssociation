using Xunit;
using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Features.GuestTests
{
    public class RegisterGuestUnitTests
    {
        [Fact]
        public void CreateGuest_Success_WhenAllFieldsAreValid()
        {
            // Arrange
            var email = "abc123@via.dk";
            var firstName = "John";
            var lastName = "Doe";
            var profilePictureUrl = "https://example.com/profile.jpg";

            // Act
            var result = Guest.Create(email, firstName, lastName, profilePictureUrl);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(email.ToLower(), result.Value.Email);
            Assert.Equal("John", result.Value.FirstName); // First letter capitalized
            Assert.Equal("Doe", result.Value.LastName); // First letter capitalized
            Assert.Equal(profilePictureUrl, result.Value.ProfilePictureUrl.ToString());
        }

        [Fact]
        public void CreateGuest_Fails_WhenEmailDomainIsIncorrect()
        {
            // Arrange
            var email = "abc123@gmail.com"; // Invalid domain
            var firstName = "John";
            var lastName = "Doe";
            var profilePictureUrl = "https://example.com/profile.jpg";

            // Act
            var result = Guest.Create(email, firstName, lastName, profilePictureUrl);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidEmail", result.Errors.First().Code);
        }

        [Fact]
        public void CreateGuest_Fails_WhenEmailFormatIsIncorrect()
        {
            // Arrange
            var email = "ab@via.dk"; // Too short
            var firstName = "John";
            var lastName = "Doe";
            var profilePictureUrl = "https://example.com/profile.jpg";

            // Act
            var result = Guest.Create(email, firstName, lastName, profilePictureUrl);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidEmail", result.Errors.First().Code);
        }

        [Fact]
        public void CreateGuest_Fails_WhenFirstNameIsInvalid()
        {
            // Arrange
            var email = "abc123@via.dk";
            var firstName = "J"; // Too short
            var lastName = "Doe";
            var profilePictureUrl = "https://example.com/profile.jpg";

            // Act
            var result = Guest.Create(email, firstName, lastName, profilePictureUrl);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidName", result.Errors.First().Code);
        }

        [Fact]
        public void CreateGuest_Fails_WhenLastNameIsInvalid()
        {
            // Arrange
            var email = "abc123@via.dk";
            var firstName = "John";
            var lastName = "D"; // Too short
            var profilePictureUrl = "https://example.com/profile.jpg";

            // Act
            var result = Guest.Create(email, firstName, lastName, profilePictureUrl);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidName", result.Errors.First().Code);
        }

        [Fact]
        public void CreateGuest_Fails_WhenProfilePictureUrlIsInvalid()
        {
            // Arrange
            var email = "abc123@via.dk";
            var firstName = "John";
            var lastName = "Doe";
            var profilePictureUrl = "invalid-url"; // Invalid URL

            // Act
            var result = Guest.Create(email, firstName, lastName, profilePictureUrl);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidUrl", result.Errors.First().Code);
        }
    }
}
