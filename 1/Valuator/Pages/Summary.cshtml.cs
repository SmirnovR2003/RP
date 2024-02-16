using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Valuator.Pages;
public class SummaryModel : PageModel
{
    private IDistributedCache _cache;
    private readonly ILogger<SummaryModel> _logger;

    public SummaryModel(ILogger<SummaryModel> logger, IDistributedCache cache)
    {
        _logger = logger;
        _cache = cache;
    }

    public double Rank { get; set; }
    public double Similarity { get; set; }

    public void OnGet(string id)
    {
        _logger.LogDebug(id);

        string similarityKey = "SIMILARITY-" + id;

        Similarity = double.Parse(_cache.GetString(similarityKey));

        string rankKey = "RANK-" + id;
        Rank = double.Parse(_cache.GetString(rankKey));

        //TODO: проинициализировать свойства Rank и Similarity значениями из БД
    }
}
