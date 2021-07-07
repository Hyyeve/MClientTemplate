using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DuckGame;
using MClient.Core.EventSystem.Events.Input;
using MClient.RenderSystem;
using MClient.UiSystem.Internal.Attributes;
using MoreLinq;

namespace MClient.UiSystem.Internal.Components.Elements
{
    /// <summary>
    /// Base class for Ui Enum Switchers. Contains all basic functionality.
    /// </summary>
    public abstract class MUiEnumSwitcherElement : MUiFieldElement
    {
        protected readonly float Padding;
        protected readonly string Title;
        protected string EnumMember;
        protected bool Pressed;
        private List<string> _enumMembers;
        
        
        protected MUiEnumSwitcherElement(Vec2 pos, Vec2 size, FieldInfo field) : base(pos, size, field)
        {
            Padding = 1f * UiScale;
            Title = GetName(field);
            size *= UiScale;
            size.x = MRenderer.GetStringWidth(Title + LongestEnumMember(field) + " ") * UiScale + Padding * 4f;
            SetSize(size, true);
        }

        private string LongestEnumMember(FieldInfo field)
        {
            VerifyFieldInfo(field);
            _enumMembers = Enum.GetNames(field.FieldType).ToList();
            return _enumMembers.MaxBy(e => e.Length).First();
        }

        protected MUiEnumSwitcherElement(Vec2 pos, Vec2 size, FieldInfo field, float padding) : base(pos, size, field)
        {
            Padding = padding * UiScale;
            Title = GetName(field);
            size = size * UiScale;
            size.x = MRenderer.GetStringWidth(Title + LongestEnumMember(field) + " ") * UiScale + Padding * 4f;
            SetSize(size, true);
        }

        private static string GetName(FieldInfo field)
        {
            var att = Attribute.GetCustomAttribute(field, typeof(MUiEnumSwitcherAttribute));
            return att != null
                ? ((MUiEnumSwitcherAttribute) att).TitleOverride == "" ? field.Name : ((MUiEnumSwitcherAttribute) att).TitleOverride
                : field.Name;
        }
        
        public override void HandleMouseEvent(MEventMouseAction e)
        {
            if(!IsOverlapping(e.MousePosGame)) return;
            switch (e.Action)
            {
                case MMouseAction.LeftPressed:
                    Pressed = true;
                    RotateEnum(1);
                    break;
                case MMouseAction.LeftReleased:
                    Pressed = false;
                    break;
                case MMouseAction.RightPressed:
                    Pressed = true;
                    RotateEnum(-1);
                    break;
                case MMouseAction.RightReleased:
                    Pressed = false;
                    break;
            }
        }

        private void RotateEnum(int dir)
        {
            int current = _enumMembers.IndexOf(EnumMember);
            current += dir;
            if (current < 0) current = _enumMembers.Count - 1;
            if (current > _enumMembers.Count-1) current = 0;
            EnumMember = _enumMembers[current];
            //dynamics are super funky and fun!
            var value = Enum.Parse(AttatchedField.FieldType, EnumMember);
            AttatchedField.SetValue(null,value);
        }
        
        public override void HandleKeyTypedEvent(MEventKeyTyped e) { }
        
        
        protected override void VerifyFieldInfo(FieldInfo fieldInfo)
        {
            if (!fieldInfo.FieldType.IsEnum)
                throw new Exception("UiEnumSwitcherElement field was not a enum!"
                                    + fieldInfo.DeclaringType?.Name + "." + fieldInfo.Name);
            if (!fieldInfo.IsStatic)
                throw new Exception("UiEnumSwitcherElement field isn't static!" + fieldInfo.DeclaringType?.Name +
                                    "." + fieldInfo.Name);
        }

        protected override void Arrange()
        {
            
        }
        
        protected override void Update()
        {
            EnumMember = GetCurrentMember();
        }

        private string GetCurrentMember()
        {
            return Enum.GetName(AttatchedField.FieldType, AttatchedField.GetValue(null));
        }
    }
}