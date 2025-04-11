using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Common.Bases;
using VIAEventAssociation.Core.Tools.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.Guests.Entities
{
    public class Guest : AggregateRoot
    {
        public GuestId GuestId { get; }
        internal Email Email { get; set; }
        internal Name FirstName { get; set; }
        internal Name LastName { get; set; }
        internal Uri ProfilePictureUrl { get; set; }
        public Guest(GuestId id) => GuestId = id;
        private Guest(){}

        private Guest(Email email, Name firstName, Name lastName, Uri profilePictureUrl)
        {
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            ProfilePictureUrl = profilePictureUrl;
            GuestId = new GuestId(Guid.NewGuid());
        }
        public static OperationResult<Guest> Create(Email email, Name firstName, Name lastName, Uri profilePictureUrl) //TODO this looks wrong
        {
            return OperationResult<Guest>.Success(new Guest(email, firstName, lastName, profilePictureUrl));
        }
    }
}

