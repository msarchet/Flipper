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
           var feature = FeatureFlipper.Get("Test");

           FeatureFlipper.ActivateFeature(feature);

           Assert.IsTrue(FeatureFlipper.IsActive(feature));
       }

       [Test]
       public void Can_Set_Active_Feature_To_Deactive()
       {
           var feature = FeatureFlipper.Get("Test");

           FeatureFlipper.ActivateFeature(feature);

           Assert.IsTrue(FeatureFlipper.IsActive(feature));

           FeatureFlipper.DeactiveFeature(feature);

           Assert.IsFalse(FeatureFlipper.IsActive(feature));
       }

       [Test]
       public void Can_Define_A_Group()
       {
           FeatureFlipper.DefineGroup("test", new List<string>() { "1" });

           Assert.IsTrue(FeatureFlipper.GetGroup("test").Contains("1"));
       }


       [Test]
       public void Can_Activate_A_Group()
       {
           FeatureFlipper.DefineGroup("test", new List<string>() { "1" });

           var feature = FeatureFlipper.Get("test");

           FeatureFlipper.ActivateGroup(feature, "test");

           Assert.IsTrue(FeatureFlipper.IsActive(feature, "1"));
       }

       [Test]
       public void Can_Deactivate_An_Activte_Group()
       {
           FeatureFlipper.DefineGroup("test", new List<string>() { "1" });
           var feature = FeatureFlipper.Get("test");

           FeatureFlipper.ActivateGroup(feature, "test");

           Assert.IsTrue(FeatureFlipper.IsActive(feature, "1"));

           FeatureFlipper.DeactivateGroup(feature, "test");

           Assert.IsFalse(FeatureFlipper.IsActive(feature, "1"));
       }

       [Test]
       public void Can_Activate_New_User_In_Feature()
       {
           var feature = FeatureFlipper.Get("Test");

           FeatureFlipper.ActivateUser(feature, "1");

           Assert.IsTrue(FeatureFlipper.IsActive(feature, "1"));

       }

       [Test]
       public void Can_Deactivate_A_User_In_A_Feature()
       {
           var feature = FeatureFlipper.Get("Test");

           FeatureFlipper.ActivateUser(feature, "1");

           Assert.IsTrue(FeatureFlipper.IsActive(feature, "1"));

           FeatureFlipper.DeactivateUser(feature, "1");

           Assert.IsFalse(FeatureFlipper.IsActive(feature, "1"));
       }

       [Test]
       public void Can_Activate_A_Percentage()
       {
           var feature = FeatureFlipper.Get("Test");
           
           FeatureFlipper.ActivatePercentage(feature, 1);

           Assert.IsTrue(FeatureFlipper.IsActive(feature, "1"));
           Assert.IsFalse(FeatureFlipper.IsActive(feature, "2"));
           Assert.IsFalse(FeatureFlipper.IsActive(feature, "21"));
           Assert.IsFalse(FeatureFlipper.IsActive(feature, "31"));

           FeatureFlipper.ActivatePercentage(feature, 21);

           Assert.IsTrue(FeatureFlipper.IsActive(feature, "1"));
           Assert.IsTrue(FeatureFlipper.IsActive(feature, "102"));
           Assert.IsTrue(FeatureFlipper.IsActive(feature, "1021"));
           Assert.IsFalse(FeatureFlipper.IsActive(feature, "31"));

       }

       [Test]
       public void Can_Deactivate_A_Percentage()
       {
           var feature = FeatureFlipper.Get("Test");

           FeatureFlipper.ActivatePercentage(feature, 1);

           Assert.IsTrue(FeatureFlipper.IsActive(feature, "1"));

           FeatureFlipper.DeactivatePercentage(feature);

           Assert.IsFalse(FeatureFlipper.IsActive(feature, "1"));
       }

       [Test]
       public void Feature_Is_Always_Active_If_Percentage_Set_To_100()
       {
           var feature = FeatureFlipper.Get("Test");

           FeatureFlipper.ActivateFeature(feature);

           Assert.IsTrue(FeatureFlipper.IsActive(feature, "1"));
           Assert.IsTrue(FeatureFlipper.IsActive(feature));
           Assert.IsTrue(FeatureFlipper.IsActive(feature, "12341"));
           
       }
   }
}
