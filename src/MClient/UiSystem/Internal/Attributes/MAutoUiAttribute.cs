using System;
using MClient.UiSystem.Internal.Components;

namespace MClient.UiSystem.Internal.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MAutoUiAttribute : Attribute
    {

        public readonly string PanelName;
        public readonly int RowWidth;
        public readonly bool AutoArrangeElements;
        public readonly MUiArrangement UiArrangement;
        
        public MAutoUiAttribute(string panelName, int rowWidth = 2, bool autoArrangeElements = true, MUiArrangement arrangement = MUiArrangement.HEIGHT_INCREASING)
        {
            PanelName = panelName;
            RowWidth = rowWidth;
            AutoArrangeElements = autoArrangeElements;
            UiArrangement = arrangement;
        }
        
    }
}