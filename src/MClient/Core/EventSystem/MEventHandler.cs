using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MClient.Core.EventSystem.Events;
using MClient.Core.EventSystem.Events.Helper;
using MoreLinq;

namespace MClient.Core.EventSystem
{
    /// <summary>
    /// This class handles all calling of events.
    /// </summary>
    /// <remarks>
    /// Event methods must either take the event as their only parameter or have no parameters.
    /// </remarks>
    public static class MEventHandler
    {
        
        //Dictionaries for storing registered types & methods, as well as ones that need to be de-registered.
        private static readonly Dictionary<object, Dictionary<Type, MethodInfo>> Registered = new Dictionary<object, Dictionary<Type, MethodInfo>>();
        private static readonly Dictionary<Type, object> ToRemove = new Dictionary<Type, object>();
        private static readonly List<MEvent> ToCall = new List<MEvent>();
        private static bool _inCallLoop = false;
        private static Type _prevCallType;
        
        /// <summary>
        /// Registers all event methods within a class.
        /// </summary>
        /// <param name="type">The type to register</param>
        /// <param name="instance">The instance of that type to register. If null, only static event methods will be registered.</param>
        public static void Register(Type type, object instance = null)
        {
            Dictionary<Type, MethodInfo> toAdd = null;
            
            bool registeredAlready = false;
            
            if (instance != null)
            {
                registeredAlready = Registered.TryGetValue(instance, out toAdd);
            }

            if (registeredAlready)
            {
                MLogger.Log(type.Name + " was registered already, skipping!", MLogger.MLogType.Warning,
                    MLogger.MLogSection.Evnt);
                return;
            }

            toAdd = new Dictionary<Type, MethodInfo>();

            MethodInfo[] memberInfo = type.GetMethods();

            foreach (var methodInfo in memberInfo)
            {
                var clientEvents = (MEvent[]) Attribute.GetCustomAttributes(methodInfo, typeof(MEvent));
                if (clientEvents.Length == 0) continue;
                clientEvents.ForEach(e => toAdd.Add(e.GetType(), methodInfo));
                MLogger.Log("Registering method " + type.Name + "." + methodInfo.Name, logSection: MLogger.MLogSection
                    .Evnt);
            }
            
            Registered.Add(instance ?? type, toAdd);
            MLogger.Log("Finished registering " + type.Name, logSection: MLogger.MLogSection.Evnt);
        }

        /// <summary>
        /// De-Registers all event methods within a class.
        /// </summary>
        /// <param name="type">The type to de-register</param>
        /// <param name="instance">The instance of that type to de-register. If null, only static event methods will be de-registered.</param>
        public static void DeRegister(Type type, object instance = null)
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
                MLogger.Log("DeRegistered instance of " + type.Name, logSection: MLogger.MLogSection.Evnt);
            }
            else if (Registered.ContainsKey(type))
            {
                Registered.Remove(type);
                MLogger.Log("DeRegistered " + type.Name, logSection: MLogger.MLogSection.Evnt);
            }
        }

        /// <summary>
        /// Calls an event for all corresponding registered event methods.
        /// </summary>
        /// <param name="clientEvent">The event to call</param>
        public static void Call(MEvent clientEvent)
        {
            if (_inCallLoop && clientEvent.GetType() == _prevCallType)
            {
                MLogger.Log("Recursive event of type: " + clientEvent.GetType().Name + " detected, skipping!", MLogger.MLogType.Warning, MLogger.MLogSection.Evnt);
                return;
            }

            _prevCallType = clientEvent.GetType();
            
            CleanRegisteredClasses();

            _inCallLoop = true;
            
            foreach (var obj in Registered.Keys)
            {
                Registered.TryGetValue(obj, out var eventDictionary);
                
                if (eventDictionary == null)
                {
                    MLogger.Log("Something went horribly wrong - Event dictionary was null for registered key!", MLogger.MLogType.Error,
                        MLogger.MLogSection.Evnt);
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
                     MLogger.Log("Exception thrown when invoking method: " + methodInfo.Name, MLogger.MLogType.Warning,
                         MLogger.MLogSection.Evnt);
                }
            }
            
            _inCallLoop = false;
        }



        /// <summary>
        /// Handles auto-event attributes - Auto-Registering and Init Events. Not intended for custom use!
        /// </summary>
        public static void InitialiseAll()
        {
            _inCallLoop = true;
            
            AutoRegisterAttributeClasses(typeof(MAutoRegisterEventsAttribute), "Auto-Registering class: ");

            InvokeAttributeMethods(typeof(MEventEarlyInit), "Invoking InitEvent on: ");

            InvokeAttributeMethods(typeof(MEventInit), "Invoking InitEvent on: ");

            InvokeAttributeMethods(typeof(MEventLateInit), "Invoking LateInitEvent on: ");
            
            _inCallLoop = false;

            CleanRegisteredClasses();
        }

        /// <summary>
        /// De-Registers all classes that are currently waiting to be de-registered. Not intended for custom use!
        /// </summary>
        /// <remarks>
        /// This is used so that classes attempting to de-register themselves during event calls does not cause a crash.
        /// Instead, when that happens, the de-registering info is stored, and then the classes are actually
        /// de-registered once the event call is finished.
        /// </remarks>
        public static void CleanRegisteredClasses()
        {
            foreach (KeyValuePair<Type, object> pair in ToRemove)
            {
                DeRegister(pair.Key, pair.Value);
            }
            ToRemove.Clear();
        }

        /// <summary>
        /// Invokes all methods with a given attribute.
        /// </summary>
        /// <param name="attributeType">The type of attribute to search for</param>
        /// <param name="logMessage">The log message for each method called. The name of the method is appended onto the end.</param>
        /// <remarks>
        /// This is used internally for the Init Events, however it can also be used for custom auto-events.
        /// </remarks>

        public static void InvokeAttributeMethods(Type attributeType, string logMessage)
        {
            var methods = Assembly.GetExecutingAssembly()
                .GetTypes()
                .SelectMany(x => x.GetMethods())
                .Where(x => x.GetCustomAttributes(attributeType, false).FirstOrDefault() !=
                            null);
            foreach (var method in methods)
            {
                MLogger.Log(logMessage + method.DeclaringType?.Name, logSection: MLogger.MLogSection.Evnt);
                method.Invoke(null, new object[] { });
            }
        }

        /// <summary>
        /// Registers all classes with a given attribute.
        /// </summary>
        /// <param name="attributeType">The type of attribute to search for</param>
        /// <param name="logMessage">The log message for each method called. The name of the class is appended onto the end.</param>
        /// <remarks>
        /// This is used internally for the AutoRegister event, however it can also be used for custom auto-registering.
        /// </remarks>
        public static void AutoRegisterAttributeClasses(Type attributeType, string logMessage)
        {
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetCustomAttributes(attributeType, false).FirstOrDefault() != null);
            foreach (var t in types)
            {
                MLogger.Log(logMessage + t.Name, logSection: MLogger.MLogSection.Evnt);
                Register(t);
            }
        }
    }
}