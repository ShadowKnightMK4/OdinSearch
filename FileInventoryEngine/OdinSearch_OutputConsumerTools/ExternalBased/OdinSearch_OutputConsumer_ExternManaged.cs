using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
        
        /// <summary>
        /// Our Type info for <see cref="OdinSearch_OutputConsumerBase.WasNotMatched(FileSystemInfo)"/>
        /// </summary>
        static readonly Type[] WasNotMatchedArgs = { typeof(FileSystemInfo) };
        /// <summary>
        /// Our Type Info for <see cref="OdinSearch_OutputConsumerBase.WasNotMatched(FileSystemInfo)"/>
        /// </summary>
        static readonly Type[] WasMatchedArgs = WasNotMatchedArgs;

        /// <summary>
        /// Our Type Info for <see cref="OdinSearch_OutputConsumerBase.Blocked(string)"/>
        /// </summary>

        static readonly Type[] BlockedArgs = { typeof(string) };
        /// <summary>
        /// Our Type Info for <see cref="OdinSearch_OutputConsumerBase.Messaging(string)"/>
        /// </summary>

        static readonly Type[] MessagingArg = BlockedArgs;
        /// <summary>
        /// Our Type Info for <see cref="OdinSearch_OutputConsumerBase.SearchBegin(DateTime)"/>
        /// </summary>

        static readonly Type[] SearchBeginArg = { typeof(DateTime) };

        
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

        /// <summary>
        /// Get a list of all loaded class types we have
        /// </summary>
        /// <returns></returns>
        public ReadOnlyCollection<Type> GetSupportedPluginList()
        {
            var rt =  ImportedTypes.AsReadOnly();
            return rt;
        }
        /// <summary>
        /// We check for an attribute of this name in classes to import.
        /// </summary>
        public const string MetaDataName = "OdinFileSearch_comsclass";




        /// <summary>
        /// make an instance of this object and assign to our holding <see cref="CurrentObject"/>
        /// </summary>
        /// <param name="t"></param>
        void MakeInstance(Type t)
        {
            this.CurrentObject = Activator.CreateInstance(t) ;

        }
        /// <summary>
        /// select a class previous loadaed via <see cref="SetTargetLocation(string, string)"/>
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

        /// <summary>
        /// Get the current type we are working with
        /// </summary>
        /// <returns></returns>
        public Type GetCurrentReflectType()
        {
            return CurrentType;
        }

        /// <summary>
        /// Get the current object instance of <see cref="GetCurrentReflectType"/> we are working with
        /// </summary>
        /// <returns></returns>
        public object GetCurrentReflectClass()
        {
            return CurrentObject;
        }
        /// <summary>
        /// Load this assembly and get all public classes that have the metadata with this name
        /// </summary>
        /// <param name="location"></param>
        /// <param name="MetaDataName"></param>
        /// <exception cref="PluginCertificateCheckFailException">thrown if plugin fails the <see cref="OdinSearch_OutputConsumer_PluginCheck.CheckForCertificate(string)/> test</exception>
        /// <remarks>This askss if <see cref="OdinSearch_OutputConsumer_PluginCheck.CheckAgainstThis"/> is ok with loading.</remarks>
        public void SetTargetLocation(string location, string MetaDataName)
        {
            bool KeepIt;


            if (Remote != null)
            {
                Remote = null;
                ImportedTypes = new List<Type>();
            }

            // first check and issue the fail if it wails
            if (!OdinSearch_OutputConsumer_PluginCheck.CheckForCertificate(location))
            {
                throw new PluginCertificateCheckFailException(location);
            }

            // load the assembly and loop thru each exported class, getting the types that have the metadataname tag
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
        /// <param name="AssemblyLocation">The location of the file holding the assembly.</param>
        /// <param name="MetaDataName">We look for exported classes with this attribute. Default is An internal value at <see cref="AssemblyLoading_Net7.MetaDataName"/></param>
        /// <param name="classname">name of the class that will impelment our plugin protocol.</param>
        /// <exception cref="EntryPointNotFoundException">This can be thrown if a class name with the specific metadata is not found</exception>
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

        /// <summary>
        /// For this instance, we got to remember to dispose the base FIRST due to us having points to dispose in our 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            AssemblyLoading_Net7.Invoke_Dispose(loader.GetCurrentReflectType(), loader.GetCurrentReflectClass());
            
            base.Dispose(disposing);
            this.loader.Dispose();
            loader = null;
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
