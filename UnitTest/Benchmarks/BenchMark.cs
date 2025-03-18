using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
using OdinSearchEngine;
using OdinSearchEngine.OdinSearch_OutputConsumerTools;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using BenchmarkDotNet.Disassemblers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using DeepDirPrune;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Diagnostics.Tracing.Extensions;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using Perfolizer.Horology;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Loggers;

namespace Benchmarks
{
public class AntiVirusFriendlyConfig : ManualConfig
{
    public AntiVirusFriendlyConfig()
    {
           
    }
}
}

public class SearchThreadedTestConfig : ManualConfig
{
    public SearchThreadedTestConfig()
    {
        AddJob(Job.Default
            .WithToolchain(InProcessNoEmitToolchain.Instance) // Prevent JIT optimizations from affecting results
            .WithIterationTime(TimeInterval.FromMilliseconds(5 * 60 * 1000)) // Set a maximum time for each benchmark run. Note, this does not stop it if it is longer.
            .WithInvocationCount(1) // Run each benchmark method once per iteration
            .WithIterationCount(1) // Run only 1 iteration
            .WithWarmupCount(0) // Disable warmup
            .WithUnrollFactor(1)
        );
        WithOptions(ConfigOptions.DisableOptimizationsValidator); // Disable optimizations validator
        AddDiagnoser(MemoryDiagnoser.Default);
        AddLogger(ConsoleLogger.Default);
    }
}



namespace varients
{
    public class BasicList: DeepDirTracking
    {
        List<string> mylist = new();
        public BasicList()
        {

        }
        public override void AddDirPath(string path)
        {
            mylist.Add(path);
        }
        public override bool DoesDirPathExist(string path)
        {
            return mylist.Contains(path);
        }

        
    }
}

namespace Benchmarks
{
    public static class list_fill
    {
        static list_fill()
        {
            fill_list(ref Tiny200_depth4, 200, 4);
            fill_list(ref Medium2000_depth6, 2000, 6);
            fill_list(ref Large20000_depth2, 20000, 2);
            fill_list(ref Gigantic200000_depth8, 200000, 8);
        }
        public static List<string> Tiny200_depth4 = new List<string>();
        public static List<string> Medium2000_depth6 = new List<string>();
        public static List<string> Large20000_depth2 = new List<string>();
        public static List<string> Gigantic200000_depth8 = new List<string>();


        public static void fill_list(ref List<string> thing, uint numbers, uint depth)
        {
            var random = new Random();
            var basePaths = new List<string> { "C:\\Alpha", "C:\\Beta", "C:\\Gamma", "C:\\Delta", "C:\\Epsilon", "C:\\Zeta", "C:\\Eta", "C:\\Theta", "C:\\Iota", "C:\\Kappa", "C:\\Lambda", "C:\\Mu", "C:\\Nu", "C:\\Xi", "C:\\Omicron", "C:\\Pi", "C:\\Rho", "C:\\Sigma", "C:\\Tau", "C:\\Upsilon", "C:\\Phi", "C:\\Chi", "C:\\Psi", "C:\\Omega" };

            while (thing.Count < numbers)
            {
                string path = basePaths[random.Next(basePaths.Count)];
                //int depth = random.Next(1, 6); // Depth between 1 and 5

                for (int i = 1; i < depth; i++)
                {
                    path = Path.Combine(path, basePaths[random.Next(basePaths.Count)].Substring(3)); // Remove the "C:\\" part and combine
                }

                thing.Add(path);
            }
        }
    }
    //[Config(typeof(AntiVirusFriendlyConfig))]
    [MemoryDiagnoser]
    /// <summary>
    /// see how long it takes to map my system.
    /// </summary>
    public class SearchAddStuff
    {
        public static int MaxMS = 30 * 1000;




        void DeepDirTracking_CommonCode(Type Template, string kind, ref List<string> folders)
        {
            uint found;
            uint lost;
            found = lost = 0;
            Console.WriteLine("Current Plan" + kind);
            DeepDirTrackingBase Testme = (DeepDirTrackingBase) Activator.CreateInstance(Template);
            bool yes = false;

            foreach (string s in folders)
                Testme.AddDirPath(s);
            foreach (string s in folders)
            {
                yes = Testme.DoesDirPathExist(s);
                if (yes) found++;
                else lost++;
            }
            //Console.WriteLine("Test done: " + kind + " " + yes);
            //Console.WriteLine($"Found {found} matches but failed with lost {lost} matches. Note lost should be zero. ");
            if (lost > 0)
            {
                throw new InvalidDataException("Somehow the code didn't find the folder added eariler. May have a bug");
            }
            else
            {
                Console.WriteLine($"Code ran as expected, {nameof(Testme.DoesDirPathExist)} returned {found} items and didn't fail to find any");
            }
        }


        [Benchmark]
        public void DeepDirTracking_ListCode_TinyList()
        {
            Task x = Task.Run(() =>
            {
                DeepDirTracking_CommonCode(typeof(varients.BasicList), $"List code: {nameof(list_fill.Tiny200_depth4)}", ref list_fill.Tiny200_depth4);
            });
            if (!x.Wait(MaxMS))
            {
                throw new TimeoutException("Ran out of time");
            }
        }


        [Benchmark]
        public void DeepDirTracking_DeepDirTracking_TinyList()
        {
            Task x = Task.Run(() =>
            {
                DeepDirTracking_CommonCode(typeof(DeepDirPrune.DeepDirTracking), $"Standard code: {nameof(list_fill.Tiny200_depth4)}", ref list_fill.Tiny200_depth4);
            });
            if (!x.Wait(MaxMS))
            {
                throw new TimeoutException("Ran out of time");
            }
        }


        [Benchmark]
        public void DeepDirTracking_ListCode_MediumList()
        {
            Task x = Task.Run(() =>
            {
                DeepDirTracking_CommonCode(typeof(varients.BasicList), $"Basic List code: {nameof(list_fill.Medium2000_depth6)}", ref list_fill.Medium2000_depth6);
            });
            if (!x.Wait(MaxMS))
            {
                throw new TimeoutException("Ran out of time");
            }
        }


        [Benchmark]
        public void DeepDirTracking_DeepDirTracking_MediumList()
        {
            Task x = Task.Run(() =>
            {
                DeepDirTracking_CommonCode(typeof(varients.BasicList), $"Standard code: {nameof(list_fill.Medium2000_depth6)}", ref list_fill.Medium2000_depth6);
            });
            if (!x.Wait(MaxMS))
            {
                throw new TimeoutException("Ran out of time");
            }
        }


        [Benchmark]
        public void DeepDirTracking_DeepDirTracking_LargeList()
        {
            Task x = Task.Run(() =>
            {
                DeepDirTracking_CommonCode(typeof(DeepDirPrune.DeepDirTracking), $"Standard code: {nameof(list_fill.Large20000_depth2)}", ref list_fill.Large20000_depth2);
            });
            if (!x.Wait(MaxMS))
            {
                throw new TimeoutException("Ran out of time");
            }
        }

        [Benchmark]
        public void DeepDirTracking_ListCode_LargeList()
        {
            Task x = Task.Run(() =>
            {
                DeepDirTracking_CommonCode(typeof(varients.BasicList), $"List code: {nameof(list_fill.Large20000_depth2)}", ref list_fill.Large20000_depth2);
            });
            if (!x.Wait(MaxMS))
            {
                throw new TimeoutException("Ran out of time");
            }
        }


        [Benchmark]
        public void DeepDirTracking_ListCode_GiganticList()
        {
            Task x = Task.Run(() =>
            {
                DeepDirTracking_CommonCode(typeof(varients.BasicList), "List code:", ref list_fill.Gigantic200000_depth8);
            });
            if (!x.Wait(MaxMS))
            {
                throw new TimeoutException("Ran out of time");
            }
        }



        [Benchmark]
        public void DeepDirTracking_DeepDirTracking_GiganticList()
        {
            Task x = Task.Run(() =>
            {
                DeepDirTracking_CommonCode(typeof(DeepDirPrune.DeepDirTracking), "Standard code:", ref list_fill.Gigantic200000_depth8);
            });
            if (!x.Wait(MaxMS))
            {
                throw new TimeoutException("Ran out of time");
            }
        }






        
        public void DeepDirTracking_LISTCMP()
        {
            //DeepDirTracking_CommonCode(typeof(varients.BasicList), "Using a list instead of the standard");
        }

    }

    
    [MemoryDiagnoser]
    public class SearchThreadedTest
    {
        void CommonCode(OdinSearch_SyncMode mode)
        {
            Task x = Task.Run(() =>
            {
                OdinSearch Search = new();
                Search.SynchMode = mode;
                SearchTarget x = SearchTarget.AllFiles;
                SearchAnchor start = new SearchAnchor(@"C:\Windows");
                //start.AddAnchor(@"C:\Windows\system32");
                start.AddAnchor(@"C:\Windows\system");
                start.AddAnchor(@"C:\Euphoria");
                start.AddAnchor(@"F:\HOSP");
                start.AddAnchor(@"R:\ISO");
                start.EnumSubFolders = true;
                //var Process = new OdinSearchEngine.OdinSearch_OutputConsumerTools.OdinSearch_OutputSimpleConsole();
                var Process = new OdinSearchEngine.OdinSearch_OutputConsumerTools.DebugOnly.OdinSearch_OutputConsumerBaseNull();
                //Process[OdinSearchEngine.OdinSearch_OutputConsumerTools.OdinSearch_OutputSimpleConsole.FlushAlways] = true;
                Search.AddSearchAnchor(start);
                Search.AddSearchTarget(x);
                Search.Search(Process);
                while (Search.HasActiveSearchThreads)
                {
                    Thread.Sleep(2000);
                }
            });
            //if (!x.Wait(MaxMS))
            x.Wait(); return;
            {
                throw new TimeoutException("Ran out of time");
            }
        }
        /*
         * 
         */
        [Benchmark]
        public void WindowsSearchSync()
        {
            CommonCode(OdinSearch_SyncMode.Sync);
        }
        public static int MaxMS = 30 * 1000;
        [Benchmark]
        public void WindowsSearchNoSync()
        {
            CommonCode(OdinSearch_SyncMode.NoWait);
            /*
             * with the thread class
             * SearchThreadedTest.WindowsSearch: Job-SGEEIX(Toolchain=InProcessNoEmitToolchain)
Runtime = ; GC =
Mean = 432.334 us, StdErr = 2.359 us (0.55%), N = 36, StdDev = 14.151 us
Min = 391.214 us, Q1 = 424.471 us, Median = 435.960 us, Q3 = 440.063 us, Max = 457.669 us
IQR = 15.592 us, LowerFence = 401.083 us, UpperFence = 463.452 us
ConfidenceInterval = [423.864 us; 440.804 us] (CI 99.9%), Margin = 8.470 us (1.96% of Mean)
Skewness = -0.85, Kurtosis = 3.66, MValue = 2
-------------------- Histogram --------------------
[389.931 us ; 401.181 us) | @@
[401.181 us ; 415.131 us) | @@
[415.131 us ; 426.381 us) | @@@@@@@
[426.381 us ; 441.155 us) | @@@@@@@@@@@@@@@@@@
[441.155 us ; 452.897 us) | @@@@@@
[452.897 us ; 463.294 us) | @
---------------------------------------------------

// * Summary *

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.5131/22H2/2022Update)
AMD Ryzen 7 2700X, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.404
  [Host] : .NET 7.0.20 (7.0.2024.26716), X64 RyuJIT AVX2

Toolchain=InProcessNoEmitToolchain

| Method        | Mean     | Error   | StdDev   | Allocated |
|-------------- |---------:|--------:|---------:|----------:|
| WindowsSearch | 432.3 us | 8.47 us | 14.15 us |   5.07 KB |

// console coms,
            Match call direct and not threadaed.

            | Method        | Mean    | Error    | StdDev   | Allocated |
|-------------- |--------:|---------:|---------:|----------:|
| WindowsSearch | 2.009 s | 0.0031 s | 0.0029 s |  34.28 KB |

// console coms
                threaded no wait
            Toolchain=InProcessNoEmitToolchain

| Method        | Mean    | Error    | StdDev   | Median  | Allocated |
|-------------- |--------:|---------:|---------:|--------:|----------:|
| WindowsSearch | 1.787 s | 0.2137 s | 0.6301 s | 2.006 s |  37.69 KB |

// * Warnings *
            */
            
        }
    }
    internal class BenchMark
    {
        static void Main(string[] args)
        {

            // Set up the benchmark config with the InProcessEmitToolchain
            var summary = BenchmarkRunner.Run<SearchThreadedTest>(
              new SearchThreadedTestConfig());

            Console.Read();

        }
    }
}
