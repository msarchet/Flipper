using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Redis;

namespace Flipper
{
    public class Feature
    {
        internal string Name { get; set; }
        internal List<string> Users { get; set; }
        internal List<string> Groups { get; set; }
        internal int Percentage { get; set; }


        /// <summary>
        /// Creates and initalizes a feature
        /// </summary>
        /// <param name="actuator">Actuator</param>
        /// <param name="Name">Name of the feature</param>
        public Feature(Actuator actuator, string Name)
        {
            this.Name = Name;
            actuator.LoadFeature(this);
        }

        /// <summary>
        /// Adds a UserID to a feature
        /// </summary>
        /// <param name="userID">The userID to add</param>
        internal void AddUser(string userID)
        {
            if (!Users.Contains(userID))
            {
                Users.Add(userID);
            }
        }

        /// <summary>
        /// Removes a UserID from a feature
        /// </summary>
        /// <param name="userID">The UserID to remove</param>
        internal void RemoveUser(string userID)
        {
            Users.Remove(userID);
        }

        /// <summary>
        /// Adds the group with the name to the feature
        /// </summary>
        /// <param name="group">The name of the group</param>
        internal void AddGroup(string group)
        {
            if (!Groups.Contains(group))
            {
                Groups.Add(group);
            }
        }

        /// <summary>
        /// Removes a group from the feature
        /// </summary>
        /// <param name="group">The name of the Group</param>
        internal void RemoveGroup(string group)
        {
            Groups.Remove(group);
        }

        /// <summary>
        /// Clears all Groups, Users and sets the percentage to 0
        /// </summary>
        internal void Clear()
        {
            Groups.Clear();
            Users.Clear();
            Percentage = 0;
        }

        /// <summary>
        /// Returns a bool indicating if the feature is active
        /// </summary>
        /// <param name="actuator">The actuator</param>
        /// <param name="userID">Optional User to check</param>
        /// <returns>Bool indicating if the feature active</returns>
        internal bool IsActive(Actuator actuator, string userID)
        {
            if (userID == "")
            {
                return Percentage == 100;
            }
            else
            {
                return IsUserInPercentage(actuator, userID) || Users.Contains(userID) || actuator.UserInGroup(userID, Groups);
            }
        }

        private bool IsUserInPercentage(Actuator actuator, string userID)
        {
      
            //Add the user as a visitor
            actuator.AddVisitorToFeature(this, userID);

            if (Percentage == 100)
            {
                return true;
            }
            else if (Percentage != 100 || Percentage != 0)
            {
                //find out of how many people who have visited have been allowed to view
                var percentageAdded = (double)Users.Count() / actuator.UsersWhoHaveVisitedFeature(this).Count() * 100;

                var isLessThanAllowedPercentage = (int)percentageAdded < Percentage;
                var userIsInPercentage = actuator.UserPercentageFunction(userID, this);
                
                //Since everything is okay add the user
                if (isLessThanAllowedPercentage && userIsInPercentage)
                {
                    actuator.ActivateUser(this, userID);

                    return true;
                }
            }

            return false;

        }
    }

    /// <summary>
    /// Class for managing features
    /// </summary>
    public class Actuator
    {
        private IRedisClientsManager manager;
        public Func<string, Feature, bool> UserPercentageFunction;
        
        /// <summary>
        /// Creates a new instance of a Flipper.Actuator
        /// </summary>
        /// <param name="Manager">A IRedisClientsManager for persistance</param>
        public Actuator(IRedisClientsManager Manager)
        {
            manager = Manager;
            UserPercentageFunction = (u, f) => { return (int.Parse(u) % 100) <= f.Percentage; };
        }

        /// <summary>
        /// Creates a new instance of a Flipper.Actuator
        /// </summary>
        /// <param name="Manager">An IRedisClientsManager ofr persistance</param>
        /// <param name="UserPercentageFunction">A Func that returns if a user is in the Percentage for a feature</param>
        public Actuator(IRedisClientsManager Manager, Func<string, Feature, bool> UserPercentageFunction)
        {
            manager = Manager;
            this.UserPercentageFunction = UserPercentageFunction;
        }

        /// <summary>
        /// Loads a feature from redis
        /// </summary>
        /// <param name="feature">The feature to load</param>
        public void LoadFeature(Feature feature)
        {
            using (var client = manager.GetClient())
            using (var featureClient = client.As<Feature>())
            {

                var f = featureClient.Lists["Flipper:Features"].FirstOrDefault(fe => fe == feature);

                if (f != null)
                {
                    feature.Percentage = f.Percentage;
                    feature.Groups = f.Groups;
                    feature.Users = f.Users;
                }
                else
                {
                    feature.Percentage = 0;
                    feature.Groups = new List<string>();
                    feature.Users = new List<string>();
                }
            }
        }


        /// <summary>
        /// Adds the UserID to the indicated Group
        /// </summary>
        /// <param name="userID">The UserID</param>
        /// <param name="group">The name of the group</param>
        public void AddUserToGroup(string userID, string group)
        {
            var groupList = GetGroup(group);
            groupList.Add(userID);
            SaveGroup(group, groupList);

        }

        /// <summary>
        /// Sets a feature to active by setting the percentage to 100
        /// </summary>
        /// <param name="feature">The feature to activate</param>
        public void ActivateFeature(Feature feature)
        {
            feature.Percentage = 100;
            SaveFeature(feature);
        }

        /// <summary>
        /// Sets a feature to inactive by Clearing the feature
        /// </summary>
        /// <param name="feature">The feature to deactivate</param>
        public void DeactiveFeature(Feature feature)
        {
            feature.Clear();
            SaveFeature(feature);
        }

        /// <summary>
        /// Activates the group on the feature
        /// </summary>
        /// <param name="feature">The feature</param>
        /// <param name="group">The group to activate</param>
        public void ActivateGroup(Feature feature, string group)
        {
            feature.AddGroup(group);
            SaveFeature(feature);
        }

        /// <summary>
        /// Deactivates the group on the feature
        /// </summary>
        /// <param name="feature">The feature</param>
        /// <param name="group">The group to deactivate</param>
        public void DeactivateGroup(Feature feature, string group)
        {
            feature.RemoveGroup(group);
            SaveFeature(feature);
        }

        /// <summary>
        /// Activates a User on the feature
        /// </summary>
        /// <param name="feature">The feature</param>
        /// <param name="userID">The UserID to activate</param>
        public void ActivateUser(Feature feature, string userID)
        {
            feature.AddUser(userID);
            SaveFeature(feature);
        }

        /// <summary>
        /// Deactivate the User on a feature
        /// </summary>
        /// <param name="feature">The feature</param>
        /// <param name="userID">The UserID to deactivate</param>
        public void DeactivateUser(Feature feature, string userID)
        {
            feature.RemoveUser(userID);
            SaveFeature(feature);
        }

        /// <summary>
        /// Activates the percentage on a feature
        /// </summary>
        /// <param name="feature">The feature</param>
        /// <param name="percentage">The percentage to allow active</param>
        public void ActivatePercentage(Feature feature, int percentage)
        {
            feature.Percentage = percentage;
            SaveFeature(feature);
        }

        /// <summary>
        /// Deactivates a the percentage on a feature (sets to 0)
        /// </summary>
        /// <param name="feature">The feature</param>
        public void DeactivatePercentage(Feature feature)
        {
            feature.Percentage = 0;
            SaveFeature(feature);
        }

        /// <summary>
        /// Defines a group, or replaces an existing group
        /// </summary>
        /// <param name="group">The name of the group</param>
        /// <param name="users">A list of userIds</param>
        public void DefineGroup(string group, List<string> users)
        {
            SaveGroup(group, users);
        }

        /// <summary>
        /// Indicates if a feature is active
        /// </summary>
        /// <param name="feature">The feature</param>
        /// <param name="userID">Opitional specific userID</param>
        /// <returns>Bool indicating if active</returns>
        public bool IsActive(Feature feature, string userID = "")
        {
            return feature.IsActive(this, userID);
        }

        /// <summary>
        /// Indicates if a user is in a group
        /// </summary>
        /// <param name="userID">The userID to check</param>
        /// <param name="groups">The groups to check</param>
        /// <returns></returns>
        public bool UserInGroup(string userID, List<string> groups)
        {
            foreach (var g in groups)
            {
                if (GetGroup(g).Contains(userID))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets an existing feature or returns a new feature
        /// </summary>
        /// <param name="name">The name of the feature to get</param>
        /// <returns>a Flipper.Feature</returns>
        public Feature Get(string name)
        {
            return new Feature(this, name);
        }
        /// <summary>
        /// Gets the users in a group
        /// </summary>
        /// <param name="groupname"></param>
        /// <returns>A List<string> of users in the group</string></returns>
        public List<string> GetGroup(string groupname)
        {
            using (var client = manager.GetClient())
            using (var redis = client.As<List<string>>())
            {
                var hash = redis.GetHash<string>("Flipper:Groups");
                return redis.GetValueFromHash(hash, groupname);
            }
        }

        private void SaveFeature(Feature feature)
        {
            using (var client = manager.GetClient())
            using (var featureClient = client.As<Feature>())
            {

                featureClient.Lists["Flipper:Features"].Remove(feature);
                featureClient.Lists["Flipper:Features"].Add(feature);
            }
        }

        private void SaveGroup(string groupname, List<string> group)
        {
            using (var client = manager.GetClient())
            using (var redis = client.As<List<string>>())
            {
                var hash = redis.GetHash<string>("Flipper:Groups");
                redis.SetEntryInHash(hash, groupname, group);
            }
        }

        internal const string VisitorKey = "Flipper:Visitors:{0}";

        internal List<string> UsersWhoHaveVisitedFeature(Feature feature)
        {
            using(var client = manager.GetClient())
            using(var redis = client.As<string>())
            {
                return redis.Lists[String.Format(VisitorKey, feature.Name)].ToList();
            }
        }

        internal void AddVisitorToFeature(Feature feature, string userid)
        {
            using (var client = manager.GetClient())
            using (var redis = client.As<string>())
            {
                var visitorList = redis.Lists[String.Format(VisitorKey, feature.Name)];
                if (!visitorList.Contains(userid))
                {
                    visitorList.Add(userid);
                }
            }
        }
    }
}
