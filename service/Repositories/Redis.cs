using System;
using System.Diagnostics;
using StackExchange.Redis;

namespace DockerGui.Repositories
{
    public static class Redis
    {
        private static IDatabase _db;
        public static IDatabase Database
        {
            get
            {
                if (_db == null)
                {
                    var connect = ConnectionMultiplexer.Connect("localhost:6379,password=1qaz2wsx3edc");
                    connect.ConnectionFailed += (obj, @event) =>
                    {
                        Debug.WriteLine("");
                        Debug.WriteLine($"-----------------------{DateTime.Now}: Redis ConnectionFailed-----------------------");
                        Debug.WriteLine(obj);
                        Debug.WriteLine(@event);
                        Debug.WriteLine("");
                    };
                    _db = connect.GetDatabase();
                }
                return _db;
            }
        }
    }

    public class RedisKeys
    {
        /// <summary>
        /// �ڱ���־�洢
        /// </summary>
        public static string SentryLog(string id) => $"list:rightInto:leftOut:sentryLog:{id}";
    }
}