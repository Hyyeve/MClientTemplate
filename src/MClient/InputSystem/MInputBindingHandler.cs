using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DuckGame;
using MClient.Core.EventSystem.Events.Helper;

namespace MClient.InputSystem
{
    /// <summary>
    /// Handler that stores and activates/deactivates input bindings, as well as creating the automatic ones from attributes.
    /// </summary>
    public static class MInputBindingHandler
    {
        private static readonly List<MInputBinding> Bindings = new List<MInputBinding>();

        [MEventInit]
        public static void Init()
        {
            List<MethodInfo> methods = Assembly.GetExecutingAssembly().GetTypes().SelectMany(x => x.GetMethods()).Where(x =>
                x.GetCustomAttributes(typeof(MInputBindingAttribute), false).FirstOrDefault() != null).ToList();
            foreach (var method in methods)
            {
                var att = (MInputBindingAttribute) Attribute.GetCustomAttribute(method,typeof(MInputBindingAttribute));
                CreateBind(att.Bind, att.PressReq, att.OrderReq,method);
            }
        }

        public static MInputBinding GetBind(Type bindClass, string methodName)
        {
            return Bindings.FindAll(b => b.Method.Name == methodName).Find(b => b.Method.DeclaringType == bindClass);
        }
        
        /// <summary>
        /// Creates and activates a method binding
        /// </summary>
        /// <param name="bind">The set of keypresses to bind the method call to</param>
        /// <param name="pressReq">The requirements for how the keys are pressed</param>
        /// <param name="orderReq">The requirements for what order the keys are pressed</param>
        /// <param name="method">The method to bind</param>
        public static MInputBinding CreateBind(Keys[] bind, MBindPressReq pressReq, MBindOrderReq orderReq, MethodInfo method)
        {
            var binding = new MInputBinding(bind, pressReq, orderReq, method);
            Bindings.Add(binding);
            binding.Activate();
            return binding;
        }

        /// <summary>
        /// Deactivates and removes a binding
        /// </summary>
        /// <param name="index">The index of the binding to remove</param>
        public static void DestroyBind(int index)
        {
            if (index <= 0 || index >= Bindings.Count) return;
            Bindings[index].Deactivate();
            Bindings.RemoveAt(index);
        }

        /// <summary>
        /// Deactivates and removes all bindings attached to the given method
        /// </summary>
        /// <param name="actionMethod">The method</param>
        public static void DestroyBind(MethodInfo actionMethod)
        {
            List<MInputBinding> temp = Bindings.Where(bind => bind.Method == actionMethod).ToList();
            temp.ForEach(bind => bind.Deactivate());
            temp.ForEach(bind => Bindings.Remove(bind));
        }

        /// <summary>
        /// Deactivates and removes a specific binding
        /// </summary>
        /// <param name="binding">The binding</param>
        public static void DestroyBind(MInputBinding binding)
        {
            binding.Deactivate();
            Bindings.Remove(binding);
        }
        
    }
}