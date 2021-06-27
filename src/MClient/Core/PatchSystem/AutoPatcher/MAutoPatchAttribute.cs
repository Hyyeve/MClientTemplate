using System;

namespace MClient.Core.PatchSystem.AutoPatcher
{
    /// <summary>
    /// This is a Auto-Attribute that allows you to automatically and easily patch methods with Harmony
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)] 
    public class MAutoPatchAttribute : Attribute
    {
        public readonly Type Type;
        public readonly string Method;
        public readonly MPatchType PatchType;
        
        /// <summary>
        /// This is a Auto-Attribute that allows you to automatically and easily patch methods with Harmony
        /// </summary>
        /// <param name="type">The type of the class that contains the method to patch</param>
        /// <param name="method">The name of the method to patch</param>
        /// <param name="patchType">The type of patch to do</param>
        public MAutoPatchAttribute(Type type, string method, MPatchType patchType)
        {
            Method = method;
            PatchType = patchType;
            Type = type;
        }
        
    }

    public enum MPatchType
    {
        Prefix,Postfix,Transpiler
    }
}