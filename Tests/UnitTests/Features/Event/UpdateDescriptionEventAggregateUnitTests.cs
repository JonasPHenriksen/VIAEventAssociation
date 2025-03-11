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
            var newDescription = "Updated Description";

            // Act
            var result = newEvent.UpdateDescription(newDescription);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(newDescription, newEvent.Description.Value);
        }
        
        [Fact]
        public void UpdateDescription_Success_WhenDescriptionIsEmpty()
        {
            // Arrange
            var newEvent = EventFactory.Init()
                .WithStatus(EventStatus.Draft)
                .Build();

            // Act
            var result = newEvent.UpdateDescription("");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("", newEvent.Description.Value);
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
            var result = newEvent.UpdateDescription(newDescription);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(newDescription, newEvent.Description.Value);
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
            var result = newEvent.UpdateDescription(longDescription);

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
            var result = newEvent.UpdateDescription("New Description");

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
            var result = newEvent.UpdateDescription("New Description");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidStatus", result.Errors.First().Code);
        }
    }
}