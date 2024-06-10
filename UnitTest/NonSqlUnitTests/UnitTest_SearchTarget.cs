using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OdinSearchEngine;

namespace NonSqlUnitTests
{
    [TestClass]
    public class UnitTest_SearchTarget
    {
        [TestMethod]
        public void SearchTarget_AnyFileTest_TriggrAndCompare()
        {
            // anyfile original
            var SearchTarget = new SearchTarget();
            SearchTarget.FileName.Add(SearchTarget.MatchAnyFileName);
            SearchTarget.DirectoryMatching = SearchTarget.MatchStyleString.Skip;
            SearchTarget.FileNameMatching = SearchTarget.MatchStyleString.Skip;
            SearchTarget.AttributeMatching1 = SearchTarget.AttributeMatching2 = FileAttributes.Normal;
            SearchTarget.AttribMatching1Style = SearchTarget.AttribMatching2Style = SearchTarget.MatchStyleFileAttributes.Skip;
            SearchTarget.AccessAnchorCheck1 = SearchTarget.AccessAnchorCheck2 = SearchTarget.MatchStyleDateTime.Disable;
            SearchTarget.WriteAnchorCheck1 = SearchTarget.WriteAnchorCheck2 = SearchTarget.MatchStyleDateTime.Disable;
            SearchTarget.CreationAnchorCheck1 = SearchTarget.CreationAnchorCheck2 = SearchTarget.MatchStyleDateTime.Disable;
            SearchTarget.CheckFileSize = false;
            SearchTarget.DirectoryMatching = SearchTarget.MatchStyleString.Skip;

            Assert.IsNotNull(SearchTarget.AllFiles);

            Assert.AreEqual(SearchTarget, SearchTarget.AllFiles);

        }
        /// <summary>
        /// Will saving and reloading the xml from a file work?
        /// </summary>
        [TestMethod]
        public void SearchTargetToXmlAndBack_AS_FILE()
        {
            SearchTarget target = new SearchTarget();
            SearchTarget Target2;
            target.DirectoryMatching = SearchTarget.MatchStyleString.Skip;
            target.FileName.Add("*.txt");
            target.FileName.Add("*.dll");
            target.DirectoryPath.Add("Windows");
            target.DirectoryMatching = SearchTarget.MatchStyleString.Invert;

            string xml = target.ToXml();

            File.WriteAllLines("C:\\Dummy\\TestFile.xml", new string[1] { xml });
            Target2 = SearchTarget.CreateFromXmlString(xml); ;

            Assert.IsTrue(target == Target2);
        }


        /// <summary>
        /// will saving and reloading the xml from a string/stream work?
        /// </summary>
        [TestMethod]
        public void SearchTarget_ToXML_AND_BACK_as_Stream()
        {
            string xml;
            SearchTarget searchTarget = new SearchTarget();
            SearchTarget searchTarget2;

            searchTarget.CheckFileSize = true;
            searchTarget.AccessAnchorCheck1 = SearchTarget.MatchStyleDateTime.NoLaterThanThis;
            searchTarget.AccessAnchorCheck2 = SearchTarget.MatchStyleDateTime.NoEarlierThanThis;
            searchTarget.AccessAnchor = new DateTime(992, 12, 4);
            searchTarget.AccessAnchor2 = new DateTime(613, 11, 4);

            searchTarget.WriteAnchor = new DateTime(120, 4, 15);
            searchTarget.WriteAnchor2 = new DateTime(440, 2, 28);
            searchTarget.WriteAnchorCheck1 = SearchTarget.MatchStyleDateTime.NoEarlierThanThis;
            searchTarget.WriteAnchorCheck2 = SearchTarget.MatchStyleDateTime.NoLaterThanThis;


            searchTarget.CreationAnchor = new DateTime(100, 10, 10);
            searchTarget.CreationAnchor2 = new DateTime(200, 12, 30);
            searchTarget.CreationAnchorCheck1 = SearchTarget.MatchStyleDateTime.NoEarlierThanThis;
            searchTarget.CreationAnchorCheck2 = SearchTarget.MatchStyleDateTime.NoLaterThanThis;



            searchTarget.FileSizeMin = 14;
            searchTarget.FileSizeMax = 32;

            searchTarget.CheckFileSize = true;

            searchTarget.AttribMatching1Style = SearchTarget.MatchStyleFileAttributes.MatchAny | SearchTarget.MatchStyleFileAttributes.Exacting;
            searchTarget.AttributeMatching1 = System.IO.FileAttributes.System | System.IO.FileAttributes.Temporary;

            searchTarget.AttribMatching2Style = SearchTarget.MatchStyleFileAttributes.Invert | SearchTarget.MatchStyleFileAttributes.MatchAll;
            searchTarget.AttributeMatching2 = System.IO.FileAttributes.Compressed | System.IO.FileAttributes.Hidden;


            searchTarget.FileName.Add("*.dll");
            searchTarget.FileName.Add("*.exe");
            searchTarget.FileName.Add("*.qwe");

            searchTarget.DirectoryPath.Add("*Windows");
            searchTarget.DirectoryPath.Add("*System");


            searchTarget.FileNameMatching = SearchTarget.MatchStyleString.MatchAny;
            searchTarget.DirectoryMatching = SearchTarget.MatchStyleString.MatchAny;


            xml = searchTarget.ToXml();

            searchTarget2 = SearchTarget.CreateFromXmlString(xml);


            Assert.IsTrue(searchTarget == searchTarget2);


        }
    }
}
