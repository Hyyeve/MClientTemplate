using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

//-------------------------------------------DISCLAIMER-------------------------------------------------------------------------
//This class was taken from a freely distributed helper library and modified to work for this mod. Most of this code is not mine.

namespace MClient.ExposerSystem
{
    
    /// <summary>
    /// A DynamicObject class that allows you to expose any classes private methods and variables easily.
    /// </summary>
    /// <remarks>MExposedClass is for static methods and variables. Use <see cref="MExposedObject"/> for instances</remarks>
    public class MExposedClass : DynamicObject
    {
        private readonly Type _mType;
        private readonly Dictionary<string, Dictionary<int, List<MethodInfo>>> _mStaticMethods;
        private readonly Dictionary<string, Dictionary<int, List<MethodInfo>>> _mGenStaticMethods;

        private MExposedClass(Type type)
        {
            _mType = type;

            _mStaticMethods =
                _mType
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(m => !m.IsGenericMethod)
                    .GroupBy(m => m.Name)
                    .ToDictionary(
                        p => p.Key,
                        p => p.GroupBy(r => r.GetParameters().Length).ToDictionary(r => r.Key, r => r.ToList()));

            _mGenStaticMethods =
                _mType
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(m => m.IsGenericMethod)
                    .GroupBy(m => m.Name)
                    .ToDictionary(
                        p => p.Key,
                        p => p.GroupBy(r => r.GetParameters().Length).ToDictionary(r => r.Key, r => r.ToList()));
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            // Get type args of the call
            Type[] typeArgs = MExposedObjectHelper.GetTypeArgs(binder);
            if (typeArgs != null && typeArgs.Length == 0) typeArgs = null;

            //
            // Try to call a non-generic instance method
            //
            if (typeArgs == null
                    && _mStaticMethods.ContainsKey(binder.Name)
                    && _mStaticMethods[binder.Name].ContainsKey(args.Length)
                    && MExposedObjectHelper.InvokeBestMethod(args, null, _mStaticMethods[binder.Name][args.Length], out result))
            {
                return true;
            }

            //
            // Try to call a generic instance method
            //
            if (_mStaticMethods.ContainsKey(binder.Name)
                    && _mStaticMethods[binder.Name].ContainsKey(args.Length))
            {
                List<MethodInfo> methods = (from method in _mGenStaticMethods[binder.Name][args.Length] where method.GetGenericArguments().Length == typeArgs.Length select method.MakeGenericMethod(typeArgs)).ToList();

                if (MExposedObjectHelper.InvokeBestMethod(args, null, methods, out result))
                {
                    return true;
                }
            }

            result = null;
            return false;
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var propertyInfo = _mType.GetProperty(
                binder.Name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            if (propertyInfo != null)
            {
                propertyInfo.SetValue(null, value, null);
                return true;
            }

            var fieldInfo = _mType.GetField(
                binder.Name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            if (fieldInfo == null) return false;
            fieldInfo.SetValue(null, value);
            return true;

        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var propertyInfo = _mType.GetProperty(
                binder.Name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            if (propertyInfo != null)
            {
                result = propertyInfo.GetValue(null, null);
                return true;
            }

            var fieldInfo = _mType.GetField(
                binder.Name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            if (fieldInfo != null)
            {
                result = fieldInfo.GetValue(null);
                return true;
            }

            result = null;
            return false;
        }

        public static dynamic From(Type type)
        {
            return new MExposedClass(type);
        }
    }
}