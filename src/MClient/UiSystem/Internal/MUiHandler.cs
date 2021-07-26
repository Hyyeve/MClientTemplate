using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DuckGame;
using MClient.Core;
using MClient.Core.EventSystem.Events.Drawing.Screen;
using MClient.Core.EventSystem.Events.Drawing.World;
using MClient.Core.EventSystem.Events.Helper;
using MClient.Core.EventSystem.Events.Input;
using MClient.InputSystem;
using MClient.RenderSystem;
using MClient.UiSystem.Default;
using MClient.UiSystem.Internal.Attributes;
using MClient.UiSystem.Internal.Components;
using MoreLinq;

namespace MClient.UiSystem.Internal
{
    /// <summary>
    /// Core handler for all Ui
    /// </summary>
    [MAutoRegisterEvents]
    public static class MUiHandler
    {
        public const float GlobalUiScale = 0.5f;

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

        /// <summary>
        /// Adds a panel, given an ID and Ui Container.
        /// </summary>
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

        /// <summary>
        /// Removes a panel with the given ID
        /// </summary>
        public static void RemovePanel(string id)
        {
            UiPanels.Remove(id);
            UpdateOrder.Remove(id);
            ReverseUpdateOrder.Remove(id);
        }
        
        /// <summary>
        /// Checks if a panel with the given ID is open
        /// </summary>
        public static bool IsOpen(string id)
        {
            return UiPanels.ContainsKey(id) && UiPanels[id].Active;
        }

        /// <summary>
        /// Opens the panel with the given ID
        /// </summary>
        public static void Open(string id)
        {
            if(UiPanels.ContainsKey(id)) UiPanels[id].EnablePanel();
            SetTop(UiPanels[id]);
            _handlingUi = true;
        }

        /// <summary>
        /// Closes the panel with the given ID
        /// </summary>
        public static void Close(string id)
        {
            if (UiPanels.ContainsKey(id)) UiPanels[id].DisablePanel();
            if (!AnyOpen()) _handlingUi = false;
        }

        /// <summary>
        /// Checks if any Ui panels are open
        /// </summary>
        public static bool AnyOpen()
        {
            return UiPanels.Max(ui => ui.Value.Active);
        }

        /// <summary>
        /// Closes all Ui panels.
        /// </summary>
        public static void CloseAll()
        {
            UiPanels.ForEach(ui => ui.Value.DisablePanel());
            _handlingUi = false;
        }

        /// <summary>
        /// Checks if a UiState is the topmost at the given position (visually)
        /// </summary>
        public static bool IsTop(MUiState state, Vec2 pos)
        {
            foreach (var ui in ReverseUpdateOrder.Select(id => UiPanels[id]))
            {
                if (ui == state) return true;
                if (ui.Active && ui.IsOverlapping(pos)) return false;
            }
            return true;
        }

        /// <summary>
        /// Sets the given UiState to be the topmost (visually)
        /// </summary>
        public static void SetTop(MUiState state)
        {
            UpdateOrder.Remove(state.Id);
            ReverseUpdateOrder.Remove(state.Id);
            UpdateOrder.Add(state.Id);
            ReverseUpdateOrder.Insert(0,state.Id);
        }

        /// <summary>
        /// Sets the base colour for a panel
        /// </summary>
        public static void SetCol(string id, MUiColorArea area, Color col)
        {
            if (UiPanels.ContainsKey(id)) UiPanels[id].SetCol(area, col);
        }


        /// <summary>
        /// Internal update call for Ui
        /// </summary>
        [MEventScreenDrawHud]
        public static void UpdateUi()
        {
            if (!_handlingUi) return;
            UpdateOrder.ForEach(id => UiPanels[id].UpdatePanel());
            DrawMouse();
        }

        /// <summary>
        /// Internal update call for Ui
        /// </summary>
        [MEventKeyTyped]
        public static void SendUiKeyEvent(MEventKeyTyped e)
        {
            if(!_handlingUi) return;
            List<string> tempUpdateOrder = UpdateOrder.GetRange(0, UpdateOrder.Count);
            tempUpdateOrder.ForEach(id => UiPanels[id].HandleKeyEvent(e));
        }


        /// <summary>
        /// Internal update call for Ui
        /// </summary>
        [MEventMouseAction]
        public static void SendUiMouseEvent(MEventMouseAction e)
        {
            if(!_handlingUi) return;
            List<string> tempUpdateOrder = UpdateOrder.GetRange(0, UpdateOrder.Count);
            tempUpdateOrder.ForEach(id => UiPanels[id].HandleMouseEvent(e));
            
        }

        /// <summary>
        /// Disables the ui mouse
        /// </summary>
        public static void HideMouse()
        {
            _shouldDrawMouse = false;
        }

        /// <summary>
        /// Enables the ui mouse
        /// </summary>
        public static void ShowMouse()
        {
            _shouldDrawMouse = true;
        }

        private static void DrawMouse()
        {
            if (!_shouldDrawMouse) return;
            var pos = MInputHandler.MousePositionGame;
            //TODO: Different mouse icons depending on what's hovered currently
            MRenderer.DrawSprite(Mouse, pos, GlobalUiScale);
        }

        private static void LoadAutoPanels()
        {
            IEnumerable<Type> types = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.GetCustomAttributes(typeof(MAutoUiAttribute), false).FirstOrDefault() !=
                            null);
            foreach (var t in types)
            {
                MLogger.Log("Attempting to auto-generate UI panel from class " + t.Name, logSection: MLogger.MLogSection.UsrI);
                bool panel = TryGeneratePanel(t, (MAutoUiAttribute) Attribute.GetCustomAttribute(t, typeof(MAutoUiAttribute)));
                if (panel)
                {
                    MLogger.Log("Generated UI panel for " + t.Name, logSection: MLogger.MLogSection.UsrI);
                    return;
                }

                MLogger.Log("Failed to generate UI panel for " + t.Name, MLogger.MLogType.Warning, MLogger.MLogSection.UsrI);
            }
        }

        private static bool TryGeneratePanel(Type type, MAutoUiAttribute attribute)
        {

            var container = new MDefaultUiContainer(Vec2.One, Vec2.One);
            container.EnableAlwaysAutoResize(attribute.RowWidth);
            
            if (!type.IsClass)
            {
                MLogger.Log("Type is not a class!", MLogger.MLogType.Warning, MLogger.MLogSection.UsrI);
                return false;
            }
            

            foreach (var info in type.GetMembers())
            {
                var att = Attribute.GetCustomAttribute(info,typeof(MUiElementAttribute));
                if (att is null) continue;
                
                switch (info.MemberType)
                {
                    case MemberTypes.Field when !((FieldInfo) info).IsStatic:
                        MLogger.Log("Field " + info.Name + " was not static, skipping!", MLogger.MLogType.Warning, MLogger.MLogSection.UsrI);
                        continue;
                    case MemberTypes.Method when !((MethodInfo) info).IsStatic:
                        MLogger.Log("Method " + info.Name + " was not static, skipping!", MLogger.MLogType.Warning, MLogger.MLogSection.UsrI);
                        continue;
                }

                var t = att.GetType();

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