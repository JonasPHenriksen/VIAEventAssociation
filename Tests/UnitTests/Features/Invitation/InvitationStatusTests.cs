using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using VIAEventAssociation.Core.Tools.OperationResult;
using Xunit;

namespace VIAEventAssociation.Core.Tests.Domain.Aggregates.VEAEvents
{
    public class InvitationStatusTests
    {
        // Happy Path Tests
        [Fact]
        public void Create_PendingStatus_ShouldBePending()
        {
            // Arrange
            var status = InvitationStatus.Pending();

            // Assert
            Assert.True(status.IsPending);
            Assert.False(status.IsAccepted);
            Assert.False(status.IsDeclined);
        }

        [Fact]
        public void Accept_PendingStatus_ShouldTransitionToAccepted()
        {
            // Arrange
            var status = InvitationStatus.Pending();

            // Act
            var result = status.Accept();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Value.IsAccepted);
        }

        [Fact]
        public void Decline_PendingStatus_ShouldTransitionToDeclined()
        {
            // Arrange
            var status = InvitationStatus.Pending();

            // Act
            var result = status.Decline();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Value.IsDeclined);
        }

        // Sad Path Tests
        [Fact]
        public void Accept_NonPendingStatus_ShouldFail()
        {
            // Arrange
            var status = InvitationStatus.Accepted();

            // Act
            var result = status.Accept();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidStatus", result.Errors[0].Code);
        }

        [Fact]
        public void Decline_NonPendingStatus_ShouldFail()
        {
            // Arrange
            var status = InvitationStatus.Declined();

            // Act
            var result = status.Decline();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidStatus", result.Errors[0].Code);
        }

        // Edge Cases
        [Fact]
        public void Equals_TwoPendingStatuses_ShouldBeEqual()
        {
            // Arrange
            var status1 = InvitationStatus.Pending();
            var status2 = InvitationStatus.Pending();

            // Assert
            Assert.True(status1.Equals(status2));
            Assert.Equal(status1.GetHashCode(), status2.GetHashCode());
        }

        [Fact]
        public void Equals_DifferentStatuses_ShouldNotBeEqual()
        {
            // Arrange
            var status1 = InvitationStatus.Pending();
            var status2 = InvitationStatus.Accepted();

            // Assert
            Assert.False(status1.Equals(status2));
            Assert.NotEqual(status1.GetHashCode(), status2.GetHashCode());
        }
    }
}