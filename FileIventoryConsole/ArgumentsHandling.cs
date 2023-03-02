using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace FileInventoryConsole
{
    /// <summary>
    /// class deals with parsing and extracting arguments
    /// </summary>
    static class ArgumentsHandling
    {
        

        /// <summary>
        /// Read the reasource at "ProjectName.Resource.UsageText.txt" and send to stdout.  Resource is assumed to be unicode text
        /// </summary>
        public static void Usage()
        {
            

            using (var text = Assembly.GetCallingAssembly().GetManifestResourceStream(Assembly.GetCallingAssembly().GetName().Name  + ".Resources.UsageText.txt"))
            {
                byte[] data =new byte[text.Length];
                text.Read(data, 0, data.Length);

                Console.WriteLine(Encoding.UTF8.GetString(data));
            }

        }
    }
}
