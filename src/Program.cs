using System.Net;
using System.Text;
using System.Xml;
using System.ServiceModel.Syndication;
using OdidoAlerter.Repositories;
using OdidoAlerter.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton<IOdidoRepository, OdidoRepository>()
    .AddSingleton<IOdidoService, OdidoService>();

var cookieContainer = new CookieContainer();

builder.Services
	.AddHttpClient("Odido", httpClient =>
	{
		httpClient.BaseAddress = new Uri("https://tv.odido.nl");
	})
	.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler() { CookieContainer = cookieContainer })
	.RemoveAllLoggers();

var app = builder.Build();

app.UseStaticFiles();

app.MapGet("/feed.rss", async () =>
{
	var searchTerm = app.Configuration["SearchTerm"];

	if (string.IsNullOrWhiteSpace(searchTerm))
	{
		return Results.Problem(detail: "Missing or empty SearchTerm configuration", statusCode: 500);
	}

	using var scope = app.Services.CreateScope();

	var odidoService = scope.ServiceProvider.GetRequiredService<IOdidoService>();

	var searchResults = await odidoService.Search(searchTerm);

	var baseUrl = app.Configuration["BASE_URL"] ?? string.Empty;

	var items = searchResults
	    .Select(playBill => new SyndicationItem
        {
            Title = new(playBill.Name),
            Id = playBill.ID,
            Content = new TextSyndicationContent(playBill.Name),
            PublishDate = DateTimeOffset.FromUnixTimeMilliseconds(playBill.StartTime),
        });

	var feed = new SyndicationFeed(
		title: $"OdidoAlerter feed",
		description: $"OdidoAlerter feed",
		feedAlternateLink: new($"{baseUrl}/feed.rss"),
		items: items)
	{
		TimeToLive = TimeSpan.FromMinutes(30)
	};

	return Results.Text(FeedToByteArray(feed), "application/rss+xml; charset=utf-8");
});

app.Run();

static byte[] FeedToByteArray(SyndicationFeed feed)
{
	using var stream = new MemoryStream();
	using var xmlWriter = XmlWriter.Create(stream, new()
	{
		Encoding = Encoding.UTF8,
		NewLineHandling = NewLineHandling.Entitize,
		NewLineOnAttributes = true,
		Indent = true,
	});

	new Rss20FeedFormatter(feed, false).WriteTo(xmlWriter);
	xmlWriter.Flush();

	return stream.ToArray();
}