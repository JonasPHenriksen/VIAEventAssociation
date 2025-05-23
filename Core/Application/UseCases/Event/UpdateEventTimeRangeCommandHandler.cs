using System.Threading.Tasks;
using AppEntry;
using VIAEventAssociation.Core.Domain.Common.Contracts;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult;

public class UpdateEventTimeRangeCommandHandler : ICommandHandler<UpdateEventTimeRangeCommand, OperationResult<Unit>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEventTimeRangeCommandHandler(IEventRepository eventRepository, IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OperationResult<Unit>> HandleAsync(UpdateEventTimeRangeCommand command)
    {
        var eventToUpdate = await _eventRepository.GetAsync(command.NewEventId);
        if (eventToUpdate == null)
        {
            return OperationResult<Unit>.Failure("EventNotFound", "The specified event does not exist.");
        }

        var updateResult = eventToUpdate.UpdateTimeRange(command.NewTimeRange);
        if (!updateResult.IsSuccess)
        {
            return OperationResult<Unit>.Failure(updateResult.Errors);
        }

        await _unitOfWork.SaveChangesAsync();

        return OperationResult<Unit>.Success();
    }
}