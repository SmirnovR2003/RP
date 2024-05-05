using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using StackExchange.Redis;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace Valuator
{
    public class DB : IDB
    {
        private const string IDS_KEY = "IDS";
        private const string TEXT_KEY = "TEXT";
        private const string RANK_KEY = "RANK";
        private const string SIMILARITY_KEY = "SIMILARITY";

        private Dictionary<string, IDatabase> _regionToDbInstance = [];
        private Dictionary<string, ConnectionMultiplexer> _regionToDbConection = [];
        private IDatabase _dbSegmenter;
        private IConfiguration _configuration;

        public DB(IConfiguration configuration)
        {
            _configuration = configuration;
            _regionToDbConection.Add(
                RegionTypes.RUS,
                ConnectionMultiplexer.Connect(configuration.GetConnectionString(RegionTypes.RUS))
            );
            _regionToDbConection.Add(
                RegionTypes.EU,
                ConnectionMultiplexer.Connect(configuration.GetConnectionString(RegionTypes.EU))
            );
            _regionToDbConection.Add(
                RegionTypes.OTHER,
                ConnectionMultiplexer.Connect(configuration.GetConnectionString(RegionTypes.OTHER))
            );

            _regionToDbInstance.Add(
                RegionTypes.RUS,
                _regionToDbConection[RegionTypes.RUS].GetDatabase()
            );
            _regionToDbInstance.Add(
                RegionTypes.EU,
                _regionToDbConection[RegionTypes.EU].GetDatabase()
            );
            _regionToDbInstance.Add(
                RegionTypes.OTHER,
                _regionToDbConection[RegionTypes.OTHER].GetDatabase()
            );

            _dbSegmenter = ConnectionMultiplexer.Connect(configuration.GetConnectionString("db_segmenter")).GetDatabase();
        }

        public void StoreText(string id, string text, string country)
        {
            string region = RegionTypes.COUNTRY_TO_REGION[country];
            LogLookup(id, region, "StoreText");
            GetDatabase(region).StringSet($"{TEXT_KEY}-{id}", text);
            _dbSegmenter.StringSet(id, region);
        }

        public void StoreRank(string id, double rank)
        {
            string region = _dbSegmenter.StringGet(id);
            LogLookup(id, region, "StoreRank");
            GetDatabase(region).StringSet($"{RANK_KEY}-{id}", rank);
        }

        public void StoreSimilarity(string id, string similarity)
        {
            string region = _dbSegmenter.StringGet(id);
            LogLookup(id, region, "StoreSimilarity");
            GetDatabase(region).StringSet($"{SIMILARITY_KEY}-{id}", similarity);
        }

        public string? GetText(string id)
        {
            string region = _dbSegmenter.StringGet(id);
            LogLookup(id, region, "GetText");
            var content = GetDatabase(region).StringGet($"{TEXT_KEY}-{id}");
            if (content.IsNull)
            {
                return null;
            }

            return content;
        }

        public string? GetSimilarity(string id)
        {
            string region = _dbSegmenter.StringGet(id);
            LogLookup(id, region, "GetSimilarity");
            var similarity = GetDatabase(region).StringGet($"{SIMILARITY_KEY}-{id}");

            return similarity;
        }

        public string? GetRank(string id)
        {
            string region = _dbSegmenter.StringGet(id);
            LogLookup(id, region, "GetRank");
            var rank = GetDatabase(region).StringGet($"{RANK_KEY}-{id}");

            return rank;
        }

        public List<string> GetAllTexts()
        {
            List<string> res = [];
            foreach (var connection in _regionToDbConection)
            {
                foreach (string? key in connection.Value.GetServer(_configuration.GetConnectionString(connection.Key)).Keys(pattern: "*TEXT-*"))
                {
                    res.Add(GetDatabase(connection.Key).StringGet(key));
                }
            }

            return res;
        }

        private IDatabase GetDatabase(string region)
        {
            return _regionToDbInstance[region];
        }

        private void LogLookup(string id, string region, string action)
        {
            Console.WriteLine($"LOOKUP: {id}, {region}, {action}");
        }
    }
}
