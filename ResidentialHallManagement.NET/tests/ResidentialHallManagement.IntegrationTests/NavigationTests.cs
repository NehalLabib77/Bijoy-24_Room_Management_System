using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ResidentialHallManagement.IntegrationTests;

public class NavigationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public NavigationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/Home/Index")]
    [InlineData("/Account/Login")]
    public async Task PublicPages_ReturnOk(string url)
    {
        var client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var response = await client.GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("/Admin/Dashboard")]
    [InlineData("/HallAdmin/Dashboard")]
    [InlineData("/StudentDashboard/Index")]
    public async Task AuthenticatedPages_RedirectToLogin_WhenNotAuthenticated(string url)
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync(url);
        if (response.StatusCode == HttpStatusCode.Redirect)
        {
            // expected redirect to login
            Assert.Contains("/Account/Login", response.Headers.Location?.OriginalString ?? string.Empty);
        }
        else
        {
            // Some stacks render login page inline (200 OK), assert that returned page is a login page
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            Assert.Contains("login", body, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
