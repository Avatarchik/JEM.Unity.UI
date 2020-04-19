//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections.Generic;
using System.IO;
using JEM.Unity.Attribute.Behaviour;
using JEM.Unity.Attribute.Defaults;
using JEM.Unity.Attribute.Style;
using JEM.Unity.Extension;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace JEM.Unity.UI.Window
{
    /// <inheritdoc cref="MonoBehaviour" />
    /// <summary>
    ///     Draggable and re-sizable window for unity's UI system.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("JEM/UI/Window/Window Base"), HelpURL("https://github.com/TylkoDemon/JEM.Unity/wiki")]
    public sealed class UIWindow : MonoBehaviour, IPointerDownHandler
    {
        /// <summary>
        ///     An unique window name.
        /// </summary>
        /// <remarks>
        ///     You can use this field to know what window is for.
        /// </remarks>
        [JEMHeader("Identity Settings")]
        public string UniqueWindowName = "Unknown_Window";

        /// <summary>
        ///     An root <see cref="RectTransform"/> of window.
        /// </summary>
        [JEMHeader("Base Settings"), JEMInfo(JEMInfoCondition.NegativeValue, "Object reference is missing!", Type = JEMInfoMessageType.Error)]
        public RectTransform WindowTransform;

        /// <summary>
        ///     Allow to drag this window.
        /// </summary>
        [JEMIndent, JEMShowIf(nameof(WindowTransform))]
        public bool AllowDragging;

        /// <summary>
        ///     Allow to resize this window.
        /// </summary>
        [JEMHeader("Resizing")]
        [JEMIndent, JEMShowIf(nameof(WindowTransform))]
        public bool AllowResize;

        /// <summary>
        ///     Defines if x axis can be resized.
        /// </summary>
        [JEMIndent(2), JEMShowIf(nameof(AllowResize))]
        public bool ResizeXAxis = true;

        /// <summary>
        ///     Defines if y axis can be resized.
        /// </summary>
        [JEMIndent(2), JEMShowIf(nameof(AllowResize))]
        public bool ResizeYAxis = true;

        /// <summary>
        ///     Defines whether system could save and restore state of this window.
        /// </summary>
        [JEMHeader("Serialization")]
        [JEMIndent]
        public bool AllowWindowSaving = true;

        /// <summary>
        ///     When true, window will also serialize and restore gameObject.activeSelf state.
        /// </summary>
        [JEMIndent(2), JEMShowIf(nameof(AllowWindowSaving))]
        public bool SerializeGameobjectState = true;

        /// <summary>
        ///     State of the window will be automatically saved at <see cref="OnDisable"/> method.
        /// </summary>
        [JEMIndent(2), JEMShowIf(nameof(AllowWindowSaving))]
        public bool AutoSaveAtDisable = true;

        /// <summary>
        ///     State of the window will be automatically restored at <see cref="OnEnable"/> method.
        /// </summary>
        [JEMIndent(2), JEMShowIf(nameof(AllowWindowSaving))]
        public bool AutoRestoreAtEnable = true;

        /// <summary>
        ///     If set to true, the window will always move on top of others if moved.
        /// </summary>
        [JEMHeader("Msc")]
        [JEMIndent, JEMShowIf(nameof(WindowTransform))]
        public bool AlwaysMoveOnTop = true;

        /// <summary>
        ///     Defines whether the window size clamp feature should be enabled.
        /// </summary>
        [JEMHeader("Size Settings")]
        public bool ClampWindowSize = true;

        /// <summary>
        ///     Maximal possible window size.
        /// </summary>
        [JEMIndent, JEMShowIf(nameof(ClampWindowSize))]
        public Rect WindowMinMaxSize;

        /// <summary>
        ///     Root transform of this window that defines on what space this window should work.
        /// </summary>
        /// <remarks>
        ///     User will be unable to move this window outside given <see cref="RectTransform"/> rect.
        /// </remarks>
        [JEMHeader("Workspace"), JEMInfo(JEMInfoCondition.NegativeValue, "RootTransform reference is missing!", Type = JEMInfoMessageType.Error)]
        public RectTransform RootTransform;

        /// <summary>
        ///     Called on object enable.
        /// </summary>
        [Space]
        [JEMFoldoutBegin("Events"), JEMUnityEventDrawer]
        public UnityEvent OnWindowEnabled;

        /// <summary>
        ///     Called on object disable.
        /// </summary>
        [JEMFoldoutEnd, JEMUnityEventDrawer]
        public UnityEvent OnWindowDisabled;

        /// <summary>
        ///     Name of <see cref="RectTransform"/> anchor.
        /// </summary>
        public JEMRectAnchorName CurrentAnchorName { get; private set; }

        /// <summary>
        ///     A initialize anchored position and size of window.
        /// </summary>
        public Rect InitialFixedRect { get; private set; }

        /// <summary>
        ///     List of additional serialized data that should be saved and restored with window state.
        /// </summary>
        public Dictionary<string, object> AdditionalSerializedData { get; set; } = new Dictionary<string, object>();

        private void Awake()
        {
            // Try to get RectTransform from this object if missing.
            if (WindowTransform == null) WindowTransform = GetComponent<RectTransform>();
            // Try to get root canvas if missing
            if (RootTransform == null)
            {
                var rootCanvas = GetComponentInParent<Canvas>();
                if (rootCanvas != null)
                    RootTransform = rootCanvas.GetComponent<RectTransform>();
            }

            Debug.Assert(WindowTransform != null, "WindowTransform is missing!", this);
            Debug.Assert(RootTransform != null, "RootTransform is missing!", this);

            // Get anchor of target transform.
            CurrentAnchorName = WindowTransform.GetAnchor();

            // Apply initial rect
            InitialFixedRect = WindowTransform.GetFixedRect();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(UniqueWindowName))
            {
                Debug.LogError("UniqueName field of this JEMInterfaceWindow is empty.", this);
            }
            else
            {
                if (UniqueWindowName.Contains(" "))
                {
                    Debug.LogError("UniqueName of JEMInterfaceWindow can't contains spaces.", this);
                }
            }

            if (RootTransform != transform.parent)
                Debug.LogError("Using RootTransform that is not a parent of window is not currently supported!", this);
        }

        private void Reset()
        {
            WindowTransform = GetComponent<RectTransform>();
            if (WindowTransform != null)
            {
                WindowMinMaxSize = new Rect(WindowTransform.sizeDelta / 2, WindowTransform.sizeDelta * 2);
            }

            var rootCanvas = GetComponentInParent<Canvas>();
            RootTransform = rootCanvas.GetComponent<RectTransform>();
        }
#endif

        private bool _firstGameobjectStateDeserialized;
        public void OnEnable()
        {
            if (AllowWindowSaving && AutoRestoreAtEnable)
            {
                RestoreState(!_firstGameobjectStateDeserialized, true);
                _firstGameobjectStateDeserialized = true;
            }

            OnWindowEnabled.Invoke();
        }

        private void OnDisable()
        {
            if (AllowWindowSaving && AutoSaveAtDisable)
            {
                SaveState();
            }

            OnWindowDisabled?.Invoke();
        }

        /// <summary>
        ///     Clamps window size and anchored position.
        /// </summary>
        public void ClampWindowTransform()
        {
            if (WindowTransform == null) return;
            if (RootTransform == null) throw new NullReferenceException(nameof(RootTransform));

            // Clamp window size.
            if (ClampWindowSize)
            {
                WindowTransform.ClampSizeToRect(WindowMinMaxSize);
            }

            // Clamp window position.
            if (RootTransform == transform.parent)
                WindowTransform.ClampPositionToRegion(CurrentAnchorName, RootTransform.rect.size);
            else
            {
                // TODO
            }
        }

        /// <inheritdoc />
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (!AlwaysMoveOnTop) return;
            MoveOnTop();
        }

        /// <summary>
        ///     Moves window on top in local container.
        /// </summary>
        public void MoveOnTop()
        {
            WindowTransform.SetAsLastSibling();
        }

        /// <summary>
        ///     Restarts this window.
        /// </summary>
        public void Restart()
        {
            if (AnyWindowIsUnderMotion) UICursorHelper.SetCursorIcon(JEMCursorIconName.Default);

            var headers = GetComponentsInChildren<UIWindowHeader>();
            foreach (var h in headers) h.Restart();

            var size = GetComponentsInChildren<UIWindowResize>();
            foreach (var s in size) s.Restart();
        }

        /// <summary>
        ///     Saves the state of window.
        /// </summary>
        public void SaveState()
        {
            if (!AllowWindowSaving)
            {
                return;
            }

            // Create serialized window obj.
            var serializedWindow = new UISerializedWindow
            {
                WindowName = UniqueWindowName,
                ActiveState = gameObject.activeSelf,
                AdditionalData = AdditionalSerializedData
            };

            // Apply fixed rect.
            serializedWindow.SetFixedRect(WindowTransform.GetFixedRect());

            // Append serialized window.
            AppendSerializedWindow(serializedWindow);
        }

        /// <summary>
        ///     Restores the state of window.
        /// </summary>
        public bool RestoreState(bool shouldActivateGameObject, bool silently = false)
        {
            if (!AllowWindowSaving)
            {
                return false;
            }

            // Get serialized window data.
            var window = GetSerializedWindow(UniqueWindowName);
            if (window == null)
            {
                if (!silently)
                    Debug.LogWarning("You are trying to restore state of window that " +
                                     "does not have it's JEMSerializedWindow data generated. To restore state of window, you need call SaveState first.", this);
                return false;
            }

            // Restore data
            AdditionalSerializedData = window.AdditionalData ?? new Dictionary<string, object>();

            // Restore transform state.
            WindowTransform.SetFixedRect(window.GetFixedRect());

            if (SerializeGameobjectState && shouldActivateGameObject)
            {
                // Restore gameObject state
                gameObject.SetActive(window.ActiveState);
            }

            // Make sure that window clamps to active workspace.
            ClampWindowTransform();

            return true;
        }

        /// <summary>
        ///     Restarts all windows.
        /// </summary>
        public static void RestartAll()
        {
            var windows = FindObjectsOfType<UIWindow>();
            foreach (var window in windows) window.Restart();
        }

        private static UISerializedWindow GetSerializedWindow(string windowName)
        {
            ResolveSerializedWindows();
            for (var index = 0; index < SerializedWindows.Count; index++)
            {
                var s = SerializedWindows[index];
                if (string.Equals(s.WindowName, windowName, StringComparison.CurrentCultureIgnoreCase))
                    return s;
            }

            return null;
        }

        private static void AppendSerializedWindow([NotNull] UISerializedWindow window)
        {
            if (window == null) throw new ArgumentNullException(nameof(window));
            var existingWindow = GetSerializedWindow(window.WindowName);
            if (existingWindow != null)
                SerializedWindows.Remove(existingWindow);
            SerializedWindows.Add(window);

            var path = Environment.CurrentDirectory + "\\" + SerializedWindowsDirectory + "\\" + window.WindowName.ToLower() + ".json";
            File.WriteAllText(path, JsonConvert.SerializeObject(window, Formatting.Indented));
        }

        private static void ResolveSerializedWindows()
        {
            if (SerializedWindows != null)
            {
                return;
            }

            SerializedWindows = new List<UISerializedWindow>();
            var dir = Environment.CurrentDirectory + "\\" + SerializedWindowsDirectory;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                return;
            }

            var files = Directory.GetFiles(dir, "*.json");
            foreach (var f in files)
            {
                var window = JsonConvert.DeserializeObject<UISerializedWindow>(File.ReadAllText(f));
                if (window == null)
                    continue;
                
                SerializedWindows.Add(window);
            }
        }

        /// <summary>
        ///     True if any window is currently moving or re-sized by user.
        /// </summary>
        public static bool AnyWindowIsUnderMotion => UIWindowHeader.AnyHeaderIsDragging || UIWindowResize.AnyWindowIsResized;

        /// <summary>
        ///     Relative path to the save directory of serialized windows data.
        /// </summary>
        public static string SerializedWindowsDirectory = "Config\\Windows";

        private static List<UISerializedWindow> SerializedWindows { get; set; }
    }
}