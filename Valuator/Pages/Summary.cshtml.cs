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
    private readonly IDB _db;
    private IConfiguration _configuration;

    public SummaryModel(ILogger<SummaryModel> logger, IConfiguration configuration)
    {
        _db = new DB(configuration);
        _logger = logger;
    }

    public double Rank { get; set; }
    public double Similarity { get; set; }

    public void OnGet(string id)
    {
        _logger.LogDebug(id);

        Similarity = double.Parse(_db.GetSimilarity(id));
        Rank = double.Parse(_db.GetRank(id));

    }
}
