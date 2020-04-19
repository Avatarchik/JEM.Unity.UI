//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using JEM.Core.Debugging;
using JEM.Unity.Common;
using JetBrains.Annotations;
using UnityEngine;

namespace JEM.Unity.UI.Systems
{
    /// <summary>
    ///     Defines what content of the UI should be currently active.
    /// </summary>
    [Flags]
    public enum UIContentState
    {
        /// <summary>
        ///     Menu content should be currently active.
        /// </summary>
        Menu,
        
        /// <summary>
        ///     In-game content should be currently active.
        /// </summary>
        InGame
    }
    
    public sealed class UIBehaviourHelper : JEMSingletonBehaviour<UIBehaviourHelper>
    {
        /// <inheritdoc />
        protected override void OnAwake()
        {
            // ignore.
        }

        /// <summary>
        ///     Sets the state of the UI content.
        /// </summary>
        /// <exception cref="InvalidEnumArgumentException"/>
        public static void DisableContent(UIContentState state)
        {
            if (!Enum.IsDefined(typeof(UIContentState), state))
                throw new InvalidEnumArgumentException(nameof(state), (int) state, typeof(UIContentState));
            
            // Disable content.
            var behaviours = GetBehaviours();
            _shouldAllowToRefreshLockState = false;
            for (var index = 0; index < behaviours.Length; index++)
            {
                var b = behaviours[index];
                if (!b.IsContentActive)
                    continue;

                var disable = !b.ContentTarget.HasFlag(state);
                if (disable)
                    b.SetContentActive(false);
            }
            _shouldAllowToRefreshLockState = true;
            
            // Update lock rules.
            RefreshLockState();
        }
        
        /// <summary>
        ///     Sets the state of the UI content.
        /// </summary>
        /// <exception cref="InvalidEnumArgumentException"/>
        public static void SetContent(UIContentState state)
        {
            if (!Enum.IsDefined(typeof(UIContentState), state))
                throw new InvalidEnumArgumentException(nameof(state), (int) state, typeof(UIContentState));

            LastContentState = state;
#if DEBUG
            JEMLogger.Log($"InterfaceContentState={state}", UILogSource.UI);
#endif
            // Reset behaviours on content change (cuz at this point, we may have missing reference because of most likely scene change).
            ResetBehaviours();
            
            // Call event on behaviours.
            var behaviours = GetBehaviours();
            for (var index = 0; index < behaviours.Length; index++)
            {
                var b = behaviours[index];
                b.OnStateChanged(state);
            }
            
            // Update active state of content.
            _shouldAllowToRefreshLockState = false;
            for (var index = 0; index < behaviours.Length; index++)
            {
                var b = behaviours[index];
                // Debug.Log($"{b.GetType().Name}, {b.IsContentActive}, {b.ContentTarget.HasFlag(state)}, {b.ContentTarget != state}, ({b.ContentTarget}/{state})");
                if (!b.IsContentActive)
                    continue;

                //     BUG: For InGameInterface ContentTarget.HasFlag returns true when the value is InGame and `state` is Menu.
                var hasFlag = false;// b.ContentTarget.HasFlag(state);
                var disable = !hasFlag && b.ContentTarget != state;
                if (disable)
                    b.SetContentActive(false);
            }
            _shouldAllowToRefreshLockState = true;
            
            // Update lock rules.
            RefreshLockState();
            
            // Invoke onStateChanged after updating UI.
            UIBehaviourHelperEvents.ForEach(behaviour => behaviour.UIBehaviourStateChanged.OnStateChanged(state));
        }

        /// <summary>
        ///     Updates the lock state.
        /// </summary>
        public static void RefreshLockState()
        {
            if (!_shouldAllowToRefreshLockState)
                return;

            ShouldLockCamera = false;
            ShouldLockBody = false;
            
            var behaviours = GetBehaviours();
            for (var index = 0; index < behaviours.Length; index++)
            {
                var b = behaviours[index];
                if (!b.IsContentActive)
                    continue;

                var camera = b.LockRules.HasFlag(UILockRules.LockCamera);
                if (camera)
                    ShouldLockCamera = true;

                var body = b.LockRules.HasFlag(UILockRules.LockBody);
                if (body)
                    ShouldLockBody = true;

                if (ShouldLockCamera && ShouldLockBody)
                    break;
            }
        }

        /// <summary>
        ///     Checks if given <see cref="IUIBehaviour"/> should currently update or not.
        /// </summary>
        public static bool ShouldUpdateBehaviour([NotNull] IUIBehaviour b)
        {
            if (b == null) throw new ArgumentNullException(nameof(b));
            return b.ContentTarget.HasFlag(LastContentState);
        }
        
        /// <summary>
        ///     Defines if camera should be locked by UI.
        /// </summary>
        public static bool ShouldLockCamera { get; private set; }
        
        /// <summary>
        ///     Gets if body should be locked by UI.
        ///     Defines if body should be locked by UI.
        /// </summary>
        public static bool ShouldLockBody { get; private set; }
        
        /// <summary>
        ///     Returns array of all <see cref="IUIBehaviour"/> implemented behaviours in active scenes.
        /// </summary>
        public static IUIBehaviour[] GetBehaviours()
        {
            if (_behaviours == null)
            {
                var list = new List<IUIBehaviour>();
                var type = FindObjectsOfType<MonoBehaviour>();
                for (var index = 0; index < type.Length; index++)
                {
                    var behaviour = type[index];
                    if (behaviour is IUIBehaviour i)
                    {
                        list.Add(i);
                    }
                }

                _behaviours = list.ToArray();
            }

            return _behaviours;
        }

        private static void ResetBehaviours()
        {
            _behaviours = null;
            GetBehaviours();
        }
        
        private static IUIBehaviour[] _behaviours;
        private static bool _shouldAllowToRefreshLockState = true;
        
        /// <summary>
        ///     Last active content state set using <see cref="SetContent"/>.
        /// </summary>
        public static UIContentState LastContentState { get; private set; }
    }
}