using System;
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
            Console.WriteLine("You got a version of this project that is intended to be tested via its UnitTests. Run the UnitTests");
            return;
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
