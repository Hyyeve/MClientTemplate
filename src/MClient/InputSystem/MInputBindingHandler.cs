using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DuckGame;
using MClient.Core.EventSystem.Events.Helper;

namespace MClient.InputSystem
{
    public static class MInputBindingHandler
    {
        private static readonly List<MInputBinding> Bindings = new List<MInputBinding>();

        [MEventInit]
        public static void Init()
        {
            List<MethodInfo> methods = Assembly.GetExecutingAssembly().GetTypes().SelectMany(x => x.GetMethods()).Where(x =>
                x.GetCustomAttributes(typeof(MInputBindingAttribute), false).FirstOrDefault() != null).ToList();
            foreach (MethodInfo method in methods)
            {
                MInputBindingAttribute att = (MInputBindingAttribute) Attribute.GetCustomAttribute(method,typeof(MInputBindingAttribute));
                CreateBind(att.bind, att.PressReq, att.OrderReq,method);
            }
        }
        
        public static void CreateBind(Keys[] bind, MBindPressReq pressReq, MBindOrderReq orderReq, MethodInfo method)
        {
            MInputBinding binding = new MInputBinding(bind, pressReq, orderReq, method);
            Bindings.Add(binding);
            binding.Activate();
        }

        public static void DestroyBind(int index)
        {
            if (index <= 0 || index >= Bindings.Count) return;
            Bindings[index].Deactivate();
            Bindings.RemoveAt(index);
        }

        public static void Destroy(MethodInfo actionMethod)
        {
            List<MInputBinding> temp = Bindings.Where(bind => bind.Method == actionMethod).ToList();
            temp.ForEach(bind => bind.Deactivate());
            temp.ForEach(bind => Bindings.Remove(bind));
        }

        public static void Destroy(MInputBinding binding)
        {
            binding.Deactivate();
            Bindings.Remove(binding);
        }
        
    }
}