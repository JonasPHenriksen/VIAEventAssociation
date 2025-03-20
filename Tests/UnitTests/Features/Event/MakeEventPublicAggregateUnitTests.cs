using System.Reflection;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;

namespace UnitTests.Features.Event.CreateEvent;

public class MakeEventPublicAggregateUnitTests
{
        [Theory]
        [InlineData(EventStatus.Draft)]
        [InlineData(EventStatus.Ready)]
        [InlineData(EventStatus.Active)]
        public void MakeEventPublic_Success_WhenEventIsInValidStatus(EventStatus status)
        {
            // Arrange
            var newEvent = EventFactory.Init()
                .WithVisibility(EventVisibility.Private)
                .WithStatus(status)
                .Build();

            // Act
            var result = newEvent.MakePublic();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(newEvent.Visibility == EventVisibility.Public);
            Assert.Equal(status, newEvent.Status); 
        }

        [Fact]
        public void MakeEventPublic_Fails_WhenEventIsCancelled()
        {
            // Arrange
            var newEvent = EventFactory.Init()
                .WithStatus(EventStatus.Cancelled)
                .Build();

            // Act
            var result = newEvent.MakePublic();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InvalidStatus", result.Errors.First().Code);
            Assert.False(newEvent.Visibility == EventVisibility.Public); 
        }
}