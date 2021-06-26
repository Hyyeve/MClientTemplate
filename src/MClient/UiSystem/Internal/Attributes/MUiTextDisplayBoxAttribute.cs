using System;
using DuckGame;

namespace MClient.UiSystem.Internal.Attributes
{
    /// <summary>
    /// Auto-Attribute that marks a field to be bound to a Ui Text Display Box. Used in combination with the AutoUi attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class MUiTextDisplayBoxAttribute : MUiElementAttribute
    {
        public readonly string TitleOverride;
        public readonly string LinePrefix;
        public readonly Vec2 Size;

        public MUiTextDisplayBoxAttribute(float xSize, float ySize, string titleOverride = "", string linePrefix = ">")
        {
            TitleOverride = titleOverride;
            LinePrefix = linePrefix;
            Size = new Vec2(xSize, ySize);
        }

        
    }
}