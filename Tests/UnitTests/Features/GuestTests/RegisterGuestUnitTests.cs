using UnitTests.Factories;
using Xunit;
using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Features.GuestTests
{
    public class RegisterGuestUnitTests
    {
        private const string ValidEmail = "330943@via.dk";
        private const string ValidEmail2 = "STEK@via.dk";
        private const string ValidFirstName = "John";
        private const string ValidLastName = "Doe";
        private const string ValidProfilePictureUrl = "https://example.com/profile.jpg";

        [Fact]
        public void CreateGuest_Succeeds_WhenAllFieldsAreValidStudent()
        {
            // Arrange
            var factory = GuestFactory.Init()
                .WithEmail(new Email(ValidEmail))
                .WithFirstName(new Name(ValidFirstName))
                .WithLastName(new Name(ValidLastName))
                .WithProfilePicture(new Uri(ValidProfilePictureUrl));

            // Act
            var result = factory.Build();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(ValidEmail.ToLower(), result.Value.Email.Value);
            Assert.Equal("John", result.Value.FirstName.Value);
            Assert.Equal("Doe", result.Value.LastName.Value);
        }

        [Fact]
        public void CreateGuest_Succeeds_WhenAllFieldsAreValidLector()
        {
            // Arrange
            var factory = GuestFactory.Init()
                .WithEmail(new Email(ValidEmail2))
                .WithFirstName(new Name(ValidFirstName))
                .WithLastName(new Name(ValidLastName))
                .WithProfilePicture(new Uri(ValidProfilePictureUrl));

            // Act
            var result = factory.Build();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(ValidEmail2.ToLower(), result.Value.Email.Value);
            Assert.Equal("John", result.Value.FirstName.Value);
            Assert.Equal("Doe", result.Value.LastName.Value);
        }

        [Fact]
        public void CreateGuest_Fails_WhenEmailDomainIsIncorrect()
        {
            // Arrange
            var invalidEmail = "abc123@gmail.com";

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
            var invalidEmail = "via.dk@test.com";

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
            var invalidFirstName = "J";

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
            var invalidProfilePictureUrl = "invalid-url";

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
            var invalidFirstName = "John123";

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
            var invalidLastName = "Doe#";

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
            var shortEmail = "ab@via.dk";
            var longEmail = "abcdef@via.dk";

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
            var invalidEmail = "abc123@via.dk";

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
            var invalidLastName = "D";

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
            var invalidFirstName = "John123";
            var invalidLastName = "Doe#";

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
            var validFirstName = "A" + new string('a', 23);
            var validLastName = "B" + new string('b', 23);

            // Act
            var result = Guest.Create(ValidEmail, validFirstName, validLastName, ValidProfilePictureUrl);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void CreateGuest_Succeeds_WhenEmailIsLowerCase()
        {
            // Arrange
            var emailUpperCase = "STEK@VIA.DK";

            // Act
            var result = Guest.Create(emailUpperCase, ValidFirstName, ValidLastName, ValidProfilePictureUrl);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(emailUpperCase.ToLower(), result.Value.Email.Value);
        }
    }
}
