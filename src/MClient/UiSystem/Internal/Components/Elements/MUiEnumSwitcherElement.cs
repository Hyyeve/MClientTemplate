using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DuckGame;
using MClient.EventSystem.Events.Input;
using MClient.UiSystem.Internal.Attributes;
using MoreLinq;

namespace MClient.UiSystem.Internal.Components.Elements
{
    public abstract class MUiEnumSwitcherElement : MUiFieldElement
    {
        protected readonly float Padding;
        protected readonly string Title;
        protected string EnumMember;
        protected bool Pressed;
        private List<String> EnumMembers;
        
        /// <inheritdoc />
        public MUiEnumSwitcherElement(Vec2 pos, Vec2 size, FieldInfo field) : base(pos, size, field)
        {
            Padding = 1f * UiScale;
            Title = GetName(field);
            size = size * UiScale;
            size.x = Graphics.GetStringWidth(Title + LongestEnumMember(field) + " ") * UiScale + Padding * 4f;
            SetSize(size, true);
        }

        private string LongestEnumMember(FieldInfo field)
        {
            VerifyFieldInfo(field);
            EnumMembers = Enum.GetNames(field.FieldType).ToList();
            return EnumMembers.MaxBy(e => e.Length).First();
        }

        public MUiEnumSwitcherElement(Vec2 pos, Vec2 size, FieldInfo field, float padding) : base(pos, size, field)
        {
            Padding = padding * UiScale;
            Title = GetName(field);
            size = size * UiScale;
            size.x = Graphics.GetStringWidth(Title + LongestEnumMember(field) + " ") * UiScale + Padding * 4f;
            SetSize(size, true);
        }

        private string GetName(FieldInfo field)
        {
            Attribute att = Attribute.GetCustomAttribute(field, typeof(MUiEnumSwitcherAttribute));
            return att != null
                ? ((MUiEnumSwitcherAttribute) att).TitleOverride == "" ? field.Name : ((MUiEnumSwitcherAttribute) att).TitleOverride
                : field.Name;
        }
        
        /// <inheritdoc />
        public override void HandleMouseEvent(MEventMouseAction e)
        {
            if(!IsOverlapping(e.MousePosGame)) return;
            switch (e.Action)
            {
                case MouseAction.LeftPressed:
                    Pressed = true;
                    RotateEnum(1);
                    break;
                case MouseAction.LeftReleased:
                    Pressed = false;
                    break;
                case MouseAction.RightPressed:
                    Pressed = true;
                    RotateEnum(-1);
                    break;
                case MouseAction.RightReleased:
                    Pressed = false;
                    break;
            }
        }

        private void RotateEnum(int dir)
        {
            int current = EnumMembers.IndexOf(EnumMember);
            current += dir;
            if (current < 0) current = EnumMembers.Count - 1;
            if (current > EnumMembers.Count-1) current = 0;
            EnumMember = EnumMembers[current];
            //dynamics are super funky and fun!
            object value = Enum.Parse(AttatchedField.FieldType, EnumMember);
            AttatchedField.SetValue(null,value);
        }

        /// <inheritdoc />
        public override void HandleKeyTypedEvent(MEventKeyTyped e) { }
        

        /// <inheritdoc />
        protected override void VerifyFieldInfo(FieldInfo fieldInfo)
        {
            if (!fieldInfo.FieldType.IsEnum)
                throw new Exception("UiEnumSwitcherElement field was not a enum!"
                                    + fieldInfo.DeclaringType.Name + "." + fieldInfo.Name);
            if (!fieldInfo.IsStatic)
                throw new Exception("UiEnumSwitcherElement field isn't static!" + fieldInfo.DeclaringType.Name +
                                    "." + fieldInfo.Name);
        }

        /// <inheritdoc />
        protected override void Arrange()
        {
            
        }

        /// <inheritdoc />
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