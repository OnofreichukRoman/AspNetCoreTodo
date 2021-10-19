using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using Xunit;
using System.Threading.Tasks;

namespace IntegrationTests
{
    public class TodoRouteShould : IClassFixture<TestFixture>
    {
        private readonly HttpClient _client;

        public TodoRouteShould(TestFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task Challenge_anonymous_user()
        {
            //Arrange
            var request = new HttpRequestMessage(
                HttpMethod.Get, "/Todo");

            //Act: request the /Todo route
            var response = await _client.SendAsync(request);

            //Assert: the user is sent to the login page
            Assert.Equal(
                @"http://localhost:8888/Identity/Account/Login?ReturnUrl=%2FTodo",
                response.Headers.Location.ToString());
        }
    }
}
