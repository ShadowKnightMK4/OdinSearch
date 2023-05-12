using OdinSearchEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    /// <summary>
    /// The Search
    /// </summary>
    [TestClass]
    public class UnitTests_SearchAnchor
    {

        /// <summary>
        /// A couple of tests branch here. 
        /// </summary>
        /// <param name="Demo">object to test</param>
        /// <param name="LocalDriveCompare">if true, we test if the Demo has only local ready drives in the root list. If false, we test if root list is empty</param>
        /// <remarks>
        /// Tests that branch here: DefaultSearchAnchor_BlankSearch_Builder_False(),  DefaultSearchAnchor_BlankSearch_Builder_True(), DefaultSearchAnchor_DoesItGetAllOnlineDrives_noremovable
        /// </remarks>
        private void SearchAnchor_BuildRoot_comparer(SearchAnchor Demo, bool LocalDriveCompare)
        {
            if (Demo != null)
            {
                if (!LocalDriveCompare)
                {
                    Assert.IsTrue(Demo.roots.Count == 0, "The Demo SearchAnchor was created with locations dispite the argument to NOT do this specified");
                }
                else
                {
                    DriveInfo[] LocalDrives = DriveInfo.GetDrives();
                    if (Demo.roots.Count == LocalDrives.LongLength)
                    {
                        foreach (DirectoryInfo D in Demo.roots)
                        {
                            DriveInfo localtest = new DriveInfo(D.FullName);
                            if (localtest.IsReady == false)
                            {
                                if ((localtest.DriveType.HasFlag(DriveType.Fixed) == false) ||
                                     (localtest.DriveType.HasFlag(DriveType.Removable) == false))
                                {
                                    Assert.Fail("SearchAnchor Default Constructor Did not get list of ready drives or a local drive's ready status has  changed. ");
                                }
                            }

                        }
                    }
                    else
                    {
                        // TODO: Something here
                    }

                }
            }
        }
        [TestCategory("Simple Anchor")]

        /// <summary>
        /// Does the constructor leave root list alone if created with false
        /// </summary>
        [TestMethod]
        public void SearchAnchorConstructChecks_IsBlankCreatedOk_boolConstructor()
        {
            SearchAnchor Demo = new SearchAnchor(false);

            SearchAnchor_BuildRoot_comparer(Demo, false);
            
        }

        [TestCategory("Simple Anchor")]
        /// <summary>
        /// Does the constructor populate the root list with all ready drives if true (same test for the default constructor for DefaultSearchAnchor_DoesItGetAllOnlineDrives_noremovable())
        /// </summary>
        public void SearchAnchorConstructChecks_IsLocalReadyDrivesCreatedOk_boolConstructor()
        {
            SearchAnchor Demo = new SearchAnchor(true);
            SearchAnchor_BuildRoot_comparer(Demo, true);
        }

        [TestCategory("Simple Anchor")]
        /// <summary>
        /// Do we have a list off all noremovable drives that are ready.  There's also a catch to ensure we don't fail if a drive is removable or if the cd/dvd drive is empty
        /// </summary>
        [TestMethod]
        public void SearchAnchorConstructChecks_IsLocalReadyDrivesCreatedOk_DefaultConstructor()
        {
            SearchAnchor Demo = new SearchAnchor();
            SearchAnchor_BuildRoot_comparer(Demo, true);


        }

        [TestCategory("Simple Anchor")]
        /// <summary>
        /// does <see cref="SearchAnchor.AddAnchor(string)"/> drop dupes
        /// </summary>

        [TestMethod]
        public void SearchAnchor_Adding_newlocation_DoesItDropDupes_DriveInfoAnchor()
        {
            SearchAnchor Demo = new SearchAnchor();
            var rootsize = Demo.roots.Count;

            if (rootsize == 0) { Assert.Fail("No ready drives for test"); }


            Demo.AddAnchor(new DriveInfo(Demo.roots[0].ToString()));

            Assert.AreEqual(rootsize, Demo.roots.Count);
        }

        [TestCategory("Simple Anchor")]
        [TestMethod]
        public void SearchANchor_Adding_newlocation_Doesitwork_StringAnchor()
        {
            string DemoLocation = "C:\\Windows";
            SearchAnchor Demo = new SearchAnchor(false);

            Demo.AddAnchor(DemoLocation);
            Assert.AreEqual(Demo.roots.Count, 1);

            Assert.IsTrue(Demo.roots[0].FullName.Equals(DemoLocation), "Did not add DemoLocation \" " + DemoLocation + "\" to root list ok");
        }

        [TestCategory("XML Anchor Handling")]
        [TestMethod]
        public void SearchAnchor_ConvertToXmlAndBack()
        {
             SearchAnchor Demo = new SearchAnchor();
            
            string xml = Demo.ToXml();

            SearchAnchor Demo2 = SearchAnchor.CreateFromXmlString(xml);

            Assert.IsTrue(Demo2 == Demo);
        }

        [TestCategory("Simple Anchor")]
        /// <summary>
        /// does <see cref="SearchAnchor.AddAnchor(string)"/> it add something that was not there already
        /// </summary>
        [TestMethod]
        public void SearchAnchor_Adding_newlocation_DoesItAddUnique_DriveInfoAnchor() 
        {
            SearchAnchor Demo = new SearchAnchor();
            if (Demo.roots.Count == 0) { Assert.Fail("No ready drives for test"); }

            DriveInfo dummy = new DriveInfo(Demo.roots[0].FullName[0].ToString());

            Demo.roots.Remove(Demo.roots[0]);

            Demo.AddAnchor(dummy);

            Assert.IsTrue( !Demo.roots.Contains(new DirectoryInfo(dummy.Name)) , "AddAnchor() failed to add new location based on drive " + dummy.Name + "after unittest removed from demo object");

        }

    }
}
