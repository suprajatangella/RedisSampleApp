using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisSampleApp
{
    internal class Program
    {
        public static readonly string RedisConnectionString = "redis-19008.c9.us-east-1-4.ec2.redns.redis-cloud.com:19008,password=dvQYlNnCUaqiFNkSOBX4K6LlddGKKzwJ,ssl=True,abortConnect=False,connectTimeout=15000, keepAlive=1, syncTimeout=10000"; // Change to your Redis server if necessary
        static async Task Main(string[] args)
        {
            // Create a connection to Redis
            ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync(RedisConnectionString);
            IDatabase db = redis.GetDatabase();

            //Set Key-Value Pair in Redis
            string cacheKey = "sampleKey";
            string cacheValue = "Redis Sample Value";

            Console.WriteLine("Setting Redis Cache");
            await db.StringSetAsync(cacheKey, cacheValue);

            // Retrieve the value from Redis
            string redisVal = await db.StringGetAsync(cacheKey);
            Console.WriteLine("Retrieved Redis Cache Value: ", redisVal);

            // Set a key with expiration time
            Console.WriteLine("Setting cache with expiration...");
            //Set Key-Value Pair in Redis
            string expKey = "expireKey";
            string expValue = "Redis Expiration Value";
            await db.StringSetAsync(expKey, expValue, TimeSpan.FromSeconds(10));

            // Retrieve value with expiration
            string expiringValue = await db.StringGetAsync(expKey);
            Console.WriteLine($"Expiring value: {expiringValue}");

            // Wait for 11 seconds
            Console.WriteLine("Waiting 11 seconds for the expiring key to disappear...");
            await Task.Delay(11000);

            // Try to get the expired value
            expiringValue = await db.StringGetAsync(expKey);
            Console.WriteLine($"Expiring value after delay: {expiringValue ?? "Key has expired"}");

            // Clean up
            Console.WriteLine("Deleting cache key...");
            await db.KeyDeleteAsync(cacheKey);

            redis.Dispose();
            Console.WriteLine("Redis connection closed.");
        }
    }
}
