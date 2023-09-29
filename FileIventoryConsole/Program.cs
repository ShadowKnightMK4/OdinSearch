﻿using System;
using OdinSearchEngine;
using OdinSearchEngine.OdinSearch_OutputConsumerTools;
using System.IO;
using System.Threading;
using System.Runtime.CompilerServices;
using FileInventoryConsole;
using System.Data.Sql;
using System.Text;
using System.Reflection;

namespace FileIventoryConsole
{   
    /// <summary>
    /// Try the SearchAnchor, SearchTarget and OdinSearch class out
    /// </summary>
    static class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Most of the testing code is in the unit tests. Try them out. ");
            Console.WriteLine("This console app can serve as an example of what to do or how to use.");
            
            SearchTarget ProgramFiles = new SearchTarget();
            SearchAnchor LocalStorage = new SearchAnchor("C:\\Windows\\");
            //ProgramFiles.FileName.Add(SearchTarget.MatchAnyFile);
            ProgramFiles.FileName.Add("*.EXE");
            ProgramFiles.FileName.Add("*.DLL");

            ProgramFiles.FileNameMatching = SearchTarget.MatchStyleString.MatchAny;
            //LocalStorage.EnumSubFolders = true;

            OdinSearch runme = new OdinSearch();
            runme.AddSearchAnchor(LocalStorage);
            runme.AddSearchTarget(ProgramFiles);

            var results = new OdinSearch_OutputSimpleConsole();
//            var results = new OdinSearchOutputConsumer_FilterCheck_WinTrust();
  //          results.WantTrusted = true;

            runme.Search(results);
            while (runme.HasActiveSearchThreads)
            {
                runme.WorkerThreadJoin();
            }

            Console.Write("{0} Files and Folders matched. {1} files and folders did not match.", results.TimesMatchCalled, results.TimesNoMatchCalled);
            return;
        }
    }
}
