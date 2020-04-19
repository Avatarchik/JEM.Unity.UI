//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Unity.Extension;
using UnityEngine;

namespace JEM.Unity.UI.Animation
{
    /// <summary>
    ///     Defines a animation type of <see cref="UIFadeAnimation"/>.
    /// </summary>
    public enum UIFadeAnimationMode
    {
        /// <summary>
        ///     The scale animations are disabled.
        /// </summary>
        Disabled,

        /// <summary>
        ///     When fading, <see cref="Transform.localScale"/> will be used for animation. 
        /// </summary>
        UsingLocalScale,

        /// <summary>
        ///     When fading, <see cref="RectTransform.sizeDelta"/> will be used for animation. 
        /// </summary>
        UsingSizeDelta
    }
    
    /// <inheritdoc />
    /// <summary>
    ///     A component that activates or de-activates target object
    ///      with fade-in/out animations.
    /// </summary>
    /// <remarks>
    ///     To fade all children, <see cref="UIFadeAnimation"/>
    ///      is using <see cref="CanvasGroup"/>.
    ///     Because of that, we except you to not use <see cref="CanvasGroup"/> of object with <see cref="UIFadeAnimation"/> attached.
    /// </remarks>
    [AddComponentMenu("JEM/UI/Interface Fade Animation")]
    [DisallowMultipleComponent, RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public sealed class UIFadeAnimation : MonoBehaviour
    {
        /// <summary>
        ///     Speed of fading out/in.
        /// </summary>
        [Header("Animation Settings")]
        public float FadeSpeed = 24f;

        /// <summary>
        ///     Target scale of fading in.
        /// </summary>
        [Space]
        public float FadeInScale = 1.06f;

        /// <summary>
        ///     Target scale of fading out.
        /// </summary>
        public float FadeOutScale = 1.03f;

        /// <summary>
        ///     Defines target animation mode.
        /// </summary>
        [Space]
        public UIFadeAnimationMode AnimationMode = UIFadeAnimationMode.UsingLocalScale;

        /// <summary>
        ///     When true, animation will be updated using <see cref="Time.unscaledDeltaTime"/> instead of <see cref="Time.deltaTime"/>.
        /// </summary>
        [Header("Experimental")] 
        public bool UnscaledTime = false;
        
        /// <summary>
        ///     Reference to the <see cref="RectTransform"/> component.
        /// </summary>
        public RectTransform RectTransform { get; private set; }

        /// <summary>
        ///     Reference to the <see cref="CanvasGroup"/> component.
        /// </summary>
        public CanvasGroup CanvasGroup { get; private set; }

        /// <summary>
        ///     Defines a target or current activation state of object.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        ///     The original <see cref="RectTransform.sizeDelta"/> value.
        /// </summary>
        public Vector2 OriginalSizeDelta { get; private set; }

        /// <summary>
        ///     The original <see cref="Transform.localScale"/> value.
        /// </summary>
        public Vector3 OriginalScale { get; private set; }

        internal Coroutine FadeWorker { get; set; }
        internal Coroutine ScaleWorker { get; set; }
        private bool _isLoaded;

        private void Awake()
        {
            IsActive = gameObject.activeSelf;
            InternalLoadScript();
        }

        private void InternalLoadScript()
        {
            if (_isLoaded)
            {
                return;
            }

            RectTransform = GetComponent<RectTransform>();
            CanvasGroup = GetComponent<CanvasGroup>();

            OriginalSizeDelta = RectTransform.sizeDelta;
            OriginalScale = transform.localScale;

            _isLoaded = true;
        }


        /// <summary>
        ///     Stops currently/last worker coroutine.
        /// </summary>
        private void StopWorker()
        {
            if (FadeWorker != null) Script.StopCoroutine(FadeWorker);
            if (ScaleWorker != null) Script.StopCoroutine(ScaleWorker);
            FadeWorker = null;
            ScaleWorker = null;
        }

        /// <summary>
        ///     Sets the active state of <see cref="UIFadeAnimation"/>.
        /// </summary>
        /// <param name="activeState"/>
        public void SetWindowActive(bool activeState) => SetActive(activeState);

        /// <summary>
        ///     Sets the active state of <see cref="UIFadeAnimation"/>.
        /// </summary>
        /// <param name="activeState"/>
        /// <param name="forced">If true, operation will be forced and object will be disabled without fading.</param>
        public void SetActive(bool activeState, bool forced = false)
        {
            if (IsActive == activeState && !forced)
            {
                // Ignore if incoming activation state is the same as active.
                return;
            }

            // Load script just for sure.
            InternalLoadScript();

            // Update target active state
            IsActive = activeState;

            if (forced)
            {
                ForceActiveState();
                return;
            }

            // Try to stop last worker.
            StopWorker();

            // Start workers.
            FadeWorker = Script.StartCoroutine(Script.FadeAnimationWorker(this));
            ScaleWorker = Script.StartCoroutine(Script.ScaleAnimationWorker(this));
        }

        /// <summary>
        ///     Forces target activation state.
        /// </summary>
        public void ForceActiveState()
        {
            // Update canvas group.
            if (IsActive)
            {
                CanvasGroup.alpha = 1f;
                CanvasGroup.interactable = true;
            }
            else
            {
                CanvasGroup.alpha = 0f;
                CanvasGroup.interactable = false;
            }

            // Set gameobject active.
            gameObject.LiteSetActive(IsActive);

            if (!_isLoaded)
                return;

            // Update scale
            switch (AnimationMode)
            {
                case UIFadeAnimationMode.UsingLocalScale:
                    gameObject.transform.localScale = OriginalScale;
                    break;
                case UIFadeAnimationMode.UsingSizeDelta:
                    RectTransform.sizeDelta = OriginalSizeDelta;
                    break;
            }
        }

        private static UIFadeAnimationScript Script
        {
            get
            {
                if (_script == null)
                    _script = UIFadeAnimationScript.GetScript();

                return _script;
            }
        }

        private static UIFadeAnimationScript _script;
    }
}