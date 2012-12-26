using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Flipper.Tests
{
   [TestFixture]
   public class FeatureTests : TestBase
   {
       [Test]
       public void Can_Activate_Feature()
       {
           var feature = flipper.Get("Test");

           flipper.ActivateFeature(feature);

           Assert.IsTrue(flipper.IsActive(feature));

       }

       [Test]
       public void Can_Set_Active_Feature_To_Deactive()
       {
           var feature = flipper.Get("Test");

           flipper.ActivateFeature(feature);

           Assert.IsTrue(flipper.IsActive(feature));

           flipper.DeactiveFeature(feature);

           Assert.IsFalse(flipper.IsActive(feature));
       }

       [Test]
       public void Can_Add_Group()
       {
           flipper.DefineGroup("test", new List<string>() { "1" });

           Assert.IsTrue(flipper.GetGroup("test").Contains("1"));
       }


       [Test]
       public void Can_Activate_A_Group()
       {
           flipper.DefineGroup("test", new List<string>() { "1" });

           var feature = flipper.Get("test");

           flipper.ActivateGroup(feature, "test");

           Assert.IsTrue(flipper.IsActive(feature, "1"));
       }

       [Test]
       public void Can_Deactivate_An_Activte_Group()
       {
           flipper.DefineGroup("test", new List<string>() { "1" });
           var feature = flipper.Get("test");

           flipper.ActivateGroup(feature, "test");

           Assert.IsTrue(flipper.IsActive(feature, "1"));

           flipper.DeactivateGroup(feature, "test");

           Assert.IsFalse(flipper.IsActive(feature, "1"));
       }

       [Test]
       public void Can_Activate_New_User_In_Feature()
       {
           var feature = flipper.Get("Test");

           flipper.ActivateUser(feature, "1");

           Assert.IsTrue(flipper.IsActive(feature, "1"));

       }
   }
}
