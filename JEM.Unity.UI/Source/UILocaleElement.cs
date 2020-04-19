//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace JEM.Unity.UI
{
    /// <inheritdoc />
    /// <summary>
    ///     Base element of every locale based interface element.
    /// </summary>
    public abstract class UILocaleElement : MonoBehaviour
    {
        private void OnEnable()
        {
            // Always refresh locale on enable...
            RefreshLocale();
        }

        /// <summary>
        ///     Refresh text of this element from locale.
        /// </summary>
        public abstract void RefreshLocale();

        /// <summary>
        ///     Refresh all active locale elements in scene.
        /// </summary>
        public static void RefreshAll()
        {
            var elements = FindObjectsOfType<UILocaleElement>();
            foreach (var e in elements)
            {
                e.RefreshLocale();
            }
        }
    }
}
