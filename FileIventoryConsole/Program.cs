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
            Console.WriteLine("Most of the testing code is in the unit tests. Try them out. ");
            Console.WriteLine("This console app can serve as an example of what to do or how to use.");



            OdinSearch SearchThis = new OdinSearch();
            SearchAnchor Desktop = new SearchAnchor(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

            SearchTarget AnyThing = new SearchTarget();
            AnyThing.FileName.Add(SearchTarget.MatchAnyFile);

            SearchThis.AddSearchAnchor(Desktop);
            SearchThis.AddSearchTarget(AnyThing);

            var Comsclass = new OdinSearch_SymbolicLinkSort();
            Comsclass[OdinSearch_SymbolicLinkSort.OutputFolderArgument] = "C:\\Results\\";
            Comsclass[OdinSearch_SymbolicLinkSort.CreateSubfoldersOption] = null;
            // default works, but if one wants to customize, they set like below with their implementation of a OdinSearch_SymbolicLinkSort.OdinSearch_SymbolicLinkSort_FileSystemHelp class
            //Comsclass[OdinSearch_SymbolicLinkSort.LinkSearchHelperClass] = new OdinSearch_SymbolicLinkSort.OdinSearch_SymbolicLinkSort_FileSystemHelp();
            SearchThis.Search(Comsclass);
            while (SearchThis.HasActiveSearchThreads)
            {
                Thread.Sleep(1000);
                SearchThis.WorkerThreadJoin();

            }


            Console.WriteLine("{0} Files and Folders matched. {1} files and folders did not match.", Comsclass.TimesMatchCalled, Comsclass.TimesNoMatchCalled);
            //Console.WriteLine("Out of the matched files, {0} failed the filter check and were excluded.", results.FilteredResults);
            return;
        }
    }
}
/*
 * 
 * 
                if (SearchThis.IsZombied)
                {
                    Console.WriteLine("Almost done. All that's left is to process filter");
                    SearchThis.WorkerThread_ResolveComs();
                    break;
                }


 *   // Thread.Sleep(1000);
                //runme.WorkerThreadJoin();
            //    results.CheckFileFilter();
            Console.WriteLine("Done. Showing results now");
            //foreach (var result in results.MatchedResults) 
            {
              //  Console.WriteLine(result.FullName.ToString());
            }
 *       
            SearchTarget ProgramFiles = new SearchTarget();
            ProgramFiles.FileName.Add("*.EXE");
            ProgramFiles.FileName.Add("*.DLL");
            ProgramFiles.FileNameMatching = SearchTarget.MatchStyleString.MatchAny;
            

            SearchAnchor LocalStorage = new SearchAnchor();
            OdinSearch SearchThis = new OdinSearch();
            SearchThis.AddSearchAnchor(LocalStorage);
            SearchThis.AddSearchTarget(ProgramFiles);

            LocalStorage.EnumSubFolders = true;
           // var results = new OdinSearch_OutputSimpleConsole();
            //             results = new OdinSearchOutputConsumer_FilterCheck_CertExample();
            // results.WantTrusted = false;

            SearchThis.Search(results);*/