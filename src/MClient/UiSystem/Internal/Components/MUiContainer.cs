using System;
using System.Collections.Generic;
using System.Linq;
using DuckGame;
using MClient.Core.Utils;
using MClient.EventSystem;
using MClient.EventSystem.Events.Input;
using MClient.InputSystem;
using MClient.Render;
using MClient.UiSystem.Internal.Attributes;
using MoreLinq;

namespace MClient.UiSystem.Internal.Components
{
    public abstract class MUiContainer : MAmUi
    {
        private MUiState State;
        
        protected Vec2 Position;
        protected Vec2 Size;
        
        private Vec2 ElementOffset;
        private int ElementRowWidth;
        private bool AlwaysAutoResize;

        protected readonly List<MAmUi> Elements;
        protected float UiScale => MUiHandler.GlobalUiScale;

        private bool needsArranging;
        private bool draggable = true;
        private bool dragging;
        private Vec2 dragOffset;
        private bool active;

        protected Color BaseColor;
        protected Color BaseAccentColor;
        protected Color TextColor;
        protected Color TextAccentColor;

        protected MUiContainer(Vec2 position, Vec2 size)
        {
            Position = MPositionConversionUtil.ScreenToGamePos(position) * UiScale;
            Size = size * UiScale;
            ElementOffset = Vec2.One * 5 * UiScale;
            Elements = new List<MAmUi>();
            needsArranging = true;
        }

        protected MUiContainer(Vec2 position, Vec2 size, Vec2 elementOffset)
        {
            Position = MPositionConversionUtil.ScreenToGamePos(position) * UiScale;
            Size = size * UiScale;
            ElementOffset = elementOffset * UiScale;
            Elements = new List<MAmUi>();
            needsArranging = true;
        }

        protected MUiContainer(Vec2 position, Vec2 size, Vec2 elementOffset, List<MAmUi>elements)
        {
            Position = MPositionConversionUtil.ScreenToGamePos(position) * UiScale;
            Size = size * UiScale;
            ElementOffset = elementOffset * UiScale;
            Elements = elements;
            needsArranging = true;
        }

        public override void SetOwningState(MUiState state)
        {
            State = state;
            Elements.ForEach(e => e.SetOwningState(state));
        }

        /// <inheritdoc />
        public override MUiState GetOwningState()
        {
            return State;
        }

        private Color GetCol(UiColorArea area)
        {
            if (State is null) return Color.White;
            return area switch
            {
                UiColorArea.Base => State.BaseCol,
                UiColorArea.BaseAccent => State.BaseAccentCol,
                UiColorArea.Text => State.TextCol,
                UiColorArea.TextAccent => State.TextAccentCol,
                _ => throw new ArgumentOutOfRangeException(nameof(area), area, null)
            };
        }

        public void Recalculate()
        {
            needsArranging = true;
        }

        public void RecalculateNow()
        {
            Recalculate();
            Arrange();
        }

        public void AddElement(MAmUi element)
        {
            Elements.Add(element);
            element.SetOwningState(State);
            element.OnTransformChanged += Recalculate;
            needsArranging = true;
        }

        public void RemoveElement(MAmUi element)
        {
            Elements.Remove(element);
            element.SetOwningState(null);
            element.OnTransformChanged -= Recalculate;
            needsArranging = true;
        }

        public void AddElements(MAmUi[] elements)
        {
            Elements.AddRange(elements);
            elements.ForEach(e => e.OnTransformChanged += Recalculate);
            Elements.ForEach(e => e.SetOwningState(State));
            needsArranging = true;
        }

        public void RemoveElements(MAmUi[] elements)
        {
            elements.ForEach(e => e.OnTransformChanged -= Recalculate);
            List<MAmUi> temp = Elements.Except(elements).ToList();
            Elements.Clear(); 
            Elements.AddRange(temp);
            needsArranging = true;
        }

        /// <inheritdoc />
        public override void HandleUiUpdate()
        {
            HandleDragging();
            Arrange();
            Draw();
            foreach (MAmUi element in Elements)
            {
                element.HandleUiUpdate();
            }
        }

        protected void Arrange()
        {
            if (!needsArranging || Elements.Count == 0) return;
            if(AlwaysAutoResize) AutoResize(ElementRowWidth);
            Vec2 elementPosition = GetPos() + ElementOffset;

            float newY = 0, newX = 0;
            for (int i = 0; i < Elements.Count; i++)
            {
                MAmUi element = Elements[i];
                element.SetPos(elementPosition);

                Vec2 nextElementSize = i < Elements.Count - 1 ? Elements[i + 1].GetSize() : Vec2.Zero;
                
                newX = elementPosition.x + element.GetSize().x + ElementOffset.x;
                newY = Math.Max(elementPosition.y + element.GetSize().y + ElementOffset.y, newY);
                
                if (newX + nextElementSize.x < RightEdge)
                {
                    elementPosition.x = newX;
                }
                else
                {
                    elementPosition.x = GetPos().x + ElementOffset.x;
                    elementPosition.y = newY;
                }
            }

            needsArranging = false;
        }

        public void EnableAlwaysAutoResize(int RowWidth)
        {
            ElementRowWidth = RowWidth;
            AlwaysAutoResize = true;
        }

        public void DisableAlwaysAutoResize()
        {
            AlwaysAutoResize = false;
        }

        public void DisableDragging()
        {
            draggable = false;
        }

        public void EnableDragging()
        {
            draggable = true;
        }

        public void AutoResize(int rowWidth)
        {
            float maxX = ElementOffset.x * 2f, maxY = ElementOffset.y;
            if (rowWidth == 1)
            {
                maxX = Elements.Max(ui => ui.Width) + ElementOffset.x * 2f;
            }
            else
            {
                int j = 0;
                for (int i = 0; i <= Elements.Count - rowWidth; i+= rowWidth)
                {
                    List<MAmUi> row = Elements.GetRange(i, rowWidth);
                    float width = ElementOffset.x;
                    row.ForEach(ui => width += ui.Width + ElementOffset.x);
                    if (width > maxX) maxX = width;
                    j = i + 2;
                }

                if (j < Elements.Count - 1)
                {
                    List<MAmUi> finalrow = Elements.GetRange(j, Elements.Count - j);
                    float finalwidth = ElementOffset.x;
                    finalrow.ForEach(ui => finalwidth += ui.Width + ElementOffset.x);
                    if (finalwidth > maxX) maxX = finalwidth;
                }
            }
            
            if (Elements.Count <= rowWidth)
            {
                maxY += Elements.Max(ui => ui.Height) + ElementOffset.y;
            }
            else
            {
                int j = 0;
                for (int i = 0; i <= Elements.Count - rowWidth; i+= rowWidth)
                {
                    List<MAmUi> row = Elements.GetRange(i, rowWidth);
                    float height = row.Max(ui => ui.Height) + ElementOffset.y;
                    maxY += height;
                    j = i + 2;
                }
                if (j < Elements.Count - 1)
                {
                    List<MAmUi> finalrow = Elements.GetRange(j, Elements.Count - j);
                    float finalheight = finalrow.Max(ui => ui.Height) + ElementOffset.y;
                    maxY += finalheight;
                }
            }
            SetSize(new Vec2(maxX,maxY),true);
            SetPos(Position);
        }

        public void AutoSortElements(MUiArrangement arrangement)
        {
            List<MAmUi> temp = new List<MAmUi>(
                arrangement switch
                {
                    MUiArrangement.HEIGHT_INCREASING => Elements.OrderBy(e => e.Height, OrderByDirection.Ascending),
                    MUiArrangement.HEIGHT_DECREASING => Elements.OrderBy(e => e.Height, OrderByDirection.Descending),
                    MUiArrangement.WIDTH_INCREASING => Elements.OrderBy(e => e.Width, OrderByDirection.Ascending),
                    MUiArrangement.WIDTH_DECREASING => Elements.OrderBy(e => e.Width, OrderByDirection.Descending),
                }
            );
                
            Elements.Clear();
            Elements.AddRange(temp);
            needsArranging = true;
        }

        /// <inheritdoc />
        public override Vec2 GetPos()
        {
            return Position;
        }

        /// <inheritdoc />
        public override Vec2 GetSize()
        {
            return Size;
        }

        /// <inheritdoc />
        public override void SetPos(Vec2 pos)
        {
            Vec2 position = MPositionConversionUtil.ClampToScreen(pos, GetSize(),MQuadrantArea.TopLeft);
            if (position == Position) return;
            Position = position;
            needsArranging = true;
            base.SetPos(pos);
        }

        /// <inheritdoc />
        public override void SetSize(Vec2 size, bool scaled = false)
        {
            if (Size == size) return;
            Size = size * (scaled ? 1f : UiScale);
            needsArranging = true;
            base.SetSize(size, scaled);
        }

        /// <inheritdoc />
        public override bool IsOverlapping(Vec2 pos)
        {
            return pos.x > Position.x && pos.x < Position.x + Size.x && pos.y > Position.y && pos.y < Position.y + Size.y;
        }

        public bool IsOverlappingHeader(Vec2 pos)
        {
            return pos.x > Position.x && pos.x < Position.x + Size.x && pos.y > Position.y &&
                   pos.y < Position.y + ElementOffset.y;
        }

        public abstract bool IsOverlappingCloseButton(Vec2 pos);

        /// <inheritdoc />
        public override void HandleMouseEvent(MEventMouseAction e)
        {
            if (!MUiHandler.IsTop(State, MInputHandler.MousePositionGame) && !e.IsReleaseAction()) return;
            
            Elements.ForEach(ele => ele.HandleMouseEvent(e));
            
            switch (e.Action)
            {
                case MouseAction.LeftPressed:
                    MUiHandler.SetTop(GetOwningState());
                    if (IsOverlappingCloseButton(e.MousePosGame))
                    {
                        MUiHandler.Close(GetOwningState().id);
                        return;
                    }
                    if (!draggable) return;
                    if (!IsOverlappingHeader(e.MousePosGame)) return;
                    dragOffset = Position - e.MousePosGame;
                    dragging = true;
                    return;
                case MouseAction.LeftReleased:
                case MouseAction.MiddlePressed:
                case MouseAction.MiddleReleased:
                case MouseAction.RightPressed:
                case MouseAction.RightReleased:
                    dragging = false;
                    return;
            }
        }

        /// <inheritdoc />
        public override void HandleKeyTypedEvent(MEventKeyTyped e)
        {
            if (!MUiHandler.IsTop(State, MInputHandler.MousePositionGame)) return;
            
            Elements.ForEach(ele => ele.HandleKeyTypedEvent(e));
        }

        private void HandleDragging()
        {
            if (!dragging || !draggable) return;
            SetPos(MInputHandler.MousePositionGame + dragOffset);
        }
        
        
        public override void UpdateCols()
        {
            BaseColor = GetCol(UiColorArea.Base);
            BaseAccentColor = GetCol(UiColorArea.BaseAccent);
            TextColor = GetCol(UiColorArea.Text);
            TextAccentColor = GetCol(UiColorArea.TextAccent);
            foreach (MAmUi el in Elements)
            {
                el.UpdateCols();
            }
        }

    }

    public enum MUiArrangement
    {
        HEIGHT_INCREASING,HEIGHT_DECREASING, WIDTH_INCREASING, WIDTH_DECREASING,
    }
}