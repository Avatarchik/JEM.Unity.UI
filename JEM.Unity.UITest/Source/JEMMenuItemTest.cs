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
            JEMMenuItem.RegisterItem("File/Open", () => { Debug.Log("Open the file!"); });
            JEMMenuItem.RegisterItem("File/Save/Me", () => { Debug.Log("Save me.!"); });
            JEMMenuItem.RegisterItem("File/Save/Yikes", () => { Debug.Log("Save yikes!"); });
            JEMMenuItem.RegisterItem("File/Save/Yikes2/Lolo", () => { Debug.Log("Save yikes lolo!"); });

            JEMMenuItem.RegisterItem("Edit/Cut", () => { Debug.Log("Cut!"); });
            JEMMenuItem.RegisterItem("Edit/Copy", () => { Debug.Log("Copy!"); });
            JEMMenuItem.RegisterItem("Edit/Paste", () => { Debug.Log("Paste!"); });
        }
    }
}
