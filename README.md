## About OdinSearch.
OdinSearch is a tool written in C# that lets users search thru local file systems for matches. The engine provides an expandable achritector by letting developer expand on what the engine does with matching output. It's recommanded the user have some programming experience to use this.



## Current Features
1. Search thru the local file systems.  OdinSearch engine was built mainly to enable searching thru the files and folders on the machine its running on. 

2.  Customizable Match response.  What OdinSearch does with matching files depends on what commuication class is fed to it when begining the search.  It already has some prebuilt ones such as sending matches to the console that can act as a starting base.

3. Ease of getting started and extensibility.    There's about 4 classes to become familiar with  if all you need is searching and dealing with output. 




## Progress (closer to being considered done)

#3 IN PROGRESS.  CSV Support.  I'm planning to adding ability o pipe matches to an CVS file to let users open up in Excel or OpenOffice at a later date. Update as  as 12/3.  This has made resonable progress.

## TO DO Features (to be moved to In progress as needed)

#2 IN PROGRESS:  SQL Support.  I'm planning on adding ability to pipe matches to an SQL file so at a later date, it can be opened in software of the developer's choice. This is going to take some time. Plan to update as needed when I consider closer to being done 




## Tabled Features (may come back later)

#1 Tabled for now :   Container Support.  I'm planning on adding to deal with searching thru container files such as zip files and check for matches also. The main issue is integration and apparent search speed penalties.


## Example Code
```
using System;
using OdinSearchEngine;
using OdinSearchEngine.OdinSearch_OutputConsumerTools;
using System.IO;

static class Program
{
   static void Main(string[] args)
        {
            // This sets the search to match to file with these possible extentions
            // This class is how to specify what to search for
            SearchTarget TargetCompiledWindowsExtension = new SearchTarget();
            TargetCompiledWindowsExtension.FileName.Add("*.exe");
            TargetCompiledWindowsExtension.FileName.Add("*.dll");
            TargetCompiledWindowsExtension.FileNameMatching = SearchTarget.MatchStyleString.MatchAny;

            // The default Contructor automatically adds all local drives that repeart 'ready'
            SearchAnchor AllLocalReadyDrives = new SearchAnchor();
            // One has to manually turn on looking in the subfolders.
            AllLocalReadyDrives.EnumSubFolders = true;
            
            
            
            // This example 'AnotherExample' shows another way to specify starting points.
            // This just shows it. It's not used in this example code otherwise.
            SearchAnchor AnotherExample = new SearchAnchor("C:\\ThisSpecificFolder");
            AnotherExample.AddAnchor("D:\\AndSearchThisFolderToo");
            AnotherExample.EnumSubFolders = true;
            
            

            // OdinSearch is the search class. Don't forget to add your 
            // SearchTargets and SearchAnchors to the lists in it.
            OdinSearch SearchMan = new OdinSearch();
            SearchMan.AddSearchAnchor(AllLocalReadyDrives);
            SearchMan.AddSearchTarget(TargetCompiledWindowsExtension);


            // the OdinSearch Class requires passing a communication class subclassed 
            // from OdinSearch_OutputConsumerBase. this particular one just stores the 
            // results in a list for later use.  
            OdinSearch_OutputConsumerGatherResults GetResults = new OdinSearch_OutputConsumerGatherResults();

            // Starts the search.  
            SearchMan.Search(GetResults);
            while (true)
            {
                // The searcher spawns threads and this routine pausing your code running until they're done.
                SearchMan.WorkerThreadJoin();
                break;
            }
            
            
            // this example just writes the results to the console screen.
            // OdinSearch_OutputSimpleConsole is a communcation that that does that too.
            Console.WriteLine("There were " + GetResults.Results.Count.ToString() + " result(s) that matched");
            foreach (FileSystemInfo s in GetResults.Results)
            {
                Console.WriteLine(s.FullName);
            }
            Console.WriteLine("End of Results");
        }
}
```
## Getting Started

1.  Clone or download Repository.
2.  Dev Enviroment is Visual Studio 2022 and should be able to be opened locally with no issues. One may have to configure a folder or two if not in that environment.
3.  Try building first.   It'll build the console demo, the engine and the unit tests.  Note that the SQL unit tests currently do not pass so don't be suprised if that happens.  
4.  Explore the Classes to see the layout.
5.  Determine what you want the matching class to do. Subclass the base Matching class and write your code.
6.  Compile and run it.


## Classes of Interest

1. [OdinSearch](https://github.com/ShadowKnightMK4/OdinSearch/blob/master/FileInventoryEngine/OdinSearch.cs) is the class that does the searching. It's here that you's start searching and set what to search for.
2. [OdinSearch_OutputConsumerBase](https://github.com/ShadowKnightMK4/OdinSearch/blob/master/FileInventoryEngine/OdinSearch_OutputConsumerTools/OdinSearch_OutputConsumerBase.cs) is the base class of the communications class with OdinSearch.  Subclass it and implement the methods to control what your code does with matching.
2. [SearchAnchor](https://github.com/ShadowKnightMK4/OdinSearch/blob/master/FileInventoryEngine/SearchAnchor.cs) is the class that describes the starting point to search.  OdinSearch can have any number of these in a list.
3. [SearchTarget](https://github.com/ShadowKnightMK4/OdinSearch/blob/master/FileInventoryEngine/SearchTarget.cs) is the class that describes what to search for.  OdinSearch can have any number of these in a list.


## Communcations Class Design Guidelines.
When beginning a search, each SearchAnchor folder gets its own thread and a call is placed to SearchBegin() in the communication class to notify it that a search is about to begin. The code is set up to call the routine AllDone() in the communication class when all threads spawned by the search engine is finished. When OdinSearch calls into the communication class, it can’t continue with its search until the communication class returns control to OdinSearch. If the Communication class takes a while to do something, it is recommended that the class puts the FileSystemItem passed to it in a buffer of some sort and returns control to OdinSearch. This will let the class do what it needs to do without slowing the search down. When Container handlers (i.e zip) get added, there’s likely going to be a bit of change in considerations.



## License
License is currently MIT version that comes with the download.

## Feedback and Contributions.
I welcome feedback and feature suggestions/ideas/bug reports.  Thanks for reading.
