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
using System.Diagnostics;

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
                    Environment.Exit(-1);
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
                            Environment.Exit(-1);
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
                            Environment.Exit(-1);
                            return;
                        }

                        if ((ArgHandling.WasNetPluginSet) && (ArgHandling.PluginHasClassNameSet == false))
                        {
                            ArgHandling.Usage();
                            Console.WriteLine("*******************");
                            Console.WriteLine("Error: Please specify a classname to use out of the NET plugin set by /managed= by using /class=");
                            Console.WriteLine("*******************");
                            Environment.Exit(-1);
                        }

                        if ((ArgHandling.MoreThanOnConsumerSet))
                        {
                            ArgHandling.Usage();
                            Console.WriteLine("*******************");
                            Console.WriteLine("Error: Please use either the /outstream settings, the /action settings, the /managed setting or the /plugin setting but not more than 1.");
                            Console.WriteLine("*******************");
                            Environment.Exit(-1);
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
                        Environment.Exit(0);
                        return;
                    }
                }
            }
            else
            {
                ArgHandling.Usage();
                Environment.Exit(0);
                return;
            }
            OdinSearch Search = new OdinSearch();
            Search.DebugVerboseMode = false;
            //var SearchDeal = new OdinSearch_OutputSimpleConsole();
            //            var SearchDeal = new OdinSearch_OutputConsumer_ExternUnmangedPlugin();


            OdinSearch_OutputConsumerBase SearchDeal = null;
            try
            {
                Search.AddSearchAnchor(ArgHandling.SearchAnchor);
                Search.AddSearchTarget(ArgHandling.SearchTarget);

                if (ArgHandling.DesiredPlugin == null)
                {
                    Console.WriteLine("Fatal Error: No valid consumer was set.");
                    Console.Write("Quitting...\r\n");
                    Environment.Exit(-1);
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

                Console.Write("Searching for things, this may take a while.    ");

                var CursorPOs = Console.GetCursorPosition();
                string[] GUISTUFF = new string[] { "-", "\\", "|", "/", "*" };
                int tick = 0;
                DateTime Start = DateTime.Now;
                Search.Search(SearchDeal);
                while (true)
                {
                    Console.SetCursorPosition(CursorPOs.Left, CursorPOs.Top);
                    Console.WriteLine(GUISTUFF[tick]);
                    tick++;
                    if (tick > GUISTUFF.Length - 1)
                    {
                        tick = 0;
                    }
                    Console.WriteLine("Elapsed Time: " + (DateTime.Now - Start).ToString());
                    Thread.Sleep(100); // thread
                                       //Search.WorkerThreadJoin();
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
                Console.WriteLine();
                Console.WriteLine("Search is finished....");
                Console.WriteLine(string.Format("You have {0} file system items that matched.", SearchDeal.TimesMatchCalled));
                if (File.Exists(ArgHandling.TargetStream.Name))
                {
                    Console.WriteLine($"Results located: {ArgHandling.TargetStream.Name}");
                }

            }
            finally
            {
                SearchDeal?.Dispose();
            }
            
            return;
        }
    }
}