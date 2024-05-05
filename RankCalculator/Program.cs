using NATS.Client;
using StackExchange.Redis;
using System.Text;
using Newtonsoft.Json;
using System;


namespace RankCalculator
{

    public class MessageModel
    {
        public string Id { get; set; }
        public string HostAndPort { get; set; }
        public string Region { get; set; }
    }

    class RankCalculator
    {
        private static IConnection _natsConnection = new ConnectionFactory().CreateConnection("127.0.0.1:4222");

        static void Main(string[] args)
        {
            _natsConnection.SubscribeAsync("text.processing", (sender, args) =>
            {
                var messageBytes = args.Message.Data;

                var messageObject = JsonConvert.DeserializeObject<MessageModel>(Encoding.UTF8.GetString(messageBytes));

                string id = messageObject.Id;

                string hostAndPort = messageObject.HostAndPort;
                IDatabase db = ConnectionMultiplexer.Connect(hostAndPort).GetDatabase();

                string text = db.StringGet("TEXT-" + messageObject.Id);
                Console.WriteLine($"LOOKUP: {id}, {messageObject.Region}, GetText");

                double rank = 1.0 - Calculate(text);

                db.StringSetAsync("RANK-" + id, rank.ToString());
                Console.WriteLine($"LOOKUP: {id}, {messageObject.Region}, SoreRank");

                var rankMessageObject = new
                {
                    Id = id,
                    Rank = rank
                };

                // Отправка текста в NATS
                string textMessage = JsonConvert.SerializeObject(rankMessageObject);
                messageBytes = Encoding.UTF8.GetBytes(textMessage);
                _natsConnection.Publish("RankCalculated", messageBytes);

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