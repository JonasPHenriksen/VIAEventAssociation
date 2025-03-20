using System.Collections.Generic;
using VIAEventAssociation.Core.Tools.OperationResult;
using VIAEventAssociation.Core.Domain.Common.Bases;

namespace VIAEventAssociation.Core.Domain.Aggregates.VEAEvents.Values
{
    public class InvitationStatus : ValueObject
    {
        public enum InvitationStatusEnum
        {
            Pending,
            Accepted,
            Declined
        }

        public InvitationStatusEnum Value { get; }

        // Private constructor to enforce immutability
        internal InvitationStatus(InvitationStatusEnum value)
        {
            Value = value;
        }

        // Factory method
        public static InvitationStatus Pending()
        {
            return new InvitationStatus(InvitationStatusEnum.Pending);
        }

        // Factory method
        public static InvitationStatus Accepted()
        {
            return new InvitationStatus(InvitationStatusEnum.Accepted);
        }

        // Factory method
        public static InvitationStatus Declined()
        {
            return new InvitationStatus(InvitationStatusEnum.Declined);
        }

        public OperationResult<InvitationStatus> Accept()
        {
            if (Value != InvitationStatusEnum.Pending)
                return OperationResult<InvitationStatus>.Failure("InvalidStatus", "Only pending invitations can be accepted.");

            return OperationResult<InvitationStatus>.Success(Accepted());
        }

        public OperationResult<InvitationStatus> Decline()
        {
            if (Value != InvitationStatusEnum.Pending)
                return OperationResult<InvitationStatus>.Failure("InvalidStatus", "Only pending invitations can be declined.");

            return OperationResult<InvitationStatus>.Success(Declined());
        }

        public bool IsPending => Value == InvitationStatusEnum.Pending;
        public bool IsAccepted => Value == InvitationStatusEnum.Accepted;
        public bool IsDeclined => Value == InvitationStatusEnum.Declined;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
