using OdidoAlerter.Models.Responses;
using OdidoAlerter.Repositories;

namespace OdidoAlerter.Services;

public interface IOdidoService
{
    Task<IEnumerable<Playbill>> Search(string searchTerm);
}

public class OdidoService(IOdidoRepository odidoRepository) : IOdidoService
{
    public async Task<IEnumerable<Playbill>> Search(string searchTerm)
    {
        var results = new List<Playbill>();
        SearchResponse? page;

        do
        {
            page = await odidoRepository
                .Search(searchTerm, 31, results.Count)
                .ConfigureAwait(false);

            var episodes = page?.Contents?
                .Select(content => content.Playbill)
                .Cast<Playbill>() ?? [];

            if (episodes.Any())
            {
                results.AddRange(episodes);
            }
        } while (results.Count < page?.Total);

        return results;
    }
}