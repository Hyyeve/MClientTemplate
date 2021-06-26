using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace MClient.ExposerSystem
{
    //-------------------------------------------DISCLAIMER-------------------------------------------------------------------------
    //This class was taken from a freely distributed helper library and modified to work for this mod. Most of this code is not mine.
    
    /// <summary>
    /// Helper class for the ExposedObject classes. Not intended for custom use.
    /// </summary>
    public static class MExposedObjectHelper
    {
        private static readonly Type SCsharpInvokePropertyType =
            typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
                .Assembly
                .GetType("Microsoft.CSharp.RuntimeBinder.ICSharpInvokeOrInvokeMemberBinder");

        internal static bool InvokeBestMethod(object[] args, object target, List<MethodInfo> instanceMethods, out object result)
        {
            if (instanceMethods.Count == 1)
            {
                // Just one matching instance method - call it
                if (TryInvoke(instanceMethods[0], target, args, out result))
                {
                    return true;
                }
            }
            else if (instanceMethods.Count > 1)
            {
                // Find a method with best matching parameters
                MethodInfo best = null;
                Type[] bestParams = null;
                Type[] actualParams = args.Select(p => p == null ? typeof(object) : p.GetType()).ToArray();

                static bool IsAssignableFrom(Type[] a, IReadOnlyList<Type> b)
                {
                    return !a.Where((t, i) => !t.IsAssignableFrom(b[i])).Any();
                }


                foreach (var method in instanceMethods.Where(m => m.GetParameters().Length == args.Length))
                {
                    Type[] mParams = method.GetParameters().Select(x => x.ParameterType).ToArray();
                    if (!IsAssignableFrom(mParams, actualParams)) continue;
                    if (best != null && !IsAssignableFrom(bestParams, mParams)) continue;
                    best = method;
                    bestParams = mParams;
                }

                if (best != null && TryInvoke(best, target, args, out result))
                {
                    return true;
                }
            }

            result = null;
            return false;
        }

        private static bool TryInvoke(MethodInfo methodInfo, object target, object[] args, out object result)
        {
            try
            {
                result = methodInfo.Invoke(target, args);
                return true;
            }
            catch (TargetInvocationException) { }
            catch (TargetParameterCountException) { }

            result = null;
            return false;

        }

        internal static Type[] GetTypeArgs(InvokeMemberBinder binder)
        {
            if (!SCsharpInvokePropertyType.IsInstanceOfType(binder)) return null;
            var typeArgsProperty = SCsharpInvokePropertyType.GetProperty("TypeArguments");
            return ((IEnumerable<Type>)typeArgsProperty.GetValue(binder, null)).ToArray();
        }

    }
}


