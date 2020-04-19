//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;
using UnityEngine.EventSystems;

namespace JEM.Unity.UI.Window
{
    /// <inheritdoc cref="UIWindowComponent" />
    /// <summary>
    ///     A component that defines Resize point of <see cref="UIWindow"/>.
    /// </summary>
    [AddComponentMenu("JEM/UI/Window/Window Resize")]
    [DisallowMultipleComponent]
    public sealed class UIWindowResize : UIWindowComponent, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
        IPointerUpHandler
    {
        private const float Multiplier = 0.875f;
        private const float PositionMultiplier = 0.575f;

        private bool _isMouseDown;
        private bool _isMouseOver;

        private bool _isWindowMoved;
        private Vector3 _mouseStartPosition;

        private Vector3 _startPosition;
        private Vector3 _startSize;

        /// <inheritdoc />
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (!CanWorkWithWindow) return;
            if (UIWindow.AnyWindowIsUnderMotion) return;
            AnyWindowIsResized = true;

            _isMouseDown = true;

            _startPosition = Window.WindowTransform.position;
            _startSize = Window.WindowTransform.sizeDelta;
            _mouseStartPosition = Input.mousePosition;

            UICursorHelper.SetCursorIcon(JEMCursorIconName.Resize);
        }

        /// <inheritdoc />
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (!CanWorkWithWindow) return;
            _isMouseOver = true;
            if (!UIWindow.AnyWindowIsUnderMotion)
                UICursorHelper.SetCursorIcon(JEMCursorIconName.Resize);
        }

        /// <inheritdoc />
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            _isMouseOver = false;
            if (!_isWindowMoved)
                UICursorHelper.SetCursorIcon(JEMCursorIconName.Default);
        }

        /// <inheritdoc />
        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            AnyWindowIsResized = false;

            _isMouseDown = false;
            _isWindowMoved = false;

            UICursorHelper.SetCursorIcon(JEMCursorIconName.Default);
        }

        /// <summary>
        ///     Restarts this window element.
        /// </summary>
        public void Restart()
        {
            if (_isWindowMoved || _isMouseDown)
                UICursorHelper.SetCursorIcon(JEMCursorIconName.Default);

            _isWindowMoved = false;
            _isMouseOver = false;
            _isMouseDown = false;
            AnyWindowIsResized = false;
        }

        private void OnEnable()
        {
            Restart();
        }

        private void OnDisable()
        {
            Restart();
        }

        private void Update()
        {
            if (!CanWorkWithWindow) return;
            if (!_isMouseOver && !_isWindowMoved || !_isMouseDown) return;

            _isWindowMoved = true;
            var mousePoint = Input.mousePosition;
            var delta = new Vector2(mousePoint.x - _mouseStartPosition.x, mousePoint.y - _mouseStartPosition.y) * Multiplier;

            var xPosition = Window.WindowTransform.position.x;
            var yPosition = Window.WindowTransform.position.y;

            var xSize = Window.WindowTransform.sizeDelta.x;
            var ySize = Window.WindowTransform.sizeDelta.y;

            if (Window.ResizeXAxis)
            {
                var xMax = false;
                xSize = _startSize.x + delta.x;
                if (xSize >= Window.WindowMinMaxSize.width)
                {
                    xSize = Window.WindowMinMaxSize.width;
                    xMax = true;
                }

                if (xSize <= Window.WindowMinMaxSize.x)
                {
                    xSize = Window.WindowMinMaxSize.x;
                    xMax = true;
                }

                if (!xMax)
                    xPosition = _startPosition.x + delta.x * PositionMultiplier;
                //else
                //{
                //    xPosition = _startPosition.x + (Window.WindowTransform.sizeDelta.x - xSize) * PositionMultiplier;
                //}
            }

            if (Window.ResizeYAxis)
            {
                var yMax = false;
                ySize = _startSize.y - delta.y;
                if (ySize >= Window.WindowMinMaxSize.height)
                {
                    ySize = Window.WindowMinMaxSize.height;
                    yMax = true;
                }

                if (ySize <= Window.WindowMinMaxSize.y)
                {
                    ySize = Window.WindowMinMaxSize.y;
                    yMax = true;
                }

                if (!yMax)
                    yPosition = _startPosition.y + delta.y * PositionMultiplier;
                //else
                //{
                //    yPosition = _startPosition.y + (Window.WindowTransform.sizeDelta.y - ySize) * PositionMultiplier;
                //}
            }

            Window.WindowTransform.position = new Vector3(xPosition, yPosition, 0f);
            Window.WindowTransform.sizeDelta = new Vector3(xSize, ySize, 0f);
            Window.ClampWindowTransform();
        }

        /// <summary>
        ///     True if this size element can work with current window.
        /// </summary>
        public bool CanWorkWithWindow => Window != null && Window.AllowResize && Window.WindowTransform != null;

        /// <summary>
        ///     True if any window is currently re-sized by user.
        /// </summary>
        public static bool AnyWindowIsResized { get; private set; }
    }
}