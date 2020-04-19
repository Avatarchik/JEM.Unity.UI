//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JEM.Unity.UI.Control
{
    /// <inheritdoc />
    /// <summary>
    ///     A script that will translate color of target graphic when target Selectable has been selected.
    /// </summary>
    [AddComponentMenu("JEM/UI/Control/Selection Colors")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Selectable))]
    public class UISelectionColors : MonoBehaviour
    {
        [Header("Settings")]
        public Graphic TargetGraphic;
        public Color NormalColor = Color.white;
        public Color SelectedColor = Color.gray;
        public float FadeDuration = 0.15f;

        public Selectable Selectable { get; private set; }
        private bool _wasSelected;
        private void Awake() => Selectable = GetComponent<Selectable>();
        private void OnEnable() => StartCoroutine(Worker());
        private IEnumerator Worker()
        {
            // Reset State
            _wasSelected = false;
            TargetGraphic.CrossFadeColor(NormalColor, 0f, true, true, true);

            while (true)
            {
                if (EventSystem.current != null)
                {
                    var isSelected = EventSystem.current.currentSelectedGameObject == Selectable.gameObject;
                    if (isSelected != _wasSelected)
                    {
                        _wasSelected = isSelected;
                        if (isSelected)
                        {
                            TargetGraphic.CrossFadeColor(SelectedColor, FadeDuration, false, true, true);
                        }
                        else
                        {
                            TargetGraphic.CrossFadeColor(NormalColor, FadeDuration, false, true, true);
                        }
                    }
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }
}
