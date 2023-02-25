using System;
using OdinSearchEngine;
using System.IO;
using System.Threading;
using System.Runtime.CompilerServices;
using FileInventoryConsole;

namespace FileIventoryConsole
{
    class Program
    {

        static void Main(string[] args)
        {
            ArgumentsHandling.Usage();
            SearchTarget ProgramFiles = new SearchTarget();
            SearchAnchor LocalStorage = new SearchAnchor();
            ProgramFiles.FileName.Add("*");
            
            ProgramFiles.FileNameMatching = SearchTarget.MatchStyleString.MatchAny;
            LocalStorage.EnumSubFolders = true;
            OdinSearch runme = new OdinSearch();
            runme.AddSearchAnchor(LocalStorage);
            runme.AddSearchTarget(ProgramFiles);

            var results = new OdinSearch_OutputSimpleConsole();
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
