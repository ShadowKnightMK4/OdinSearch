#include <Windows.h>
#include <iostream>
#include <fstream>

/*
* This TestPlugin exports everything needing for a C# land OdinSearch_OutputConsumer_ExternUnmangedPlugin to hook into.
* 
* 
*/
/*
* This example outputs matched files here
*/
#define EXAMPLE_TEXT L"C:\\Dummy\\Output.txt"
using namespace std;

std::wfstream* OutText;

extern "C" {

    /// <summary>
    /// As the protocol matures, release builds will have instances of it set in stone be assigned a version.  
    /// Currently this is not actually called 
    /// </summary>
    /// <param name="CommicationVersion"></param>
    /// <returns></returns>
    bool __declspec(dllexport) PluginInit(DWORD CommicationVersion)
    {
        return TRUE;
    }

    /// <summary>
    /// Something wants to set a custom arg for the plugin.
    /// </summary>
    /// <param name="name"></param>
    void __declspec(dllexport) SetCustomArg(const wchar_t* name, LPVOID ArgValue)
    {

    }
    void __declspec(dllexport) WasNotMatched(const wchar_t* info)
    {
        return;
    }

    bool __declspec(dllexport) SearchBegin(INT64 Start)
    {
        OutText = new std::wfstream(EXAMPLE_TEXT, ios_base::out);
        return false;
    }

    bool __declspec(dllexport) ResolvePendingActions()
    {
        return false;
    }

    void __declspec(dllexport) Match(const wchar_t* info)
    {
        if (info != nullptr)
        {
            *OutText << info << std::endl;
        }
    }

    void __declspec(dllexport) Messaging(const wchar_t* Message)
    {
        return;
    }

    void __declspec(dllexport) Blocked(const wchar_t* BlockedMessage)
    {
        return;
    }

    bool __declspec(dllexport) HasPendingActions()
    {
        return false;
    }

    void __declspec(dllexport) AllDone()
    {

    }

}