using System.Threading.Tasks;
using AppEntry;
using VIAEventAssociation.Core.Domain.Common.Contracts;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult;

public class InviteGuestCommandHandler : ICommandHandler<InviteGuestCommand, OperationResult<Unit>>
{
    private readonly IEventRepository _eventRepository; 
    private readonly IUnitOfWork _unitOfWork;

    public InviteGuestCommandHandler(IEventRepository eventRepository, IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OperationResult<Unit>> HandleAsync(InviteGuestCommand command)
    {
        var eventToUpdate = await _eventRepository.GetAsync(command.newEventId);
        if (eventToUpdate == null)
        {
            return OperationResult<Unit>.Failure("EventNotFound", "The specified event does not exist.");
        }

        var inviteResult = eventToUpdate.InviteGuest(command.GuestId);
        if (!inviteResult.IsSuccess)
        {
            return OperationResult<Unit>.Failure(inviteResult.Errors);
        }
        
        await _unitOfWork.SaveChangesAsync();

        return OperationResult<Unit>.Success();
    }
}