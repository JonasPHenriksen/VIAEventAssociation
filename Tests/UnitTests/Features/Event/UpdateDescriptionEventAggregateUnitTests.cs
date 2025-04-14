using UnitTests.Factories;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using Xunit;

namespace UnitTests.Features.Event.CreateEvent
{
    public class UpdateDescriptionEventAggregateUnitTests
    {
        [Fact]
        public void UpdateDescription_Success_WhenEventIsInDraftStatus()
        {
            // Arrange
            var newEvent = EventFactory.Init()
                .WithStatus(EventStatus.Draft)
                .Build();
            
            var newDescription = new EventDescription("New event description"); 

            // Act
            var result = newEvent.UpdateDescription(newDescription);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(newDescription.Get, newEvent.Description.Get);
        }
        
        [Fact]
        public void UpdateDescription_Success_WhenDescriptionIsEmpty()
        {
            // Arrange
            var newEvent = EventFactory.Init()
                .WithStatus(EventStatus.Draft)
                .Build();
            
            var newEventDescription = new EventDescription("");
            
            // Act
            var result = newEvent.UpdateDescription(newEventDescription);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(newEventDescription.Get, newEvent.Description.Get);
        }

        [Fact]
        public void UpdateDescription_Success_WhenEventIsInReadyStatus()
        {
            // Arrange
            var newEvent = EventFactory.Init()
                .WithStatus(EventStatus.Ready)
                .Build();
            var newDescription = "Updated Description";

            // Act
            var result = newEvent.UpdateDescription(new EventDescription(newDescription));

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(newDescription, newEvent.Description.Get);
            Assert.Equal(EventStatus.Draft, newEvent.Status); // Status should revert to Draft
        }

        [Fact]
        public void UpdateDescription_Fails_WhenDescriptionIsTooLong()
        {
            // Arrange
            var newEvent = EventFactory.Init()
                .WithStatus(EventStatus.Draft)
                .Build();
            var longDescription = new string('A', 251); // 251 characters

            // Act
            var resultDescription = EventDescription.Create(longDescription);
            Assert.False(resultDescription.IsSuccess);
            var result = newEvent.UpdateDescription(resultDescription.Value);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidDescriptionLength", result.Errors.First().Code);
        }
        
        [Fact]
        public void UpdateDescription_Fails_WhenEventIsCancelled()
        {
            // Arrange
            var newEvent = EventFactory.Init()
                .WithStatus(EventStatus.Cancelled)
                .Build();

            // Act
            var result = newEvent.UpdateDescription(new EventDescription("New event description"));

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidStatus", result.Errors.First().Code);
        }

        [Fact]
        public void UpdateDescription_Fails_WhenEventIsActive()
        {
            // Arrange
            var newEvent = EventFactory.Init()
                .WithStatus(EventStatus.Active)
                .Build();

            // Act
            var result = newEvent.UpdateDescription(new EventDescription("New event description"));

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidStatus", result.Errors.First().Code);
        }
    }
}