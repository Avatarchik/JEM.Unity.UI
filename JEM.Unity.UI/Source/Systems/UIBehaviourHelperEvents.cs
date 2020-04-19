//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections;
using JEM.Core.Common;
using JEM.Unity.Common;

namespace JEM.Unity.UI.Systems
{
    public abstract class UIBehaviourHelperEvents : JEMEventBehaviour
    {
        internal IUIBehaviourStateChanged UIBehaviourStateChanged;

        /// <inheritdoc />
        protected override byte ResolveGroup() => (byte) UI.UIEvents.UIBehaviour;

        /// <inheritdoc />
        protected override void LoadMethods()
        {
            UIBehaviourStateChanged = JEMSmartMethodBase.GetType<IUIBehaviourStateChanged>(this);
        }

        /// <inheritdoc cref="JEMEventBehaviour.ForEachCoroutine"/>
        internal static void ForEach(Action<UIBehaviourHelperEvents> action)
        {
            if (action == null) return;
            ForEach((byte) UI.UIEvents.UIBehaviour, action);
        }
        
        /// <inheritdoc cref="JEMEventBehaviour.ForEachCoroutine"/>
        internal static IEnumerator ForEachCoroutine(Func<UIBehaviourHelperEvents, IEnumerator> action)
        {
            if (action == null) yield return null;
            yield return ForEachCoroutine((byte) UI.UIEvents.UIBehaviour, action ?? throw new ArgumentNullException(nameof(action)));
        }
    }
}