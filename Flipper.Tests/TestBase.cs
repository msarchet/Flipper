using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ninject;
using ServiceStack.Redis;
using Actuator;

namespace Flipper.Tests
{
    [TestFixture]
    public class TestBase
    {
        protected Actuator actuator;
        private IRedisClientsManager manager;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            manager = new BasicRedisClientManager("localhost:6379");
            actuator = new Actuator(manager);           
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            actuator = null;
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
