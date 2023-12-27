using FileInventoryConsole;
using FileIventoryConsole;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Bson;

namespace ConsoleAppUnitTests
{
    [TestClass]
    public class ConsoleApp_ArgHandlingClass_UnitTests
    {
        /*
         * Keeping straight the link between the unit tests are arghandling class.
         * 
         * The DateTime varients always set  the positive one as the #1 var - exmaple CreationAnchor1 and the negative one the #2
         * varirble  - CreateAnchor2 along with  the appropriate checker enum.
         * 
         * Although the API/Engine (OdinSearch) supports multuple FileName and DirectoryName compares, we aren't targeting them for 
         * the front end which this unit test collection does.
         * 
         * Important!!!!!
         *          THE argument parser is case insentivie for testing and assingign its SearchTarget/SearchAnchor stuff.
         *          
         *          Ensure that when comparing strings in this unit test, run it to ToLower() first.
         * naming conventio0n
         * 
         * argumentflg_what_it_does
         */
        [TestMethod]
        public void default_arg_test()
        {
            ArgHandling testme = new ArgHandling();
            testme.DoTheThing(new string[] { });
        }

        [TestMethod]
        public void Specialarg_anyfile_set()
        {
            ArgHandling testme = new ArgHandling();
            testme.DoTheThing(new string[] { "/anyfile" });
            Assert.IsTrue(testme.was_anyfile_flag_set);
        }

        [TestMethod]
        public void specialarg_anywhere_set()
        {
            ArgHandling testme = new ArgHandling();
            testme.DoTheThing(new string[] { "/anywhere" });
            Assert.IsTrue(testme.was_wholemachine_flag_set); ;
        }

        [TestMethod]
        public void AnchorArg_Set_NoSpace()
        {
            string loc = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            ArgHandling testme = new ArgHandling();
            testme.DoTheThing(new string[] { "/anchor=" + loc});

            Assert.AreEqual(testme.SearchAnchor.roots[0].ToString(), loc);
        }

        [TestMethod]
        public void AnchorArg_Set_Space()
        {
            string loc = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            ArgHandling testme = new ArgHandling();
            testme.DoTheThing(new string[] { "/anchor=" + loc });

            Assert.AreEqual(testme.SearchAnchor.roots[0].ToString(), loc);
        }
        [TestMethod]
        public void AnchorArg_Set_NoSpaceQuoted()
        {
            string loc = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            ArgHandling testme = new ArgHandling();
            testme.DoTheThing(new string[] { "/anchor=" + '\"' + loc + "\"" });
            loc = ArgHandling.Trim(loc);
            Assert.AreEqual(testme.SearchAnchor.roots[0].ToString(), loc);
        }

        [TestMethod]
        public void AnchorArg_Set_SpaceQuoted()
        {
            string loc = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            ArgHandling testme = new ArgHandling();
            testme.DoTheThing(new string[] { "/anchor=" + '\"' + loc + "\"" });
            loc = ArgHandling.Trim(loc);
            Assert.AreEqual(testme.SearchAnchor.roots[0].ToString(), loc);
        }
        [TestMethod]
        public void Plugins_arg_SetUnmanaged()
        {
            ArgHandling testme = new ArgHandling();
            testme.DoTheThing(new string[] { "-f" });
            Assert.IsTrue(testme.AllowUntrustedPlugin);
        }
        [TestMethod]
        public void Plugins_arg_AllowManaged_abs_path_no_quote_no_space()
        {
            string loc = "C:\\Plugins\\NetPlugin.dll";
            ArgHandling testme = new ArgHandling();
            testme.DoTheThing(new string[] { "/managed=" + loc});
            loc = ArgHandling.Trim(loc);
            Assert.AreEqual(testme.ExternalPluginDll, loc);
            Assert.AreEqual(testme.ExternalPluginName, string.Empty);
        }

        [TestMethod]
        public void Plugins_arg_AllowManaged_abs_path_quote_space()
        {
            string loc = "\"C:\\Plugins and Stuff\\NetPlugin.dll\"";
            ArgHandling testme = new ArgHandling();
            testme.DoTheThing(new string[] { "/managed="+loc });
            loc = ArgHandling.Trim(loc);
            Assert.AreEqual(testme.ExternalPluginDll, loc);
            Assert.AreEqual(testme.ExternalPluginName, string.Empty);
        }

        [TestMethod]
        public void Plugins_arg_AllowManaged_abs_path_no_quote_space()
        {
            string loc = "C:\\Plugins and Stuff\\NetPlugin.dll";
            ArgHandling testme = new ArgHandling();
            testme.DoTheThing(new string[] { "/managed=" + loc });
            loc = ArgHandling.Trim(loc);
            Assert.AreEqual(testme.ExternalPluginDll, loc);
            Assert.AreEqual(testme.ExternalPluginName, string.Empty);
        }

        [TestMethod]
        public void Plugins_arg_AllowNative_abs_path_no_quote_no_space()
        {
            string loc = "C:\\Plugins\\CBasedPlugin.dll";
            ArgHandling testme = new ArgHandling();
            testme.DoTheThing(new string[] { "/plugin=" + loc });
            loc = ArgHandling.Trim(loc);
            Assert.AreEqual(testme.ExternalPluginDll, loc);
        }

        [TestMethod]
        public void Plugins_arg_AllowNative_abs_path_quote_space()
        {
            string loc = "\"C:\\Plugins and Stuff\\CBasedPlugin.dll\"";
            ArgHandling testme = new ArgHandling();
            testme.DoTheThing(new string[] { "/plugin=" + loc });
            loc = ArgHandling.Trim(loc);
            Assert.AreEqual(testme.ExternalPluginDll, loc);
        }

        [TestMethod]
        public void Plugins_arg_AllowNative_abs_path_no_quote_space()
        {
            string loc = "C:\\Plugins and Stuff\\CBasedPlugin.dll";
            ArgHandling testme = new ArgHandling();
            testme.DoTheThing(new string[] { "/plugin=" + loc });
            loc = ArgHandling.Trim(loc);
            Assert.AreEqual(testme.ExternalPluginDll, loc);
        }



        /// <summary>
        /// test for correct /outstream=stdout handling
        /// </summary>
        [TestMethod]
        public void outstream_arg_set_stdout()
        {
            ArgHandling testme = new ArgHandling();
            testme.DoTheThing(new string[] {"/outstream=stdout" });
            Assert.IsTrue(testme.TargetStream == null);
            Assert.IsTrue(testme.TargetStreamHandling == ArgHandling.ConsoleLines.Stdout);
        }


        /// <summary>
        /// test for correct /outstream=stderr handling
        /// </summary>
        [TestMethod]
        public void outstream_arg_set_stderr()
        {
            ArgHandling testme = new ArgHandling();
            testme.DoTheThing(new string[] { "/outstream=stderr" });
            Assert.IsTrue(testme.TargetStream == null);
            Assert.IsTrue(testme.TargetStreamHandling == ArgHandling.ConsoleLines.Stderr);
        }


        /// <summary>
        /// test for correct /outstream=random_file.txt
        /// </summary>
        [TestMethod]
        public void outstream_arg_set_stream_no_quotes()
        {
            string filename = Path.GetTempFileName();
            ArgHandling testme = new ArgHandling();
            testme.DoTheThing(new string[] { "/outstream=" + filename });
            Assert.IsTrue( (testme.TargetStream != null) && (testme.TargetStream.Name.ToLower() == filename.ToLower()));
            Assert.IsTrue(testme.TargetStreamHandling == ArgHandling.ConsoleLines.NoRedirect);
            
        }

        /// <summary>
        /// test for correct /outstream="C:\\Windows\\random.tmp"  
        /// </summary>
        [TestMethod]
        public void outstream_arg_set_stream_quotes()
        {
            string filename = Path.GetTempFileName();
            ArgHandling testme = new ArgHandling();
            testme.DoTheThing(new string[] { "/outstream=\"" + filename +"\""});
            Assert.IsTrue((testme.TargetStream != null) && (testme.TargetStream.Name.ToLower() == filename.ToLower()));
            Assert.IsTrue(testme.TargetStreamHandling == ArgHandling.ConsoleLines.NoRedirect);
            
        }
        /// <summary>
        /// does this parse the string to set to unicode text and does it assign ok. 
        /// 
        /// </summary>
        [TestMethod]
        public void outformat_arg_test_for_unicode_assign()
        {
            ArgHandling testme_unicode = new ArgHandling();
            Assert.IsTrue(testme_unicode.DoTheThing(new string[] { "/outformat=unicode" }));

            Assert.AreEqual(testme_unicode.UserFormat, ArgHandling.TargetFormat.Unicode);
        }

        /// <summary>
        /// does this parse the string for excel and does in assign ok
        /// </summary>
        [TestMethod]
        public void outformat_arg_test_for_excel_assign()
        {
            ArgHandling testme_unicode = new ArgHandling();
            Assert.IsTrue(testme_unicode.DoTheThing(new string[] { "/outformat=cvsfile" }));

            Assert.AreEqual(testme_unicode.UserFormat, ArgHandling.TargetFormat.CVSFile);
        }


        /// <summary>
        /// does this regonize that this is a bad argument
        /// </summary>
        [TestMethod]
        public void outformat_arg_test_for_error_assign()
        {
            ArgHandling testme_unicode = new ArgHandling();
            Assert.IsFalse(testme_unicode.DoTheThing(new string[] { "/outformat=donotusethisvalue" }));

            Assert.AreEqual(testme_unicode.UserFormat, ArgHandling.TargetFormat.Error);
        }

        /// <summary>
        /// does this regonize that this is a bad argument
        /// </summary>
        [TestMethod]
        public void outformat_arg_test_for_blank_assign()
        {
            ArgHandling testme_unicode = new ArgHandling();
            Assert.IsFalse(testme_unicode.DoTheThing(new string[] { "/outformat=" }));

            Assert.AreEqual(testme_unicode.UserFormat, ArgHandling.TargetFormat.Error);
        }

        /// <summary>
        /// test the variouis /filecompare=int that we support. Reject bad ones.
        /// </summary>
        /// <remarks>code is currently the same as <see cref="FULLcompare_set_general_as_int"/></remarks> but different targets
        [TestMethod]
        public void filecompare_set_general_as_int()
        {

            ArgHandling testme_unicode = new ArgHandling();
            // do we know MatchStyleString.MatchAny?
            Assert.IsTrue(testme_unicode.DoTheThing(new string[] { "/filecompare=1" }));

            Assert.AreEqual((int)testme_unicode.SearchTarget.FileNameMatching, 1);


            // do we know MatchStyleString.MatchAll?
            testme_unicode = new ArgHandling();
            Assert.IsTrue(testme_unicode.DoTheThing(new string[] { "/filecompare=2" }));

            Assert.AreEqual((int)testme_unicode.SearchTarget.FileNameMatching, 2);

            // do we know MatchStyleString.Invert?
            testme_unicode = new ArgHandling();
            Assert.IsTrue(testme_unicode.DoTheThing(new string[] { "/filecompare=4" }));

            Assert.AreEqual((int)testme_unicode.SearchTarget.FileNameMatching, 4);

            // this particulater instance ReservedUnused value (8) should be catched and returned as failure
            testme_unicode = new ArgHandling();

            // do we know MatchStyleString.ReservedUnused and fail it?
            Assert.IsFalse(testme_unicode.DoTheThing(new string[] { "/filecompare=8" }));
            // this test not needed
            //Assert.AreEqual((int)testme_unicode.SearchTarget.DirectoryMatching, 8);


            // do we know MatchStringStyle.Skip?
            testme_unicode = new ArgHandling();
            Assert.IsTrue(testme_unicode.DoTheThing(new string[] { "/filecompare=16" }));

            Assert.AreEqual((int)testme_unicode.SearchTarget.FileNameMatching, 16);

            // do we know MatchStyleString.CaseImportant?
            testme_unicode = new ArgHandling();
            Assert.IsTrue(testme_unicode.DoTheThing(new string[] { "/filecompare=32" }));

            Assert.AreEqual((int)testme_unicode.SearchTarget.FileNameMatching, 32);

            // do we fail a random numver
            testme_unicode = new ArgHandling();
            Assert.IsFalse(testme_unicode.DoTheThing(new string[] { "/filecompare=42130" }));

            // do we fail a blank argument?
            testme_unicode = new ArgHandling();
            Assert.IsFalse(testme_unicode.DoTheThing(new string[] { "/filecompare=" }));

            // do we know  something the MatchStyle.ReservedUnused Flag and fail it?
            testme_unicode = new ArgHandling();
            Assert.IsFalse(testme_unicode.DoTheThing(new string[] { "/filecompare=24" }));

            //Assert.AreEqual((int)testme_unicode.SearchTarget.DirectoryMatching, 24);

            // do we know a mix of MatchStyleString.Skip and Invert?
            testme_unicode = new ArgHandling();
            Assert.IsTrue(testme_unicode.DoTheThing(new string[] { "/filecompare=20" }));

            Assert.AreEqual((int)testme_unicode.SearchTarget.FileNameMatching, 20);


            // do we fail a random set of ints
            testme_unicode = new ArgHandling();
            Assert.IsFalse(testme_unicode.DoTheThing(new string[] { "/filecompare=12398013289058319078538471-431290=421" }));

            // do we fail a non int value that we aren't checking for.
            testme_unicode = new ArgHandling();
            Assert.IsFalse(testme_unicode.DoTheThing(new string[] { "/filecompare=itshighnoon" }));
        }


        /// <summary>
        /// Test for  /fullcompare=int compbintions and reject some invalid ones 
        /// </summary>
        /// <remarks>code is alot like <see cref="filecompare_set_general_as_int"/></remarks>
        [TestMethod]
        public void FULLcompare_set_general_as_int()
        {
            ArgHandling testme_unicode = new ArgHandling();
            // do we know MatchStyleString.MatchAny?
            Assert.IsTrue(testme_unicode.DoTheThing(new string[] { "/fullcompare=1" }));

            Assert.AreEqual((int)testme_unicode.SearchTarget.DirectoryMatching, 1);


            // do we know MatchStyleString.MatchAll?
            testme_unicode = new ArgHandling();
            Assert.IsTrue(testme_unicode.DoTheThing(new string[] { "/fullcompare=2" }));

            Assert.AreEqual((int)testme_unicode.SearchTarget.DirectoryMatching, 2);

            // do we know MatchStyleString.Invert?
            testme_unicode = new ArgHandling();
            Assert.IsTrue(testme_unicode.DoTheThing(new string[] { "/fullcompare=4" }));

            Assert.AreEqual((int)testme_unicode.SearchTarget.DirectoryMatching, 4);

            // this particulater instance ReservedUnused value (8) should be catched and returned as failure
            testme_unicode = new ArgHandling();
        
            // do we know MatchStyleString.ReservedUnused and fail it?
            Assert.IsFalse(testme_unicode.DoTheThing(new string[] { "/fullcompare=8" }));
            // this test not needed
            //Assert.AreEqual((int)testme_unicode.SearchTarget.DirectoryMatching, 8);


            // do we know MatchStringStyle.Skip?
            testme_unicode = new ArgHandling();
            Assert.IsTrue(testme_unicode.DoTheThing(new string[] { "/fullcompare=16" }));

            Assert.AreEqual((int)testme_unicode.SearchTarget.DirectoryMatching, 16);

            // do we know MatchStyleString.CaseImportant?
            testme_unicode = new ArgHandling();
            Assert.IsTrue(testme_unicode.DoTheThing(new string[] { "/fullcompare=32" }));

            Assert.AreEqual((int)testme_unicode.SearchTarget.DirectoryMatching, 32);

            // do we fail a random numver
            testme_unicode = new ArgHandling();
            Assert.IsFalse(testme_unicode.DoTheThing(new string[] { "/fullcompare=42130" }));

            // do we fail a blank argument?
            testme_unicode = new ArgHandling();
            Assert.IsFalse(testme_unicode.DoTheThing(new string[] { "/fullcompare=" }));

            // do we know  something the MatchStyle.ReservedUnused Flag and fail it?
            testme_unicode = new ArgHandling();
            Assert.IsFalse(testme_unicode.DoTheThing(new string[] { "/fullcompare=24" }));

            //Assert.AreEqual((int)testme_unicode.SearchTarget.DirectoryMatching, 24);

            // do we know a mix of MatchStyleString.Skip and Invert?
            testme_unicode = new ArgHandling();
            Assert.IsTrue(testme_unicode.DoTheThing(new string[] { "/fullcompare=20" }));

            Assert.AreEqual((int)testme_unicode.SearchTarget.DirectoryMatching, 20);


            // do we fail a random set of ints
            testme_unicode = new ArgHandling();
            Assert.IsFalse(testme_unicode.DoTheThing(new string[] { "/fullcompare=12398013289058319078538471-431290=421" }));

            // do we fail a non int value that we aren't checking for.
            testme_unicode = new ArgHandling();
            Assert.IsFalse(testme_unicode.DoTheThing(new string[] { "/fullcompare=bobdosomething" }));
        }


        /// <summary>
        /// test if the /notcreatedbefore=date can be parsed and assinged correctly.
        /// </summary>
        [TestMethod] 
        public void datetime_creation_arg_not_before_steampunk()
        {
            ArgHandling test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/notcreatedbefore=Jan 1, 1800" }));
            Assert.IsTrue((test.SearchTarget.CreationAnchor.Month == 1) &&
                          (test.SearchTarget.CreationAnchor.Day == 1) && 
                          (test.SearchTarget.CreationAnchor.Year == 1800));


            Assert.IsTrue(test.SearchTarget.CreationAnchorCheck1 == OdinSearchEngine.SearchTarget.MatchStyleDateTime.NoEarlierThanThis);

        }

        /// <summary>
        /// test if /notcreatedafter=date can be parsed and assigned ok
        /// </summary>
        [TestMethod]
        public void datetime_creation_arg_not_after_starfleet()
        {
            ArgHandling test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/notcreatedafter=May 6, 2457" }));
            Assert.IsTrue((test.SearchTarget.CreationAnchor2.Month == 5) &&
                          (test.SearchTarget.CreationAnchor2.Day == 6) &&
                          (test.SearchTarget.CreationAnchor2.Year == 2457));

            Assert.IsTrue(test.SearchTarget.CreationAnchorCheck2 == OdinSearchEngine.SearchTarget.MatchStyleDateTime.NoLaterThanThis);
        }


        /// <summary>
        /// test if /nolastmodifiedbefore can be parsed and asinged ok.
        /// </summary>
        [TestMethod]
        public void datetime_modified_arg_not_before_steampunk()
        {
            ArgHandling test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/nolastmodifiedbefore=Aug 8, 1842" }));
            Assert.IsTrue((test.SearchTarget.WriteAnchor.Month == 8) &&
                          (test.SearchTarget.WriteAnchor.Day == 8) &&
                          (test.SearchTarget.WriteAnchor.Year == 1842));
            Assert.IsTrue(test.SearchTarget.WriteAnchorCheck1 == OdinSearchEngine.SearchTarget.MatchStyleDateTime.NoEarlierThanThis);
        }

        /// <summary>
        /// test if /lastmodifiedbefore can be parsed and assinged ok
        /// </summary>
        [TestMethod]
        public void datetime_modified_arg_not_after_starfleet()
        {
            ArgHandling test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/lastmodifiedbefore=Dec 25, 2105" }));
            Assert.IsTrue((test.SearchTarget.WriteAnchor2.Month == 12) &&
                          (test.SearchTarget.WriteAnchor2.Day == 25) &&
                          (test.SearchTarget.WriteAnchor2.Year == 2105));

            Assert.IsTrue(test.SearchTarget.WriteAnchorCheck2 == OdinSearchEngine.SearchTarget.MatchStyleDateTime.NoLaterThanThis);
        }




        /// <summary>
        /// test if /lastaccessedbefore can be parsed and assigned ok
        /// </summary>
        [TestMethod]
        public void datetime_accessed_arg_not_before_steampunk()
        {
            ArgHandling test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/lastaccessedbefore=Sep 11, 1776" }));
            Assert.IsTrue((test.SearchTarget.AccessAnchor.Month == 9) &&
                          (test.SearchTarget.AccessAnchor.Day == 11) &&
                          (test.SearchTarget.AccessAnchor.Year == 1776));
            Assert.IsTrue(test.SearchTarget.AccessAnchorCheck1 == OdinSearchEngine.SearchTarget.MatchStyleDateTime.NoEarlierThanThis);

        }

        /// <summary>
        /// test if the /nolastaccessedbefore can be set and parsed ok
        /// </summary>
        [TestMethod]
        public void datetime_accessed_arg_not_after_starfleet()
        {
            ArgHandling test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/nolastaccessedbefore=Dec 27, 2500" }));
            Assert.IsTrue((test.SearchTarget.AccessAnchor2.Month == 12) &&
                          (test.SearchTarget.AccessAnchor2.Day == 27) &&
                          (test.SearchTarget.AccessAnchor2.Year == 2500));
            Assert.IsTrue(test.SearchTarget.AccessAnchorCheck2 == OdinSearchEngine.SearchTarget.MatchStyleDateTime.NoLaterThanThis);
        }





        /// <summary>
        /// can /fullname handle a path with no wildcard and spaces
        /// </summary>
        [TestMethod]
        public void FULLNAME_arg_setno_wildcard_no_space_no_quote()
        {
            const string checkme = "C:\\Windows\\system32\\cmd.exe";
            string lcheck = checkme;
            ArgHandling test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fullname=" + checkme }));

            Assert.IsTrue((test.SearchTarget.DirectoryPath.Count > 0) && (test.SearchTarget.DirectoryPath[0].Equals(lcheck)));
        }

        /// <summary>
        /// can /fullname handle a wildcard with no spaces
        /// </summary>
        [TestMethod]
        public void FULLNAME_arg_set_yes_wildcard_no_space_no_quote()
        {

            const string checkme = "C:\\Windows\\system32\\*.exe";
            string lcheck = checkme;
            ArgHandling test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fullname=" + checkme }));


            Assert.IsTrue((test.SearchTarget.DirectoryPath.Count > 0) && (test.SearchTarget.DirectoryPath[0].Equals(lcheck)));
        }


        /// <summary>
        /// can /fullname handle a space?
        /// </summary>
        [TestMethod]
        public void FULLNAME_arg_set_yes_wildcard_yes_space_no_quote()
        {
            const string checkme = "C:\\windows\\system32\\* .exe";
            string lcheck = checkme;
            ArgHandling test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fullname=" + checkme }));

            

            Assert.IsTrue((test.SearchTarget.DirectoryPath.Count > 0) && (test.SearchTarget.DirectoryPath[0].Equals(lcheck)));
        }



        /// <summary>
        /// can /fullname handle a wildcard, space and be quoted?
        /// </summary>
        [TestMethod]
        public void FULLNAME_arg_set_yes_wildcard_yes_space_yes_quote()
        {
            const string checkme = "\"C:\\windows\\system32\\* 32.dll\"";
            // note quotes are trimmed off before assigning. don't forget this call
            string lcheck = ArgHandling.Trim(checkme);
            ArgHandling test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fullname=" + checkme }));



            
            Assert.IsTrue((test.SearchTarget.DirectoryPath.Count > 0) && (test.SearchTarget.DirectoryPath[0].Equals(lcheck)));
        }


        /// <summary>
        /// can /fullname handle being quoted with no spaces and a wildcard
        /// </summary>
        [TestMethod]
        public void FULLNAME_arg_set_yes_wildcard_no_space_yes_quote()
        {
            const string checkme = "\"C:\\testapps\\32*.exe\"";
            // note quotes are trimmed off before assigment, don't forget this call.
            string lcheck = ArgHandling.Trim(checkme);
            ArgHandling test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fullname=" + checkme }));

            Assert.IsTrue((test.SearchTarget.DirectoryPath.Count > 0) && (test.SearchTarget.DirectoryPath[0].Equals(lcheck)));
        }


        /// <summary>
        /// can /fullname handle no wildcard, no space and being quoted
        /// </summary>
        [TestMethod]
        public void FULLNAME_arg_set_no_wildcard_no_space_yes_quote()
        {
            const string checkme = "\"C:\\Stuff\\cool.jpg\"";
            // note quotes are trimmed off before assigment, don't forget this call.
            string lcheck = ArgHandling.Trim(checkme);
            ArgHandling test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fullname=" + checkme }));

            Assert.IsTrue((test.SearchTarget.DirectoryPath.Count > 0) && (test.SearchTarget.DirectoryPath[0].Equals(lcheck)));
        }




        

        /// <summary>
        /// can /filename handle no spaces, no wildcard
        /// </summary>
        [TestMethod]
        public void filname_arg_setno_wildcard_no_space_no_quote()
        {
            ArgHandling test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/filename=cmd.exe" }));

            Assert.IsTrue( (test.SearchTarget.FileName.Count > 0) && (test.SearchTarget.FileName[0].Equals("cmd.exe"))); 
        }

        /// <summary>
        /// can /filename handle a wildcard, no spaces
        /// </summary>
        [TestMethod]
        public void filename_arg_set_yes_wildcard_no_space_no_quote()
        {
            ArgHandling test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/filename=*.exe" }));

            Assert.IsTrue((test.SearchTarget.FileName.Count > 0) && (test.SearchTarget.FileName[0].Equals("*.exe")));
        }


        /// <summary>
        /// can /filename handle a wildcard, and a space ?
        /// </summary>
        [TestMethod]
        public void filename_arg_set_yes_wildcard_yes_space_no_quote()
        {
            ArgHandling test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/filename=* .exe" }));

            Assert.IsTrue((test.SearchTarget.FileName.Count > 0) && (test.SearchTarget.FileName[0].Equals("* .exe")));
        }



        /// <summary>
        /// can /fiename handle a wildcard, a space and being in quotes
        /// </summary>
        [TestMethod]
        public void filename_arg_set_yes_wildcard_yes_space_yes_quote()
        {
            ArgHandling test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/filename=\" 1 *.exe\"" }));

            Assert.IsTrue((test.SearchTarget.FileName.Count > 0) && (test.SearchTarget.FileName[0].Equals("1 *.exe")));
        }


        /// <summary>
        /// can /filename handle being quoted and a wildcard
        /// </summary>
        [TestMethod]
        public void filename_arg_set_yes_wildcard_no_space_yes_quote()
        {
            ArgHandling test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/filename=\"32*.exe\"" }));

            Assert.IsTrue((test.SearchTarget.FileName.Count > 0) && (test.SearchTarget.FileName[0].Equals("32*.exe")));
        }


        /// <summary>
        /// can /filename handle no wildcard nor space but quotes
        /// </summary>
        [TestMethod]
        public void filename_arg_set_no_wildcard_no_space_yes_quote()
        {
            ArgHandling test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/filename=\"hello.exe\"" }));

            Assert.IsTrue((test.SearchTarget.FileName.Count > 0) && (test.SearchTarget.FileName[0].Equals("hello.exe")));
        }

        [TestMethod]
        public void FileAttrib_arg_set_named_Enums()
        {
            
            ArgHandling test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fileattrib=Directory" }));

            Assert.IsTrue((test.SearchTarget.AttributeMatching1 == FileAttributes.Directory));

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fileattrib=Archive" }));

            Assert.IsTrue((test.SearchTarget.AttributeMatching1 == FileAttributes.Archive));

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fileattrib=Hidden" }));

            Assert.IsTrue((test.SearchTarget.AttributeMatching1 == FileAttributes.Hidden));
        }

        public void FileAttrig_DirStyle_Arg_check()
        {
            ArgHandling test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/A=R" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == FileAttributes.ReadOnly);

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/A=H" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == FileAttributes.Hidden);

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/A=S" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == FileAttributes.System);

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/A=D" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == FileAttributes.Directory);


            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/A=A" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == FileAttributes.Archive);

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/A=T" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == FileAttributes.Temporary);


            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/A=P" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == FileAttributes.SparseFile);

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/A=L" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == FileAttributes.ReparsePoint);


            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/A=C" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == FileAttributes.Compressed);

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/A=O" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == FileAttributes.Offline);

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/A=I" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == FileAttributes.NotContentIndexed);

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/A=E" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == FileAttributes.Encrypted);

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/A=N" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == FileAttributes.IntegrityStream);

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/A=U" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == FileAttributes.NoScrubData);

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/A=HSA" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == (FileAttributes.Hidden | FileAttributes.Archive | FileAttributes.System));

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/A=SAH" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == (FileAttributes.Hidden | FileAttributes.Archive | FileAttributes.System));

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/A=TCE" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == (FileAttributes.Temporary | FileAttributes.Compressed | FileAttributes.Encrypted));
        }

        [TestMethod]
        public void FileAttrib_arg_set_as_int()
        {
            ArgHandling test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fileattrib=1" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == FileAttributes.ReadOnly);

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fileattrib=2" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == FileAttributes.Hidden);

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fileattrib=4" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == FileAttributes.System);

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fileattrib=16" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == FileAttributes.Directory);

            test = new ArgHandling();   
            Assert.IsTrue(test.DoTheThing(new string[] { "/fileattrib=32" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == FileAttributes.Archive);

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fileattrib=256" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == FileAttributes.Temporary);

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fileattrib=512" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == FileAttributes.SparseFile);

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fileattrib=1024" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == FileAttributes.ReparsePoint);

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fileattrib=2048" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == FileAttributes.Compressed);

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fileattrib=4096" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == FileAttributes.Offline);

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fileattrib=8192" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == FileAttributes.NotContentIndexed);

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fileattrib=16384" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == FileAttributes.Encrypted);

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fileattrib=32768" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == FileAttributes.IntegrityStream);

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fileattrib=131072" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == FileAttributes.NoScrubData);


            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fileattrib=3" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == (FileAttributes.ReadOnly  | FileAttributes.Hidden));

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fileattrib=2304" }));
            Assert.IsTrue(test.SearchTarget.AttributeMatching1 == (FileAttributes.Compressed | FileAttributes.Temporary));
        }

        [TestMethod]
        public void FileAttribChecker_arg_set_int()
        {
            ArgHandling test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fileattrib_check=1" }));
            Assert.IsTrue(test.SearchTarget.AttribMatching1Style == OdinSearchEngine.SearchTarget.MatchStyleFileAttributes.MatchAny);

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fileattrib_check=2" }));
            Assert.IsTrue(test.SearchTarget.AttribMatching1Style == OdinSearchEngine.SearchTarget.MatchStyleFileAttributes.MatchAll);

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fileattrib_check=4" }));
            Assert.IsTrue(test.SearchTarget.AttribMatching1Style == OdinSearchEngine.SearchTarget.MatchStyleFileAttributes.Invert);

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fileattrib_check=8" }));
            Assert.IsTrue(test.SearchTarget.AttribMatching1Style == OdinSearchEngine.SearchTarget.MatchStyleFileAttributes.Exacting);

            test = new ArgHandling();
            Assert.IsTrue(test.DoTheThing(new string[] { "/fileattrib_check=16" }));
            Assert.IsTrue(test.SearchTarget.AttribMatching1Style == OdinSearchEngine.SearchTarget.MatchStyleFileAttributes.Skip);

            // reserved flag fails it .
            test = new ArgHandling();
            Assert.IsFalse(test.DoTheThing(new string[] { "/fileattrib_check=32" }));
            //Assert.IsTrue(test.SearchTarget.AttribMatching1Style == OdinSearchEngine.SearchTarget.MatchStyleFileAttributes.Reserved);
            
            
            // should fail. has reserved flag
            test = new ArgHandling();
            Assert.IsFalse(test.DoTheThing(new string[] { "/fileattrib_check=34" }));

        }
    }
}