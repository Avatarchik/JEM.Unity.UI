//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections;
using JEM.Unity.Extension;
using JEM.Unity.Systems;
using UnityEngine;

namespace JEM.Unity.UI.Animation
{
    /// <inheritdoc />
    internal sealed class JEMInterfaceFadeAnimationScript : JEMRegenerableScript<JEMInterfaceFadeAnimationScript>
    {
        /// <inheritdoc />
        protected override void OnAwake()
        {
            // ignore
        }

        internal IEnumerator FadeAnimationWorker(JEMInterfaceFadeAnimation window)
        {
            if (window.IsActive) window.gameObject.LiteSetActive(true);
            else if (window == null || window.gameObject == null || !window.gameObject.activeSelf) yield break;

            var c = window.CanvasGroup;
            if (window.IsActive)
            {
                c.alpha = 0f;
                while (c.alpha < 0.995f)
                {
                    c.alpha = Mathf.Lerp(c.alpha, 1f, deltaTime * window.FadeSpeed);
                    yield return new WaitForEndOfFrame();
                }

                c.alpha = 1f;
            }
            else
            {
                c.alpha = 1f;
                while (c.alpha > 0.005f)
                {
                    c.alpha = Mathf.Lerp(c.alpha, 0f, deltaTime * window.FadeSpeed);
                    yield return new WaitForEndOfFrame();
                }

                c.alpha = 0f;
            }

            // No need to activate gameObject here
            // Activation state of object is updated in ForceActiveState tho
            //if (!window.IsActive)
            //{
            //    window.gameObject.LiteSetActive(false);
            //}

            // Force window just to make sure that everything is drawn.
            window.ForceActiveState();

            // Restart current work.
            window.FadeWorker = null;
        }

        internal IEnumerator ScaleAnimationWorker(JEMInterfaceFadeAnimation window)
        {
            switch (window.AnimationMode)
            {
                case JEMFadeAnimationMode.Disabled:
                    // Animation disabled.
                    break;
                case JEMFadeAnimationMode.UsingLocalScale:
                    yield return LocalScaleAnimationWorker(window);
                    break;
                case JEMFadeAnimationMode.UsingSizeDelta:
                    yield return DeltaScaleAnimationWorker(window);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            window.ScaleWorker = null;
        }

        private static IEnumerator LocalScaleAnimationWorker(JEMInterfaceFadeAnimation window)
        {
            if (window.IsActive)
            {
                window.transform.localScale = window.OriginalScale * window.FadeInScale;
                while (window.transform.ScaleDistance2D(window.OriginalScale) > 0.005f)
                {
                    window.transform.LerpLocalScale(window.OriginalScale, deltaTime * window.FadeSpeed);
                    yield return new WaitForEndOfFrame();
                }

                window.transform.localScale = window.OriginalScale;
            }
            else
            {
                window.RectTransform.localScale = window.OriginalScale;
                var outScale = window.OriginalScale * window.FadeOutScale;
                while (window.transform.ScaleDistance2D(outScale) > 0.005f)
                {
                    window.transform.LerpLocalScale(outScale, deltaTime * window.FadeSpeed);
                    yield return new WaitForEndOfFrame();
                }

                window.transform.localScale = outScale;
            }
        }

        private static IEnumerator DeltaScaleAnimationWorker(JEMInterfaceFadeAnimation window)
        {
            if (window.IsActive)
            {
                window.RectTransform.sizeDelta = window.OriginalSizeDelta * window.FadeInScale;
                while (window.RectTransform.SizeDistance(window.OriginalSizeDelta) > 1f)
                {
                    window.RectTransform.sizeDelta = Vector2.LerpUnclamped(window.RectTransform.sizeDelta,
                        window.OriginalSizeDelta, deltaTime * window.FadeSpeed);
                    yield return new WaitForEndOfFrame();
                }

                window.RectTransform.sizeDelta = window.OriginalSizeDelta;
            }
            else
            {
                window.RectTransform.sizeDelta = window.OriginalSizeDelta;
                var outSize = window.OriginalSizeDelta * window.FadeOutScale;
                while (window.RectTransform.SizeDistance(outSize) > 1f)
                {
                    window.RectTransform.sizeDelta = Vector2.LerpUnclamped(window.RectTransform.sizeDelta, outSize,
                        deltaTime * window.FadeSpeed);
                    yield return new WaitForEndOfFrame();
                }

                window.RectTransform.sizeDelta = outSize;
            }
        }

        [JEMGlobalEvent((byte) JEMGlobalEvents.PrepareJEMScripts)]
        private new static void RegenerateScript() => JEMRegenerableScript<JEMInterfaceFadeAnimationScript>.RegenerateScript();
        
        private static float deltaTime => Time.timeScale <= 0f ? Time.unscaledDeltaTime : Time.deltaTime;
    }
}