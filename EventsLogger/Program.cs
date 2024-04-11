using NATS.Client;
using StackExchange.Redis;
using System.Text;
using Newtonsoft.Json;

namespace EventsLogger
{

    public class MessageModel
    {
        public string Id { get; set; }
    }

    class EventsLogger
    {
        private static readonly IDatabase db = ConnectionMultiplexer.Connect("127.0.0.1:6379").GetDatabase();
        private static readonly IConnection natsConnection = new ConnectionFactory().CreateConnection("127.0.0.1:4222");

        static void Main(string[] args)
        {
            natsConnection.SubscribeAsync("RankCalculated", (sender, args) =>
            {
                var messageBytes = args.Message.Data;

                var messageObject = JsonConvert.DeserializeObject<MessageModel>(Encoding.UTF8.GetString(messageBytes));

                string id = messageObject.Id;
                string rank = db.StringGet("RANK-" + messageObject.Id);
                
                Console.WriteLine($"RankCalculated");
                Console.WriteLine($"{id}");
                Console.WriteLine($"rank: {rank}");
                Console.WriteLine($"---------------");
            });

            natsConnection.SubscribeAsync("SimilarityCalculated", (sender, args) =>
            {
                var messageBytes = args.Message.Data;

                var messageObject = JsonConvert.DeserializeObject<MessageModel>(Encoding.UTF8.GetString(messageBytes));

                string id = messageObject.Id;
                string similarity = db.StringGet("SIMILARITY-" + messageObject.Id);

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