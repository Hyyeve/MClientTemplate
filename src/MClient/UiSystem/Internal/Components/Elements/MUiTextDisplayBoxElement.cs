using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DuckGame;
using MClient.Core.EventSystem.Events.Input;
using MClient.Core.Utils;
using MClient.UiSystem.Internal.Attributes;

namespace MClient.UiSystem.Internal.Components.Elements
{
    /// <summary>
    /// Base class for Ui Text Display Boxes. Contains all basic functionality.
    /// </summary>
    public abstract class MUiTextDisplayBoxElement : MUiFieldElement
    {
        protected readonly float Padding;
        protected readonly string Title;
        protected readonly string LinePrefix;
        protected readonly float TextScale = 0.8f;
        protected Vec2 TextStartPos;
        protected Vec2 TitlePos;
        
        protected readonly List<string> DrawList = new List<string>();

        private float _textStartOffset;

    
        protected MUiTextDisplayBoxElement(Vec2 pos, Vec2 size, FieldInfo field) : base(pos, size, field)
        {
            Padding = 1f * UiScale;
            var attribute = (MUiTextDisplayBoxAttribute) GetAttribute(field);
            Title = attribute.TitleOverride == "" ? field.Name : attribute.TitleOverride;
            Size = attribute.Size;
            Size.x = MMathUtils.Max(Size.x, Graphics.GetStringWidth(Title) * UiScale + Padding * 8f);
            LinePrefix = attribute.LinePrefix;
        }

        protected MUiTextDisplayBoxElement(Vec2 pos, Vec2 size, FieldInfo field, float padding) : base(pos, size, field)
        {
            Padding = padding * UiScale;
            var attribute = (MUiTextDisplayBoxAttribute) GetAttribute(field);
            Title = attribute.TitleOverride == "" ? field.Name : attribute.TitleOverride;
            Size = attribute.Size;
            Size.x = MMathUtils.Max(Size.x, Graphics.GetStringWidth(Title) * UiScale + Padding * 8f);
            LinePrefix = attribute.LinePrefix;
        }

        private static Attribute GetAttribute(MemberInfo field)
        {
            return Attribute.GetCustomAttribute(field, typeof(MUiTextDisplayBoxAttribute));
        }


        public override void HandleMouseEvent(MEventMouseAction e) { }


        public override void HandleKeyTypedEvent(MEventKeyTyped e) { }

      
        protected override void VerifyFieldInfo(FieldInfo fieldInfo)
        {
            if (fieldInfo.FieldType != typeof(List<string>))
                throw new Exception("UiTextDisplayBox field was not a List<string>! " 
                                    + fieldInfo.DeclaringType?.Name + "." + fieldInfo.Name);
            if (!fieldInfo.IsStatic)
                throw new Exception("UiTextDisplayBox field isn't static!" + fieldInfo.DeclaringType?.Name +
                                    "." + fieldInfo.Name);
        }

        private IEnumerable<string> GetVal()
        {
            return (List<string>) AttatchedField.GetValue(null);
        }

      
        protected override void Arrange()
        {
            if (!NeedsArranging) return;
            TitlePos = Position + Vec2.One * Padding * 4f;
            TextStartPos = TitlePos + Vec2.Unity * Graphics.GetStringHeight(Title) * UiScale + Padding * 4f * Vec2.Unity;
            _textStartOffset = Graphics.GetStringHeight(Title) * UiScale + Padding * 4f;
            NeedsArranging = false;
        }

  
        protected override void Update()
        {
            DrawList.Clear();

            float y = _textStartOffset + Padding * 24f;
            foreach (string s in GetVal())
            {
                if (CheckStringLength(s))
                {
                    List<string> strings = s.Split(' ').ToList();
                    if (strings.Count == 1)
                    {
                        while (CheckStringLength(strings[0]))
                        {
                            strings[0] = strings[0].Substring(0, strings[0].Length - 1);
                        }
                        strings[0] = strings[0].Substring(0, strings[0].Length - 2) + "..";
                    }
                    else
                    {
                        for (int i = 0; i < strings.Count; i++)
                        {
                            while (i <= strings.Count - 2 && !CheckStringLength(strings[i] + strings[i + 1] + " "))
                            {
                                strings[i] = strings[i] + " " + strings[i + 1];
                                strings.Remove(strings[i + 1]);
                            }
                        }
                    }

                    strings[0] = strings[0].Insert(0, LinePrefix);
                    DrawList.AddRange(strings);
                    y += (Graphics.GetStringHeight(s) * UiScale * TextScale) * strings.Count;
                }
                else
                {
                    string toAdd = s.Insert(0, LinePrefix);
                    DrawList.Add(toAdd);
                    y += Graphics.GetStringHeight(toAdd) * UiScale * TextScale;
                }
                if(y > Size.y) DrawList.RemoveAt(0);
            }
        }

        private bool CheckStringLength(string s)
        {
            return Graphics.GetStringWidth(s + LinePrefix) * UiScale * TextScale > Size.x - Padding * UiScale * 8f;
        }
    }
}