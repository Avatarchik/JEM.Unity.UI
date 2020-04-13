//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JEM.Unity.UI.Control
{
    /// <inheritdoc cref="MonoBehaviour" />
    /// <summary>
    ///     Script that translates color of target graphic when Pointer events received.
    /// </summary>
    public class JEMInterfaceColorTranslation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerClickHandler
    {
        [Header("Settings")]
        public Graphic TargetGraphic;
        public Color NormalColor = Color.white;
        public Color HighlightedColor = Color.gray;
        public Color PressedColor = Color.white;
        public float FadeDuration = 0.15f;

        private void OnEnable() => TargetGraphic.CrossFadeColor(NormalColor, 0f, true, true, true);

        /// <inheritdoc />
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) =>
            TargetGraphic.CrossFadeColor(HighlightedColor, FadeDuration, false, true, true);

        /// <inheritdoc />
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) =>
            TargetGraphic.CrossFadeColor(NormalColor, FadeDuration, false, true, true);

        /// <inheritdoc />
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) =>
            TargetGraphic.CrossFadeColor(PressedColor, FadeDuration, false, true, true);
        
        /// <inheritdoc />
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) =>
            TargetGraphic.CrossFadeColor(NormalColor, FadeDuration, false, true, true);
    }
}
