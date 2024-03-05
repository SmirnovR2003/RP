using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
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


        string rankKey = "RANK-" + id;
        double notAlphabetSymbolsCount = 0;

        foreach (var symbol in text)
        {
            if (!Char.IsLetter(symbol) ) ++notAlphabetSymbolsCount;
        }

        double rank = (text.Length - notAlphabetSymbolsCount) / text.Length;

        db.StringSetAsync(rankKey, rank.ToString());



        string similarityKey = "SIMILARITY-" + id;
        double similarity = 0;

        foreach (string? key in connection.GetServer("localhost:6379").Keys(pattern: "*TEXT-*"))
        {
            string? tesxtByDB = db.StringGet(key);
            if(text.Equals(tesxtByDB))
            {
                similarity = 1;
                break;
            }
            
        }
        db.StringSetAsync(similarityKey, similarity.ToString());


        string textKey = "TEXT-" + id;
        db.StringSetAsync(textKey, text);

        return Redirect($"summary?id={id}");
    }
}
