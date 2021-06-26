using System;

namespace MClient.UiSystem.Internal.Attributes
{
    /// <summary>
    /// Auto-Attribute that marks a field to be bound to a Ui Enum Switcher. Used in combination with the AutoUi attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class MUiEnumSwitcherAttribute : MUiElementAttribute
    {
        public readonly string TitleOverride;

        public MUiEnumSwitcherAttribute(string titleOverride = "")
        {
            TitleOverride = titleOverride;
        }
    }
}