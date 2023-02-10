using System;
using OdinSearchEngine;
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
            ProgramFiles.FileName.Add("*.DLL");
            ProgramFiles.FileName.Add("*.EXE");
            ProgramFiles.FileNameMatching = SearchTarget.MatchStyleString.MatchAny;
            

            return;
        }
    }
}
