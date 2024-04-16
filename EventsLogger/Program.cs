using NATS.Client;
using StackExchange.Redis;
using System.Text;
using Newtonsoft.Json;

namespace EventsLogger
{

    public class RankCalculatedMessageModel
    {
        public string Id { get; set; }
        public string Rank { get; set; }
    }
    public class SimilarityCalculatedMessageModel
    {
        public string Id { get; set; }
        public string Similarity { get; set; }
    }

    class EventsLogger
    {
        private static readonly IConnection natsConnection = new ConnectionFactory().CreateConnection("127.0.0.1:4222");

        static void Main(string[] args)
        {
            natsConnection.SubscribeAsync("RankCalculated", (sender, args) =>
            {
                var messageBytes = args.Message.Data;

                var messageObject = JsonConvert.DeserializeObject<RankCalculatedMessageModel>(Encoding.UTF8.GetString(messageBytes));

                string id = messageObject.Id;
                string rank = messageObject.Rank;
                
                Console.WriteLine($"RankCalculated");
                Console.WriteLine($"{id}");
                Console.WriteLine($"rank: {rank}");
                Console.WriteLine($"---------------");
            });

            natsConnection.SubscribeAsync("SimilarityCalculated", (sender, args) =>
            {
                var messageBytes = args.Message.Data;

                var messageObject = JsonConvert.DeserializeObject<SimilarityCalculatedMessageModel>(Encoding.UTF8.GetString(messageBytes));

                string id = messageObject.Id;
                string similarity = messageObject.Similarity;

                Console.WriteLine($"SimilarityCalculated");
                Console.WriteLine($"{id}");
                Console.WriteLine($"similarity: {similarity}");
                Console.WriteLine($"---------------");
            });

            // Ожидание сообщений
            Console.WriteLine("EventsLogger запущен. Ожидание сообщений...");
            Console.ReadLine();
        }
    }
}