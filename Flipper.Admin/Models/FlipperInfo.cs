using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.Redis;

namespace Flipper.Admin.Models
{
    public class FlipperInfo
    {
        private IRedisClientsManager manager;

        public List<Feature> Features { get; private set; }
        public Dictionary<string, List<string>> Groups { get; private set; }

        public FlipperInfo(IRedisClientsManager manager)
        {
            using (var client = manager.GetClient())
            {
                var fetaureRedis = client.As<Feature>();
                Features = fetaureRedis.Lists["Flipper:Features"].ToList();

                var groupRedis = client.As<List<string>>();
                var hash = groupRedis.GetHash<string>("Flipper:Groups");
                Groups = hash.GetAll();
            }
        }
    }
}