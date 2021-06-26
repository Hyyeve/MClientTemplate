using System;

namespace MClient.UiSystem.Internal.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class MUiValueScrollerAttribute : UiElementAttribute
    {
        public readonly string TitleOverride;

        public MUiValueScrollerAttribute(string titleOverride = "")
        {
            TitleOverride = titleOverride;
        }
    }
}