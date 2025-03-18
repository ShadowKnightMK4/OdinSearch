using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using VirusTotalNet;
using OdinSearchEngine;
using OdinSearchEngine.OdinSearch_OutputConsumerTools;
namespace VirusTotalDemo
{
    internal class ThrottledVirusTotal
    {
        VirusTotal OnlineAsk = new(@"88cf717e5bdadb027624ccc6753dac12a6e6594494265b3e24270947b875c4a9");
        public List<VirusTotalNet.Results.ScanResult> Results = new();
        public int UpLoadCount { get; private set; } = 0;

        public Queue<FileSystemInfo> Info=new();
        public ThrottledVirusTotal() { }
        public void AddEntry(FileSystemInfo info)
        {
            Info.Enqueue(info);
        }

        Task MyTask = null;
        public void Begin()
        {
            OnlineAsk.UseTLS = true;

            MyTask = Task.Run(() =>
            {
                while (true)
                {
                    if (Info.Count != 0)
                    {
                        FileSystemInfo info = Info.Dequeue();
                        UpLoadCount++;
                        if (UpLoadCount > 4)
                        {
                            Thread.Sleep(1000);
                            UpLoadCount = 0;
                        }
                        var vs = OnlineAsk.ScanFileAsync(info.FullName);
                        vs.Wait();
                        Results.Add(vs.Result);
                    }
                    else
                    {
                        Thread.Sleep(200);
                    }
                }
            });
        }
    }
    internal class TotalDemo
    {
        OdinSearchEngine.OdinSearch FileSearch = new();
        TrustCheck TrustMe = new();
        class TrustCheck: OdinSearchOutputConsumer_FilterCheck_CertExample
        {
            ThrottledVirusTotal Tots = new ThrottledVirusTotal();
            public TrustCheck():base()
            {
                this.WantTrusted = false;

            }

            public override bool SearchBegin(DateTime Start)
            {
                Tots.Begin();
                return false;
            }
            public override bool FilterHandleRoutine(FileSystemInfo Info)
            {
                bool res = base.FilterHandleRoutine(Info);
                if (res)
                {
                    Tots.AddEntry(Info);
                }
                return res;
            }
            public override void AllDone()
            {
                
                
                // now we work
                base.AllDone();
            }
        }
        
        public void Begin()
        {
            FileSearch.Search(TrustMe);
        }
        
        public bool IsSearching()
        {
            return FileSearch.IsZombied == false;
        }
        public void AddAllReady()
        {
            var Anchor = new SearchAnchor(true);
            FileSearch.AddSearchAnchor(Anchor);
            return;
        }
        public void AddSearchFolder(string[] Folders)
        {
            var Anchor = new SearchAnchor(false);
            FileSearch.AddSearchAnchor(Anchor);
            Anchor.AddAnchor(Folders);
        }

        public void AddSearchTargetDefaults()
        {
            FileSearch.AddSearchTarget(SearchTarget.AllFiles);
        }
        public void ClearSearchAnchorList()
        {
            FileSearch.ClearSearchAnchorList();
        }


    }
}
