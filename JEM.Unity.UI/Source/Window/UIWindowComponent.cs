//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace JEM.Unity.UI.Window
{
    /// <inheritdoc />
    /// <summary>
    ///     An base of every <see cref="UIWindow" /> component.
    /// </summary>
    public abstract class UIWindowComponent : MonoBehaviour
    {
        /// <summary>
        ///     <see cref="UIWindow"/> component reference.
        /// </summary>
        [Header("Base Component Settings")]
        public UIWindow Window;

        private void Awake()
        {
            if (Window == null)
            {
                Window = GetComponentInParent<UIWindow>();
            }

            Debug.Assert(Window != null, "Window is missing!", this);
        }

        private void Reset()
        {
            Window = GetComponentInParent<UIWindow>();
        }
    }
}
