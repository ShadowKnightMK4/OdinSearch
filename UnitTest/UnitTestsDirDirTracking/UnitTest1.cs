using DeepDirPrune;
namespace UnitTestsDirDirTracking
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void LimbCheck_AndRoot2_DoesGetLimbReturnTheFinalOneInQuestion()
        {
            DeepDirTracking testroot = new();
            DeepDirTracking FirstLevel = new DeepDirTracking();
            FirstLevel.toplevel = "C:";
            testroot.branches.Add(FirstLevel);

            var L2 = new DeepDirTracking();
            L2.toplevel = "Windows";
            FirstLevel.branches.Add(L2);

            var L3 = new DeepDirTracking();
            L3.toplevel = "System32";
            L2.branches.Add(L3);

            Assert.IsTrue(testroot.DoesDirPathExist("C:\\Windows\\System32"));
            Assert.IsTrue(ReferenceEquals(L3, testroot.GetLimb("C:\\Windows\\System32")));
        }
        [TestMethod]
        public void LimbCheck_AndRoot1_DoesGetLimbReturnOneWithTheSameNameAsTheFinalPart()
        {
            DeepDirTracking testroot = new();
            testroot.AddDirPath("C:\\Windows\\system32");
            Assert.IsTrue(testroot.DoesDirPathExist("C:\\Windows\\system32"));
            DeepDirTracking branch = testroot.GetLimb("C:\\Windows\\system32");
            Assert.IsTrue(branch.toplevel == "system32");
        }
        [TestMethod]
        public void RootCheck_DoesDirPathExist()
        {
            DeepDirTracking testroot = new();
            DeepDirTracking FirstLevel = new DeepDirTracking();
            FirstLevel.toplevel = "C:";
            testroot.branches.Add(FirstLevel);

            var L2 = new DeepDirTracking();
            L2.toplevel = "Windows";
            FirstLevel.branches.Add(L2);

            var L3 = new DeepDirTracking();
            L3.toplevel = "System32";
            L2.branches.Add(L3);

            Assert.IsTrue(testroot.DoesDirPathExist("C:\\Windows\\System32"));
        }
        [TestMethod]
        public void  RootCheck_AddDirTest()
        {
            DeepDirTracking testroot = new();
            DeepDirTracking FirstLevel = new DeepDirTracking();
            FirstLevel.toplevel = "C:";
            testroot.branches.Add(FirstLevel);

            var L2 = new DeepDirTracking();
            L2.toplevel = "Windows";
            FirstLevel.branches.Add(L2);

            var L3 = new DeepDirTracking();
            L3.toplevel = "System32";
            L2.branches.Add(L3);

            Assert.IsTrue(testroot.DoesDirPathExist("C:\\Windows\\System32"));

            DeepDirTracking testfinale = new();
            testfinale.AddDirPath("C:\\Windows\\System32");
            Assert.IsTrue(testfinale.DoesDirPathExist("C:\\Windows\\System32"));
        }

        [TestMethod]
        public void RootCheck_AddPath()
        {
            DeepDirTrackingBase test = new DeepDirTracking();
            Assert.IsTrue(test.NextDirLevel("C:\\Windows") == "C:");
            Assert.IsTrue(test.NextDirLevel("Windows\\System32\\") == "Windows");
            Assert.IsTrue(test.NextDirLevel("\\System32\\") == "System32\\");
            Assert.IsTrue(test.NextDirLevel("endpoint") == "endpoint");
        }
        [TestMethod]
        public void TestMethod1()
        {
            DeepDirTrackingBase testme = new DeepDirTracking();
            testme.AddDirPath("C:\\Windows\\system32");



            Assert.IsTrue(testme.toplevel == null);
            DeepDirTrackingBase check = null;
            check = testme["C:"];
            Assert.IsTrue(check.toplevel == "C:");
            check = check["Windows"];
            Assert.IsTrue(check.toplevel == "Windows");
            check = check["system32"];
            Assert.IsTrue(check.toplevel == "system32");
            Assert.IsTrue(check.branches.Count == 0);


            Assert.IsTrue(testme["C:"]["Windows"]["system32"] != null);

            Assert.IsTrue(testme.DoesDirPathExist("C:\\Windows\\system32\\"));
        }
    }
}