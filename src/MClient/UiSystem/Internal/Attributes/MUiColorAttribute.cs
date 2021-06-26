using System;
using DuckGame;

namespace MClient.UiSystem.Internal.Attributes
{
    /// <summary>
    /// Auto-Attribute to be used in combination with other Auto-Ui elements. Allows you to set custom colours for specific Ui Elements.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class MUiColorAttribute : Attribute
    {
        public readonly MUiColorArea ColorArea;
        public readonly Color Color;
        
        public MUiColorAttribute(int r, int g, int b, MUiColorArea area)
        {
            Color = new Color(r, g, b);
            ColorArea = area;
        }
    }
    
    public enum MUiColorArea
    {
        Base, BaseAccent, Text, TextAccent,
    }
}