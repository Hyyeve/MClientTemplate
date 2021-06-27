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
    /// A DynamicObject class that allows you to expose any classes private methods and variables easily.
    /// </summary>
    /// <remarks>MExposedObject is for instance methods and variables. Use <see cref="MExposedClass"/> for statics</remarks>
    public class MExposedObject : DynamicObject
    {
        private readonly Type _mType;
        private readonly Dictionary<string, Dictionary<int, List<MethodInfo>>> _mInstanceMethods;
        private readonly Dictionary<string, Dictionary<int, List<MethodInfo>>> _mGenInstanceMethods;

        private MExposedObject(object obj)
        {
            Object = obj;
            _mType = obj.GetType();

            _mInstanceMethods =
                _mType
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                                BindingFlags.FlattenHierarchy)
                    .Where(m => !m.IsGenericMethod)
                    .GroupBy(m => m.Name)
                    .ToDictionary(
                        p => p.Key,
                        p => p.GroupBy(r => r.GetParameters().Length).ToDictionary(r => r.Key, r => r.ToList()));

            _mGenInstanceMethods =
                _mType
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                                BindingFlags.FlattenHierarchy)
                    .Where(m => m.IsGenericMethod)
                    .GroupBy(m => m.Name)
                    .ToDictionary(
                        p => p.Key,
                        p => p.GroupBy(r => r.GetParameters().Length).ToDictionary(r => r.Key, r => r.ToList()));
        }

        public object Object { get; }

        public static dynamic New<T>()
        {
            return New(typeof(T));
        }

        public static dynamic New(Type type)
        {
            return new MExposedObject(Create(type));
        }

        private static object Create(Type type)
        {
            var constructorInfo = GetConstructorInfo(type);
            return constructorInfo.Invoke(new object[0]);
        }

        private static ConstructorInfo GetConstructorInfo(Type type, params Type[] args)
        {
            var constructorInfo = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                                                      BindingFlags.FlattenHierarchy, null, args, null);
            if (constructorInfo != null)
            {
                return constructorInfo;
            }

            throw new MissingMemberException(type.FullName,
                $".ctor({string.Join(", ", Array.ConvertAll(args, t => t.FullName))})");
        }

        public static dynamic From(object obj)
        {
            return new MExposedObject(obj);
        }

        public static T Cast<T>(MExposedObject t)
        {
            return (T)t.Object;
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
                    && _mInstanceMethods.ContainsKey(binder.Name)
                    && _mInstanceMethods[binder.Name].ContainsKey(args.Length)
                    && MExposedObjectHelper.InvokeBestMethod(args, Object, _mInstanceMethods[binder.Name][args.Length], out result))
            {
                return true;
            }

            //
            // Try to call a generic instance method
            //
            if (_mInstanceMethods.ContainsKey(binder.Name)
                    && _mInstanceMethods[binder.Name].ContainsKey(args.Length))
            {
                List<MethodInfo> methods = (from method in _mGenInstanceMethods[binder.Name][args.Length] where method.GetGenericArguments().Length == typeArgs.Length select method.MakeGenericMethod(typeArgs)).ToList();

                if (MExposedObjectHelper.InvokeBestMethod(args, Object, methods, out result))
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
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            if (propertyInfo != null)
            {
                propertyInfo.SetValue(Object, value, null);
                return true;
            }

            var fieldInfo = _mType.GetField(
                binder.Name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            if (fieldInfo == null) return false;
            fieldInfo.SetValue(Object, value);
            return true;

        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var propertyInfo = Object.GetType().GetProperty(
                binder.Name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            if (propertyInfo != null)
            {
                result = propertyInfo.GetValue(Object, null);
                return true;
            }

            var fieldInfo = Object.GetType().GetField(
                binder.Name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            if (fieldInfo != null)
            {
                result = fieldInfo.GetValue(Object);
                return true;
            }

            result = null;
            return false;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            result = Object;
            return true;
        }
    }

}


