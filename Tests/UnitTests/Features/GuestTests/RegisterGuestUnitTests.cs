using UnitTests.Factories;
using Xunit;
using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.Guests.Entities;
using VIAEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Features.GuestTests
{
    public class RegisterGuestUnitTests
    {
        private const string EmailAddress = "330943@via.dk";
        private static readonly Email ValidEmail = new Email(EmailAddress);
        private const string EmailAddress2 = "STEK@via.dk";
        private static readonly Email ValidEmail2 = new Email(EmailAddress2);
        private const string FirstName = "John";
        private static readonly Name ValidFirstName = new Name(FirstName);
        private const string LastName = "Doe";
        private static readonly Name ValidLastName = new Name(LastName);
        private const string ProfilePictureUrl = "https://example.com/profile.jpg";
        private static readonly Uri ValidProfilePictureUrl = new Uri(ProfilePictureUrl);

        [Fact]
        public void CreateGuest_Succeeds_WhenAllFieldsAreValidStudent()
        {
            // Arrange
            var factory = GuestFactory.Init()
                .WithEmail(ValidEmail)
                .WithFirstName(ValidFirstName)
                .WithLastName(ValidLastName)
                .WithProfilePicture(ValidProfilePictureUrl);

            // Act
            var result = factory.Build();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(ValidEmail.Value.ToLower(), result.Value.Email.Value);
            Assert.Equal("John", result.Value.FirstName.Value);
            Assert.Equal("Doe", result.Value.LastName.Value);
        }

        [Fact]
        public void CreateGuest_Succeeds_WhenAllFieldsAreValidLector()
        {
            // Arrange
            var factory = GuestFactory.Init()
                .WithEmail(ValidEmail2)
                .WithFirstName(ValidFirstName)
                .WithLastName(ValidLastName)
                .WithProfilePicture(ValidProfilePictureUrl);

            // Act
            var result = factory.Build();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(ValidEmail2.Value.ToLower(), result.Value.Email.Value);
            Assert.Equal("John", result.Value.FirstName.Value);
            Assert.Equal("Doe", result.Value.LastName.Value);
        }

        [Fact]
        public void CreateGuest_Fails_WhenEmailDomainIsIncorrect()
        {
            // Arrange
            var invalidEmail = new Email("abc123@gmail.com");

            // Act
            var result = Guest.Create(invalidEmail, ValidFirstName, ValidLastName, ValidProfilePictureUrl);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidEmail", result.Errors.First().Code);
        }

        [Fact]
        public void CreateGuest_Fails_WhenEmailFormatIsIncorrect()
        {
            // Arrange
            var invalidEmail = new Email("via.dk@test.com");

            // Act
            var result = Guest.Create(invalidEmail, ValidFirstName, ValidLastName, ValidProfilePictureUrl);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidEmail", result.Errors.First().Code);
        }

        [Fact]
        public void CreateGuest_Fails_WhenFirstNameIsTooShort()
        {
            // Arrange
            var invalidFirstName = new Name("J");

            // Act
            var result = Guest.Create(ValidEmail, invalidFirstName, ValidLastName, ValidProfilePictureUrl);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidName", result.Errors.First().Code);
        }

        [Fact]
        public void CreateGuest_Fails_WhenProfilePictureUrlIsInvalid()
        {
            // Arrange
            var invalidProfilePictureUrl = new Uri("invalid-url");

            // Act
            var result = Guest.Create(ValidEmail, ValidFirstName, ValidLastName, invalidProfilePictureUrl);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidUrl", result.Errors.First().Code);
        }

        [Fact]
        public void CreateGuest_Fails_WhenFirstNameContainsNonLetters()
        {
            // Arrange
            var invalidFirstName = new Name("John123");

            // Act
            var result = Guest.Create(ValidEmail, invalidFirstName, ValidLastName, ValidProfilePictureUrl);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidName", result.Errors.First().Code);
        }

        [Fact]
        public void CreateGuest_Fails_WhenLastNameContainsNonLetters()
        {
            // Arrange
            var invalidLastName = new Name("Doe#");

            // Act
            var result = Guest.Create(ValidEmail, ValidFirstName, invalidLastName, ValidProfilePictureUrl);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidName", result.Errors.First().Code);
        }

        [Fact]
        public void CreateGuest_Fails_WhenEmailLocalPartIsTooShortOrTooLong()
        {
            // Arrange
            var shortEmail = new Email("ab@via.dk");
            var longEmail = new Email("abcdef@via.dk");

            // Act
            var resultShort = Guest.Create(shortEmail, ValidFirstName, ValidLastName, ValidProfilePictureUrl);
            var resultLong = Guest.Create(longEmail, ValidFirstName, ValidLastName, ValidProfilePictureUrl);

            // Assert
            Assert.False(resultShort.IsSuccess);
            Assert.Equal("InvalidEmail", resultShort.Errors.First().Code);
            Assert.False(resultLong.IsSuccess);
            Assert.Equal("InvalidEmail", resultLong.Errors.First().Code);
        }

        [Fact]
        public void CreateGuest_Fails_WhenEmailLocalPartHasInvalidCharacters()
        {
            // Arrange
            var invalidEmail = new Email("abc123@via.dk");

            // Act
            var result = Guest.Create(invalidEmail, ValidFirstName, ValidLastName, ValidProfilePictureUrl);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidEmail", result.Errors.First().Code);
        }

        [Fact]
        public void CreateGuest_Fails_WhenLastNameIsTooShort()
        {
            // Arrange
            var invalidLastName = new Name("D");

            // Act
            var result = Guest.Create(ValidEmail, ValidFirstName, invalidLastName, ValidProfilePictureUrl);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidName", result.Errors.First().Code);
        }

        [Fact]
        public void CreateGuest_Fails_WhenFirstNameOrLastNameContainNonLetterCharacters()
        {
            // Arrange
            var invalidFirstName = new Name("John123");
            var invalidLastName = new Name("Doe#");

            // Act
            var resultFirstName = Guest.Create(ValidEmail, invalidFirstName, ValidLastName, ValidProfilePictureUrl);
            var resultLastName = Guest.Create(ValidEmail, ValidFirstName, invalidLastName, ValidProfilePictureUrl);

            // Assert
            Assert.False(resultFirstName.IsSuccess);
            Assert.Equal("InvalidName", resultFirstName.Errors.First().Code);
            Assert.False(resultLastName.IsSuccess);
            Assert.Equal("InvalidName", resultLastName.Errors.First().Code);
        }

        [Fact]
        public void CreateGuest_Succeeds_WhenNamesAreValidLength()
        {
            // Arrange
            var validFirstName = new Name("A" + new string('a', 23));
            var validLastName = new Name("B" + new string('b', 23));

            // Act
            var result = Guest.Create(ValidEmail, validFirstName, validLastName, ValidProfilePictureUrl);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void CreateGuest_Succeeds_WhenEmailIsLowerCase()
        {
            // Arrange
            var emailUpperCase = new Email("STEK@VIA.DK");

            // Act
            var result = Guest.Create(emailUpperCase, ValidFirstName, ValidLastName, ValidProfilePictureUrl);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(emailUpperCase.Value.ToLower(), result.Value.Email.Value);
        }
    }
}
