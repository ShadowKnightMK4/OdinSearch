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

namespace Benchmarks
{
public class AntiVirusFriendlyConfig : ManualConfig
{
    public AntiVirusFriendlyConfig()
    {
        //AddJob(Job.MediumRun
          //  .WithToolchain(Process));
    }
}
}


namespace varients
{
    public class DeepDirPrune_FasterPathExistTest: DeepDirTracking
    {
        public override bool DoesDirPathExist(string path)
        {
            throw new NotImplementedException();
            var walk = this;
            StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            if (CaseSensitive)
            {
                comparison = StringComparison.InvariantCulture;
            }
            else
            {
                comparison = StringComparison.InvariantCultureIgnoreCase;
            }

            if (!toplevel.Equals( walk.toplevel) )
            {
                return false;
            }
            else
            {
                if (walk.branches.Count == 0)
                {
                    return false;
                }
                else
                {
                    //for (int step = 0; step < walk.branches.Count;step=++)
                    {
                        //if (walk.branches[step].toplevel == )
                    }
                }
            }
            
        }
    }
    public class DDDP_LIST: DeepDirTrackingBase
    {
        List<string> items = new();
        public override void AddDirPath(string path)
        {
            items.Add(path);
        }
        public override bool DoesDirPathExist(string path)
        {
            return items.Contains(path);
        }
        public override string sani_path(string path)
        {
            return path.Trim();
        }
    }
}

namespace Benchmarks
{
    [Config(typeof(AntiVirusFriendlyConfig))]
    [MemoryDiagnoser]
    /// <summary>
    /// see how long it takes to map my system.
    /// </summary>
    public class SearchAddStuff
    {
        public SearchAddStuff()
        {
             void Main()
            {
                var random = new Random();
                var basePaths = new List<string> { "C:\\Alpha", "C:\\Beta", "C:\\Gamma", "C:\\Delta", "C:\\Epsilon", "C:\\Zeta", "C:\\Eta", "C:\\Theta", "C:\\Iota", "C:\\Kappa", "C:\\Lambda", "C:\\Mu", "C:\\Nu", "C:\\Xi", "C:\\Omicron", "C:\\Pi", "C:\\Rho", "C:\\Sigma", "C:\\Tau", "C:\\Upsilon", "C:\\Phi", "C:\\Chi", "C:\\Psi", "C:\\Omega" };

                while (folders.Count < 200000)
                {
                    string path = basePaths[random.Next(basePaths.Count)];
                    int depth = random.Next(1, 6); // Depth between 1 and 5

                    for (int i = 1; i < depth; i++)
                    {
                        path = Path.Combine(path, basePaths[random.Next(basePaths.Count)].Substring(3)); // Remove the "C:\\" part and combine
                    }

                    folders.Add(path);
                }


            }
            Main();
        }
        OdinSearch WithStuff;
        OdinSearch WithoutStuff;


        void StandardSearch(OdinSearch e)
        {

            SearchTarget t = SearchTarget.AllFiles;
            SearchAnchor a = new SearchAnchor();
            a.EnumSubFolders = true;
            e.AddSearchAnchor(a);
            e.AddSearchTarget(t);
        }

        List<string> folders = new();


        void DeepDirTracking_CommonCode(Type Template, string kind)
        {
            Console.WriteLine("Current Plan" + kind);
            DeepDirTrackingBase Testme = (DeepDirTrackingBase) Activator.CreateInstance(Template);
            bool yes = false;

            foreach (string s in folders)
                Testme.AddDirPath(s);
            foreach (string s in folders)
            {
                yes = Testme.DoesDirPathExist(s);
            }
            Console.WriteLine("Test done: " + kind + " " + yes);
        }

        [Benchmark]
        public void DeepDirTracking_HASHCMP()
        {
            DeepDirTracking_CommonCode(typeof(DeepDirPrune.DeepDirTracking), "Standard code");
        }

        [Benchmark]
        public void DeepDirTracking_LISTCMP()
        {
            DeepDirTracking_CommonCode(typeof(varients.DDDP_LIST), "Using a list instead of the standard");
        }


    }
    internal class BenchMark
    {
        static void Main(string[] args)
        {
            var summery = BenchmarkRunner.Run(typeof(SearchAddStuff));
            Console.WriteLine(summery.ToString());
        }
    }
}
