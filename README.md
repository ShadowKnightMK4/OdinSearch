# FileInventory

This is to build myself a better local file search engine that is customizable.  The OdinSearchEngine project within this solution containg the actual code.  


Building a better local file search engine.  Repo/Project name subject to change


Plan is to build the Search Engine side to be flexible with searching local sources and be able to output the matched results to comething to consume results - for example a console window / text file.  There are plans to include a way to include a class to output results to a sql database.  Currently it is not functional as I'm still learning about what I need to do for it.


The FileInventoryConsole project defaults to listing all files it can access to the console.


# Using FileIventory
This is not really at the general user stage yet.  It's more developer writes code to use it stage. The class SearchAnchor are were the programmer describes in the local system where to start searching. The class SearchTarget is how to tell the what the search for.  The actual Search class itself is called 'OdinSearch'. You're need to make instance of it an add the SearchTarget + SearchAnchors to lists in it.  Finally, you're going to need to derive from either OdinSearch_OutputConsumerBase and code what you want to do with the output use one of the base ones such as OdinSearch_OutputSimpleConsole or OdinSearch_OutputConsumerGatherResults


# Branch Related Info
This branch is there for me to focus on tightening up the core parts before attempting to build on them the rest.