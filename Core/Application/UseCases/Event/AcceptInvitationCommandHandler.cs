using System.Threading.Tasks;
using AppEntry;
using VIAEventAssociation.Core.Domain.Common.Contracts;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult;

public class AcceptInvitationCommandHandler : ICommandHandler<AcceptInvitationCommand, OperationResult<Unit>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AcceptInvitationCommandHandler(IEventRepository eventRepository, IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OperationResult<Unit>> HandleAsync(AcceptInvitationCommand command)
    {
        var veaEvent = await _eventRepository.GetAsync(command.EventId);
        if (veaEvent == null)
            return OperationResult<Unit>.Failure("EventNotFound", "The specified event does not exist.");

        var result = veaEvent.AcceptInvitation(command.GuestId);
        if (!result.IsSuccess)
            return OperationResult<Unit>.Failure(result.Errors);

        await _unitOfWork.SaveChangesAsync();
        return OperationResult<Unit>.Success();
    }
}


