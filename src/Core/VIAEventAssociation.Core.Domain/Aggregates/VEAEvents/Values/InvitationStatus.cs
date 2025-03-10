using VIAEventAssociation.Core.Tools.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.VEAEvents
{
    public class InvitationStatus
    {
        public enum InvitationStatusEnum
        {
            Pending,
            Accepted,
            Declined
        }

        public InvitationStatusEnum Value { get; }

        // Private constructor to enforce immutability
        private InvitationStatus(InvitationStatusEnum value)
        {
            Value = value;
        }

        // Factory method to create a Pending status
        public static InvitationStatus Pending()
        {
            return new InvitationStatus(InvitationStatusEnum.Pending);
        }

        // Factory method to create an Accepted status
        public static InvitationStatus Accepted()
        {
            return new InvitationStatus(InvitationStatusEnum.Accepted);
        }

        // Factory method to create a Declined status
        public static InvitationStatus Declined()
        {
            return new InvitationStatus(InvitationStatusEnum.Declined);
        }

        // Method to transition to Accepted status
        public OperationResult<InvitationStatus> Accept()
        {
            if (Value != InvitationStatusEnum.Pending)
                return OperationResult<InvitationStatus>.Failure("InvalidStatus", "Only pending invitations can be accepted.");

            return OperationResult<InvitationStatus>.Success(Accepted());
        }

        // Method to transition to Declined status
        public OperationResult<InvitationStatus> Decline()
        {
            if (Value != InvitationStatusEnum.Pending)
                return OperationResult<InvitationStatus>.Failure("InvalidStatus", "Only pending invitations can be declined.");

            return OperationResult<InvitationStatus>.Success(Declined());
        }

        // Helper properties to check the current state
        public bool IsPending => Value == InvitationStatusEnum.Pending;
        public bool IsAccepted => Value == InvitationStatusEnum.Accepted;
        public bool IsDeclined => Value == InvitationStatusEnum.Declined;

        // Override Equals and GetHashCode for value object equality
        public override bool Equals(object? obj)
        {
            if (obj is InvitationStatus other)
                return Value == other.Value;

            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}