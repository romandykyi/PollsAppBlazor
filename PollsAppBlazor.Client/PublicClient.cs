namespace PollsAppBlazor.Client;

public class PublicClient
{
    public HttpClient Client { get; }

    public PublicClient(HttpClient httpClient)
    {
        Client = httpClient;
    }
}
