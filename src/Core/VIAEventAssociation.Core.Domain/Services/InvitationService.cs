using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult;

namespace VIAEventAssociation.Core.Domain.Services;

public class InvitationService
{
    private readonly IInvitationRepository _invitationRepository;

    public InvitationService(IInvitationRepository invitationRepository)
    {
        _invitationRepository = invitationRepository;
    }

    public async Task<OperationResult<Invitation>> SendInvitation(EventId eventId, GuestId guestId)
    {
        if (await _invitationRepository.GetByEventAndGuestAsync(eventId, guestId) != null)
            return OperationResult<Invitation>.Failure("InvitationExists", "An invitation already exists for this guest.");

        var invitationResult = Invitation.Create(eventId, guestId);
        if (!invitationResult.IsSuccess)
            return invitationResult;

        await _invitationRepository.SaveAsync(invitationResult.Value);
        return invitationResult;
    }
}