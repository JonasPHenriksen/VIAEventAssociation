using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Common.Contracts;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult;

namespace VIAEventAssociation.Core.Domain.Services;

public class GuestService
{
    /*
    public async Task<OperationResult<Guest>> RegisterGuest(Email email, Name firstName, Name lastName, Uri profilePictureUrl)
    {
        if (await _guestRepository.GetByEmailAsync(email) != null)
            return OperationResult<Guest>.Failure("GuestExists", "A guest with this email already exists.");

        var guestResult = Guest.Create(email, firstName, lastName, profilePictureUrl);
        return guestResult;
    }
    */

}


