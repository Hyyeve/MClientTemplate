using System;

namespace MClient.UiSystem.Internal.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MUiActionButtonAttribute : UiElementAttribute
    {
        public readonly string TitleOverride;

        public MUiActionButtonAttribute(string titleOverride = "")
        {
            TitleOverride = titleOverride;
        }
    }
}