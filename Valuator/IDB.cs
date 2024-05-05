using StackExchange.Redis;

namespace Valuator
{
    public interface IDB
    {
        public void StoreText(string id,  string text, string country);
        public void StoreRank(string id, double rank);
        public void StoreSimilarity(string id, string similarity);
        public string? GetText(string id);
        public string? GetSimilarity(string id);
        public string? GetRank(string id);
        public List<string> GetAllTexts();
    }
}
