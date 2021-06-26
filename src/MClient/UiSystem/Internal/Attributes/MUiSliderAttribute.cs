using System;

namespace MClient.UiSystem.Internal.Attributes
{
    [AttributeUsage(AttributeTargets.Field)] 
    public class MUiSliderAttribute : UiElementAttribute
    {
        public readonly double Max;
        public readonly double Min;
        public readonly string TitleOverride;

        public MUiSliderAttribute(double min, double max, string titleOverride = "")
        {
            Max = max;
            Min = min;
            TitleOverride = titleOverride;
        }
    }
}