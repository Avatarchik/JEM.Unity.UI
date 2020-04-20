//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections;
using JEM.Unity.Common;
using JEM.Unity.Extension;
using JEM.Unity.Systems;
using UnityEngine;

namespace JEM.Unity.UI.Animation
{
    /// <inheritdoc />
    internal sealed class UIFadeAnimationScript : JEMRegenerableBehaviour<UIFadeAnimationScript>
    {
        /// <inheritdoc />
        protected override void OnAwake()
        {
            // ignore
        }

        internal IEnumerator FadeAnimationWorker(UIFadeAnimation window)
        {
            if (window.IsActive) window.gameObject.LiteSetActive(true);
            else if (window == null || window.gameObject == null || !window.gameObject.activeSelf) yield break;

            var c = window.CanvasGroup;
            if (window.IsActive)
            {
                c.alpha = 0f;
                while (c != null && c.alpha < 0.995f)
                {
                    var deltaTime = window.UnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                    c.alpha = Mathf.Lerp(c.alpha, 1f, deltaTime * window.FadeSpeed);
                    yield return new WaitForEndOfFrame();
                } if (c == null) yield break; // Object may be destroyed while loop is running so check here and break.

                c.alpha = 1f;
            }
            else
            {
                c.alpha = 1f;
                while (c != null && c.alpha > 0.005f)
                {
                    var deltaTime = window.UnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                    c.alpha = Mathf.Lerp(c.alpha, 0f, deltaTime * window.FadeSpeed);
                    yield return new WaitForEndOfFrame();
                } if (c == null) yield break; // Object may be destroyed while loop is running so check here and break.

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

        internal IEnumerator ScaleAnimationWorker(UIFadeAnimation window)
        {
            switch (window.AnimationMode)
            {
                case UIFadeAnimationMode.Disabled:
                    // Animation disabled.
                    break;
                case UIFadeAnimationMode.UsingLocalScale:
                    yield return LocalScaleAnimationWorker(window);
                    break;
                case UIFadeAnimationMode.UsingSizeDelta:
                    yield return DeltaScaleAnimationWorker(window);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            window.ScaleWorker = null;
        }

        private static IEnumerator LocalScaleAnimationWorker(UIFadeAnimation window)
        {
            if (window.IsActive)
            {
                window.transform.localScale = window.OriginalScale * window.FadeInScale;
                while (window.transform.ScaleDistance2D(window.OriginalScale) > 0.005f)
                {
                    var deltaTime = window.UnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
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
                    var deltaTime = window.UnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                    window.transform.LerpLocalScale(outScale, deltaTime * window.FadeSpeed);
                    yield return new WaitForEndOfFrame();
                }

                window.transform.localScale = outScale;
            }
        }

        private static IEnumerator DeltaScaleAnimationWorker(UIFadeAnimation window)
        {
            if (window.IsActive)
            {
                window.RectTransform.sizeDelta = window.OriginalSizeDelta * window.FadeInScale;
                while (window.RectTransform.SizeDistance(window.OriginalSizeDelta) > 1f)
                {
                    var deltaTime = window.UnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                    window.RectTransform.sizeDelta = Vector2.LerpUnclamped(window.RectTransform.sizeDelta, window.OriginalSizeDelta, deltaTime * window.FadeSpeed);
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
                    var deltaTime = window.UnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                    window.RectTransform.sizeDelta = Vector2.LerpUnclamped(window.RectTransform.sizeDelta, outSize,
                        deltaTime * window.FadeSpeed);
                    yield return new WaitForEndOfFrame();
                }

                window.RectTransform.sizeDelta = outSize;
            }
        }

        [JEMGlobalEvent((byte) JEMGlobalEvents.PrepareJEMScripts)]
        private new static void RegenerateScript() => JEMRegenerableBehaviour<UIFadeAnimationScript>.RegenerateScript();
    }
}