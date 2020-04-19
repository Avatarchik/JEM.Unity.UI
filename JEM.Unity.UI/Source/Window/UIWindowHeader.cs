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
    ///     An component that defines a Header of <see cref="UIWindow"/>.
    ///     Defines draggable area of window.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("JEM/UI/Window/Window Header")]
    public sealed class UIWindowHeader : UIWindowComponent, 
        IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        ///     True if this window header can work with current window.
        /// </summary>
        public bool CanWorkWithWindow => Window != null && Window.AllowDragging &&  Window.WindowTransform != null;

        private bool _isMouseDown;
        private bool _isMouseOver;

        private bool _isWindowMoved;
        private Vector3 _mouseStartPosition;

        private Vector3 _startPosition;

        /// <inheritdoc />
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (!CanWorkWithWindow) return;
            if (UIWindow.AnyWindowIsUnderMotion) return;
            AnyHeaderIsDragging = true;

            _isMouseDown = true;

            _startPosition = Window.WindowTransform.position;
            _mouseStartPosition = Input.mousePosition;

            UICursorHelper.SetCursorIcon(JEMCursorIconName.Move);
        }

        /// <inheritdoc />
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (!CanWorkWithWindow) return;
            _isMouseOver = true;
            if (!UIWindow.AnyWindowIsUnderMotion)
                UICursorHelper.SetCursorIcon(JEMCursorIconName.Move);
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
            AnyHeaderIsDragging = false;

            _isMouseDown = false;
            _isWindowMoved = false;

            UICursorHelper.SetCursorIcon(JEMCursorIconName.Default);
        }

        /// <summary>
        ///     Restarts this window header.
        /// </summary>
        public void Restart()
        {
            if (_isWindowMoved || _isMouseDown || _isMouseOver)
                UICursorHelper.SetCursorIcon(JEMCursorIconName.Default);

            _isWindowMoved = false;
            _isMouseOver = false;
            _isMouseDown = false;
            AnyHeaderIsDragging = false;
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
            var delta = new Vector2(mousePoint.x - _mouseStartPosition.x, mousePoint.y - _mouseStartPosition.y);

            Window.WindowTransform.position = new Vector3(_startPosition.x + delta.x, _startPosition.y + delta.y, 0f);
            Window.ClampWindowTransform();
        }

        /// <summary>
        ///     True if any window header is currently moving by user.
        /// </summary>
        public static bool AnyHeaderIsDragging { get; private set; }
    }
}