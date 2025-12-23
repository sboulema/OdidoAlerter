namespace OdidoAlerter.Models.Requests;

public class SearchRequest
{
    public string SearchKey { get; set; } = string.Empty;

    public List<string> ContentTypes { get; set; } = [];

    public List<string> SearchScopes { get; set; } = [];

    public string Count { get; set; } = string.Empty;

    public string Offset { get; set; } = string.Empty;

    public List<string> SortType { get; set; } = [];

    public Excluder? Excluder { get; set; }

    public Filter? Filter { get; set; }
}

public class Excluder
{
    public string RatingID { get; set; } = string.Empty;
}

public class Filter
{
    public string ChannelScope { get; set; } = string.Empty;
}