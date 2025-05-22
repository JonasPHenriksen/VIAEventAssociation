using System.Net;
using System.Text;
using EfcDataAccess.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
using WebAPI.Endpoints.Guests;
using WebAPI.Endpoints.VeaEvents;
using EventId = VIAEventAssociation.Core.Domain.Aggregates.VEAEvents.EventId;

namespace IntegrationTests.WebApi;

public class EventApiTest
{
    [Fact]
    public async Task CreateEvent_ShouldReturnOk()
    {
        await using WebApplicationFactory<Program> webAppFac = new VeaWebApplicationFactory();
        HttpClient client = webAppFac.CreateClient();
        
        HttpResponseMessage response = await client.PostAsync("/api/events/create", JsonContent.Create(new { }));
        CreateEventResponse createEventResponse = (await response.Content.ReadFromJsonAsync<CreateEventResponse>())!;
        
        IServiceScope serviceScope = webAppFac.Services.CreateScope();
        MyDbContext context = (MyDbContext)serviceScope.ServiceProvider.GetService(typeof(MyDbContext))!;
        EventId id = EventId.FromString(createEventResponse.Id).Value;
        VeaEvent veaEvent = context.Events.SingleOrDefault(evt => evt.EventId == id)!;
        Assert.True(response.IsSuccessStatusCode);
        Assert.True(response.StatusCode == HttpStatusCode.OK);
        Assert.NotNull(veaEvent);
    }
    
    [Fact]
    public async Task AcceptInvitation_ShouldReturnNoContent()
    {
        await using WebApplicationFactory<Program> webAppFac = new VeaWebApplicationFactory();
        HttpClient client = webAppFac.CreateClient();

        // Arrange
        var createEventResponse = await client.PostAsync("/api/events/create", JsonContent.Create(new { }));
        var createEventBody = (await createEventResponse.Content.ReadFromJsonAsync<CreateEventResponse>())!;
        string eventId = createEventBody.Id;
        
        var requestBody = new
        {
            Email = "330943@via.dk",
            FirstName = "Jonas",
            LastName = "Henriksen",
            ProfilePictureUrl = "https://example.com/profile.jpg"
        };
        
        var createGuestResponse = await client.PostAsync("/api/guests/create", JsonContent.Create(requestBody)); var createBody = (await createGuestResponse.Content.ReadFromJsonAsync<CreateGuestResponse>())!;
        string guestId = createBody.Id;
        
        //TODO Requires the event to be ready
        
        var invitationResponse = await client.PostAsync(
            $"/api/events/{eventId}/guests/{guestId}/invite", JsonContent.Create(new { }));
        
        // Act
        var acceptResponse = await client.PostAsync(
            $"/api/events/{eventId}/guests/{guestId}/accept", JsonContent.Create(new { }));

        // Assert
        Assert.True(acceptResponse.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NoContent, acceptResponse.StatusCode);

        IServiceScope scope = webAppFac.Services.CreateScope();
        MyDbContext context = (MyDbContext)scope.ServiceProvider.GetService(typeof(MyDbContext))!;
        EventId id = EventId.FromString(eventId).Value;
        VeaEvent veaEvent = context.Events.SingleOrDefault(e => e.EventId == id)!;

        Assert.NotNull(veaEvent);
        Assert.Contains(veaEvent._invitations, i => i.GuestId.Value.ToString() == guestId && i.Status.IsAccepted);
    }
    
    [Fact]
    public async Task ViewSingleEvent_ShouldReturnEventData()
    {
        await using var webAppFac = new VeaWebApplicationFactory();
        var client = webAppFac.CreateClient();

        var createResp = await client.PostAsync("/api/events/create", JsonContent.Create(new { }));
        var created = await createResp.Content.ReadFromJsonAsync<CreateEventResponse>();
        var eventId = created!.Id.ToUpper(); //TODO this should be consistent, upper vs lower case GUIDs

        //MyDbContext correctly makes a test db and uses that, but Scaffold keeps using the real one despite we tell it not to. Probably part of the queryhandler DI registration as it is in the constructor?
        
        var getResp = await client.GetAsync($"/api/events/{eventId}");
        var eventData = await getResp.Content.ReadFromJsonAsync<ViewSingleEventResponse>();

        Assert.True(getResp.IsSuccessStatusCode);
        Assert.Equal(eventData!.GuestCount, 0);
    }
    
    /*
    [Fact]
    public async Task UpdateTitle_ValidInput_ShouldReturnOk()
    {
        using WebApplicationFactory<Program> webAppFac = new VeaWebApplicationFactory();
        HttpClient client = webAppFac.CreateClient();
        HttpResponseMessage createdResponse = await client.PostAsync("/api/events/create", JsonContent.Create(new { }));
        CreateEventResponse createEventResponse = (await createdResponse.Content.ReadFromJsonAsync<CreateEventResponse>())!;
        string newTitle = "New Title";
        UpdateEventTitleRequest.Body request = new(newTitle);
        // act
        HttpResponseMessage response = await client.PostAsync($"/api/events/{createEventResponse.RequestBody.Id}/update-title", JsonContent.Create(request));
        // assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        VeaEvent veaEvent = LoadEvent(webAppFac, createEventResponse.RequestBody.Id);
        Assert.Equal(newTitle, veaEvent.title.Get);
    }
    */
}