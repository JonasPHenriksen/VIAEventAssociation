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
            var result = GuestFactory.Init().WithEmail(ValidEmail)
                                      .WithFirstName(ValidFirstName)
                                      .WithLastName(ValidLastName)
                                      .WithProfilePicture(ValidProfilePictureUrl)
                                      .Build();

            Assert.True(result.IsSuccess);
            Assert.Equal(ValidEmail.ToLower(), result.Value.Email.Value);
            Assert.Equal("John", result.Value.FirstName.Value);
            Assert.Equal("Doe", result.Value.LastName.Value);
        }
        [Fact]
        public void CreateGuest_Succeeds_WhenAllFieldsAreValidLector()
        {
            var result = GuestFactory.Init().WithEmail(ValidEmail2)
                .WithFirstName(ValidFirstName)
                .WithLastName(ValidLastName)
                .WithProfilePicture(ValidProfilePictureUrl)
                .Build();

            Assert.True(result.IsSuccess);
            Assert.Equal(ValidEmail2.ToLower(), result.Value.Email.Value);
            Assert.Equal("John", result.Value.FirstName.Value);
            Assert.Equal("Doe", result.Value.LastName.Value);
        }

        [Fact]
        public void CreateGuest_Fails_WhenEmailDomainIsIncorrect()
        {
            var invalidEmail = "abc123@gmail.com"; 
            var result = Guest.Create(invalidEmail, ValidFirstName, ValidLastName, ValidProfilePictureUrl);
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidEmail", result.Errors.First().Code);
        }

        [Fact]
        public void CreateGuest_Fails_WhenEmailFormatIsIncorrect()
        {
            var invalidEmail = "via.dk@test.com"; 
            var result = Guest.Create(invalidEmail, ValidFirstName, ValidLastName, ValidProfilePictureUrl);
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidEmail", result.Errors.First().Code);
        }

        [Fact]
        public void CreateGuest_Fails_WhenFirstNameIsTooShort()
        {
            var invalidFirstName = "J"; 
            var result = Guest.Create(ValidEmail, invalidFirstName, ValidLastName, ValidProfilePictureUrl);
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidName", result.Errors.First().Code);
        }

        [Fact]
        public void CreateGuest_Fails_WhenProfilePictureUrlIsInvalid()
        {
            var invalidProfilePictureUrl = "invalid-url"; 
            var result = Guest.Create(ValidEmail, ValidFirstName, ValidLastName, invalidProfilePictureUrl);
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidUrl", result.Errors.First().Code);
        }

        [Fact]
        public void CreateGuest_Fails_WhenFirstNameContainsNonLetters()
        {
            var invalidFirstName = "John123"; 
            var result = Guest.Create(ValidEmail, invalidFirstName, ValidLastName, ValidProfilePictureUrl);
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidName", result.Errors.First().Code);
        }

        [Fact]
        public void CreateGuest_Fails_WhenLastNameContainsNonLetters()
        {
            var invalidLastName = "Doe#"; 
            var result = Guest.Create(ValidEmail, ValidFirstName, invalidLastName, ValidProfilePictureUrl);
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidName", result.Errors.First().Code);
        }
        
                [Fact]
        public void CreateGuest_Fails_WhenEmailLocalPartIsTooShortOrTooLong()
        {
            var shortEmail = "ab@via.dk";  // less than 3 chars before @
            var longEmail = "abcdef@via.dk";  // more than 6 chars before @

            var resultShort = Guest.Create(shortEmail, ValidFirstName, ValidLastName, ValidProfilePictureUrl);
            var resultLong = Guest.Create(longEmail, ValidFirstName, ValidLastName, ValidProfilePictureUrl);

            Assert.False(resultShort.IsSuccess);
            Assert.Equal("InvalidEmail", resultShort.Errors.First().Code);

            Assert.False(resultLong.IsSuccess);
            Assert.Equal("InvalidEmail", resultLong.Errors.First().Code);
        }

        [Fact]
        public void CreateGuest_Fails_WhenEmailLocalPartHasInvalidCharacters()
        {
            var invalidEmail = "abc123@via.dk";  
            var result = Guest.Create(invalidEmail, ValidFirstName, ValidLastName, ValidProfilePictureUrl);
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidEmail", result.Errors.First().Code);
        }

        [Fact]
        public void CreateGuest_Fails_WhenLastNameIsTooShort()
        {
            var invalidLastName = "D"; 
            var result = Guest.Create(ValidEmail, ValidFirstName, invalidLastName, ValidProfilePictureUrl);
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidName", result.Errors.First().Code);
        }

        [Fact]
        public void CreateGuest_Fails_WhenFirstNameOrLastNameContainNonLetterCharacters()
        {
            var invalidFirstName = "John123"; 
            var invalidLastName = "Doe#"; 
            var resultFirstName = Guest.Create(ValidEmail, invalidFirstName, ValidLastName, ValidProfilePictureUrl);
            var resultLastName = Guest.Create(ValidEmail, ValidFirstName, invalidLastName, ValidProfilePictureUrl);

            Assert.False(resultFirstName.IsSuccess);
            Assert.Equal("InvalidName", resultFirstName.Errors.First().Code);

            Assert.False(resultLastName.IsSuccess);
            Assert.Equal("InvalidName", resultLastName.Errors.First().Code);
        }
        
        [Fact]
        public void CreateGuest_Succeeds_WhenNamesAreValidLength()
        {
            var validFirstName = "A" + new string('a', 23); // 25 chars
            var validLastName = "B" + new string('b', 23); // 25 chars
            var result = Guest.Create(ValidEmail, validFirstName, validLastName, ValidProfilePictureUrl);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void CreateGuest_Succeeds_WhenEmailIsLowerCase()
        {
            var emailUpperCase = "STEK@VIA.DK"; 
            var result = Guest.Create(emailUpperCase, ValidFirstName, ValidLastName, ValidProfilePictureUrl);
            Assert.True(result.IsSuccess);
            Assert.Equal(emailUpperCase.ToLower(), result.Value.Email.Value);
        }
    }
}
