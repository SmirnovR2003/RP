using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using NATS.Client;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    private readonly IConnection _natsConnection;
    private readonly IDB _db;
    private IConfiguration _configuration;

    public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _db = new DB(configuration);
        _natsConnection = new ConnectionFactory().CreateConnection("127.0.0.1:4222");
    }

    public void OnGet()
    {

    }

    public IActionResult OnPost(string text, string country)
    {

        if (text == "") return Redirect($"/");
        _logger.LogDebug(text);

        string id = Guid.NewGuid().ToString();

        _db.StoreText(id, text, country);

        var messageObject = new
        {
            Id = id,
            HostAndPort = _configuration.GetConnectionString(RegionTypes.COUNTRY_TO_REGION[country]),
            Region = RegionTypes.COUNTRY_TO_REGION[country]
        }; 

        // Отправка текста в NATS
        string textMessage = JsonConvert.SerializeObject(messageObject);
        byte[] messageBytes = Encoding.UTF8.GetBytes(textMessage);
        _natsConnection.Publish("text.processing", messageBytes);

        int count = 0;
        foreach (string? tesxtByDB in _db.GetAllTexts())
        {
            if(text.Equals(tesxtByDB))
            {
                count++;
            }
        }

        double similarity = count == 1 ? 0 : 1; 

        _db.StoreSimilarity(id, similarity.ToString());

        var similarityMessageObject = new
        {
            Id = id,
            Similarity = similarity
        };

        // Отправка текста в NATS
        textMessage = JsonConvert.SerializeObject(similarityMessageObject);
        messageBytes = Encoding.UTF8.GetBytes(textMessage);
        _natsConnection.Publish("SimilarityCalculated", messageBytes);

        Thread.Sleep(1000);

        return Redirect($"summary?id={id}");
    }
}
