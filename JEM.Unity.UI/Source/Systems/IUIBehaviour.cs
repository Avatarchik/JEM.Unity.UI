//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;

namespace JEM.Unity.UI.Systems
{
    /// <summary>
    ///     A UI lock rules that are used when content of the UI controller is active.
    /// </summary>
    [Flags]
    public enum UILockRules : byte
    {
        /// <summary>
        ///     UI  does not lock anything.
        /// </summary>
        Nothing,
        
        /// <summary>
        ///     Camera will be locked in place.
        /// </summary>
        LockCamera,
        
        /// <summary>
        ///     Character's body will be locked in place.
        /// </summary>
        LockBody
    }
    
    /// <summary>
    ///     A base class that implements behaviour of in-game or menu UI.
    /// </summary>
    public interface IUIBehaviour
    {
        /// <summary>
        ///     Defined whether this controller is for in-game or menu UI.
        /// </summary>
        UIContentState ContentTarget { get; }
        
        /// <summary>
        ///     Lock rules of the UI.
        /// </summary>
        UILockRules LockRules { get; }

        /// <summary>
        ///     Defines whether the content of the behaviour is active or not.
        /// </summary>
        bool IsContentActive { get; set; }

        /// <summary>
        ///     Called when the UI content state changes.
        /// </summary>
        void OnStateChanged(UIContentState state);
        
        /// <summary>
        ///     Sets the active state of the content.
        /// </summary>
        void SetContentActive(bool activeState);
    }
}