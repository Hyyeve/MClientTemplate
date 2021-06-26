using DuckGame;
using MClient.Core.EventSystem.Events.Input;

namespace MClient.UiSystem.Internal.Components
{
    /// <summary>
    /// Base class for most of my Ui system
    /// </summary>
    public abstract class MAmUi
    {
        public event MOnChanged OnTransformChanged;
        
        public delegate void MOnChanged();
        
        public float LeftEdge => GetPos().x;
        public float RightEdge => GetPos().x + GetSize().x;
        public float TopEdge => GetPos().y;
        public float BottomEdge => GetPos().y + GetSize().y;
        public float Width => GetSize().x;
        public float Height => GetSize().y;
        
        /// <summary>
        /// This method ALSO HANDLES DRAWING - Do NOT call with a normal Update event.
        /// </summary>
        public abstract void HandleUiUpdate();

        public abstract void HandleMouseEvent(MEventMouseAction e);
        public abstract void HandleKeyTypedEvent(MEventKeyTyped e);
        public abstract void Draw();
        public abstract Vec2 GetPos();
        public abstract Vec2 GetSize();
        public virtual void SetPos(Vec2 pos)
        {
            OnTransformChanged?.Invoke();
        }

        public virtual void SetSize(Vec2 size, bool scaled)
        {
            OnTransformChanged?.Invoke();
        }
        public abstract bool IsOverlapping(Vec2 pos);
        public abstract void SetOwningState(MUiState state);
        public abstract MUiState GetOwningState();

        public abstract void UpdateCols();
    }
}