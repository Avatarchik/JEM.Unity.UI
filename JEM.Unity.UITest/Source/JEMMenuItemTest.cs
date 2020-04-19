//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Unity.UI.Systems;
using UnityEngine;

namespace JEM.Unity.UITest
{
    internal class JEMMenuItemTest : MonoBehaviour
    {
        private void Awake()
        {
            UIMenuItem.RegisterItem("File/Open", () => { Debug.Log("Open the file!"); });
            UIMenuItem.RegisterItem("File/Save/Me", () => { Debug.Log("Save me.!"); });
            UIMenuItem.RegisterItem("File/Save/Yikes", () => { Debug.Log("Save yikes!"); });
            UIMenuItem.RegisterItem("File/Save/Yikes2/Lolo", () => { Debug.Log("Save yikes lolo!"); });

            UIMenuItem.RegisterItem("Edit/Cut", () => { Debug.Log("Cut!"); });
            UIMenuItem.RegisterItem("Edit/Copy", () => { Debug.Log("Copy!"); });
            UIMenuItem.RegisterItem("Edit/Paste", () => { Debug.Log("Paste!"); });
        }
    }
}
