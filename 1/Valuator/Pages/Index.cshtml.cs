using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private IDistributedCache _cache;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger, IDistributedCache cache)
    {
        _logger = logger;
        _cache = cache;
    }

    public void OnGet()
    {

    }

    public IActionResult OnPost(string text)
    {
        if (text == "") return Redirect($"/");
        _logger.LogDebug(text);

        string id = Guid.NewGuid().ToString();

        string textKey = "TEXT-" + id;
        //TODO: сохранить в БД text по ключу textKey

        string rankKey = "RANK-" + id;
        //TODO: посчитать rank и сохранить в БД по ключу rankKey
        double notAlphabetSymbolsCount = 0;

        foreach (var symbol in text)
        {
            if (!Char.IsLetter(symbol) ) ++notAlphabetSymbolsCount;
        }

        double rank = (text.Length - notAlphabetSymbolsCount) / text.Length;

        _cache.SetStringAsync(rankKey, rank.ToString());

        string similarityKey = "SIMILARITY-" + id;
        //TODO: посчитать similarity и сохранить в БД по ключу similarityKey
        double similarity = 0;

        ConnectionMultiplexer m = ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true");

        

        foreach (string? key in m.GetServer("localhost:6379").Keys(pattern:"*TEXT-*"))
        {
            string? tesxtByDB = _cache.GetString(key.Substring(5, key.Length-5));
            if(text.Equals(tesxtByDB))
            {
                similarity = 1;
                break;
            }
            
        }

        _cache.SetStringAsync(similarityKey, similarity.ToString());

        _cache.SetStringAsync(textKey, text);

        return Redirect($"summary?id={id}");
    }
}
