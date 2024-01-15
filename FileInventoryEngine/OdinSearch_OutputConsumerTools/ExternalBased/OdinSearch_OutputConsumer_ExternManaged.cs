using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ObjectiveC;
using System.Text;
using System.Threading.Tasks;

namespace OdinSearchEngine.OdinSearch_OutputConsumerTools.ExternalBased

{
    
    /// <summary>
    /// While <see cref"OdinSearch_OutputConsumer_ExternManaged"/> is the front end, this is what loads the assembly and makes the calls.
    /// </summary>
    /// <remarks>There's no guarentee that this wont change as needed. You're welcome to use it. </remarks>
    internal sealed class AssemblyLoading_Net7: IDisposable
    {
        static Type[] WasNotMatchedArgs = { typeof(FileSystemInfo) };
        static Type[] WasMatchedArgs = WasNotMatchedArgs;
        static Type[] BlockedArgs = { typeof(string) };
        static Type[] MessagingArg = BlockedArgs;
        static Type[] SearchBeginArg = { typeof(DateTime) };
        List<Type> ImportedTypes = new();
        Assembly Remote = null;
        object CurrentObject;
        Type CurrentType;
        public static void Invoke_WasNotMatched(Type t,object that, FileSystemInfo Info)
        {
            t.GetMethod("WasNotMatched", WasNotMatchedArgs).Invoke(that, new object[] { Info});
        }

        public static void Invoke_Matched(Type t, object that, FileSystemInfo Info)
        {
            t.GetMethod("Match", WasMatchedArgs).Invoke(that, new object[] { Info });
        }

        public static void Invoke_Blocked(Type t, object that, string Message)
        {
            t.GetMethod("Blocked", BlockedArgs).Invoke(that, new object[] { Message });
        }

        public static void Invoke_Messaging(Type t, object that, string Message)
        {
            t.GetMethod("Messaging", MessagingArg).Invoke(that, new object[] { Message });
        }

        public static bool Invoke_SearchBegin(Type t, object that, DateTime Start)
        {
            return (bool)t.GetMethod("SearchBegin", SearchBeginArg).Invoke(that, new object[] { Start });
          
        }

        public static void Invoke_Dispose(Type t,object that)
        {
            t.GetMethod("Dispose", Array.Empty<Type>()).Invoke(that, Array.Empty<object>());
        }

        public static void Invoke_AllDone(Type t, object that)
        {
            t.GetMethod("AllDone", Array.Empty<Type>()).Invoke(that, Array.Empty<object>());
        }

        public static bool Invoke_HasPendingActions(Type t,object that)
        {
            return (bool) t.GetMethod("HasPendingActions", Array.Empty<Type>()).Invoke(that, null);
        }

        public static bool Invoke_ResolvePendingActions(Type t,object that)
        {
            return (bool)t.GetMethod("ResolvePendingActions", Array.Empty<Type>()).Invoke(that, null);
        }
        public ReadOnlyCollection<Type> GetSupportedPluginList()
        {
            var rt =  ImportedTypes.AsReadOnly();
            return rt;
        }
        /// <summary>
        /// We check for an attribute of this name in classes to import.
        /// </summary>
        public const string MetaDataName = "OdinFileSearch_comsclass";




        void MakeInstance(Type t)
        {
            this.CurrentObject = Activator.CreateInstance(t) ;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ClassName"></param>
        /// <exception cref="EntryPointNotFoundException">If your classname is not there</exception>
        public void SelectReflectClass(string ClassName)
        {
            foreach (Type t in ImportedTypes)
            {
                if (t.Name ==  ClassName)
                {
                    MakeInstance(t);
                    CurrentType = t;
                    return;
                }
                
            }
            throw new EntryPointNotFoundException(ClassName);
        }

        public Type GetCurrentReflectType()
        {
            return CurrentType;
        }

        public object GetCurrentReflectClass()
        {
            return CurrentObject;
        }
        public void SetTargetLocation(string location, string MetaDataName)
        {
            bool KeepIt = false;
            if (Remote != null)
            {
                // TODO: code to free the loaded assembly/
                Remote = null;
                ImportedTypes = new List<Type>();
            }
            Remote = Assembly.LoadFrom(location);
            if (Remote != null)
            {
                foreach (var PublicType in Remote.GetExportedTypes() )
                {
                    KeepIt = false;
                    foreach (var Attrib in PublicType.CustomAttributes)
                    {
                        if (Attrib.AttributeType.Name.Contains(MetaDataName))
                        {
                            KeepIt = true;
                            break;
                        }
                    }
                    if (KeepIt)
                    {
                        ImportedTypes.Add(PublicType);
                    }
                }
            }
        }

        public void Dispose()
        {
            this.CurrentObject = null;
            this.ImportedTypes = null;
            this.Remote = null;
        }
    }

    /// <summary>
    /// This class is for loading a specific assembly to use
    /// </summary>
    public class OdinSearch_OutputConsumer_ExternManaged : OdinSearch_OutputConsumerBase
    {
        /// <summary>
        /// Load the target assembly, look for metadata of the name and select the classname if known
        /// </summary>
        /// <param name="AssemblyLocation"></param>
        /// <param name="MetaDataName"></param>
        /// <param name="classname"></param>
        public OdinSearch_OutputConsumer_ExternManaged(string AssemblyLocation,  string MetaDataName , string classname)
        {
            loader = new AssemblyLoading_Net7();
            if (MetaDataName == null)
            {
                MetaDataName = AssemblyLoading_Net7.MetaDataName;
            }
            loader.SetTargetLocation(AssemblyLocation, MetaDataName);

            var type_list = loader.GetSupportedPluginList();
            for (int i = 0; i < type_list.Count;i++)
            {
                if (type_list[i].Name.Contains(classname))
                {
                    loader.SelectReflectClass(classname);
                }
            }
        }

        AssemblyLoading_Net7 loader;

        
        public override void AllDone()
        {
            AssemblyLoading_Net7.Invoke_AllDone(loader.GetCurrentReflectType(), loader.GetCurrentReflectClass());
            base.AllDone();
        }

        public override void Blocked(string Blocked)
        {
            AssemblyLoading_Net7.Invoke_Blocked(loader.GetCurrentReflectType(), loader.GetCurrentReflectClass(), Blocked);
            base.Blocked(Blocked);
        }

        protected override void Dispose(bool disposing)
        {
            AssemblyLoading_Net7.Invoke_Dispose(loader.GetCurrentReflectType(), loader.GetCurrentReflectClass());
            this.loader.Dispose();
            loader = null;
            base.Dispose(disposing);
        }

        public override bool HasPendingActions()
        {
            return AssemblyLoading_Net7.Invoke_HasPendingActions(loader.GetCurrentReflectType(), loader.GetCurrentReflectClass());
        }

        public override void Match(FileSystemInfo info)
        {
            AssemblyLoading_Net7.Invoke_Matched(loader.GetCurrentReflectType(), loader.GetCurrentReflectClass(), info);
            base.Match(info);
        }

        public override void Messaging(string Message)
        {
            AssemblyLoading_Net7.Invoke_Messaging(loader.GetCurrentReflectType(), loader.GetCurrentReflectClass(), Message);
            base.Messaging(Message);
        }

        public override bool ResolvePendingActions()
        {
            return AssemblyLoading_Net7.Invoke_ResolvePendingActions(loader.GetCurrentReflectType(), loader.GetCurrentReflectClass());
        }

        public override bool SearchBegin(DateTime Start)
        {
            return AssemblyLoading_Net7.Invoke_SearchBegin( loader.GetCurrentReflectType(), loader.GetCurrentReflectClass(), Start);
        }

        public override void WasNotMatched(FileSystemInfo info)
        {
            AssemblyLoading_Net7.Invoke_WasNotMatched(loader.GetCurrentReflectType(), loader.GetCurrentReflectClass(), info);   
            base.WasNotMatched(info);
        }
        

        
    }

}
