using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult;

namespace VIAEventAssociation.Core.Domain.Services;

public class GuestService
{
    private readonly IGuestRepository _guestRepository;

    public GuestService(IGuestRepository guestRepository)
    {
        _guestRepository = guestRepository;
    }

    public async Task<OperationResult<Guest>> RegisterGuest(string email, string firstName, string lastName, string profilePictureUrl)
    {
        if (await _guestRepository.GetByEmailAsync(email) != null)
            return OperationResult<Guest>.Failure("GuestExists", "A guest with this email already exists.");

        var guestResult = Guest.Create(email, firstName, lastName, profilePictureUrl);
        return guestResult;
    }
}