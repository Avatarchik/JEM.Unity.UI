//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections;
using JEM.Unity.Attribute.Behaviour;
using JEM.Unity.Attribute.Style;
using JEM.Unity.Common;
using JEM.Unity.UI.Animation;
using UnityEngine;

namespace JEM.Unity.UI
{
    /// <summary>
    ///     A universal component that may be used as scenario for your loading screen.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public sealed class UILoadingScreen : JEMSingletonBehaviour<UILoadingScreen>
    {
        /// <summary>
        ///     Root panel of the loading UI.
        /// </summary>
        [Header("Settings")]
        [JEMInfo(JEMInfoCondition.NegativeValue, "Value need to be set.", Type = JEMInfoMessageType.Error)]
        public UIFadeAnimation LoadingPanel;

        /// <summary>
        ///     Defines whether an additional loading animation is being drawn to user.
        /// </summary>
        public bool HasLoadingAnimation;

        /// <summary>
        ///     Reference to the root of loading animation.
        /// </summary>
        [JEMSpace]
        [JEMIndent, JEMShowIf(nameof(HasLoadingAnimation))]
        [JEMInfo(JEMInfoCondition.NegativeValue, "Value need to be set.", Type = JEMInfoMessageType.Error)]
        public UIFadeAnimation LoadingAnimationPanel;

        /// <summary>
        ///     Time in seconds of how much loading animation should wait to fadeout after loading ends.
        ///     If you have <see cref="WaitForUserContinue"/> root loading panel will wait for user input to get disabled.
        /// </summary>
        [JEMIndent, JEMShowIf(nameof(HasLoadingAnimation))]
        public float LoadingAnimationFadeout = 2f;
        
        /// <summary>
        ///     While true, loading screen will continue only if user continues.
        ///     NOTE: You need to call <see cref="ContinueByUser"/> method to continue if <see cref="CanContinueByAnyKey"/> is not enabled.
        /// </summary>
        [JEMIndent, JEMShowIf(nameof(HasLoadingAnimation))]
        public bool WaitForUserContinue = true;

        /// <summary>
        ///     When true, <see cref="Time.timeScale"/> will be set to zero while loading screen is active.
        /// </summary>
        [JEMIndent, JEMShowIf(nameof(HasLoadingAnimation))]
        public bool FreezeTime = true;

        /// <summary>
        ///     Reference to panel that could tell that input is needed in order to continue.
        /// </summary>
        [JEMSpace]
        [JEMIndent(2), JEMShowIf(nameof(WaitForUserContinue))]
        [JEMInfo(JEMInfoCondition.NegativeValue, "Value need to be set.", Type = JEMInfoMessageType.Error)]
        public UIFadeAnimation ContinueInfoPanel;
        
        /// <summary>
        ///     When enabled, loading screen will continue by entering any key.
        /// </summary>
        [JEMIndent(2), JEMShowIf(nameof(WaitForUserContinue))]
        public bool CanContinueByAnyKey = true;

        /// <summary>
        ///     Called when user wants to continue after loading ends.
        /// </summary>
        public event Action OnUserContinue;

        /// <summary>
        ///     Called at the beginning of <see cref="ReportLoadingState"/> call/When loading state is reported to UI.
        /// </summary>
        public event Action<bool> OnLoadingStateReported;
        
        /// <summary>
        ///     Defines whether the loading UI is active or not.
        /// </summary>
        public bool IsLoadingActive { get; private set; }
        
        /// <summary>
        ///     True when UI is waiting for user input to continue.
        /// </summary>
        public bool IsWaitingForUser { get; private set; }
        
        private bool _dontWaitForContinue;
        private bool _loadingFadeOut;

        private bool _timeScaleCached;
        private float _previousTimeScale;
        
        
        /// <inheritdoc />
        protected override void OnAwake()
        {
            if (LoadingPanel == null) throw new NullReferenceException("LoadingPanel property is not set to an instance of object.");
            
            // make sure that loading panel is disabled
            LoadingPanel.SetActive(false, true);
            IsLoadingActive = false;
        }

        private void LateUpdate()
        {
            if (!CanContinueByAnyKey || !IsWaitingForUser) return;
            if (Input.anyKeyDown)
            {
                ContinueByUser();
            }
        }
        
        /// <summary>
        ///     Activates loading UI.
        /// </summary>
        /// <param name="dontWaitForContinue">When true, loading UI will be forced to ignore 'WaitForUserContinue' feature.</param>
        public void ActiveLoading(bool dontWaitForContinue = false)
        {
            if (IsLoadingActive) return;
            IsLoadingActive = true;
            IsWaitingForUser = false;
            
            _dontWaitForContinue = dontWaitForContinue;
            _loadingFadeOut = false;
            
            LoadingPanel.SetActive(true);
            
            // Reset state.
            if (HasLoadingAnimation) LoadingAnimationPanel.SetActive(true, true);
            if (ContinueInfoPanel != null)
                ContinueInfoPanel.SetActive(false, true);

            // Freeze time.
            if (FreezeTime)
            {
                if (!_timeScaleCached)
                {
                    _timeScaleCached = true;
                    _previousTimeScale = Time.timeScale;
                }

                Time.timeScale = 0f;
            }

            OnLoadingStateReported?.Invoke(true);
        }

        /// <summary>
        ///     Try to disable loading UI.
        ///     If active scenario does not need user input UI will be disabled immediately.
        /// </summary>
        /// <param name="dontWaitForContinue">
        ///     When true, loading UI will be forced to ignore 'WaitForUserContinue' feature.
        ///     Note: that if <see cref="ActiveLoading"/> has been called with dontWaitForContinue true, this value basically don't matter.
        /// </param>
        public void TryDisableLoading(bool dontWaitForContinue = false)
        {
            if (!IsLoadingActive) return;
            if (IsWaitingForUser) return;
            if (_loadingFadeOut) return;
            
            // Disable loading animation.
            if (HasLoadingAnimation)
                LoadingAnimationPanel.SetActive(false);
            
            // Try to wait for fadeout.
            _loadingFadeOut = true;
            StartCoroutine(FadeOutWorker(dontWaitForContinue));
            
            OnLoadingStateReported?.Invoke(false);
        }

        private IEnumerator FadeOutWorker(bool dontWaitForContinue)
        {
            yield return new WaitForSecondsUnscaled(LoadingAnimationFadeout);
            _loadingFadeOut = false;
            
            var shouldWaitForUser = HasLoadingAnimation && WaitForUserContinue && !dontWaitForContinue && !_dontWaitForContinue;
            if (shouldWaitForUser)
            {
                // Wait for user input.
                IsWaitingForUser = true;
                if (ContinueInfoPanel != null)
                    ContinueInfoPanel.SetActive(true);
                
                yield break;
            }

            DisableLoading();
        }

        private void DisableLoading()
        {
            if (!IsLoadingActive) return;
            IsLoadingActive = false;
            IsWaitingForUser = false;
            
            LoadingPanel.SetActive(false);
            
            // Unfeeze here.
            if (_timeScaleCached)
            {
                _timeScaleCached = false;
                Time.timeScale = _previousTimeScale;
            }
        }
        
        // /// <summary>
        // ///     Reports loading state to UI.
        // /// </summary>
        // public void ReportLoadingState(bool isLoading)
        // {
        //     OnLoadingStateReported?.Invoke(isLoading);            
        // }
        
        /// <summary>
        ///     Reports that user wants to continue.
        /// </summary>
        public void ContinueByUser()
        {
            // By continuing we just disabling panel.
            DisableLoading();

            OnUserContinue?.Invoke();
        }
    }
}