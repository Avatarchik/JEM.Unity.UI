//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System.Collections.Generic;
using JEM.Unity.Common;
using JEM.Unity.UI.Animation;
using UnityEngine;
using UnityEngine.UI;

namespace JEM.Unity.UI.Systems
{
    /// <summary>
    ///     'Pause' UI Controller.
    /// </summary>
    public sealed class UIPause : JEMSingletonBehaviour<UIPause>, IUIBehaviour, IPauseDependent
    {
        [Header("Settings")]
        [SerializeField] public UIFadeAnimation Panel;
        [SerializeField] public Button Resume;
        [SerializeField] public Button Settings;
        [SerializeField] public Button Quit;
        
        /// <inheritdoc />
        UIContentState IUIBehaviour.ContentTarget => UIContentState.InGame;

        /// <inheritdoc />
        UILockRules IUIBehaviour.LockRules => UILockRules.LockCamera | UILockRules.LockBody;

        /// <inheritdoc />
        int IPauseDependent.SortOrder => -1;

        /// <inheritdoc cref="IUIBehaviour" />
        public bool IsContentActive { get; set; }

        /// <inheritdoc />
        protected override void OnAwake()
        {
            Resume.onClick.AddListener(() => SetContentActive(false));
            Settings.onClick.AddListener(() => { });
            Quit.onClick.AddListener(Application.Quit);
        }

        private void Update()
        {
            var update = UIBehaviourHelper.ShouldUpdateBehaviour(this);
            if (!update)
                return;
            
            update = Input.GetKeyDown(KeyCode.Escape);
            if (!update)
                return;
            
            // Check if any IPauseDependent is active.
            var behaviours = GetBehaviours();
            for (var index = 0; index < behaviours.Length; index++)
            {
                var b = behaviours[index];
                if (b.IsContentActive)
                {
                    b.SetContentActive(false);
                    return;
                }
            }

            // All clear, activate pause.
            SetContentActive(true);
        }

        /// <inheritdoc />
        void IUIBehaviour.OnStateChanged(UIContentState state)
        {
            // ignore.
        }

        /// <inheritdoc cref="IUIBehaviour" />
        public void SetContentActive(bool activeState)
        {
            IsContentActive = activeState;
            Panel.SetActive(activeState);
            if (activeState)
            {
                UICursorHelper.ClearCursorLock();
            }
            
            UIBehaviourHelper.RefreshLockState();
        }

        /// <summary>
        ///     Checks if <see cref="IPauseDependent"/> object could be currently activated.
        /// </summary>
        public static bool CanActivatePauseDependent()
        {
            return !Instance.IsContentActive;
        }
        
        /// <summary>
        ///     Returns array of all <see cref="IPauseDependent"/> implemented behaviours in active scenes.
        /// </summary>
        private static IPauseDependent[] GetBehaviours()
        {
            if (_behaviours == null)
            {
                var list = new List<IPauseDependent>();
                var type = FindObjectsOfType<MonoBehaviour>();
                for (var index = 0; index < type.Length; index++)
                {
                    var behaviour = type[index];
                    if (behaviour is IPauseDependent i)
                    {
                        list.Add(i);
                    }
                }

                // Sort by SortOrder.
                list.Sort((x,y) => x.SortOrder.CompareTo(y.SortOrder));
                _behaviours = list.ToArray();
            }

            return _behaviours;
        }
        
        private static IPauseDependent[] _behaviours;
    }
}