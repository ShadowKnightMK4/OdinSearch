using System;
using FileInventoryEngine;
using System.IO;
using System.Threading;
namespace FileIventoryConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            SearchTarget ProgramFiles = new SearchTarget();
            SearchAnchor LocalStorage = new SearchAnchor("C:\\Windows");
            ProgramFiles.FileName.AddRange(new string[] { "*.EXE" , "*.DLL" });
            ProgramFiles.FileNameMatching = SearchTarget.MatchStyleString.MatchAny;
            ProgramFiles.AttribMatching1Style = SearchTarget.MatchStyleString.MatchAll;
            ProgramFiles.AttributeMatching1 = FileAttributes.Normal;    // same as 0 for this search i.e. match any
            ProgramFiles.FileSizeMin = 20 * 1000;
            ProgramFiles.FileSizeMax = 50 * 1000;
            LocalStorage.EnumSubFolders = true;
            MatchMaker NewMatch = new MatchMaker();
            NewMatch.Anchors.Add(LocalStorage);
            NewMatch.SearchFor.Add(ProgramFiles);
            

            NewMatch.Search();

            while (NewMatch.IsSearching)
            {
                Console.WriteLine("Am looking for it. please wait");
                Thread.Sleep(2000);
            }

            Console.WriteLine("Found  These Below");

            foreach (FileSystemInfo Item in NewMatch.Results)
            {
                Console.WriteLine(Item.FullName);
            }

            Console.WriteLine("****Was not able to access these to see if they matched*****\r\nIt is possible there are matches here.");
            foreach (FileSystemInfo Item in NewMatch.BlockedResults)
            {
                Console.WriteLine(Item.FullName);

                
            }
            return;
        }
    }
}
