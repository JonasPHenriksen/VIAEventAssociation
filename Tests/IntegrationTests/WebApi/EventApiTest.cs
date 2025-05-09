using System.Net;
using EfcDataAccess.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using VIAEventAssociation.Core.Domain.Aggregates.VEAEvents;
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
        // act
        
        HttpResponseMessage response = await client.PostAsync("/api/events/create", JsonContent.Create(new { }));
        // assert part
        CreateEventResponse createEventResponse = (await response.Content.ReadFromJsonAsync<CreateEventResponse>())!;
        
        IServiceScope serviceScope = webAppFac.Services.CreateScope();
        MyDbContext context = (MyDbContext)serviceScope.ServiceProvider.GetService(typeof(MyDbContext))!;
        EventId id = EventId.FromString(createEventResponse.Id).Value;
        VeaEvent veaEvent = context.Events.SingleOrDefault(evt => evt.EventId == id)!;
        Assert.True(response.IsSuccessStatusCode);
        Assert.True(response.StatusCode == HttpStatusCode.OK);
        Assert.NotNull(veaEvent);
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