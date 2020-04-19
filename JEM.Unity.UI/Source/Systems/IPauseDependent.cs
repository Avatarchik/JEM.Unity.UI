//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

namespace JEM.Unity.UI.Systems
{
    /// <summary>
    ///     Implements pause dependent UI content.
    /// </summary>
    public interface IPauseDependent
    {
        /// <summary>
        ///     Sort order of this object.
        /// </summary>
        int SortOrder { get; }
        
        /// <summary>
        ///     Defines whether the content of the behaviour is active or not.
        /// </summary>
        bool IsContentActive { get; set; }
        
        /// <summary>
        ///     Sets the active state of the content.
        /// </summary>
        void SetContentActive(bool activeState);
    }
}