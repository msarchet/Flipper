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
           var feature = actuator.Get("Test");

           actuator.ActivateFeature(feature);

           Assert.IsTrue(actuator.IsActive(feature));
       }

       [Test]
       public void Can_Set_Active_Feature_To_Deactive()
       {
           var feature = actuator.Get("Test");

           actuator.ActivateFeature(feature);

           Assert.IsTrue(actuator.IsActive(feature));

           actuator.DeactiveFeature(feature);

           Assert.IsFalse(actuator.IsActive(feature));
       }

       [Test]
       public void Can_Define_A_Group()
       {
           actuator.DefineGroup("test", new List<string>() { "1" });

           Assert.IsTrue(actuator.GetGroup("test").Contains("1"));
       }


       [Test]
       public void Can_Activate_A_Group()
       {
           actuator.DefineGroup("test", new List<string>() { "1" });

           var feature = actuator.Get("test");

           actuator.ActivateGroup(feature, "test");

           Assert.IsTrue(actuator.IsActive(feature, "1"));
       }

       [Test]
       public void Can_Deactivate_An_Activte_Group()
       {
           actuator.DefineGroup("test", new List<string>() { "1" });
           var feature = actuator.Get("test");

           actuator.ActivateGroup(feature, "test");

           Assert.IsTrue(actuator.IsActive(feature, "1"));

           actuator.DeactivateGroup(feature, "test");

           Assert.IsFalse(actuator.IsActive(feature, "1"));
       }

       [Test]
       public void Can_Activate_New_User_In_Feature()
       {
           var feature = actuator.Get("Test");

           actuator.ActivateUser(feature, "1");

           Assert.IsTrue(actuator.IsActive(feature, "1"));

       }

       [Test]
       public void Can_Deactivate_A_User_In_A_Feature()
       {
           var feature = actuator.Get("Test");

           actuator.ActivateUser(feature, "1");

           Assert.IsTrue(actuator.IsActive(feature, "1"));

           actuator.DeactivateUser(feature, "1");

           Assert.IsFalse(actuator.IsActive(feature, "1"));
       }

       [Test]
       public void Can_Activate_A_Percentage()
       {
           var feature = actuator.Get("Test");
           
           actuator.ActivatePercentage(feature, 1);

           Assert.IsTrue(actuator.IsActive(feature, "1"));
           Assert.IsFalse(actuator.IsActive(feature, "2"));
           Assert.IsFalse(actuator.IsActive(feature, "21"));
           Assert.IsFalse(actuator.IsActive(feature, "31"));

           actuator.ActivatePercentage(feature, 21);

           Assert.IsTrue(actuator.IsActive(feature, "1"));
           Assert.IsTrue(actuator.IsActive(feature, "102"));
           Assert.IsTrue(actuator.IsActive(feature, "1021"));
           Assert.IsFalse(actuator.IsActive(feature, "31"));

       }

       [Test]
       public void Can_Deactivate_A_Percentage()
       {
           var feature = actuator.Get("Test");

           actuator.ActivatePercentage(feature, 1);

           Assert.IsTrue(actuator.IsActive(feature, "1"));

           actuator.DeactivatePercentage(feature);

           Assert.IsFalse(actuator.IsActive(feature, "1"));
       }

       [Test]
       public void Feature_Is_Always_Active_If_Percentage_Set_To_100()
       {
           var feature = actuator.Get("Test");

           actuator.ActivateFeature(feature);

           Assert.IsTrue(actuator.IsActive(feature, "1"));
           Assert.IsTrue(actuator.IsActive(feature));
           Assert.IsTrue(actuator.IsActive(feature, "12341"));
           
       }
   }
}
