using System.Text;
using Application.DTO;
using DataModel.Repository;
using Domain.Model;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebApi.IntegrationTests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace WebApi.IntegrationTests;

public class SomeIntegrationTests : IClassFixture<IntegrationTestsWebApplicationFactory<Program>>
{
    private readonly IntegrationTestsWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public SomeIntegrationTests(IntegrationTestsWebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _output = output;
        _factory = factory;
        Environment.SetEnvironmentVariable("Arg", "Repl1");  // Ensure this is set before creating the client
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Theory]
    [InlineData("/api/Project")]
    public async Task Post_EndpointCreatesProjectAndReturnsSuccess(string url)
    {
        // Arrange
        var newProject = new
        {
            name = "Test Project",
            startDate = new DateOnly(2023,01,01),
            endDate =new DateOnly(2023,02,02)
        };

        // var jsonContent = JsonConvert.SerializeObject(newProject);
        var jsonContent = JsonConvert.SerializeObject(newProject);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(url, content);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var responseContent = await response.Content.ReadAsStringAsync();
        var createdProject = JsonConvert.DeserializeObject<ProjectDTO>(responseContent);

        Assert.NotNull(createdProject);
        Assert.Equal(newProject.name, createdProject.Name);
        Assert.Equal(newProject.startDate, createdProject.StartDate);
        Assert.Equal(newProject.endDate, createdProject.EndDate);
    }

    [Theory]
    [InlineData("/api/Project")]
    public async Task Post_EndpointCreatesProjectAndReturnsInsuccess(string url)
    {
        // Arrange

        var newProject = new
        {
            name = "Test Project",
            startDate = new DateOnly(2023,02,02),
            endDate =new DateOnly(2023,01,01)
        };

        // var jsonContent = JsonConvert.SerializeObject(newProject);
        var jsonContent = JsonConvert.SerializeObject(newProject);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(url, content);

        // Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseContent.ToString());

        // Assert.NotNull(createdProject);
        Assert.Equal("BadRequest", response.StatusCode.ToString());

    }

    [Theory]
    [InlineData("/api/Project/2")]
    public async Task Put_EndpointUpdatesProjectAndReturnsSuccess(string url)
    {
        // Arrange
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<AbsanteeContext>();

            Utilities.ReinitializeDbForTests(db);
        }

        var newProject = new
        {
            id = 2,
            name = "Project 1",
            startDate = new DateOnly(2022, 1, 1),
            endDate =new DateOnly(2023, 11, 2)
        };

        var jsonContent = JsonConvert.SerializeObject(newProject);
        _output.WriteLine(jsonContent);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync(url, content);

        // Assert

        _output.WriteLine(response.ToString());
        response.EnsureSuccessStatusCode(); // Status Code 200-299

        Assert.Contains("OK", response.StatusCode.ToString());
    }

    [Theory]
    [InlineData("/api/Project")]
    public async Task Post_EndpointReturnsInsuccessOnDuplicatedProject(string url)
    {
        // Arrange

        using (var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<AbsanteeContext>();

            Utilities.ReinitializeDbForTests(db);
        }

        var newProject = new
        {
            name = "Project 1",
            startDate = new DateOnly(2023,01,01),
            endDate =new DateOnly(2023,01,02)
        };

        // var jsonContent = JsonConvert.SerializeObject(newProject);
        var jsonContent = JsonConvert.SerializeObject(newProject);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(url, content);

        // Assert
        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert.NotNull(createdProject);
        Assert.Equal("BadRequest", response.StatusCode.ToString());

    }

}
