using OdidoAlerter.Models.Requests;
using OdidoAlerter.Models.Responses;

namespace OdidoAlerter.Repositories;

public interface IOdidoRepository
{
    Task<SearchResponse?> Search(string searchTerm, int count, int offset);
}

public class OdidoRepository(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration) : IOdidoRepository
{
    private readonly string _deviceId = configuration["Odido:DeviceId"] ?? string.Empty;

    private string _csrfToken = string.Empty;

    /// <summary>
    /// Searches for content on Odido with the specified search term, count, and offset.
    /// </summary>
    /// <remarks>
    /// Result are sorted by start time in ascending order, meaning the next episode to be aired is first.
    /// </remarks>
    /// <param name="searchTerm">The term to search for.</param>
    /// <param name="count">The number of results to return.</param>
    /// <param name="offset">The offset for pagination.</param>
    /// <returns>A <see cref="SearchResponse"/> containing the search results.</returns>
    public async Task<SearchResponse?> Search(string searchTerm, int count, int offset)
    {
        await Authenticate();

        var client = httpClientFactory.CreateClient("Odido");
        client.DefaultRequestHeaders.Add("X_CSRFToken", _csrfToken);
        var response = await client.PostAsJsonAsync("VSP/V3/SearchContent", new SearchRequest
        {
            SearchKey = searchTerm,
            ContentTypes = ["CHANNEL","VOD","PROGRAM","TVOD"],
            SearchScopes = ["ALL"],
            Count = count.ToString(),
            Offset = offset.ToString(),
            SortType = ["START_TIME:ASC"],
            Excluder = new()
            {
                RatingID = "7",
            },
            Filter = new()
            {
                ChannelScope = "1",
            },
        });

        return await response.Content.ReadFromJsonAsync<SearchResponse>();
    }

    private async Task<bool> Authenticate()
    {
        if (!string.IsNullOrEmpty(_csrfToken))
        {
            return true; // Already authenticated
        }

        var client = httpClientFactory.CreateClient("Odido");
        var response = await client.PostAsJsonAsync("VSP/V3/Authenticate", new AuthenticateRequest
        {
            AuthenticateBasic = new()
            {
                UserID = configuration["Odido:UserId"] ?? string.Empty,
                ClientPasswd = configuration["Odido:ClientPassword"] ?? string.Empty,
            },
            AuthenticateDevice = new()
            {
                PhysicalDeviceID = $"\"{_deviceId}\"",
                TerminalID = $"\"{_deviceId}=\"",
                DeviceModel = configuration["Odido:DeviceModel"] ?? string.Empty,
            }
        });

        var authenticateResponse = await response.Content.ReadFromJsonAsync<AuthenticateResponse>();

        _csrfToken = authenticateResponse?.CSRFToken ?? string.Empty;

        return !string.IsNullOrEmpty(_csrfToken);
    }
}