using System;

namespace MClient.UiSystem.Internal.Attributes
{
    /// <summary>
    /// Auto-Attribute that marks a field to be bound to a Ui Color Picker. Used in combination with the AutoUi attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class MUiColorPickerAttribute : MUiElementAttribute
    {
        public readonly string TitleOverride;

        public MUiColorPickerAttribute(string titleOverride = "")
        {
            TitleOverride = titleOverride;
        }
    }
}