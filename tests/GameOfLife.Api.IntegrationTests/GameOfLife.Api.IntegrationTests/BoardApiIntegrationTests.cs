using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace GameOfLife.Api.IntegrationTests
{
    public class BoardApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public BoardApiIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateBoard_ReturnsCreated()
        {
            var initialState = new bool[][]
            {
                new[] { false, true, false },
                new[] { true, true, false },
                new[] { false, false, false }
            };
            var content = new StringContent(JsonSerializer.Serialize(initialState), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/boards", content);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }
    }
}
