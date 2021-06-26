using System;

namespace MClient.UiSystem.Internal.Attributes
{
    /// <summary>
    /// Auto-Attribute that marks a field to be bound to a Ui Slider. Used in combination with the AutoUi attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)] 
    public class MUiSliderAttribute : MUiElementAttribute
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