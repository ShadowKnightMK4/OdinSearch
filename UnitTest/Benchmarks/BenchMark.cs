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

namespace Benchmarks
{
public class AntiVirusFriendlyConfig : ManualConfig
{
    public AntiVirusFriendlyConfig()
    {
          //  AddJob(InProcessEmitToolchain.Instance())
        //AddJob(Job.MediumRun
          //  .WithToolchain(Process));
    }
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
    internal class BenchMark
    {
        static void Main(string[] args)
        {
            Console.WriteLine("This benchmark tests the DeepDirTracking, comparing memory usage and runtime between that and one that uses a simple list.");
            
            var summery = BenchmarkRunner.Run(typeof(SearchAddStuff));
            Console.WriteLine(summery.ToString());
        }
    }
}
