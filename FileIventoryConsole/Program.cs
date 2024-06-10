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
using System.ComponentModel.Design;
using System.Security.Cryptography.X509Certificates;
using OdinSearchEngine.OdinSearch_OutputConsumerTools.ExternalBased;
using OdinSearchEngine.OdinSearch_OutputConsumerTools.CmdProcessorTools;

namespace FileIventoryConsole
{   
    /// <summary>
    /// Try the SearchAnchor, SearchTarget and OdinSearch class out
    /// </summary>
    static class Program
    {
        static void DisplayArguments(string[] args)
        {
            Console.WriteLine("Arguments Seen:");
            foreach (string arg in args)
            {
                Console.WriteLine($"\t{arg}");
            }
        }

#if DEBUG
        static bool IsDebugMode = true;
#else
        static bool IsDebugMode = false;
#endif
        static void Main(string[] args)
        {


            #region scracth pad
            #endregion

            
            OdinSearch_OutputConsumer_PluginCheck.Init();
            if (IsDebugMode)
            {
                Console.WriteLine("DEBUG BUILD: ");
                DisplayArguments(args);
                Console.WriteLine("Status Messages follow:");
                Console.WriteLine("Plugin cert signed only status: " + OdinSearch_OutputConsumer_PluginCheck.WeAreSigned);
                Console.WriteLine("END DEBUG INFO:");
                System.Diagnostics.Debugger.Launch();
            }
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
                    if (ArgHandling.AllowUntrustedPlugin)
                    {
                        // this code functionally disables the guard to prevent loading extern DLL/whatever in the plugin path if unsigned.
                        OdinSearch_OutputConsumer_PluginCheck.CheckAgainstThis?.Dispose();
                        OdinSearch_OutputConsumer_PluginCheck.CheckAgainstThis = null;
                    }

                    if (ArgHandling.WasActionSet)
                    {
                        if ( (ArgHandling.CommandString == null) && (ArgHandling.DesiredPlugin == null))
                        {
                            Console.WriteLine("*******************");
                            Console.WriteLine("Error: This command needed a function string set with the /command flag");
                            Console.WriteLine("*******************");
                            return;

                        }
                    }
                    if (!ArgHandling.WantUserExplaination)
                    {
                        if ((ArgHandling.WasStartPointSet == false) && (ArgHandling.WasWholeMachineSet == false))
                        {
                            ArgHandling.Usage();

                            Console.WriteLine("*******************");
                            Console.WriteLine("Error: Please specify a starting point via /anchor= or /anywhere");
                            Console.WriteLine("*******************");
                            return;
                        }

                        if ((ArgHandling.WasNetPluginSet) && (ArgHandling.PluginHasClassNameSet == false))
                        {
                            ArgHandling.Usage();
                            Console.WriteLine("*******************");
                            Console.WriteLine("Error: Please specify a classname to use out of the NET plugin set by /managed= by using /class=");
                            Console.WriteLine("*******************");
                        }

                        if ((ArgHandling.MoreThanOnConsumerSet))
                        {
                            ArgHandling.Usage();
                            Console.WriteLine("*******************");
                            Console.WriteLine("Error: Please use either the /outstream settings, the /action settings, the /managed setting or the /plugin setting but not more than 1.");
                            Console.WriteLine("*******************");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Explanation Mode Active - no search was performed.");
                        Console.WriteLine("*******************");
                        Console.WriteLine("Arguments were parsed as follows with one on each line below. If it's weird, check the quote (\") chars.");
                        Console.WriteLine("If you are using the /command flag, ensure all \" symbol in your string has a \\ prefix as in \\\".\r\n");
                        DisplayArguments(args);
                        Console.WriteLine("*******************");
                        Console.WriteLine("Explaining what the arguments will do. To execute the commands drop the /explain flag");
                        Console.WriteLine("*******************\r\n");
                        ArgHandling.DisplayExplain();
                        return;
                    }
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
            ArgHandling.SearchTarget.RegEscapeMode = false;
            if (ArgHandling.DesiredPlugin == null)
            {
                Console.WriteLine("Fatal Error: No valid consumer was set.");
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

            Console.WriteLine("Searching for things, this may take a while.\r\n");

            static void error_workerThread(Thread t, Exception e)
            {
                Console.WriteLine($"WARNING: One of the worker threads crashed and that starting point is not being searched. Error message {e.Message}\r\n");
            }
            Search.Search(SearchDeal, error_workerThread);
            while(true)
            {
                if (Search.HasActiveSearchThreads)
                    Search.WorkerThreadJoin();
                else
                {
                    if (Search.IsZombied)
                    {
                        Search.WorkerThread_ResolveComs();
                        break;
                    }
                    break;
                }
            }
            var Errors = Search.GetWorkerThreadException();
            Console.Write("Search is finished....");
            if (Errors.Keys.Count == 0)
            {
                Console.WriteLine(string.Format("You have {0} file system items that matched.", SearchDeal.TimesMatchCalled));
             
                return;
            }
            else
            {
                Console.WriteLine("but a few starting threads failed to begin ok as weren't seached.");
                Console.WriteLine("Please be mindful that due to this, there may be additional matches in said points that were not checked.");
                Console.WriteLine(string.Format("You have {0} file system items that matched.", SearchDeal.TimesMatchCalled));
            }
            SearchDeal.Dispose();
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