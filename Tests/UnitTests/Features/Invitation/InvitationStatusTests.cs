using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents.Values;
using VIAEventAssociation.Core.Tools.OperationResult;
using Xunit;

namespace VIAEventAssociation.Core.Tests.Domain.Aggregates.VEAEvents
{
    public class InvitationStatusTests
    {
        [Fact]
        public void InvitationStatus_Succeeds_WhenCreatedAsPending()
        {
            var status = InvitationStatus.Pending();

            Assert.True(status.IsPending);
            Assert.False(status.IsAccepted);
            Assert.False(status.IsDeclined);
        }

        [Fact]
        public void InvitationStatus_Succeeds_WhenAcceptedFromPending()
        {
            var status = InvitationStatus.Pending();
            var result = status.Accept();

            Assert.True(result.IsSuccess);
            Assert.True(result.Value.IsAccepted);
        }

        [Fact]
        public void InvitationStatus_Succeeds_WhenDeclinedFromPending()
        {
            var status = InvitationStatus.Pending();
            var result = status.Decline();

            Assert.True(result.IsSuccess);
            Assert.True(result.Value.IsDeclined);
        }

        [Fact]
        public void InvitationStatus_Fails_WhenAcceptedFromNonPending()
        {
            var status = InvitationStatus.Accepted();
            var result = status.Accept();

            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidStatus", result.Errors[0].Code);
        }

        [Fact]
        public void InvitationStatus_Fails_WhenDeclinedFromNonPending()
        {
            var status = InvitationStatus.Declined();
            var result = status.Decline();

            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidStatus", result.Errors[0].Code);
        }

        [Fact]
        public void InvitationStatus_Succeeds_WhenComparingTwoPendingStatuses()
        {
            var status1 = InvitationStatus.Pending();
            var status2 = InvitationStatus.Pending();

            Assert.True(status1.Equals(status2));
            Assert.Equal(status1.GetHashCode(), status2.GetHashCode());
        }

        [Fact]
        public void InvitationStatus_Fails_WhenComparingDifferentStatuses()
        {
            var status1 = InvitationStatus.Pending();
            var status2 = InvitationStatus.Accepted();

            Assert.False(status1.Equals(status2));
            Assert.NotEqual(status1.GetHashCode(), status2.GetHashCode());
        }
    }
}
