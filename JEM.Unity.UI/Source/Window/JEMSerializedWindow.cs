//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections.Generic;
using UnityEngine;

namespace JEM.Unity.UI.Window
{
    /// <summary>
    ///     A serialized <see cref="JEMInterfaceWindow"/> data.
    /// </summary>
    [Serializable]
    internal class JEMSerializedWindow
    {
        /// <summary>
        ///     A name of the window.
        /// </summary>
        public string WindowName;

        /// <summary>
        ///     Defines anchored position of the window.
        /// </summary>
        public float X, Y;

        /// <summary>
        ///     Defines size of the window.
        /// </summary>
        public float Width, Height;

        /// <summary>
        ///     Defines active state of window's <see cref="GameObject"/>.
        /// </summary>
        public bool ActiveState;

        /// <summary>
        ///     Additional data related to this serialized window.
        /// </summary>
        public Dictionary<string, object> AdditionalData = new Dictionary<string, object>();

        public Rect GetFixedRect() => new Rect(X, Y, Width, Height);
        public void SetFixedRect(Rect r)
        {
            X = r.x;
            Y = r.y;
            Width = r.width;
            Height = r.height;
        }
    }
}
