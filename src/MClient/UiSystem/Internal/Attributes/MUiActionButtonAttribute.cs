using System;

namespace MClient.UiSystem.Internal.Attributes
{
    /// <summary>
    /// Auto-Attribute that marks a method to be bound to a Ui Button. Used in combination with the AutoUi attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MUiActionButtonAttribute : MUiElementAttribute
    {
        public readonly string TitleOverride;

        public MUiActionButtonAttribute(string titleOverride = "")
        {
            TitleOverride = titleOverride;
        }
    }
}