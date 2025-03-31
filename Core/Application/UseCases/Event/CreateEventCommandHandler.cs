using System.Threading;
using System.Threading.Tasks;
using AppEntry;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using VIAEventAssociation.Core.Domain.Common.Contracts;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult;

public class CreateEventCommandHandler : ICommandHandler<CreateEventCommand, OperationResult<VeaEvent>>
{
    private readonly IEventRepository _eventRepository; 
    private readonly IUnitOfWork _unitOfWork;

    public CreateEventCommandHandler(IEventRepository eventRepository, IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OperationResult<VeaEvent>> HandleAsync(CreateEventCommand command)
    {
        var eventResult = VeaEvent.Create();

        if (!eventResult.IsSuccess)
        {
            return OperationResult<VeaEvent>.Failure(eventResult.Errors);
        }

        await _eventRepository.AddAsync(eventResult.Value);
        await _unitOfWork.SaveChangesAsync();

        return OperationResult<VeaEvent>.Success(eventResult.Value);
    }
}