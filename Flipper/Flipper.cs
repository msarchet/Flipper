using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Redis;

namespace Flipper
{
    public class Feature
    {
        public string Name { get; set; }
        public List<string> Users { get; set; }
        public List<string> Groups { get; set; }
        public int Percentage { get; set; }

        public Feature(Flipper flipper, string Name)
        {
            this.Name = Name;
            flipper.LoadFeature(this);
        }

        public void AddUser(string user)
        {
            if (!Users.Contains(user))
            {
                Users.Add(user);
            }
        }

        public void RemoveUser(string user)
        {
            Users.Remove(user);
        }

        public void AddGroup(string group)
        {
            if (!Groups.Contains(group))
            {
                Groups.Add(group);
            }
        }

        public void RemoveGroup(string group)
        {
            Groups.Remove(group);
        }

        public void Clear()
        {
            Groups.Clear();
            Users.Clear();
            Percentage = 0;
        }

        public bool IsActive(Flipper flipper, string user)
        {
            if (user == "")
            {
                return Percentage == 100;
            }
            else
            {
                return int.Parse(user) % 100 < Percentage || Users.Contains(user) || flipper.UserInGroup(user, Groups);
            }
        }
    }

    public class Flipper
    {
        private IRedisClientsManager manager;

        public Flipper(IRedisClientsManager Manager)
        {
            manager = Manager;
        }

        public void LoadFeature(Feature feature)
        {
            using (var client = manager.GetClient())
            using (var featureClient = client.As<Feature>())
            {

                var f = featureClient.Lists["Flipper:Features"].FirstOrDefault(fe => fe == feature);
                
                if (f != null)
                {
                    feature.Name = f.Name;
                    feature.Percentage = f.Percentage;
                    feature.Groups = f.Groups;
                }
            }
        }


        public void AddUserToGroup(string user, string group)
        {
            var groupList = GetGroup(group);
            groupList.Add(user);
            SaveGroup(group, groupList);

        }

        public void ActivateFeature(Feature feature)
        {
            feature.Percentage = 100;
            SaveFeature(feature);
        }

        public void DeactiveFeature(Feature feature)
        {
            feature.Clear();
            SaveFeature(feature);
        }

        public void ActivateGroup(Feature feature, string group)
        {
            feature.AddGroup(group);
            SaveFeature(feature);
        }

        public void DeactivateGroup(Feature feature, string group)
        {
            feature.RemoveGroup(group);
            SaveFeature(feature);
        }

        public void ActivateUser(Feature feature, string user)
        {
            feature.AddUser(user);
            SaveFeature(feature);
        }

        public void DeactivateUser(Feature feature, string user)
        {
            feature.RemoveUser(user);
            SaveFeature(feature);
        }

        public void DefineGroup(string group, List<string> users)
        {
            SaveGroup(group, users);
        }

        public bool IsActive(Feature feature, string user = "")
        {
            return feature.IsActive(this, user);
        }

        public void ActivatePercentage(Feature feature, int percentage)
        {
            feature.Percentage = percentage;
            SaveFeature(feature);
        }

        public void DeactivatePercentage(Feature feature)
        {
            feature.Percentage = 0;
            SaveFeature(feature);
        }

        public bool UserInGroup(string user, List<string> groups)
        {
            foreach(var g in groups)
            {
                if (GetGroup(g).Contains(user))
                {
                    return true;
                }
           }
            return false;
        }

        public Feature Get(string name)
        {
            return new Feature(this, name);
        }

        public void SaveGroup(string groupname, List<string> group)
        {
            using (var client = manager.GetClient())
            using (var redis = client.As<List<string>>())
            {
               var hash =  redis.GetHash<string>("Flipper:Groups");
               redis.SetEntryInHash(hash, groupname, group);
            }
        }

        public List<string> GetGroup(string groupname)
        {
            using (var client = manager.GetClient())
            using (var redis = client.As<List<string>>())
            {
               var hash =  redis.GetHash<string>("Flipper:Groups");
               return redis.GetValueFromHash(hash, groupname);
            }
        }

        public void SaveFeature(Feature feature)
        {
            using (var client = manager.GetClient())
            using (var featureClient = client.As<Feature>())
            {
                
                featureClient.Lists[String.Format("Flipper:Features")].Remove(feature);
                featureClient.Lists[String.Format("Flipper:Features")].Add(feature);
            }
        }
    }
}
