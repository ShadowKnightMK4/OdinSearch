## About OdinSearch.
OdinSearch is a tool written in C# that lets users search thru local file systems for matches and do something based on said matches. The class library has an expandable architecture in which developers can subclass a ‘communcations’  class and pass it to the searcher class to feed matching results to it.  Included in the class library to serve both as examples and starting points for custom communication classes are examples ranging from feeding results to a file, saving results to an Excel comma separated value file, creating symbolic links based on matching and executing shell commands on matches. The current end user facing aspect of this project – FileInventoryConsole is built to show off some of the flexibility class design including loading extern DLLs in a plugin fashion for this project.

## About this Branch
This is the main branch for my project.  It’s currently based off of the recent Windows release for the console front end located [here](https://github.com/ShadowKnightMK4/OdinSearch/tree/WindowsReleaseVersion1.0.0.0). If and when version two comes out, the branch main is based on will change.


## Getting Started for End Users
1.	Grab the release of your choice on the Repo. It’s already ready to run.
2.	You’ll need .NET 7 [here]( https://dotnet.microsoft.com/en-us/download/dotnet/7.0) installed on your OS.
3.  Note that the official release currently targets Windows only. Source code may be buildable under other OS but has not been assessed there by me. 


## Getting Started for Programmers

1.  Clone or download Repository.
2.  Dev Environment is Visual Studio 2022 and should be able to be opened locally with no issues. One may have to configure a folder or two if not in that environment.
3.  Try building first.   It'll build the front end (FileInventoryConsole), the engine and the unit tests.  Note that there references to SQL in the engine classes and Unit Tests, those are leftovers from a current TODO feature. 
4.  Explore the Classes to see the layout.
5.  Determine what you want the matching class to do. Subclass the base Matching class (OdinSearch_OutputConsumerBase) and write your code to do the thing you need to do.
6.  Compile and run it.
7.  (Optional) Enjoy and offer feedback.


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

	     // You may possibly need to have this call before waiting pending threads. I’ve had exceptions sometimes if it’s not there, but if you aren’t getting any, this call to Thread.Sleep() is unneeded.
	      Thread.Sleep(200); // Just a 1/5 sec to let things spin up.

            while (true)
            {
                // The searcher spawns threads and this routine pausing your code running until they're done.
                SearchMan.WorkerThreadJoin();
                break;
            }
            
            
            // this example just writes the results to the console screen.
            // OdinSearch_OutputSimpleConsole is a communication class that that does that too.
            Console.WriteLine("There were " + GetResults.Results.Count.ToString() + " result(s) that matched");
            foreach (FileSystemInfo s in GetResults.Results)
            {
                Console.WriteLine(s.FullName);
            }
            Console.WriteLine("End of Results");
        }
}
```


## The 4 main classes of interest (skippable if you don’t need source code info)
1. [OdinSearch](https://github.com/ShadowKnightMK4/OdinSearch/blob/master/FileInventoryEngine/OdinSearch.cs) is the class that does the searching. It is this class that you make an instance of, add where and what to search and pass the communication class to it.
2. [SearchAnchor](https://github.com/ShadowKnightMK4/OdinSearch/blob/master/FileInventoryEngine/SearchAnchor.cs) is the class that describes the starting point for a search.  OdinSearch can have any number of these in a list. This can be converted to XML and back.
3. [SearchTarget](https://github.com/ShadowKnightMK4/OdinSearch/blob/master/FileInventoryEngine/SearchTarget.cs) is the class that describes what to search for.  OdinSearch can have any number of these in a list. This also can be converted to XML and back.
4. [OdinSearch_OutputConsumerBase](https://github.com/ShadowKnightMK4/OdinSearch/blob/master/FileInventoryEngine/OdinSearch_OutputConsumerTools/OdinSearch_OutputConsumerBase.cs) is the base class of the communications class that OdinSearch uses. There are plenty of examples to get started (see below) and it is this class that you subclass to customize what to do with responses.


## Example OdinSearch_OutputConsumerBase class (skippable if you don’t need source code info)

#1 [OdinSearch_OutputSimpleConsole](https://github.com/ShadowKnightMK4/OdinSearch/blob/master/FileInventoryEngine/OdinSearch_OutputConsumerTools/OdinSearch_OutputSimpleConsole.cs) 

This class can write to the console or a file stream. There are a few settings to adjust for control  such as flushing the stream after each write, outputs a friendly string or strictly the match’s location. Note the one the front end uses a class similar to this – called [OdinSearch_OutputConsole](https://github.com/ShadowKnightMK4/OdinSearch/blob/master/FileInventoryEngine/OdinSearch_OutputConsumerTools/StreamWriterCommonBased/OdinSearch_OutputConsole.cs)

#2 [OdinSearch_OutputSimpleCSVWriter](https://github.com/ShadowKnightMK4/OdinSearch/blob/master/FileInventoryEngine/OdinSearch_OutputConsumerTools/OdinSearch_OutputSimpleCSVWriter.cs) 
This class writes the match’s info (such as, file name, location, attributes and the rest) out to a comma separated value text file (for Excel) to open up later. There are options to customize what to put in the list; however, default is all possible items. The console front end users one called [OdinSearchOutputCVSWriter](https://github.com/ShadowKnightMK4/OdinSearch/blob/master/FileInventoryEngine/OdinSearch_OutputConsumerTools/StreamWriterCommonBased/OdinSearch_OutputCVSWriter.cs). 

#3 [OdinSearch_OutputConsumer_FilterCheck](https://github.com/ShadowKnightMK4/OdinSearch/blob/master/FileInventoryEngine/OdinSearch_OutputConsumerTools/OdinSearch_OutputConsumer_FilterCheck.cs)
This class introduces a way to filter matches based on a routine beyond the file searching to help control matching. There’s also an example of what do to – check if an executable file has a trusted certificate or not. The example has a possibility to control if the list it makes of those that are trusted vs those that are not trusted. [OdinSearchOutputConsumer_FilterCheck_CertExample](https://github.com/ShadowKnightMK4/OdinSearch/blob/master/FileInventoryEngine/OdinSearch_OutputConsumerTools/OdinSearchOutputConsumer_FilterCheck_WinTrust.cs)

#4 [OdinSearch_SymbolicLinkSort](https://github.com/ShadowKnightMK4/OdinSearch/blob/master/FileInventoryEngine/OdinSearch_OutputConsumerTools/OdinSearch_SymbolicLinkSort.cs)
This class is not directly accessible by users of the front end, you’ll need to clone the project and build it to get use. It will create symbolic links in a target location (and sub folder) based on file extensions. For example,  assuming target is C:\SearchResults. All matching  files with a .PDF extension would get a placed in the target location at C:\SearchResults\PDF. The class also has a way in place to customize how it creates symbolic links and folders too.




## Highlighted Features 


1.	This project was built to mainly search local file systems for matches and do something based on said matches.
2.	The front end (FileInventoryConsole project) is built to serve as a way to use this library without needing to be familiar with the source or programming in general. 
3.	One of the most useful features is the customizable match response. Subclass the communication class (or write a plugin for the front end) and feed it to an instance of the searcher class.
4.	Easy of getting started and extending it. There are 4 C# classes to become familiar with if all you need is searching for something and dealing with results the front end can’t help with.

## Current Design Plans
1.	I would still like to add a way to handle container formats (zip files/ect…) transparently for the communication class, without speed penalties.
2.	I would like to eventually offer Unit Tested Linux builds for this software.
3.	I would like to add a communication class that will save matching files into a possibly remote SQL database for later use.
3.  For the Comma Seperated value class [](https://github.com/ShadowKnightMK4/OdinSearch/blob/master/FileInventoryEngine/OdinSearch_OutputConsumerTools/StreamWriterCommonBased/OdinSearch_OutputCVSWriter.cs), I plan on adde more options to customize how it exports. One such plan is exported a hash of a files.
 

## Finished Design Plans
1.	 External plugin support: (there’s two communication classes that offer this)
2.	 Export as Comma Separated Value: (done)

 

## Communcations Class Design Guidelines (plugin writers and C# developers).

When beginning a search (OdinSearch.Search()), search folder in the SearchAnchors in list of SearchAnchors will get its own search thread to search. For example, passing 4 instances of SearchAnchors with total 20 different roots ‘starting points’ will get 20 threads searching. The threads are synched with the lock keyword on any call into your communication class. 

When the search is about the begin, a call to the communication class routine SearchBegin() occurs. Here is the last stop before the search begins.  Here you can initialize your stuff to manage processing matches. Thrown an exception if you can’t initialize ok.  There’s code also setup to trigger a call to the communication class AllDone() routine once all threads are finished processing. 
Of VITAL IMPORTANCE is that while a thread is calling into your communication class, it can’t continue processing items until you return control to it. If you need to do something computationally expensive, it’s recommended to put the FileSystemItem into a buffer of some sort – hence the example class above, [OdinSearch_OutputConsumer_FilterCheck]() That class introduces a way to make a list of possible matches and discard the possible matches that don’t. If circumstances allow me to add container handling (i.e. zip files) transparently, this may change.

For plugin writers,  your managed plugin gets an instance of its class created and OdinSearch uses a wrapper class to commicate with it. For umananged plugins, we use a similar plan i.e. a wrapper class that lets OdinSearch treat it as if a c# class effectively.




## License
License is currently MIT version that comes with the download named LICENSE.txt.

## Feedback and Contributions.
I welcome feedback and feature suggestions/ideas/bug reports.  Thanks for reading.

