using StackExchange.Redis;

namespace Valuator
{
    public class DB
    {
        private IConnectionMultiplexer _connection;
        public DB(string host) 
        {
            _connection = ConnectionMultiplexer.Connect(host+",allowAdmin=true");
        }

        public IDatabase GetDatabase()
        {
            return _connection.GetDatabase();
        }

        public IServer GetServer(string host)
        {
            return _connection.GetServer(host);
        }
    }
}
