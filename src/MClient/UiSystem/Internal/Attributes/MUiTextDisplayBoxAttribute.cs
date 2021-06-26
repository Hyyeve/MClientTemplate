using System;
using DuckGame;

namespace MClient.UiSystem.Internal.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class MUiTextDisplayBoxAttribute : UiElementAttribute
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