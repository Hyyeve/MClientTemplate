using System;
using MClient.UiSystem.Internal.Components;

namespace MClient.UiSystem.Internal.Attributes
{
    /// <summary>
    /// Auto-Attribute that marks a class for automatic UI generation
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MAutoUiAttribute : Attribute
    {

        public readonly string PanelName;
        public readonly int RowWidth;
        public readonly bool AutoArrangeElements;
        public readonly MUiArrangement UiArrangement;
        
        public MAutoUiAttribute(string panelName, int rowWidth = 2, bool autoArrangeElements = true, MUiArrangement arrangement = MUiArrangement.HeightIncreasing)
        {
            PanelName = panelName;
            RowWidth = rowWidth;
            AutoArrangeElements = autoArrangeElements;
            UiArrangement = arrangement;
        }
        
    }
}