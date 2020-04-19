//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

namespace JEM.Unity.UI.Systems
{
    public interface IUIBehaviourStateChanged
    {
        /// <summary>
        ///     Called when the <see cref="UIBehaviourHelper.LastContentState"/> changes.
        /// </summary>
        void OnStateChanged(UIContentState newState);
    }
}