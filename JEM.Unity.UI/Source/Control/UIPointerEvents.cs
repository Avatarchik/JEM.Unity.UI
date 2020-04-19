//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace JEM.Unity.UI.Control
{
    /// <inheritdoc cref="MonoBehaviour" />
    /// <summary>
    ///     A simple script that will forward IPointerHandlers in to <see cref="UnityEvent"/> that can be accessed via editor inspector.
    /// </summary>
    [AddComponentMenu("JEM/UI/Control/Pointer Events")]
    [DisallowMultipleComponent]
    public class UIPointerEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [Header("Events")] public UnityEvent OnEnter;
        public UnityEvent OnExit;
        public UnityEvent OnDown;
        public UnityEvent OnUp;
        public UnityEvent OnClick;

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => OnEnter.Invoke();
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) => OnExit.Invoke();
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) => OnDown.Invoke();
        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) => OnUp.Invoke();
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) => OnClick.Invoke();
    }
}
