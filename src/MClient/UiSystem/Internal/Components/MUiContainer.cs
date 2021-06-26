using System;
using System.Collections.Generic;
using System.Linq;
using DuckGame;
using MClient.Core.EventSystem.Events.Input;
using MClient.Core.Utils;
using MClient.InputSystem;
using MClient.UiSystem.Internal.Attributes;
using MoreLinq;

namespace MClient.UiSystem.Internal.Components
{
    /// <summary>
    /// Base class for Ui Containers. Contains all basic functionality.
    /// </summary>
    public abstract class MUiContainer : MAmUi
    {
        private MUiState _state;
        
        protected Vec2 Position;
        protected Vec2 Size;
        
        private readonly Vec2 _elementOffset;
        private int _elementRowWidth;
        private bool _alwaysAutoResize;

        protected readonly List<MAmUi> Elements;
        protected float UiScale => MUiHandler.GlobalUiScale;

        private bool _needsArranging;
        private bool _draggable = true;
        private bool _dragging;
        private Vec2 _dragOffset;

        protected Color BaseColor;
        protected Color BaseAccentColor;
        protected Color TextColor;
        protected Color TextAccentColor;

        protected MUiContainer(Vec2 position, Vec2 size)
        {
            Position = MPositionConversionUtil.ScreenToGamePos(position) * UiScale;
            Size = size * UiScale;
            _elementOffset = Vec2.One * 5 * UiScale;
            Elements = new List<MAmUi>();
            _needsArranging = true;
        }

        protected MUiContainer(Vec2 position, Vec2 size, Vec2 elementOffset)
        {
            Position = MPositionConversionUtil.ScreenToGamePos(position) * UiScale;
            Size = size * UiScale;
            _elementOffset = elementOffset * UiScale;
            Elements = new List<MAmUi>();
            _needsArranging = true;
        }

        protected MUiContainer(Vec2 position, Vec2 size, Vec2 elementOffset, List<MAmUi>elements)
        {
            Position = MPositionConversionUtil.ScreenToGamePos(position) * UiScale;
            Size = size * UiScale;
            _elementOffset = elementOffset * UiScale;
            Elements = elements;
            _needsArranging = true;
        }

        /// <summary>
        /// Sets the UiState that "owns" this container
        /// </summary>
        public override void SetOwningState(MUiState state)
        {
            _state = state;
            Elements.ForEach(e => e.SetOwningState(state));
        }

        /// <summary>
        /// Gets the UiState that "owns" this container
        /// </summary>
        public override MUiState GetOwningState()
        {
            return _state;
        }

        private Color GetCol(MUiColorArea area)
        {
            if (_state is null) return Color.White;
            return area switch
            {
                MUiColorArea.Base => _state.BaseCol,
                MUiColorArea.BaseAccent => _state.BaseAccentCol,
                MUiColorArea.Text => _state.TextCol,
                MUiColorArea.TextAccent => _state.TextAccentCol,
                _ => throw new ArgumentOutOfRangeException(nameof(area), area, null)
            };
        }

        /// <summary>
        /// Marks this container as needing to re-calculate its sizing and spacing
        /// </summary>
        public void Recalculate()
        {
            _needsArranging = true;
        }

        /// <summary>
        /// Recalculates this containers sizing and spacing immediately
        /// </summary>
        public void RecalculateNow()
        {
            Recalculate();
            Arrange();
        }

        /// <summary>
        /// Adds a Ui element to this container
        /// </summary>
        public void AddElement(MAmUi element)
        {
            Elements.Add(element);
            element.SetOwningState(_state);
            element.OnTransformChanged += Recalculate;
            _needsArranging = true;
        }

        /// <summary>
        /// Removes a Ui element from this container
        /// </summary>
        public void RemoveElement(MAmUi element)
        {
            Elements.Remove(element);
            element.SetOwningState(null);
            element.OnTransformChanged -= Recalculate;
            _needsArranging = true;
        }

        /// <summary>
        /// Adds a set of elements to this container
        /// </summary>
        public void AddElements(MAmUi[] elements)
        {
            Elements.AddRange(elements);
            elements.ForEach(e => e.OnTransformChanged += Recalculate);
            Elements.ForEach(e => e.SetOwningState(_state));
            _needsArranging = true;
        }

        /// <summary>
        /// Removes a set of elements from this container
        /// </summary>
        public void RemoveElements(MAmUi[] elements)
        {
            elements.ForEach(e => e.OnTransformChanged -= Recalculate);
            List<MAmUi> temp = Elements.Except(elements).ToList();
            Elements.Clear(); 
            Elements.AddRange(temp);
            _needsArranging = true;
        }
        
        /// <summary>
        /// Internal call that updates and renders this container
        /// </summary>
        public override void HandleUiUpdate()
        {
            HandleDragging();
            Arrange();
            Draw();
            foreach (var element in Elements)
            {
                element.HandleUiUpdate();
            }
        }

        protected void Arrange()
        {
            if (!_needsArranging || Elements.Count == 0) return;
            if(_alwaysAutoResize) AutoResize(_elementRowWidth);
            var elementPosition = GetPos() + _elementOffset;

            float newY = 0;
            for (int i = 0; i < Elements.Count; i++)
            {
                var element = Elements[i];
                element.SetPos(elementPosition);

                var nextElementSize = i < Elements.Count - 1 ? Elements[i + 1].GetSize() : Vec2.Zero;
                
                var newX = elementPosition.x + element.GetSize().x + _elementOffset.x;
                newY = Math.Max(elementPosition.y + element.GetSize().y + _elementOffset.y, newY);
                
                if (newX + nextElementSize.x < RightEdge)
                {
                    elementPosition.x = newX;
                }
                else
                {
                    elementPosition.x = GetPos().x + _elementOffset.x;
                    elementPosition.y = newY;
                }
            }

            _needsArranging = false;
        }

        /// <summary>
        /// Enables Auto-Resizing for this container, with a given number of elements per row
        /// </summary>
        /// <param name="rowWidth">The number of elements per row</param>
        public void EnableAlwaysAutoResize(int rowWidth)
        {
            _elementRowWidth = rowWidth;
            _alwaysAutoResize = true;
        }

        /// <summary>
        /// Disables Auto-Resizing for this container
        /// </summary>
        public void DisableAlwaysAutoResize()
        {
            _alwaysAutoResize = false;
        }

        /// <summary>
        /// Disables dragging for this container
        /// </summary>
        public void DisableDragging()
        {
            _draggable = false;
        }

        /// <summary>
        /// Enables dragging for this container
        /// </summary>
        public void EnableDragging()
        {
            _draggable = true;
        }

        /// <summary>
        /// Attempts to automatically arrange the elements in this Ui, given the number of elements per row
        /// </summary>
        /// <param name="rowWidth">The number of elements per row</param>
        public void AutoResize(int rowWidth)
        {
            float maxX = _elementOffset.x * 2f, maxY = _elementOffset.y;
            if (rowWidth == 1)
            {
                maxX = Elements.Max(ui => ui.Width) + _elementOffset.x * 2f;
            }
            else
            {
                int j = 0;
                for (int i = 0; i <= Elements.Count - rowWidth; i+= rowWidth)
                {
                    List<MAmUi> row = Elements.GetRange(i, rowWidth);
                    float width = _elementOffset.x;
                    row.ForEach(ui => width += ui.Width + _elementOffset.x);
                    if (width > maxX) maxX = width;
                    j = i + 2;
                }

                if (j < Elements.Count - 1)
                {
                    List<MAmUi> finalRow = Elements.GetRange(j, Elements.Count - j);
                    float finalWidth = _elementOffset.x;
                    finalRow.ForEach(ui => finalWidth += ui.Width + _elementOffset.x);
                    if (finalWidth > maxX) maxX = finalWidth;
                }
            }
            
            if (Elements.Count <= rowWidth)
            {
                maxY += Elements.Max(ui => ui.Height) + _elementOffset.y;
            }
            else
            {
                int j = 0;
                for (int i = 0; i <= Elements.Count - rowWidth; i+= rowWidth)
                {
                    List<MAmUi> row = Elements.GetRange(i, rowWidth);
                    float height = row.Max(ui => ui.Height) + _elementOffset.y;
                    maxY += height;
                    j = i + 2;
                }
                if (j < Elements.Count - 1)
                {
                    List<MAmUi> finalRow = Elements.GetRange(j, Elements.Count - j);
                    float finalHeight = finalRow.Max(ui => ui.Height) + _elementOffset.y;
                    maxY += finalHeight;
                }
            }
            SetSize(new Vec2(maxX,maxY),true);
            SetPos(Position);
        }

        /// <summary>
        /// Attempts to automatically sort the elements in this Ui, given the specified sort arrangement
        /// </summary>
        /// <param name="arrangement">The sorting arrangement to use</param>
        public void AutoSortElements(MUiArrangement arrangement)
        {
            List<MAmUi> temp = new List<MAmUi>(
                arrangement switch
                {
                    MUiArrangement.HeightIncreasing => Elements.OrderBy(e => e.Height, OrderByDirection.Ascending),
                    MUiArrangement.HeightDecreasing => Elements.OrderBy(e => e.Height, OrderByDirection.Descending),
                    MUiArrangement.WidthIncreasing => Elements.OrderBy(e => e.Width, OrderByDirection.Ascending),
                    MUiArrangement.WidthDecreasing => Elements.OrderBy(e => e.Width, OrderByDirection.Descending),
                    _ => throw new ArgumentOutOfRangeException(nameof(arrangement), arrangement, null)
                }
            );
                
            Elements.Clear();
            Elements.AddRange(temp);
            _needsArranging = true;
        }
        
        public override Vec2 GetPos()
        {
            return Position;
        }
        
        public override Vec2 GetSize()
        {
            return Size;
        }
        
        public override void SetPos(Vec2 pos)
        {
            var position = MPositionConversionUtil.ClampToScreen(pos, GetSize(),MQuadrantArea.TopLeft);
            if (position == Position) return;
            Position = position;
            _needsArranging = true;
            base.SetPos(pos);
        }
        
        public override void SetSize(Vec2 size, bool scaled = false)
        {
            if (Size == size) return;
            Size = size * (scaled ? 1f : UiScale);
            _needsArranging = true;
            base.SetSize(size, scaled);
        }
        
        /// <summary>
        /// Checks whether this container is overlapping a point
        /// </summary>
        public override bool IsOverlapping(Vec2 pos)
        {
            return pos.x > Position.x && pos.x < Position.x + Size.x && pos.y > Position.y && pos.y < Position.y + Size.y;
        }

        /// <summary>
        /// Checks whether the header of this container is overlapping a point
        /// </summary>
        public bool IsOverlappingHeader(Vec2 pos)
        {
            return pos.x > Position.x && pos.x < Position.x + Size.x && pos.y > Position.y &&
                   pos.y < Position.y + _elementOffset.y;
        }

        /// <summary>
        /// Checks whether the close button of this container is overlapping a point
        /// </summary>
        public abstract bool IsOverlappingCloseButton(Vec2 pos);

        /// <summary>
        /// Internal call that handles mouse input
        /// </summary>
        public override void HandleMouseEvent(MEventMouseAction e)
        {
            if (!MUiHandler.IsTop(_state, MInputHandler.MousePositionGame) && !e.IsReleaseAction()) return;
            
            Elements.ForEach(ele => ele.HandleMouseEvent(e));
            
            switch (e.Action)
            {
                case MMouseAction.LeftPressed:
                    MUiHandler.SetTop(GetOwningState());
                    if (IsOverlappingCloseButton(e.MousePosGame))
                    {
                        MUiHandler.Close(GetOwningState().Id);
                        return;
                    }
                    if (!_draggable) return;
                    if (!IsOverlappingHeader(e.MousePosGame)) return;
                    _dragOffset = Position - e.MousePosGame;
                    _dragging = true;
                    return;
                case MMouseAction.LeftReleased:
                case MMouseAction.MiddlePressed:
                case MMouseAction.MiddleReleased:
                case MMouseAction.RightPressed:
                case MMouseAction.RightReleased:
                    _dragging = false;
                    return;
            }
        }

        /// <summary>
        /// Internal call that handles typing input
        /// </summary>
        /// <param name="e"></param>
        public override void HandleKeyTypedEvent(MEventKeyTyped e)
        {
            if (!MUiHandler.IsTop(_state, MInputHandler.MousePositionGame)) return;
            
            Elements.ForEach(ele => ele.HandleKeyTypedEvent(e));
        }

        private void HandleDragging()
        {
            if (!_dragging || !_draggable) return;
            SetPos(MInputHandler.MousePositionGame + _dragOffset);
        }
        
        /// <summary>
        /// Updates the colours for this container and all elements it contains
        /// </summary>
        public override void UpdateCols()
        {
            BaseColor = GetCol(MUiColorArea.Base);
            BaseAccentColor = GetCol(MUiColorArea.BaseAccent);
            TextColor = GetCol(MUiColorArea.Text);
            TextAccentColor = GetCol(MUiColorArea.TextAccent);
            foreach (var el in Elements)
            {
                el.UpdateCols();
            }
        }

    }

    public enum MUiArrangement
    {
        HeightIncreasing,HeightDecreasing, WidthIncreasing, WidthDecreasing,
    }
}