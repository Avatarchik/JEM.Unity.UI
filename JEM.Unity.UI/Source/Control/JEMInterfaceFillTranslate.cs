//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JEM.Unity.UI.Control
{
    public class JEMInterfaceFillTranslate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Settings")]
        public Image TargetImage;
        public float NormalFill = 1f;
        public float SelectedFill = 0f;
        public float FillDuration = 1f;

        private Coroutine _coroutine;
        private void OnEnable() => TargetImage.fillAmount = NormalFill;

        /// <inheritdoc />
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(Worker(true));
        }

        /// <inheritdoc />
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(Worker(false));
        }

        private IEnumerator Worker(bool selected)
        {
            var target = selected ? SelectedFill : NormalFill;
            while (Math.Abs(TargetImage.fillAmount - target) > float.Epsilon)
            {
                TargetImage.fillAmount = Mathf.Lerp(TargetImage.fillAmount, target, (10f * Time.deltaTime) / FillDuration);
                yield return new WaitForEndOfFrame();
            }

            _coroutine = null;
        }
    }
}
