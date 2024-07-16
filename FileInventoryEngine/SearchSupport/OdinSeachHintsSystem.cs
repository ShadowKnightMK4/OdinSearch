using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinSearchEngine.SearchSupport
{
    /// <summary>
    /// The hints system. It's a class that 
    /// </summary>
    public class OdinSeachHintsSystem
    {
        /// <summary>
        /// get the next entry to process if it exists.
        /// </summary>
        /// <returns>Returns next <see cref="DirectoryInfo>"/> to process in the hints system or null</returns>
        public DirectoryInfo GetNextEntry()
        { 
            if (!Folders.TryDequeue(out var entry))
            {
                return null; 
            }
            return entry;
        }
        ConcurrentQueue<DirectoryInfo> Folders = new();

        /// <summary>
        /// Add a new entry to the queue ot process
        /// </summary>
        /// <param name="directoryInfo">entry to check before full search</param>
        /// <exception cref="ArgumentNullException">Is thrown if argument is null</exception>
        public void AddEntry(DirectoryInfo directoryInfo)
        {
            if (directoryInfo is null)
            {
                throw new ArgumentNullException(nameof(directoryInfo));
            }
            Folders.Enqueue(directoryInfo);
        }

        /// <summary>
        /// Add a collection of new entries to the queue to process
        /// </summary>
        /// <param name="Entries">entries to check before full search</param>
        /// <exception cref="ArgumentNullException">Is thrown if argument is null or an entry in the enumerator is null</exception>
        public void AddEntry(IEnumerable<DirectoryInfo> Entries)
        {
            if (Entries is null)
            {
                throw new ArgumentNullException(nameof(Entries));
            }
            foreach (DirectoryInfo entry in Entries)
            {
                if (entry is not null)
                {
                    AddEntry(entry);
                    continue;
                }
                throw new ArgumentNullException(nameof(entry));

            }
        }

        /// <summary>
        /// Add this folder to the entry to check. 
        /// </summary>
        /// <param name="Entry">Entry to add. Note the caller must be able to have a <see cref="DirectoryInfo"/> instance created pointing to this entry ok</param>
        /// <exception cref="ArgumentNullException">Is thrown if argument is null or empty</exception>
        public void AddEntry(string Entry)
        {
            if (string.IsNullOrEmpty(Entry))
                throw new ArgumentException(nameof(Entry)); 
            DirectoryInfo D = new DirectoryInfo(Entry);
            AddEntry(D);
        }

        /// <summary>
        /// Source a collection of entries from a <see cref="SearchAnchor"/>
        /// </summary>
        /// <param name="Anchor">Anchor to check</param>
        /// <exception cref="ArgumentNullException">Is thrown if argument is null. Note this code assumes <see cref="SearchAnchor"/> checks its roots before adding them.</exception>
        public void AddEntry(SearchAnchor Anchor)
        {
            if (Anchor is null)
                throw new ArgumentNullException(nameof(Anchor));
            foreach (DirectoryInfo Entry in Anchor.roots)
            {
                AddEntry(Entry);
            }
        }

        /// <summary>
        /// Search a collection of entries from a collection of Anchors
        /// </summary>
        /// <param name="Anchors"></param>
        /// <exception cref="ArgumentNullException">Is thrown if argument is null. Note this code assumes <see cref="SearchAnchor"/> checks its roots before adding them.</exception>
        public void AddEntry(IEnumerable<SearchAnchor> Anchors)
        {
            if (Anchors is null)
                throw new ArgumentNullException(nameof(Anchors));
            foreach (SearchAnchor Anchor in  Anchors)
            {
                if (Anchor is not null)
                    AddEntry(Anchor);
                else
                    throw new ArgumentNullException(nameof(Anchor));
            }
        }
    }
}
