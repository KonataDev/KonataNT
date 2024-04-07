using KonataNT.Message;

namespace KonataNT.Core.Handlers;

internal class HighwayHandler(BaseClient client)
{
    private readonly HttpClient _client = new();

    public Task UploadResources(MessageChain chain)
    {
        foreach (var baseChain in chain)
        {
            
        }
        
        throw new NotImplementedException();
    }
}