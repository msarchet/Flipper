using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ninject;
using ServiceStack.Redis;
using Flipper;

namespace Flipper.Tests
{
    [TestFixture]
    public class TestBase
    {
        protected FeatureFlipper FeatureFlipper;
        private IRedisClientsManager manager;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            manager = new BasicRedisClientManager("localhost:6379");
            FeatureFlipper = new FeatureFlipper(manager);           
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            FeatureFlipper = null;
        }

        [SetUp]
        public void GlobalStart()
        {
            using (var client = manager.GetClient())
            {
                client.FlushAll();
            }
        }
    }
}
