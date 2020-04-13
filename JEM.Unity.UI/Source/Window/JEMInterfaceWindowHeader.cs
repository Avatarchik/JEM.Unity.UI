//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;
using UnityEngine.EventSystems;

namespace JEM.Unity.UI.Window
{
    /// <inheritdoc cref="JEMInterfaceWindowComponent" />
    /// <summary>
    ///     An component that defines a Header of <see cref="JEMInterfaceWindow"/>.
    ///     Defines draggable area of window.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("JEM/UI/Window/JEM Window Header")]
    public sealed class JEMInterfaceWindowHeader : JEMInterfaceWindowComponent, 
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
            if (JEMInterfaceWindow.AnyWindowIsUnderMotion) return;
            AnyHeaderIsDragging = true;

            _isMouseDown = true;

            _startPosition = Window.WindowTransform.position;
            _mouseStartPosition = Input.mousePosition;

            JEMInterfaceCursor.SetCursorIcon(JEMCursorIconName.Move);
        }

        /// <inheritdoc />
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (!CanWorkWithWindow) return;
            _isMouseOver = true;
            if (!JEMInterfaceWindow.AnyWindowIsUnderMotion)
                JEMInterfaceCursor.SetCursorIcon(JEMCursorIconName.Move);
        }

        /// <inheritdoc />
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            _isMouseOver = false;
            if (!_isWindowMoved)
                JEMInterfaceCursor.SetCursorIcon(JEMCursorIconName.Default);
        }

        /// <inheritdoc />
        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            AnyHeaderIsDragging = false;

            _isMouseDown = false;
            _isWindowMoved = false;

            JEMInterfaceCursor.SetCursorIcon(JEMCursorIconName.Default);
        }

        /// <summary>
        ///     Restarts this window header.
        /// </summary>
        public void Restart()
        {
            if (_isWindowMoved || _isMouseDown || _isMouseOver)
                JEMInterfaceCursor.SetCursorIcon(JEMCursorIconName.Default);

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