using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using VIAEventAssociation.Core.Tools.OperationResult;
using Xunit;

namespace VIAEventAssociation.Core.Tests.Domain.Aggregates.VEAEvents
{
    public class InvitationTests
    {
        [Fact]
        public void Invitation_Succeeds_WhenCreatedWithValidEventAndGuestId()
        {
            var eventId = EventId.New();
            var guestId = GuestId.New();

            var result = Invitation.Create(eventId, guestId);

            Assert.True(result.IsSuccess);
            Assert.True(result.Value.Status.IsPending);
        }

        [Fact]
        public void Invitation_Succeeds_WhenAcceptedFromPending()
        {
            var eventId = EventId.New();
            var guestId = GuestId.New();
            var invitation = Invitation.Create(eventId, guestId).Value;

            var result = invitation.Accept();

            Assert.True(result.IsSuccess);
            Assert.True(invitation.Status.IsAccepted);
        }

        [Fact]
        public void Invitation_Succeeds_WhenDeclinedFromPending()
        {
            var eventId = EventId.New();
            var guestId = GuestId.New();
            var invitation = Invitation.Create(eventId, guestId).Value;

            var result = invitation.Decline();

            Assert.True(result.IsSuccess);
            Assert.True(invitation.Status.IsDeclined);
        }

        [Fact]
        public void Invitation_Fails_WhenAcceptedFromNonPending()
        {
            var eventId = EventId.New();
            var guestId = GuestId.New();
            var invitation = Invitation.Create(eventId, guestId).Value;
            invitation.Accept();

            var result = invitation.Accept();

            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidStatus", result.Errors[0].Code);
        }

        [Fact]
        public void Invitation_Fails_WhenDeclinedFromNonPending()
        {
            var eventId = EventId.New();
            var guestId = GuestId.New();
            var invitation = Invitation.Create(eventId, guestId).Value;
            invitation.Decline();

            var result = invitation.Decline();

            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidStatus", result.Errors[0].Code);
        }

        [Fact]
        public void Invitation_Fails_WhenCreatedWithInvalidEventId()
        {
            EventId eventId = null!;
            var guestId = GuestId.New();

            var result = Invitation.Create(eventId, guestId);

            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidData", result.Errors[0].Code);
        }

        [Fact]
        public void Invitation_Fails_WhenDuplicateCreatedWithSameIds()
        {
            var eventId = EventId.New();
            var guestId = GuestId.New();
    
            var result1 = Invitation.Create(eventId, guestId);
            var result2 = Invitation.Create(eventId, guestId);

            Assert.True(result1.IsSuccess);
            Assert.False(result2.IsSuccess);
            Assert.Equal("Duplicate", result2.Errors.First().Code);
            Assert.Equal("An invitation with the same credentials already exists.", result2.Errors.First().Message);
        }
    }
}
