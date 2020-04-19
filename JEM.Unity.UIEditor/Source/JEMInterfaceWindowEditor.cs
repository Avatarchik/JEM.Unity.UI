//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Unity.UI.Window;
using UnityEditor;
using UnityEngine;

namespace JEM.Unity.UIEditor
{
    [CustomEditor(typeof(UIWindow))]
    internal sealed class JEMInterfaceWindowEditor : Editor
    {
        private UIWindow _script;

        private void OnEnable() => _script = target as UIWindow;
        
        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            // Do base GUI.
            base.OnInspectorGUI();

            if (_script.WindowTransform == null)
                return;

            if (_script.WindowTransform.pivot != new Vector2(0.5f, 0.5f))
            {
                EditorGUILayout.HelpBox("It's looks like you are using other pivot than (0.5f, 0.5f). " +
                                        "In current version, InterfaceWindow.UpdateDisplay " +
                                        "may not work properly.", MessageType.Error, true);

                if (GUILayout.Button("Reset"))
                {
                    _script.WindowTransform.pivot = new Vector2(0.5f, 0.5f);
                }
            }
        }
    }
}