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
                .Build(); 

            // Act
            var result = newEvent.MakePrivate();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(newEvent.Visibility == EventVisibility.Private);
            Assert.Equal(status, newEvent.Status);
        }

        [Theory]
        [InlineData(EventStatus.Draft)]
        [InlineData(EventStatus.Ready)]
        public void MakeEventPrivate_Success_WhenEventIsPublic(EventStatus status)
        {
            // Arrange
            var newEvent = EventFactory.Init()
                .WithStatus(status)
                .WithVisibility(EventVisibility.Public) 
                .Build();

            // Act
            var result = newEvent.MakePrivate();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(newEvent.Visibility == EventVisibility.Private); 
            Assert.True(newEvent.Status == EventStatus.Draft);
        }

        [Theory]
        [InlineData(EventStatus.Active)]
        [InlineData(EventStatus.Cancelled)]
        public void MakeEventPrivate_Fails_WhenEventIsInInvalidStatus(EventStatus status)
        {
            // Arrange
            var newEvent = EventFactory.Init()
                .WithStatus(status)
                .WithVisibility(EventVisibility.Public) 
                .Build();

            // Act
            var result = newEvent.MakePrivate();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidStatus", result.Errors.First().Code);
            Assert.True(newEvent.Visibility == EventVisibility.Public); 
        }
}