## About OdinSearch.
OdinSearch is a tool written in C# that lets users search thru local file systems for matches. The engine provides an expandable achritector by letting developer expand on what the engine does with matching output. It's recommanded the user have some programming experience to use this.



## Current Features
1. Search thru the local file systems.  OdinSearch engine was built mainly to enable searching thru the files and folders on the machine its running on. 

2.  Customizable Match response.  What OdinSearch does with matching files depends on what commuication class is fed to it when begining the search.  It already has some prebuilt ones such as sending matches to the console that can act as a starting base.

3. Ease of getting started and extensibility.    There's about 4 classes to become familier with  if all you need is searching and dealing with output. 


## TO DO Features

#1 IN PROGRESS:   Container Support.  I'm planning on adding to deal with searching thru container files such as zip files and check for matches also.

#2 IN PROGRESS:  SQL Support.  I'm planning on adding ability to pipe matches to an SQL file so at a later date, it can be opened in software of the developer's choice. This is going to take come time.

#3 IN PROGRESS.  CSV Support.  I'm planning to adding ability o pipe matches to an CVS file to let users open up in Excel or OpenOffice at a later date.


## Getting Started

1.  Clone or download Repository.
2.  Dev Enviroment is Visual Studio 2022 and should be able to be opened locally with no issues. One may have to configure a folder or two if not in that environment.
3.  Try building first.   It'll build the console demo, the engine and the unit tests.  Note that the SQL unit tests currently do not pass so don't be suprised if that happens.  
4.  Explore the Classes to see the layout.
5.  Determine what you want the matching class to do. Subclass the base Matching class and write your code.
6.  Compile and run it.


## Classes of Interest

1. OdinSearch is the class that does the searching. It's here that you's start searching and set what to search for.
2. OdinSearch_OutputConsumerBase is the base class of the communications class with OdinSearch.  Subclass it and implement the methods to control what your code does with matching.
2. SearchAnchor is the class that describes the starting point to search.  OdinSearch can have any number of these in a list.
3. SearchTarget is the class that describes what to search for.  OdinSearch can have any number of these in a list.


## Communcations Class Design Guidelines.
When begining the search, each SearchAnchor folder will gets its own thread and a call is placed to SearchBegin() in your commincation class just before starting.  Also, code is set up to call AllDone() when each thread is finished.    When your code gets a call outside of these -- such as Matched(), Blocked() or Messaging(), OdinSearch locks an object in its own class with the C# keyword to assist in thread synchronization.   There currently one big important consideration in Commuication class design.  When OdinSearch calls into the commucation class, it can't continue with its search until the communcation class returns control to OdinSearch.  If the Communcation class takes a while to do something, I recommand it put the FileSystemItem passed to it in buffer of somesort and return control to OdinSearch.  When Container handlers (i.e zip) get added, there's likely going to be a bit of change in considerations.




