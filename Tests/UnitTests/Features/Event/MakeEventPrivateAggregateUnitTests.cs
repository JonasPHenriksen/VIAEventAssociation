using System.Reflection;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

namespace UnitTests.Features.Event.CreateEvent;

public class MakeEventPrivateAggregateUnitTests
{
        [Theory]
        [InlineData(EventStatus.Draft)]
        [InlineData(EventStatus.Ready)]
        public void MakeEventPrivate_Success_WhenEventIsAlreadyPrivate(EventStatus status)
        {
            // Arrange
            var newEvent = EventFactory.Init()
                .WithStatus(status)
                .Build(); //Init to private by default

            // Act
            var result = newEvent.MakePrivate();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(newEvent.Visibility == EventVisibility.Private); // Event remains private
            Assert.Equal(status, newEvent.Status); // Status remains unchanged
        }

        [Theory]
        [InlineData(EventStatus.Draft)]
        [InlineData(EventStatus.Ready)]
        public void MakeEventPrivate_Success_WhenEventIsPublic(EventStatus status)
        {
            // Arrange
            var newEvent = EventFactory.Init()
                .WithStatus(status)
                .WithVisibility(EventVisibility.Public) // Event is public
                .Build();

            // Act
            var result = newEvent.MakePrivate();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(newEvent.Visibility == EventVisibility.Private); // Event is now private
            Assert.True(newEvent.Status == EventStatus.Draft); // Status remains unchanged
        }

        [Theory]
        [InlineData(EventStatus.Active)]
        [InlineData(EventStatus.Cancelled)]
        public void MakeEventPrivate_Fails_WhenEventIsInInvalidStatus(EventStatus status)
        {
            // Arrange
            var newEvent = EventFactory.Init()
                .WithStatus(status)
                .WithVisibility(EventVisibility.Public) // Event is public
                .Build();

            // Act
            var result = newEvent.MakePrivate();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidStatus", result.Errors.First().Code);
            Assert.True(newEvent.Visibility == EventVisibility.Public); // Event remains public
        }
}