using System;
using System.Reflection;
using DuckGame;
using MClient.EventSystem.Events.Input;
using MClient.UiSystem.Internal.Attributes;

namespace MClient.UiSystem.Internal.Components.Elements
{
    public abstract class MUiActionButtonElement : MUiMethodElement
    {

        protected readonly float Padding;
        protected readonly string Title;
        protected bool Pressed;
        
        /// <inheritdoc />
        public MUiActionButtonElement(Vec2 pos, Vec2 size, MethodInfo method) : base(pos, size, method)
        {
            Padding = 1f * UiScale;
            Title = GetName(method);
            size = size * UiScale;
            size.x = Graphics.GetStringWidth(Title) * UiScale + Padding * 4f;
            SetSize(size, true);
        }

        public MUiActionButtonElement(Vec2 pos, Vec2 size, MethodInfo method, float padding) : base(pos, size, method)
        {
            Padding = padding * UiScale;
            Title = GetName(method);
            size = size * UiScale;
            size.x = Graphics.GetStringWidth(Title) * UiScale + Padding * 4f;
            SetSize(size, true);
        }

        private string GetName(MethodInfo method)
        {
            Attribute att = Attribute.GetCustomAttribute(method, typeof(MUiActionButtonAttribute));
            return att != null
                ? ((MUiActionButtonAttribute) att).TitleOverride == "" ? method.Name : ((MUiActionButtonAttribute) att).TitleOverride
                : method.Name;
        }
        
        /// <inheritdoc />
        public override void HandleMouseEvent(MEventMouseAction e)
        {
            if(!IsOverlapping(e.MousePosGame)) return;
            switch (e.Action)
            {
                case MouseAction.LeftPressed:
                    Pressed = true;
                    CallAction();
                    break;
                case MouseAction.LeftReleased:
                    Pressed = false;
                    break;
            }
        }

        private void CallAction()
        {
            AttatchedMethod.Invoke(null, null);
        }

        /// <inheritdoc />
        public override void HandleKeyTypedEvent(MEventKeyTyped e) { }
        

        /// <inheritdoc />
        protected override void VerifyMethodInfo(MethodInfo methodInfo)
        {
            if (methodInfo.GetParameters().Length > 0)
                throw new Exception("UiActionButtonElement method has parameters!" + methodInfo.DeclaringType.Name +
                                    "." + methodInfo.Name);
            if (!methodInfo.IsStatic)
                throw new Exception("UiActionButtonElement method isn't static!" + methodInfo.DeclaringType.Name +
                                    "." + methodInfo.Name);
        }

        /// <inheritdoc />
        protected override void Arrange()
        {
            
        }

        /// <inheritdoc />
        protected override void Update() { }
    }
}