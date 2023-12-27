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
            ArgHandling ArgHandling = new();
            ArgHandling.DisplayBannerText();
            if (args.Length > 0)
            {
                if (!ArgHandling.DoTheThing(args))
                {
                    Console.Write("Quitting...\r\n");
                    return;
                }
                else
                {
                    ArgHandling.FinalizeCommands();
                }
            }
            else
            {
                ArgHandling.Usage();
                return;
            }
            OdinSearch Search = new OdinSearch();
            Search.DebugVerboseMode = false;
            //var SearchDeal = new OdinSearch_OutputSimpleConsole();
            //            var SearchDeal = new OdinSearch_OutputConsumer_ExternUnmangedPlugin();


            OdinSearch_OutputConsumerBase SearchDeal;
            Search.AddSearchAnchor(ArgHandling.SearchAnchor);
            Search.AddSearchTarget(ArgHandling.SearchTarget);

            if (ArgHandling.DesiredPlugin == null)
            {
                Console.WriteLine("Fatal Error: No output was set. Note this should not be reached in normal execution.");
                Console.Write("Quitting...\r\n");
                return;
            }
            else
            {
                SearchDeal = ArgHandling.DesiredPlugin;
            }


            /*
            if (ArgHandling.DesiredPlugin == null)
            {
                Console.WriteLine("No Handler specified.  Defaulting to showing matching results to stdout via OdinSearch_OutputSimpleConsole.");
                SearchDeal = new OdinSearch_OutputSimpleConsole();
                SearchDeal[OdinSearch_OutputSimpleConsole.OutputOnlyFileName] = true;   
            }
            else
            {
                SearchDeal = ArgHandling.DesiredPlugin;
                Console.WriteLine("Handler " + ArgHandling.DesiredPlugin.GetType().Name + " in use");
            }
            Console.WriteLine("Searching for things, this may take a while.");*/


            Search.Search(SearchDeal);
            while(true)
            {
                Search.WorkerThreadJoin();
                if (!Search.HasActiveSearchThreads)
                {
                    if (Search.IsZombied)
                    {
                        Search.WorkerThread_ResolveComs();
                        break;
                    }
                    break;
                }
            }

            Console.WriteLine("Search is finished....");
            Console.WriteLine(string.Format("You have {0} file system items that matched.", SearchDeal.TimesMatchCalled));
            SearchDeal.Dispose();
            return;
            OdinSearch SearchThis = new OdinSearch();
            SearchAnchor Desktop = new SearchAnchor(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

            SearchTarget AnyThing = new SearchTarget();
            AnyThing.FileName.Add(SearchTarget.MatchAnyFile);

            SearchThis.AddSearchAnchor(Desktop);
            SearchThis.AddSearchTarget(AnyThing);

            var Comsclass = new OdinSearch_SymbolicLinkSort();
            Comsclass[OdinSearch_SymbolicLinkSort.OutputFolderArgument] = "C:\\Results\\";
            // currently unsured but is a required setting. It always creates subfolders
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