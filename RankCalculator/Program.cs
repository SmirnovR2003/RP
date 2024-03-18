using NATS.Client;
using StackExchange.Redis;
using System.Text;
using Newtonsoft.Json;

namespace RankCalculator
{

    public class MessageModel
    {
        public string Text { get; set; }
        public string Id { get; set; }
    }

    class RankCalculator
    {
        private static readonly IDatabase db = ConnectionMultiplexer.Connect("127.0.0.1:6379").GetDatabase();
        private static readonly IConnection natsConnection = new ConnectionFactory().CreateConnection("127.0.0.1:4222");

        static void Main(string[] args)
        {
            natsConnection.SubscribeAsync("text.processing", (sender, args) =>
            {
                var messageBytes = args.Message.Data;

                var messageObject = JsonConvert.DeserializeObject<MessageModel>(Encoding.UTF8.GetString(messageBytes));

                string text = messageObject.Text;
                string id = messageObject.Id;

                double rank = Calculate(text);

                string rankKey = "RANK-" + id;
                db.StringSet(rankKey, rank.ToString());
                Console.WriteLine($"запись ранга для текста с id {rankKey}");
            });

            // Ожидание сообщений
            Console.WriteLine("RankCalculator запущен. Ожидание сообщений...");
            Console.ReadLine();
        }

        static double Calculate(string text)
        {
            double notAlphabetSymbolsCount = 0;

            foreach (var symbol in text)
            {
                if (!Char.IsLetter(symbol)) ++notAlphabetSymbolsCount;
            }
            return ( text.Length - notAlphabetSymbolsCount) / text.Length;

        }
    }
}