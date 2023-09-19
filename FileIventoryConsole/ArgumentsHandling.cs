using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using OdinSearchEngine;
namespace FileInventoryConsole
{
    class ArgumentUnknown : Exception
    {
        public ArgumentUnknown(string message) : base(message) { }
    }

    class ArgumentExpectsFileSystemLocation: Exception
    {
        public ArgumentExpectsFileSystemLocation(string message) : base(message) { }
    }

    /*
     * -anchor   "starting search location as folder"
     * -anchorxml  "xml file contaiing description of the anchor class"
     * -target  "*.*"  
     * -targetxml  "xml file showing what to search for"
     * -oupput "type of output consume
     * 
     */
    static class ArgHandling
    {
        static string[] keyargs = { "-anchor", "-anchorxml",  "-target", "-fileattrib", "-targetxml", "-subfolders", "-output" };
        public static void ParseArguments(string[] input)
        {
            
            List<SearchTarget> searchTargets= new List<SearchTarget>();
            List<SearchAnchor> searchAnchors= new List<SearchAnchor>();

            StringBuilder Token = new StringBuilder();
            for (int step =0; step < input.Length;step++)
            {
                string lower = input[step].ToLowerInvariant();
                if (keyargs.Contains(lower) == false)
                {
                    throw new ArgumentUnknown(input[step]);
                }
                else
                {
                    SearchTarget newTarget = null;
                    SearchAnchor newAnchor = null;
                    // this one deals with arguments that expect a follow out
                    switch (lower)
                    {
                        case "-anchor":
                            if (step+1 > input.Length)
                            {
                                throw new ArgumentExpectsFileSystemLocation(input[step]);
                            }
                            step += 1;
                            newAnchor = new SearchAnchor(input[step]);
                            searchAnchors.Add(newAnchor);
                            break;
                        case "-anchorxml":
                            if (step + 1 > input.Length)
                            {
                                throw new ArgumentExpectsFileSystemLocation(input[step]);
                            }
                            step += 1;
                            newAnchor = SearchAnchor.CreateFromXmlString(File.ReadAllText(input[step]));
                            searchAnchors.Add(newAnchor);
                            break;
                        case "-target":
                            if (step + 1 > input.Length)
                            {
                                throw new ArgumentExpectsFileSystemLocation(input[step]);
                            }
                            step += 1;
                            newTarget.FileName.Add(input[step]);
                            break;
                        case "-targetxml":
                            if (step +1 > input.Length)
                            {
                                throw new ArgumentExpectsFileSystemLocation(input[step]);
                            }
                            step += 1;
                            newTarget = SearchTarget.CreateFromXmlString(File.ReadAllText(input[step]));
                            break;
                        case "-output":
                            throw new NotImplementedException("0output");
                            
                            break;
                    }

                    switch (lower)
                    {
                        case "-subfolders":
                            break;
                    }
                }
            }
        }
    }

}