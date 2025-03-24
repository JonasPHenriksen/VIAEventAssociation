using System.Threading;
using System.Threading.Tasks;
using AppEntry;
using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Common.Contracts;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult;

public class CreateGuestCommandHandler :  ICommandHandler<CreateGuestCommand, OperationResult<Guest>>
{
    private readonly IGuestRepository _guestRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateGuestCommandHandler(IGuestRepository guestRepository, IUnitOfWork unitOfWork)
    {
        _guestRepository = guestRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OperationResult<Guest>> HandleAsync(CreateGuestCommand command)
    {
        var existingGuest = await _guestRepository.GetByEmailAsync(command.Email);
        if (existingGuest != null)
        {
            return OperationResult<Guest>.Failure(new List<Error>
            {
                new Error("EmailAlreadyExists", "A guest with this email already exists.")
            });
        }

        var guest = Guest.Create(command.Email, command.FirstName, command.LastName, command.ProfilePictureUrl);
        
        await _guestRepository.AddAsync(guest.Value);
        await _unitOfWork.SaveChangesAsync();

        return OperationResult<Guest>.Success(guest.Value);
    }
}