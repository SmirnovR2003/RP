using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using NATS.Client;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    private readonly ConnectionMultiplexer _redis;
    private readonly IConnection _natsConnection;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
        _redis = ConnectionMultiplexer.Connect("127.0.0.1:6379");
        try
        {
            Options options = ConnectionFactory.GetDefaultOptions();
            options.Url = "127.0.0.1:4222";
            _natsConnection = new ConnectionFactory().CreateConnection(options);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка при подключении к NATS: {ex.Message}");
            throw;
        }
    }

    public void OnGet()
    {

    }

    public IActionResult OnPost(string text)
    {

        if (text == "") return Redirect($"/");
        _logger.LogDebug(text);

        string id = Guid.NewGuid().ToString();


        var connection = ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true");
        var db = connection.GetDatabase();

        string textKey = "TEXT-" + id;
        bool wait = db.StringSetAsync(textKey, text).Result;


        var messageObject = new
        {
            Id = id
        };

        // Отправка текста в NATS
        string textMessage = JsonConvert.SerializeObject(messageObject);
        byte[] messageBytes = Encoding.UTF8.GetBytes(textMessage);
        _natsConnection.Publish("text.processing", messageBytes);


        string similarityKey = "SIMILARITY-" + id;
        double similarity = 0;
        int count = 0;

        foreach (string? key in connection.GetServer("localhost:6379").Keys(pattern: "*TEXT-*"))
        {
            string? tesxtByDB = db.StringGet(key);
            if(text.Equals(tesxtByDB))
            {
                count++;
            }
        }

        similarity = count == 1 ? 0 : 1; 

        wait = db.StringSetAsync(similarityKey, similarity.ToString()).Result;

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
