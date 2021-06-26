using System.Collections.Generic;
using DuckGame;
using MClient.Core.EventSystem.Events.Helper;
using MClient.InputSystem;
using MClient.UiSystem.Internal;
using MClient.UiSystem.Internal.Attributes;
using MClient.UiSystem.Internal.Components;

namespace MClient.UiSystem
{

    [MAutoUi("Test UI", 3, true, MUiArrangement.WIDTH_INCREASING)]
    public static class MTestUi
    {
        
        [MUiToggle("BoolVal")]
        public static bool testbool;
        
        [MUiSlider(0, 10, "FloatVal")]
        public static int testfloat;

        [MUiValueScroller("ScrollVal")] 
        public static int testfloat2;

        [MUiColorPicker("ColourVal")] 
        public static Color testcol;

        [MUiEnumSwitcher("EnumVal")]
        public static TestEnum testenum;
        
        [MUiTextDisplayBox(100,100, "ListVal")] 
        public static List<string> testlist = new List<string>();
        

        [MUiActionButton("Click Me!")]
        public static void domorestuff()
        {
            MUiHandler.SetCol("Test UI", UiColorArea.Base, testcol);
        }

        [MEventLateInit]
        [MInputBinding(new[] {Keys.LeftControl, Keys.RightControl}, MBindPressReq.AllPressed, MBindOrderReq.InOrder)]
        public static void stuff()
        {
            MUiHandler.Open("Test UI");
        }
        
        public enum TestEnum
        {
            A,B,C,HELLO,D42069
        }
    }
}