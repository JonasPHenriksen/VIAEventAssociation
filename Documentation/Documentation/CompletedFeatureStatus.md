# Event Title Update Checklist

## ID: 1 – Create New Event

### Success Scenarios

- [x] **S1**: Create empty event
- [x] **S2**: Set title to "Working Title"
- [x] **S3**: Set description to empty text
- [x] **S4**: Set visibility to private

## ID: 2 – Update Event Title

### Success Scenarios

- [x] **S1**: Update title for draft event
- [x] **S2**: Update title for ready event

### Failure Scenarios

- [x] **F1**: Title is empty
- [x] **F2**: Title is too short
- [x] **F3**: Title is too long
- [x] **F4**: Title is null
- [x] **F5**: Event is active
- [x] **F6**: Event is cancelled

## ID: 3 – Update Event Description

### Success Scenarios

- [x] **S1**: Update description for draft event
- [x] **S2**: Set description to empty
- [x] **S3**: Update description for ready event

### Failure Scenarios

- [ ] **F1**: Description is too long
- [x] **F2**: Event is cancelled
- [x] **F3**: Event is active

# Event Time Update Checklist

## ID: 4 – Update Event Start and End Time

### Success Scenarios

- [x] **S1**: Update times for draft event (same date)
- [x] **S2**: Update times for draft event (different dates)
- [x] **S3**: Update times for ready event
- [x] **S4**: Update times with future start time
- [x] **S5**: Update times with duration of 10 hours or less

### Failure Scenarios

- [x] **F1**: Start date is after end date
- [x] **F2**: Start time after end time
- [x] **F3**: Event duration too short (same date)
- [x] **F4**: Event duration too short (different dates)
- [x] **F5**: Event start time too early
- [x] **F6**: Event end time after 01:00
- [x] **F7**: Event is active
- [x] **F8**: Event is cancelled
- [x] **F9**: Event duration too long
- [x] **F10**: Event start time is in the past
- [ ] **F11**: Event duration spans invalid time

## ID: 5 – Make Event Public

### Success Scenarios

- [x] **S1**: Make event public (status: draft, ready, or active)

### Failure Scenarios

- [x] **F1**: Event is cancelled

## ID: 6 – Make Event Private

### Success Scenarios

- [x] **S1**: Make event private (already private)
- [x] **S2**: Make public event private

### Failure Scenarios

- [x] **F1**: Event is active
- [x] **F2**: Event is cancelled

## ID: 7 – Set Maximum Number of Guests

### Success Scenarios

- [x] **S1**: Set maximum guests (less than 50)
- [x] **S2**: Set maximum guests (greater than or equal to 5)
- [x] **S3**: Set maximum guests for active event (increase only)

### Failure Scenarios

- [x] **F1**: Event is active (reduce guests)
- [x] **F2**: Event is cancelled
- [x] **F3**: Guest number higher than location’s max
- [x] **F4**: Guest number too small (less than 5)
- [x] **F5**: Guest number exceeds maximum (greater than 50)

## ID: 8 – Ready Event

### Success Scenarios

- [x] **S1**: Ready event with valid data (title, description, times, visibility, maximum guests)

### Failure Scenarios

- [x] **F1**: Event is draft (missing data)
- [x] **F2**: Event is cancelled
- [x] **F3**: Event is in the past
- [x] **F4**: Event title is default

## ID: 9 – Activate Event

### Success Scenarios

- [x] **S1**: Activate event from draft status (make ready then active)
- [x] **S2**: Activate event from ready status
- [x] **S3**: Activate event that is already active

### Failure Scenarios

- [x] **F1**: Event is draft (missing data)
- [x] **F2**: Event is cancelled

## ID: 10 – Register New Guest Account

### Success Scenarios

- [x] **S1**: Register with valid email, first name, last name, and profile picture URL

### Failure Scenarios

- [x] **F1**: Email domain incorrect
- [x] **F2**: Email format incorrect
- [x] **F3**: First name is invalid
- [x] **F4**: Last name is invalid
- [x] **F5**: Email already used
- [x] **F6**: Names contain non-letter characters
- [x] **F7**: URL invalid format

## ID: 11 – Participate in Public Event

### Success Scenarios

- [x] **S1**: Guest successfully registers to participate in an active public event

### Failure Scenarios

- [x] **F1**: Event not active (draft, ready, or cancelled)
- [x] **F2**: No more room (maximum guests reached)
- [x] **F3**: Cannot join active event (start time in the past)
- [x] **F4**: Cannot join private event
- [x] **F5**: Guest is already participating


## ID: 12 – Cancel Event Participation

### Success Scenarios

- [x] **S1**: Guest successfully cancels participation in the event
- [x] **S2**: Guest attempts to cancel participation when not participating (no change)

### Failure Scenarios

- [x] **F1**: Event is active (cannot cancel participation for past or ongoing events)

## ID: 13 – Invite Guests to Event

### Success Scenarios

- [x] **S1**: Guest successfully invited to a ready or active event

### Failure Scenarios

- [x] **F1**: Event is draft or cancelled (cannot invite guests)
- [x] **F2**: No more room (event is full)
- [x] **F3**: Guest is already invited
- [x] **F4**: Guest is already participating

## ID: 14 – Accept Guest Invitation

### Success Scenarios

- [x] **S1**: Guest successfully accepts invitation to an active event

### Failure Scenarios

- [x] **F1**: Invitation not found (guest not invited)
- [x] **F2**: Too many guests (event is full)
- [x] **F3**: Event is cancelled (cannot join)
- [x] **F4**: Event is ready (cannot join yet)
- [x] **F5**: Event is in the past (cannot join)

## ID: 15 – Decline Guest Invitation

### Success Scenarios

- [x] **S1**: Guest successfully declines invitation to an active event
- [x] **S2**: Guest successfully declines an accepted invitation

### Failure Scenarios

- [x] **F1**: Invitation not found (guest not invited)
- [x] **F2**: Event is cancelled (cannot decline invitation)