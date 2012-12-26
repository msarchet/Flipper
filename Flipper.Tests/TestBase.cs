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
        protected Flipper flipper;
        private IRedisClientsManager manager;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            manager = new BasicRedisClientManager("localhost:6379");
            flipper = new Flipper(manager);           
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            flipper = null;
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
