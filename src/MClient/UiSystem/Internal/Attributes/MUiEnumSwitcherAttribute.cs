using System;

namespace MClient.UiSystem.Internal.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class MUiEnumSwitcherAttribute : UiElementAttribute
    {
        public readonly string TitleOverride;

        public MUiEnumSwitcherAttribute(string titleOverride = "")
        {
            TitleOverride = titleOverride;
        }
    }
}