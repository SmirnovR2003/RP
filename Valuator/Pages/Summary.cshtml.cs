using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Valuator.Pages;
public class SummaryModel : PageModel
{
    private readonly ILogger<SummaryModel> _logger;

    public SummaryModel(ILogger<SummaryModel> logger)
    {
        _logger = logger;
    }

    public double Rank { get; set; }
    public double Similarity { get; set; }

    public void OnGet(string id)
    {
        _logger.LogDebug(id);

        var connection = ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true");

        var db = connection.GetDatabase();


        string similarityKey = "SIMILARITY-" + id;

        Similarity = double.Parse(db.StringGet(similarityKey));

        string rankKey = "RANK-" + id;
        Rank = double.Parse(db.StringGet(rankKey));

        //TODO: проинициализировать свойства Rank и Similarity значениями из БД
    }
}
