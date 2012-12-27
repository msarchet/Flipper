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
       public void Can_Define_A_Group()
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

       [Test]
       public void Can_Deactivate_A_User_In_A_Feature()
       {
           var feature = flipper.Get("Test");

           flipper.ActivateUser(feature, "1");

           Assert.IsTrue(flipper.IsActive(feature, "1"));

           flipper.DeactivateUser(feature, "1");

           Assert.IsFalse(flipper.IsActive(feature, "1"));
       }

       [Test]
       public void Can_Activate_A_Percentage()
       {
           var feature = flipper.Get("Test");
           
           flipper.ActivatePercentage(feature, 1);

           Assert.IsTrue(flipper.IsActive(feature, "1"));
           Assert.IsFalse(flipper.IsActive(feature, "2"));
           Assert.IsFalse(flipper.IsActive(feature, "21"));
           Assert.IsFalse(flipper.IsActive(feature, "31"));

           flipper.ActivatePercentage(feature, 21);

           Assert.IsTrue(flipper.IsActive(feature, "1"));
           Assert.IsTrue(flipper.IsActive(feature, "102"));
           Assert.IsTrue(flipper.IsActive(feature, "1021"));
           Assert.IsFalse(flipper.IsActive(feature, "31"));

       }

       [Test]
       public void Can_Deactivate_A_Percentage()
       {
           var feature = flipper.Get("Test");

           flipper.ActivatePercentage(feature, 1);

           Assert.IsTrue(flipper.IsActive(feature, "1"));

           flipper.DeactivatePercentage(feature);

           Assert.IsFalse(flipper.IsActive(feature, "1"));
       }

       [Test]
       public void Feature_Is_Always_Active_If_Percentage_Set_To_100()
       {
           var feature = flipper.Get("Test");

           flipper.ActivateFeature(feature);

           Assert.IsTrue(flipper.IsActive(feature, "1"));
           Assert.IsTrue(flipper.IsActive(feature));
           Assert.IsTrue(flipper.IsActive(feature, "12341"));
           
       }
   }
}
