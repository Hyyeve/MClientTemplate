using System;

namespace MClient.UiSystem.Internal.Attributes
{
    [AttributeUsage(AttributeTargets.Field)] 
    public class MUiToggleAttribute : UiElementAttribute
    {
        public readonly string TitleOverride;
        public MUiToggleAttribute(string titleOverride = "")
        {
            TitleOverride = titleOverride;
        }
        
    }
}