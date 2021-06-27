using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DuckGame;

namespace MClient.Core.DLLSystem
{
    public static class MDependencyResolver
    {
        
        /// <summary>
        /// Registers this class as a handler for the Assembly Resolve Event
        /// </summary>
        public static void ResolveDependencies()
        {
            AppDomain.CurrentDomain.AssemblyResolve += Resolve;
        }

        /// <summary>
        /// The event handler. Attempts to resolve a dependency when it's not found by the system.
        /// </summary>
        private static Assembly Resolve(object sender, ResolveEventArgs args)
        {
            
            string assemblyFullName = args.Name;
            string assemblyShortName = assemblyFullName.Substring(0, assemblyFullName.IndexOf(",", StringComparison.Ordinal));
            
            bool external;
            
            /*
            Checks if the dependency is part of this mod.
            We don't need to do this - we could ignore events from outside the assembly,
            but I decided to make the mod play nice and try and help out other mods if it can.
            */
            if (Assembly.GetCallingAssembly() != Assembly.GetExecutingAssembly())
            {
                MLogger.Log("Attempting to resolve external dependency", logSection: MLogger.MLogSection.Asmb);
                external = true;
            }
            else
            {
                MLogger.Log("Attempting to resolve " + assemblyShortName, logSection: MLogger.MLogSection.Asmb);
                external = false;
            }

            //Checks if the assembly is already loaded in the program, and just returns it if it is.
            try
            {
                var assembly = AppDomain.CurrentDomain.GetAssemblies().First(x => x.FullName == assemblyFullName);

                if (!(assembly is null))
                {
                    //Hooray we found it!
                    MLogger.Log("Found assembly " + assemblyShortName + " already loaded!", logSection: MLogger.MLogSection.Asmb);
                    return assembly;
                }
            }
            catch(InvalidOperationException)
            {
                //Oh well we don't care
            }

            //Time to look in the mod dlls folder!
            string path = Mod.GetPath<MModClass>("/dlls/" + assemblyShortName + ".dll");
            
            if (!File.Exists(path))
            {
                //We don't have it :(
                if (external)
                {
                    //It's not part of our mod so we don't bother logging it.
                    return null;
                }
                
                //It IS part of our mod and we're probably about to crash D:
                MLogger.Log("Unable to resolve assembly " + assemblyShortName, MLogger.MLogType.Warning, MLogger.MLogSection.Asmb);
                return null;
            }
 
            Assembly loadedAssembly = null;
            
            //Down here we know the file exists, so let's try and load it!
            try
            {
                //Attempt #1 - LoadFrom()
                loadedAssembly = Assembly.LoadFrom(path);
            }
            catch (Exception)
            {
                //Attempt #1 didn't work. Try again!
                try
                {
                    //Attempt #2 - Load(bytes[])
                    loadedAssembly = Assembly.Load(File.ReadAllBytes(path));
                }
                catch (Exception)
                {
                    //Attempt #2 didn't work. Weird.
                    MLogger.Log("Failed to load assembly " + assemblyShortName, MLogger.MLogType.Warning, MLogger.MLogSection.Asmb);
                }
            }
            
            MLogger.Log("Loaded assembly " + assemblyShortName + " from disk!", logSection: MLogger.MLogSection.Asmb);

            return loadedAssembly;
        }
    }
}