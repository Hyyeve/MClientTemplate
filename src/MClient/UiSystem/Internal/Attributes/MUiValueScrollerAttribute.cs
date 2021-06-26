using System;

namespace MClient.UiSystem.Internal.Attributes
{
    /// <summary>
    /// Auto-Attribute that marks a field to be bound to a Ui Value Scroller. Used in combination with the AutoUi attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class MUiValueScrollerAttribute : MUiElementAttribute
    {
        public readonly string TitleOverride;

        public MUiValueScrollerAttribute(string titleOverride = "")
        {
            TitleOverride = titleOverride;
        }
    }
}