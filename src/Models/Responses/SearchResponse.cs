namespace OdidoAlerter.Models.Responses;

public class Channel
{
    public string ID { get; set; } = string.Empty;
}

public class Content
{
    public Playbill? Playbill { get; set; }
}

public class Playbill
{
    public Channel? Channel { get; set; }

    public long StartTime { get; set; }

    public string ID { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public long EndTime { get; set; }
}

public class SearchResponse
{
    public int Total { get; set; }

    public List<Content> Contents { get; set; } = [];
}