using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DuckGame;
using MClient.Core.EventSystem.Events.Drawing.World;
using MClient.Core.EventSystem.Events.Helper;
using MClient.Core.EventSystem.Events.Input;
using MClient.InputSystem;
using MClient.PatchSystem.Internal;
using MClient.Render;
using MClient.UiSystem.Default;
using MClient.UiSystem.Internal.Attributes;
using MClient.UiSystem.Internal.Components;
using MClient.UiSystem.Internal.Components.Elements;
using MClient.Utils;
using MClientCore.MClient.Core;
using MoreLinq;

namespace MClient.UiSystem.Internal
{
    [MAutoRegisterEvents]
    public static class MUiHandler
    {
        public static readonly float GlobalUiScale = 0.5f;
        
        private static readonly SpriteMap Mouse = new SpriteMap(Mod.GetPath<MModClass>("Ui/UiCursor"), 16, 16);
        private static readonly Dictionary<string, MUiState> UiPanels = new Dictionary<string, MUiState>();
        private static readonly List<string> UpdateOrder = new List<string>();
        private static readonly List<string> ReverseUpdateOrder = new List<string>();
        private static bool _handlingUi;
        private static bool _shouldDrawMouse;

        [MEventInit]
        public static void Init()
        {
            LoadAutoPanels();
            Mouse.center = new Vec2(6.5f, 6.5f);
            ShowMouse();
        }


        public static void AddPanel(string id, MUiContainer ui)
        {
            UiPanels.Add(id, new MUiState(id, ui));
            UpdateOrder.Add(id);
            ReverseUpdateOrder.Insert(0, id);
        }

        private static void AddPanel(string id, MUiContainer ui, Type type)
        {
            UiPanels.Add(id, new MUiState(id, ui, type));
            UpdateOrder.Add(id);
            ReverseUpdateOrder.Insert(0, id);
        }

        public static void RemovePanel(string id)
        {
            UiPanels.Remove(id);
            UpdateOrder.Remove(id);
            ReverseUpdateOrder.Remove(id);
        }
        
        public static bool IsOpen(string id)
        {
            return UiPanels.ContainsKey(id) && UiPanels[id].Active;
        }

        public static void Open(string id)
        {
            if(UiPanels.ContainsKey(id)) UiPanels[id].EnablePanel();
            SetTop(UiPanels[id]);
            _handlingUi = true;
        }

        public static void Close(string id)
        {
            if (UiPanels.ContainsKey(id)) UiPanels[id].DisablePanel();
            if (!AnyOpen()) _handlingUi = false;
        }

        public static bool AnyOpen()
        {
            return UiPanels.Max(ui => ui.Value.Active);
        }

        public static void CloseAll()
        {
            UiPanels.ForEach(ui => ui.Value.DisablePanel());
            _handlingUi = false;
        }

        public static bool IsTop(MUiState state, Vec2 pos)
        {
            foreach (var ui in ReverseUpdateOrder.Select(id => UiPanels[id]))
            {
                if (ui == state) return true;
                if (ui.Active && ui.IsOverlapping(pos)) return false;
            }
            return true;
        }

        public static void SetTop(MUiState state)
        {
            UpdateOrder.Remove(state.id);
            ReverseUpdateOrder.Remove(state.id);
            UpdateOrder.Add(state.id);
            ReverseUpdateOrder.Insert(0,state.id);
        }

        public static void SetCol(string id, UiColorArea area, Color col)
        {
            if (UiPanels.ContainsKey(id)) UiPanels[id].SetCol(area, col);
        }


        [MEventWorldDrawHud]
        public static void UpdateUi()
        {
            if (!_handlingUi) return;
            UpdateOrder.ForEach(id => UiPanels[id].UpdatePanel());
            DrawMouse();
        }

        [MEventKeyTyped]
        public static void SendUiKeyEvent(MEventKeyTyped e)
        {
            if(!_handlingUi) return;
            List<string> tempUpdateOrder = UpdateOrder.GetRange(0, UpdateOrder.Count);
            tempUpdateOrder.ForEach(id => UiPanels[id].HandleKeyEvent(e));
        }

        [MEventMouseAction]
        public static void SendUiMouseEvent(MEventMouseAction e)
        {
            if(!_handlingUi) return;
            List<string> tempUpdateOrder = UpdateOrder.GetRange(0, UpdateOrder.Count);
            tempUpdateOrder.ForEach(id => UiPanels[id].HandleMouseEvent(e));
            
        }

        public static void HideMouse()
        {
            _shouldDrawMouse = false;
        }

        public static void ShowMouse()
        {
            _shouldDrawMouse = true;
        }

        private static void DrawMouse()
        {
            if (!_shouldDrawMouse) return;
            Vec2 pos = MInputHandler.MousePositionGame;
            //TODO: Different mouse icons depending on what's hovered currently
            MRenderer.DrawSprite(Mouse, pos, GlobalUiScale);
        }

        private static void LoadAutoPanels()
        {
            IEnumerable<Type> types = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.GetCustomAttributes(typeof(MAutoUiAttribute), false).FirstOrDefault() !=
                            null);
            foreach (Type t in types)
            {
                MLogger.Log("Attempting to auto-generate UI panel from class " + t.Name, logSection: ".UI");
                bool panel = TryGeneratePanel(t, (MAutoUiAttribute) Attribute.GetCustomAttribute(t, typeof(MAutoUiAttribute)));
                if (panel)
                {
                    MLogger.Log("Generated UI panel for " + t.Name, logSection: ".UI");
                    return;
                }

                MLogger.Log("Failed to generate UI panel for " + t.Name, MLogger.LogType.Warning, ".UI");
            }
        }

        private static bool TryGeneratePanel(Type type, MAutoUiAttribute attribute)
        {

            MDefaultUiContainer container = new MDefaultUiContainer(Vec2.One, Vec2.One);
            container.EnableAlwaysAutoResize(attribute.RowWidth);
            
            if (!type.IsClass)
            {
                MLogger.Log("Type is not a class!", MLogger.LogType.Warning, ".UI");
                return false;
            }
            

            foreach (MemberInfo info in type.GetMembers())
            {
                Attribute att = Attribute.GetCustomAttribute(info,typeof(UiElementAttribute));
                if (att is null) continue;
                
                switch (info.MemberType)
                {
                    case MemberTypes.Field when !((FieldInfo) info).IsStatic:
                        MLogger.Log("Field " + info.Name + " was not static, skipping!", MLogger.LogType.Warning, ".UI");
                        continue;
                    case MemberTypes.Method when !((MethodInfo) info).IsStatic:
                        MLogger.Log("Method " + info.Name + " was not static, skipping!", MLogger.LogType.Warning, ".UI");
                        continue;
                }

                Type t = att.GetType();

                if (t == typeof(MUiActionButtonAttribute) && info.MemberType == MemberTypes.Method)
                {
                    container.AddElement(new MDefaultUiActionButtonElement(Vec2.One, (MethodInfo)info));
                }

                if (t == typeof(MUiColorPickerAttribute) && info.MemberType == MemberTypes.Field)
                {
                    container.AddElement(new MDefaultUiColorPickerElement(Vec2.One, (FieldInfo) info));
                }

                if (t == typeof(MUiEnumSwitcherAttribute) && info.MemberType == MemberTypes.Field)
                {
                    container.AddElement(new MDefaultUiEnumSwitcherElement(Vec2.One, (FieldInfo) info));
                }

                if (t == typeof(MUiSliderAttribute) && info.MemberType == MemberTypes.Field)
                {
                    container.AddElement(new MDefaultUiSliderElement(Vec2.One, (FieldInfo) info));
                }

                if (t == typeof(MUiTextDisplayBoxAttribute) && info.MemberType == MemberTypes.Field)
                {
                    container.AddElement(new MDefaultUiTextDisplayBoxElement(Vec2.One, Vec2.One, (FieldInfo) info));
                }

                if (t == typeof(MUiToggleAttribute) && info.MemberType == MemberTypes.Field)
                {
                    container.AddElement(new MDefaultUiToggleElement(Vec2.One, (FieldInfo) info));
                }

                if (t == typeof(MUiValueScrollerAttribute) && info.MemberType == MemberTypes.Field)
                {
                    container.AddElement(new MDefaultUiValueScrollerElement(Vec2.One, (FieldInfo) info));
                }
            }

            if(attribute.AutoArrangeElements) container.AutoSortElements(attribute.UiArrangement);
            
            AddPanel(attribute.PanelName, container, type);

            return true;
        }


    }
}