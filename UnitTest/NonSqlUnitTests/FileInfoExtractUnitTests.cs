using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System;
using OdinSearchEngine;

namespace UnitTest
{
    [TestClass]
    public class FileInfoExtractUnitTest
    {
        FileInfoExtract CanWeFolder = null;
        FileInfoExtract CanWeFile = null;
        [TestInitialize]
        public void SetupForTests()
        {
            CanWeFolder = new FileInfoExtract(Environment.GetFolderPath(Environment.SpecialFolder.Windows));
            CanWeFile = new FileInfoExtract(Assembly.GetExecutingAssembly().Location);
        }

        [TestMethod]
        public void CanWeCall_FullName()
        {
            Assert.IsNotNull(CanWeFolder);

            Assert.IsNotNull(CanWeFolder.FullName);
        }


        [TestMethod]
        public void CanWeCall_Name()
        {
            Assert.IsNotNull(CanWeFolder);

            Assert.IsNotNull(CanWeFolder.Name);
        }

        [TestMethod]
        public void CanWeCall_SizeBytes()
        {
            Assert.IsNotNull(CanWeFolder);
            Assert.AreEqual(CanWeFolder.SizeBytes, 0);

            
            try
            {
                Assert.IsNotNull(CanWeFile);
                long size = CanWeFile.SizeBytes;
            }
            catch (AssertFailedException e)
            {
                throw e;
            }

        }

        [TestMethod]
        public void CanWeCall_SizeBytesKB()
        {
            Assert.IsNotNull(CanWeFolder);
            Assert.AreEqual(CanWeFolder.SizeKB, 0);


            try
            {
                Assert.IsNotNull(CanWeFile);
                long size = CanWeFile.SizeKB;
            }
            catch (AssertFailedException e)
            {
                throw e;
            }

        }

        [TestMethod]
        public void CanWeCall_SizeMB()
        {
            Assert.IsNotNull(CanWeFolder);
            Assert.AreEqual(CanWeFolder.SizeMB, 0);


            try
            {
                Assert.IsNotNull(CanWeFile);
                long size = CanWeFile.SizeMB;
            }
            catch (AssertFailedException e)
            {
                throw e;
            }

        }


        [TestMethod]
        public void CanWeCall_SizeGB()
        {
            Assert.IsNotNull(CanWeFolder);
            Assert.AreEqual(CanWeFolder.SizeGB, 0);


            try
            {
                Assert.IsNotNull(CanWeFile);
                long size = CanWeFile.SizeGB;
            }
            catch (AssertFailedException e)
            {
                throw e;
            }

        }

        [TestMethod]
        public void CanWeCall_Exists()
        {
            Assert.IsNotNull(CanWeFolder);
            if (CanWeFolder.Exists)
            {

            }

            Assert.IsNotNull(CanWeFile);
            if (CanWeFile.Exists)
            {

            }
        }
        [TestMethod]
        public void CanWeCall_ParentLocationPath_AsString()
        {

            Assert.IsNotNull(CanWeFolder);
            try
            {
                var result = CanWeFolder.ParentLocationPath;
            }
            catch (AssertFailedException e) 
            {
                throw e;
            }
            
        }
        [TestMethod]
        public void CanWeCall_ParentLocationPath_AsDirectionInfo()
        {

            Assert.IsNotNull(CanWeFolder);
            try
            {
                var result = CanWeFolder.ParentLocation;
            }
            catch (AssertFailedException e)
            {
                throw e;
            }

        }

        [TestMethod]
        public void CanWeCall_Refresh()
        {
            Assert.IsNotNull(CanWeFile);
            CanWeFile.Refresh();

            Assert.IsNotNull(CanWeFolder);
            CanWeFolder.Refresh();
        }

        [TestMethod]
        public void CanWeInstance_BasedOnWinDir()
        {
            Assert.IsNotNull(CanWeFolder);
        }

        [TestMethod]
        public void GetSha512_self()
        {
            Assert.IsNotNull(CanWeFile);
            Assert.IsNotNull(CanWeFile.GetSha512());
        }


    }
}
