using System;
using DuckGame;

namespace MClient.UiSystem.Internal.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = true)]
    public class MUiColorAttribute : Attribute
    {
        public readonly UiColorArea ColorArea;
        public readonly Color Color;
        
        public MUiColorAttribute(int r, int g, int b, UiColorArea area)
        {
            Color = new Color(r, g, b);
            ColorArea = area;
        }
    }
    
    public enum UiColorArea
    {
        Base, BaseAccent, Text, TextAccent,
    }
}