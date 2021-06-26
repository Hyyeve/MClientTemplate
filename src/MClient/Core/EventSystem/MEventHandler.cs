using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MClient.EventSystem.Events;
using MClient.EventSystem.Events.Helper;
using MClientCore.MClient.Core;

namespace MClient.EventSystem
{
    public static class MEventHandler
    {
        private static readonly Dictionary<Object, Dictionary<Type, MethodInfo>> Registered = new Dictionary<Object, Dictionary<Type, MethodInfo>>();
        private static readonly Dictionary<Type, object> ToRemove = new Dictionary<Type, object>();
        private static bool _inCallLoop = false;
        
        public static void Register(Type type, Object instance = null)
        {
            Dictionary<Type, MethodInfo> toAdd = null;
            
            bool registeredAlready = false;
            
            if (instance != null)
            {
                registeredAlready = Registered.TryGetValue(instance, out toAdd);
            }

            if (registeredAlready)
            {
                MLogger.Log(type.Name + " was registered already, skipping!", MLogger.LogType.Warning, ".EVNT");
                return;
            }

            toAdd ??= new Dictionary<Type, MethodInfo>();

            MethodInfo[] memberInfo = type.GetMethods();

            foreach (MethodInfo methodInfo in memberInfo)
            {
                var clientEvent = (MEvent) Attribute.GetCustomAttribute(methodInfo, typeof(MEvent));
                if (clientEvent == null) continue;
                toAdd.Add(clientEvent.GetType(), methodInfo);
                MLogger.Log("Registering method " + type.Name + "." + methodInfo.Name, logSection: ".EVNT");
            }
            
            Registered.Add(instance ?? type, toAdd);
            MLogger.Log("Finished registering " + type.Name, logSection: ".EVNT");
        }

        public static void DeRegister(Type type, Object instance = null)
        {
            if (_inCallLoop)
            {
                if (ToRemove.ContainsKey(type)) return;
                ToRemove.Add(type, instance);
                return;
            }
            if (instance != null && Registered.ContainsKey(instance))
            {
                Registered.Remove(instance);
                MLogger.Log("DeRegistered instance of " + type.Name, logSection: ".EVNT");
            }
            else if (Registered.ContainsKey(type))
            {
                Registered.Remove(type);
                MLogger.Log("DeRegistered " + type.Name, logSection: ".EVNT");
            }
        }

        public static void Call(MEvent clientEvent)
        {
            foreach (KeyValuePair<Type, object> pair in ToRemove)
            {
                DeRegister(pair.Key, pair.Value);
            }
            
            _inCallLoop = true;
            
            foreach (Object obj in Registered.Keys)
            {
                Registered.TryGetValue(obj, out var eventDictionary);
                
                if (eventDictionary == null)
                {
                    MLogger.Log("Something went horribly wrong - Event dictionary was null for registered key!", MLogger.LogType.Error, ".EVNT");
                    continue;
                }

                eventDictionary.TryGetValue(clientEvent.GetType(), out var methodInfo);

                if (methodInfo == null) continue;
                
                try
                {
                    methodInfo.Invoke(obj, methodInfo.GetParameters().Length > 0 ? new object[] {clientEvent} : null);
                }
                catch
                {
                     MLogger.Log("Exception thrown when invoking method: " + methodInfo.Name, MLogger.LogType.Warning, ".EVNT");
                }
            }
            
            _inCallLoop = false;
        }

        public static void InitialiseAll()
        {
            _inCallLoop = true;
            
            var methods = Assembly.GetExecutingAssembly()
                .GetTypes()
                .SelectMany(x => x.GetMethods())
                .Where(x => x.GetCustomAttributes(typeof(MInitEvent), false).FirstOrDefault() !=
                            null);

            var types = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.GetCustomAttributes(typeof(MAutoRegisterMEventsAttribute), false).FirstOrDefault() !=
                            null);
            foreach (Type t in types)
            {
                MLogger.Log("Auto-Registering class " + t.Name, logSection: ".EVNT");
                Register(t);
            }
            
            foreach (MethodInfo method in methods)
            {
                MLogger.Log("Invoking init method on " + method.DeclaringType?.Name, logSection: ".EVNT");
                method.Invoke(null, new object[] { });
            }

            methods = Assembly.GetExecutingAssembly()
                .GetTypes()
                .SelectMany(x => x.GetMethods())
                .Where(x => x.GetCustomAttributes(typeof(MPostInitEvent), false).FirstOrDefault() !=
                            null);
            foreach (MethodInfo method in methods)
            {
                MLogger.Log("Invoking post init method on " + method.DeclaringType?.Name, logSection: ".EVNT");
                method.Invoke(null, new object[] { });
            }
            
            
            _inCallLoop = false;

            foreach (KeyValuePair<Type, object> pair in ToRemove)
            {
                DeRegister(pair.Key, pair.Value);
            }
        }
    }
}