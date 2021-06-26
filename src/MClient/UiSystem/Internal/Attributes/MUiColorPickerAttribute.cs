using System;

namespace MClient.UiSystem.Internal.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class MUiColorPickerAttribute : UiElementAttribute
    {
        public readonly string TitleOverride;

        public MUiColorPickerAttribute(string titleOverride = "")
        {
            TitleOverride = titleOverride;
        }
    }
}