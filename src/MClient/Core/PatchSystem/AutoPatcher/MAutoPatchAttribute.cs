using System;

namespace MClient.PatchSystem.AutoPatcher
{
    [AttributeUsage(AttributeTargets.Method)] 
    public class MAutoPatchAttribute : Attribute
    {
        public readonly Type type;
        public readonly string method;
        public readonly PatchType patchType;
        
        public MAutoPatchAttribute(Type type, string method, PatchType patchType)
        {
            this.method = method;
            this.patchType = patchType;
            this.type = type;
        }
        
    }

    public enum PatchType
    {
        Prefix,Postfix,Transpiler
    }
}