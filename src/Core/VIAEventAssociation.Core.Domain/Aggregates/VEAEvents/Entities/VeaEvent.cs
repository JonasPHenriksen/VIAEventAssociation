using VIAEventAssociation.Core.Domain.Aggregates.Guests;
using VIAEventAssociation.Core.Domain.Common.Bases;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Domain.Services;
using VIAEventAssociation.Core.Tools.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

public class VeaEvent : AggregateRoot
{
    public EventId EventId { get; init; }
    internal EventStatus Status { get; set; }
    internal EventTitle Title { get; set; }
    internal EventDescription Description { get; set; }
    internal EventVisibility Visibility { get; set; }
    internal int MaxGuests { get; set; }
    internal EventTimeRange? TimeRange { get; set; }
    internal List<GuestId> Participants { get; private set; } = new List<GuestId>();
    private List<Invitation> _invitations = new List<Invitation>();
    public VeaEvent(EventId id) => EventId = id;
    private VeaEvent(){}
    

    private VeaEvent(EventId id, EventTitle title, EventDescription description, EventStatus status,
        EventVisibility visibility, int maxGuests)
    {
        EventId = id;
        Title = title;
        Description = description;
        Status = status;
        Visibility = visibility;
        MaxGuests = maxGuests;
    }

    public static OperationResult<VeaEvent> Create()
    {
        var titleResult = EventTitle.Create("Working Title");
        var descriptionResult = EventDescription.Create("");

        if (!titleResult.IsSuccess)
            return OperationResult<VeaEvent>.Failure(titleResult.Errors);

        if (!descriptionResult.IsSuccess)
            return OperationResult<VeaEvent>.Failure(descriptionResult.Errors);

        return OperationResult<VeaEvent>.Success(new VeaEvent(
            EventId.New(),
            titleResult.Value,
            descriptionResult.Value,
            EventStatus.Draft,
            EventVisibility.Private,
            5
        ));
    }

    public OperationResult<Unit> Publish()
    {
        if (Status != EventStatus.Draft)
            return OperationResult<Unit>.Failure("InvalidStatus",
                "Event cannot be published unless it's in draft status.");

        Status = EventStatus.Published;
        return OperationResult<Unit>.Success();
    }

    public OperationResult<Unit> UpdateDescription(EventDescription newDescription)
    {
        if (Status != EventStatus.Draft && Status != EventStatus.Ready)
            return OperationResult<Unit>.Failure("InvalidStatus",
                "Description can only be updated when the event is in Draft or Ready status.");

        Description = newDescription;

        if (Status == EventStatus.Ready)
            Status = EventStatus.Draft;

        return OperationResult<Unit>.Success();
    }

    public OperationResult<Unit> UpdateTitle(EventTitle newTitle)
    {
        if (Status != EventStatus.Draft && Status != EventStatus.Ready)
            return OperationResult<Unit>.Failure("InvalidStatus",
                "Title can only be updated when the event is in Draft or Ready status.");

        Title = newTitle;

        if (Status == EventStatus.Ready)
            Status = EventStatus.Draft;

        return OperationResult<Unit>.Success();
    }

    public OperationResult<Unit> UpdateTimeRange(EventTimeRange timeRange)
    {
        if (Status != EventStatus.Draft && Status != EventStatus.Ready)
            return OperationResult<Unit>.Failure("InvalidStatus",
                "Time range can only be updated when the event is in Draft or Ready status.");

        TimeRange = timeRange;

        if (Status == EventStatus.Ready)
            Status = EventStatus.Draft;

        return OperationResult<Unit>.Success();
    }

    public OperationResult<Unit> MakePublic()
    {
        if (Status == EventStatus.Cancelled)
            return OperationResult<Unit>.Failure("InvalidStatus", "A cancelled event cannot be made public.");

        Visibility = EventVisibility.Public;

        return OperationResult<Unit>.Success();
    }

    public OperationResult<Unit> MakePrivate()
    {
        if (Status == EventStatus.Active)
            return OperationResult<Unit>.Failure("InvalidStatus", "An active event cannot be made private.");

        if (Status == EventStatus.Cancelled)
            return OperationResult<Unit>.Failure("InvalidStatus", "A cancelled event cannot be modified.");

        if (Visibility == EventVisibility.Private)
            return OperationResult<Unit>.Success();

        Visibility = EventVisibility.Private;

        if (Status == EventStatus.Ready)
            Status = EventStatus.Draft;

        return OperationResult<Unit>.Success();
    }

    public OperationResult<Unit> SetMaxGuests(int maxGuests)
    {
        if (Status == EventStatus.Cancelled)
            return OperationResult<Unit>.Failure("InvalidStatus", "A cancelled event cannot be modified.");

        if (maxGuests < 5)
            return OperationResult<Unit>.Failure("InvalidMaxGuests",
                "The maximum number of guests must be at least 5.");

        if (maxGuests > 50)
            return OperationResult<Unit>.Failure("InvalidMaxGuests",
                "The maximum number of guests cannot exceed 50.");

        if (Status == EventStatus.Active && maxGuests < MaxGuests)
            return OperationResult<Unit>.Failure("InvalidMaxGuests",
                "The maximum number of guests for an active event cannot be reduced. It may only be increased.");

        MaxGuests = maxGuests;

        return OperationResult<Unit>.Success();
    }

    public OperationResult<Unit> ReadyEvent() 
    {
        if (Status == EventStatus.Cancelled)
            return OperationResult<Unit>.Failure("InvalidStatus", "A cancelled event cannot be readied.");

        if (Status != EventStatus.Draft)
            return OperationResult<Unit>.Failure("InvalidStatus", "Only events in Draft status can be readied.");

        if (Title.Value == "Working Title")
            return OperationResult<Unit>.Failure("InvalidTitle",
                "The title must be changed from the default value.");

        if (string.IsNullOrWhiteSpace(Title.Value))
            return OperationResult<Unit>.Failure("InvalidTitle", "The title cannot be empty");

        if (string.IsNullOrWhiteSpace(Description.Get))
            return OperationResult<Unit>.Failure("InvalidDescription", "The description must be set.");

        if (TimeRange == null)
            return OperationResult<Unit>.Failure("InvalidTimeRange", "The event times must be set.");

        if (MaxGuests <= 5 || MaxGuests >= 50)
            return OperationResult<Unit>.Failure("InvalidMaxGuests",
                "The maximum number of guests must be between 5 and 50.");
            
        if (TimeRange.Start < TimeRange.GetCurrentTime() || TimeRange.End < TimeRange.GetCurrentTime())
            return OperationResult<Unit>.Failure("InvalidTimeRange", "An event in the past cannot be readied.");

        Status = EventStatus.Ready;

        return OperationResult<Unit>.Success();
    }

    public OperationResult<Unit> ActivateEvent()
    {
        if (Status == EventStatus.Cancelled)
            return OperationResult<Unit>.Failure("InvalidStatus", "A cancelled event cannot be activated.");

        if (Status == EventStatus.Draft)
        {
            var readyResult = ReadyEvent();
            if (!readyResult.IsSuccess)
                return readyResult;
        }

        Status = EventStatus.Active;

        return OperationResult<Unit>.Success();
    }

    public OperationResult<Unit> Participate(GuestId guestId)
    {
        var invitation = _invitations.FirstOrDefault(i => i.GuestId == guestId);
        if (invitation != null)
            return OperationResult<Unit>.Failure("ParticipationNotPossibleWithInvitation",
                "You are already invited to this event.");

        if (Status != EventStatus.Active)
            return OperationResult<Unit>.Failure("InvalidStatus", "Only active events can be joined.");

        if (Visibility != EventVisibility.Public)
            return OperationResult<Unit>.Failure("InvalidVisibility", "Only public events can be participated.");

        if (TimeRange == null || TimeRange.Start < DateTime.Now)
            return OperationResult<Unit>.Failure("InvalidTimeRange", "Only future events can be participated.");

        if (Participants.Count + _invitations.Count(i => i.Status.IsAccepted) >= MaxGuests)
            return OperationResult<Unit>.Failure("NoMoreRoom", "There is no more room for additional guests.");

        if (Participants.Contains(guestId))
            return OperationResult<Unit>.Failure("DuplicateParticipation",
                "A guest cannot participate more than once in an event.");

        Participants.Add(guestId);

        return OperationResult<Unit>.Success();
    }

    public OperationResult<Unit> CancelParticipation(GuestId guestId)
    {
        if (TimeRange != null && TimeRange.Start < DateTime.Now)
            return OperationResult<Unit>.Failure("InvalidTimeRange",
                "You cannot cancel your participation in past or ongoing events.");

        if (Participants.Contains(guestId))
        {
            Participants.Remove(guestId);
            return OperationResult<Unit>.Success();
        }

        return OperationResult<Unit>.Success();
    }

    public OperationResult<Unit> InviteGuest(GuestId guestId)
    {
        if (Participants.Contains(guestId))
            return OperationResult<Unit>.Failure("GuestAlreadyParticipate",
                "The guest is already participating and can therefore not be invited");
        if (Status != EventStatus.Ready && Status != EventStatus.Active)
            return OperationResult<Unit>.Failure("InvalidStatus",
                "Guests can only be invited to events that are ready or active.");

        if (_invitations.Count(i => !i.Status.IsDeclined) >= MaxGuests)
            return OperationResult<Unit>.Failure("NoMoreRoom", "You cannot invite guests if the event is full.");

        if (_invitations.Any(i => i.GuestId == guestId && !i.Status.IsDeclined))
            return OperationResult<Unit>.Failure("DuplicateInvitation", "The guest is already invited.");

        var invitationResult = Invitation.Create(EventId, guestId);
        if (!invitationResult.IsSuccess)
            return OperationResult<Unit>.Failure(invitationResult.Errors);

        _invitations.Add(invitationResult.Value);
        return OperationResult<Unit>.Success();
    }

    public OperationResult<Unit> AcceptInvitation(GuestId guestId)
    {
        if (EventStatus.Cancelled == Status)
            return OperationResult<Unit>.Failure("InvalidStatus",
                "The event is not open for accepting invitations.");

        if (EventStatus.Ready == Status)
            return OperationResult<Unit>.Failure("InvalidStatus", "The event cannot yet be joined.");

        var invitation = _invitations.FirstOrDefault(i => i.GuestId == guestId && i.Status.IsPending);
        if (invitation == null)
            return OperationResult<Unit>.Failure("InvitationNotFound", "You are not invited to this event.");

        if (_invitations.Count(i => i.Status.IsAccepted) + Participants.Count >= MaxGuests)
            return OperationResult<Unit>.Failure("NoMoreRoom", "The event is full, you cannot join.");

        return invitation.Accept();
    }

    public OperationResult<Unit> DeclineInvitation(GuestId guestId)
    {
        var invitation = _invitations.FirstOrDefault(i => i.GuestId == guestId);
        if (invitation == null)
            return OperationResult<Unit>.Failure("InvitationNotFound", "The guest is not invited to the event.");

        if (Status == EventStatus.Cancelled)
        {
            return OperationResult<Unit>.Failure("InvitationDeclineCancel",
                "Invitations to cancelled events cannot be declined");
        }

        return invitation.Decline();
    }

    internal List<GuestId> GetParticipants()
    {
        return _invitations
            .Where(i => i.Status.IsAccepted)
            .Select(i => i.GuestId)
            .ToList();
    }

    internal List<GuestId> GetInvitedGuests()
    {
        return _invitations
            .Where(i => i.Status.IsPending)
            .Select(i => i.GuestId)
            .ToList();
    }

    internal List<GuestId> GetDeclinedGuests()
    {
        return _invitations
            .Where(i => i.Status.IsDeclined)
            .Select(i => i.GuestId)
            .ToList();
    }
}