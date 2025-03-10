using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using VIAEventAssociation.Core.Tools.OperationResult;
using Xunit;

namespace VIAEventAssociation.Core.Tests.Domain.Aggregates.VEAEvents
{
    public class InvitationTests
    {
        // Happy Path Tests
        [Fact]
        public void Create_ValidEventIdAndGuestId_ShouldCreatePendingInvitation()
        {
            // Arrange
            var eventId = EventId.New();
            var guestId = GuestId.New();

            // Act
            var result = Invitation.Create(eventId, guestId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Value.Status.IsPending);
        }

        [Fact]
        public void Accept_PendingInvitation_ShouldTransitionToAccepted()
        {
            // Arrange
            var eventId = EventId.New();
            var guestId = GuestId.New();
            var invitation = Invitation.Create(eventId, guestId).Value;

            // Act
            var result = invitation.Accept();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(invitation.Status.IsAccepted);
        }

        [Fact]
        public void Decline_PendingInvitation_ShouldTransitionToDeclined()
        {
            // Arrange
            var eventId = EventId.New();
            var guestId = GuestId.New();
            var invitation = Invitation.Create(eventId, guestId).Value;

            // Act
            var result = invitation.Decline();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(invitation.Status.IsDeclined);
        }

        // Sad Path Tests
        [Fact]
        public void Accept_NonPendingInvitation_ShouldFail()
        {
            // Arrange
            var eventId = EventId.New();
            var guestId = GuestId.New();
            var invitation = Invitation.Create(eventId, guestId).Value;
            invitation.Accept(); // Transition to Accepted

            // Act
            var result = invitation.Accept();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidStatus", result.Errors[0].Code);
        }

        [Fact]
        public void Decline_NonPendingInvitation_ShouldFail()
        {
            // Arrange
            var eventId = EventId.New();
            var guestId = GuestId.New();
            var invitation = Invitation.Create(eventId, guestId).Value;
            invitation.Decline(); // Transition to Declined

            // Act
            var result = invitation.Decline();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidStatus", result.Errors[0].Code);
        }

        [Fact]
        public void Create_InvalidEventId_ShouldFail()
        {
            // Arrange
            EventId eventId = null!;
            var guestId = GuestId.New();

            // Act
            var result = Invitation.Create(eventId, guestId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidEventId", result.Errors[0].Code);
        }

        // Edge Cases
        [Fact]
        public void Equals_TwoInvitationsWithSameIds_ShouldBeEqual()
        {
            // Arrange
            var eventId = EventId.New();
            var guestId = GuestId.New();
            var invitation1 = Invitation.Create(eventId, guestId).Value;
            var invitation2 = Invitation.Create(eventId, guestId).Value;

            // Assert
            Assert.True(invitation1.Equals(invitation2));
            Assert.Equal(invitation1.GetHashCode(), invitation2.GetHashCode());
        }
    }
}