using System;

namespace MClient.UiSystem.Internal.Attributes
{
    /// <summary>
    /// Auto-Attribute that marks a field to be bound to a Ui Toggle. Used in combination with the AutoUi attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)] 
    public class MUiToggleAttribute : MUiElementAttribute
    {
        public readonly string TitleOverride;
        public MUiToggleAttribute(string titleOverride = "")
        {
            TitleOverride = titleOverride;
        }
        
    }
}