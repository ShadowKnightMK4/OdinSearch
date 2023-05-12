# FileInventory
FileInventory is a project aimed at building a flexible simple search engine for locating files on a local machine. It allows customization of what do to with search output. This project is not yet ready for general use, and some knowledge of C# or programming in general is recommended. On the plus side, one only has to worry about 4 main different classes and the rest are support for these classes.


# Using FileInventory
There are four core classes in this project. The SearchAnchor class is used to specify starting locations for the search. When the search is initiated, a thread is created for each SearchAnchor class adding to OdinSearch’s list. The SearchTarget class is used to define the search criteria aka what to look for. The class called OdinSearch, is the part of the project that does the searching.  Both SearchTarget and SearchAnchor classes are added to a list in OdinSearch. To start the search, you need to call the Search() method located in OdinSearch and provide a class based on OdinSearch_OutputConsumerBase.


The class based on OdinSearch_OutputConsumerBase receives notifications from the OdinSearch class during the search process. These notifications can include matches, non-matches, possible errors, and messages. There are a few prebuilt classes available, such as OdinSearch_OutputConsumerGatherResults, which collects a list of matches, or OdinSearch_OutputSimpleConsole, which outputs the results to the console. The intention is for the user of this project to either use a prebuilt OutputConsumeBase class or subclass from the general one for his or her needs.


# Design Goals
The project aims to include a Windows front end and provide a functional consumer base that allows users to send the output to an SQL file. Additional, I'm hoping to add a console based version so that the user can use the project on something other than Windows.


