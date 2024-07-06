using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace DeepDirPrune
{
 
    public abstract class DeepDirTrackingBase
    {
        public virtual string NextDirLevel(string dir)
        {
            if (string.IsNullOrWhiteSpace(dir))
            {
                return null;
            }
            else
            {
                int slash_first = dir.IndexOf("\\");
                int slash_next = dir.IndexOf('\\', slash_first + 1);

                if ((slash_next == slash_first) && (slash_first == -1))
                {
                    return dir;
                }
                else
                {
                    if (slash_first == 0)
                        return dir.Substring(1);
                    return dir.Substring(0, slash_first);
                }
            }
            return null;
            int count = 0;
            char c;
            if (string.IsNullOrWhiteSpace(dir))
                return null;
            else
            {
                for (int i = 0; i < dir.Length; i++)
                {
                    c = dir[i];
                    if ((c == Path.AltDirectorySeparatorChar) || (c == Path.DirectorySeparatorChar))
                    {
                        if (i != 0)
                            count++;
                    }
                    if (count != 0)
                        return dir.Substring(i);
                }
                if (count == 0) { return string.Empty; } else { }
                return dir;
            }
        }
        public DeepDirTrackingBase()
        {
            common_ini();
        }
        void common_ini()
        {
            BackingCase = false;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                BackingCase = false;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                BackingCase = true;
            }
        }


        /// <summary>
        /// return if we are checking with case. Defaults Windows - NOPE, for Linux YEP. Can be configured.
        /// </summary>
        public bool CaseSensitive
        {
            get; set;
        }

        private bool BackingCase;

        /*
         * Name storage (Windows)
         * C:\Windows\system32
         * 
         * 
         * toplevel = C:\
         * Next level = Windows
         * final level = system32
         * 
         * Notice how the slashes are stripped out with the exception of the root ?
         * 
         * 
         * Also not tested with Linux yet
         * 
         * 
         * Path sanitation rules
         * Anything received from outside the class *must* be first passed to sani_path()
         */
        /// <summary>
        /// This is the root of the containing folders
        /// </summary>
        public string toplevel;
        /// <summary>
        /// This is the subfolders of toplevel. Each one may have more folders and so on.
        /// </summary>
        public List<DeepDirTracking> branches = new List<DeepDirTracking>();

        public DeepDirTrackingBase this[string index]
        {
            get
            {
                DeepDirTrackingBase ret;
                ret = branches.Find(p => { return p.toplevel == index; });
                if (ret == null)
                    throw new KeyNotFoundException(index);
                return ret;
            }
        }


        public abstract void AddDirPath(string path);
        public abstract bool DoesDirPathExist(string path);
        public abstract string sani_path(string path);
        protected Type MyType = typeof(DeepDirTrackingBase);
    }
    /// <summary>
    /// This class stores a dir path as a series of class levels.
    /// </summary>
    public class DeepDirTracking: DeepDirTrackingBase
    {

        
        /// <summary>
        /// return the next component of the path (assuming C:\\Windows\\System32,
        /// 
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        
        bool Exists(string path)
        {
            return Directory.Exists(path);  
        }

        /// <summary>
        /// the sanitize user input routine.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <remarks>
        /// This routine does this:
        /// Strip outlining spaces with string.Trim().
        /// replace all instances of / with \.
        /// Trim quotes (', ").
        /// Remove a trailing slash char (C:\Windows\system32\ would be C:\Windows\system32)
        /// </remarks>
        public override string sani_path(string path)
        {
            const string special_win = @"\\?\";
            // let us sanitites
            path = path.Trim();
            path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            if (path.StartsWith("\"") || (path.StartsWith("\'")))
            {
                path = path.Substring(1);
            }
            if (path.EndsWith("\"") || (path.EndsWith("\'")))
            {
                path = path.Substring(0, path.Length - 1);
            }

            if (path.EndsWith(Path.DirectorySeparatorChar.ToString()) || path.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
            {
                path = path.Substring(0, path.Length - 1);
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                if (path.StartsWith(special_win))
                {
                    path = path.Substring(special_win.Length);
                }
            return path;
        }

        string expand_path(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (path.Contains("~"))
                {
                    path = path.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
                }
            }
            return path;
        }
        
        internal enum InternalRoutine_WalkPathMode
        {
            /// <summary>
            /// We run the the tree and then add our tail end.
            /// </summary>
            Add= 1,
            /// <summary>
            /// We run thru the tree and then return if existsw
            /// </summary>
            Exists = 2,
            /// <summary>
            /// We run thru the tree and return the instance of the final branch.
            /// </summary>
            GetLimb,

        }

        
        /// <summary>
        /// Kinda of f*cky, the alrogthym seems good. Rather than revent the wheel for <see cref="AddDirPath(string)"/> and <see cref="DoesDirPathExist(string)"/>, they both call into this with a flag to contril action.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="Mode"></param>
        /// <returns>Returns null if mode == <see cref="InternalRoutine_WalkPathMode.Add"/>,
        ///          Returns (true or false) if <see cref="InternalRoutine_WalkPathMode.Exists"/></returns>
        ///          
        internal object  InternalRoutine_WalkPath(string path, InternalRoutine_WalkPathMode Mode)
        {

            bool LastTime = false;
            string part;
            string walker = sani_path(path);
            DeepDirTracking pin = this;
            bool Bail = false;
            bool First = true;
        begin:
            Bail = false;

            if (First)
            {
                part = Path.GetPathRoot(walker);
                First = false;
            }
            else
            {
                part = NextDirLevel(walker);
            }
            LastTime = false;
            if (walker.Contains("\\") == false)
            {
                LastTime = true;
            }
            if  ( (pin.toplevel != null) && (part != null))
            {
                
                if (!pin.toplevel.Equals(part))
                {
                    Bail = true;
                }
                else
                {
                    if (Mode == InternalRoutine_WalkPathMode.GetLimb)
                    {
                        if (LastTime)
                            return pin;
                        
                    }
                    Bail = false;
                }
                
            }
            else
            {
                Bail = false;
            }

            if (!Bail)
            {
                if (pin.toplevel != null)
                    walker = walker.Substring(Math.Min(part.Length + 1, walker.Length));

                if (walker != string.Empty)
                    part = NextDirLevel(walker);

                bool MatchFound = false;
                DeepDirTracking step=null;
                for (int i = 0; i < pin.branches.Count; i++)
                {
                    step = pin.branches[i];
                    if (step.toplevel.Equals(part))
                    {
                        MatchFound = true;
                        pin = step;
                        if (LastTime && (Mode == InternalRoutine_WalkPathMode.Exists))
                            return true;
                        goto begin;
                    }
                    if (Mode != InternalRoutine_WalkPathMode.GetLimb)
                    step = null;
                }

                final_add:
                if (!MatchFound && (Mode == InternalRoutine_WalkPathMode.Add))
                {

                    if (!LastTime)
                    {
                        DeepDirTracking newone = new DeepDirTracking();
                        newone.toplevel = part;
                        pin.branches.Add(newone);
                        pin = newone;
                        goto begin;
                    }
                    else
                    {
                        goto nt;
                        DeepDirTracking newone = new DeepDirTracking();
                        newone.toplevel = part;
                        pin.branches.Add(newone);
                        pin = newone;
                    nt:
                        ;
                    }
                    
                    
                }

                if (LastTime)
                {
                    switch (Mode)
                    {
                        case InternalRoutine_WalkPathMode.Add:
                            // this is done at the label final_add;
                            //DeepDirTracking newone = new DeepDirTracking();
                            //newone.toplevel = part;
                            //pin.branches.Add(newone);
                            return null;
                            break;
                        case InternalRoutine_WalkPathMode.Exists:
                            return true;
                        case InternalRoutine_WalkPathMode.GetLimb:
                            {
                                return step;
                            }
                            break;
                    }
                    if (Mode == InternalRoutine_WalkPathMode.Exists)
                    {
                        return true;
                    }
                    
                }
            }



            return false;
        }

        public DeepDirTracking GetLimb(string path)
        {
            return InternalRoutine_WalkPath(path, InternalRoutine_WalkPathMode.GetLimb) as DeepDirTracking;
        }
        /// <summary>
        /// Is this path in our list?
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override bool DoesDirPathExist(string path)
        {
            return (bool)InternalRoutine_WalkPath(path, InternalRoutine_WalkPathMode.Exists);
            return false;
            bool LastTime = false;
            string part;
            string walker = sani_path(path);
            DeepDirTracking pin = this;
            bool Bail = false;
            bool First = true;
        begin:
            Bail = false;

            if (First)
            {
                part = Path.GetPathRoot(walker);
                First = false;
            }
            else
            {
                part = NextDirLevel(walker);
            }
            LastTime = false;
            if (walker.Contains("\\") == false)
            {
                LastTime = true;
            }
            if (pin.toplevel != null)
            {
                if (!pin.toplevel.Equals(part))
                {
                    Bail = true;
                }
            }
            else
            {
                Bail = false;
            }

            if (!Bail)
            {
                if (pin.toplevel != null)
                    walker = walker.Substring(Math.Min(part.Length+1, walker.Length));
                part = NextDirLevel(walker);
                DeepDirTracking step;
                for (int i = 0; i < pin.branches.Count; i++)
                {
                   step = pin.branches[i];
                   if (step.toplevel.Equals(part))
                   {
                        pin = step;
                        goto begin;
                   }
                }
                
                if (LastTime)
                {
                    return true;
                }
            }



            return false;
        }

        /// <summary>
        /// Add this path to our list
        /// </summary>
        /// <param name="path"></param>
        public override void AddDirPath(string path)
        {
            InternalRoutine_WalkPath(path, InternalRoutine_WalkPathMode.Add);
            return;
            bool First=true;
        begin:
            string walker = sani_path(path);
            string part;

            bool Last = false;
            if (First)
            {
                part = Path.GetPathRoot(walker);
            }
            else
            {
                part = NextDirLevel(walker);
            }

            if (!walker.Contains("\\"))
            {
                Last = true;
            }
            
        }
    }
}
