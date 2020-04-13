//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using JEM.Core.Debugging;
using JEM.Unity.Attribute.Style;
using UnityEngine;
using UnityEngine.UI;

namespace JEM.Unity.UI
{
    /// <summary>
    ///     Cursor icon name.
    /// </summary>
    public enum JEMCursorIconName
    {
        /// <summary>
        ///     Default icon.
        /// </summary>
        Default,

        /// <summary>
        ///     Icon for moving something.
        /// </summary>
        Move,

        /// <summary>
        ///     Icon for resizing something.
        /// </summary>
        Resize,

        /// <summary>
        ///     Icon of rotating something.
        /// </summary>
        Rotate
    }

    /// <inheritdoc />
    /// <summary>
    ///     Interface cursor.
    /// </summary>
    [AddComponentMenu("JEM/UI/JEM Cursor Manager")]
    [DisallowMultipleComponent]
    public sealed class JEMInterfaceCursor : JEMSingletonBehaviour<JEMInterfaceCursor>
    {
        /// <summary>
        ///     Cursor UI image.
        /// </summary>
        [Header("Interface Cursor")]
        public Image CursorImage;

        /// <summary>
        ///     Cursor icon - default.
        /// </summary>
        [Space]
        public Sprite CursorIconDefault;

        /// <summary>
        ///     Cursor icon - move.
        /// </summary>
        public Sprite CursorIconMove;

        /// <summary>
        ///     Cursor icon - resize.
        /// </summary>
        public Sprite CursorIconResize;

        /// <summary>
        ///     Cursor icon - rotate.
        /// </summary>
        public Sprite CursorIconRotate;

        [Header("Debug")]
        [JEMReadonly]
        public List<string> ListOfLookcers;

        [JEMReadonly]
        public List<string> ListOfUnlockers;

        /// <inheritdoc />
        protected override void OnAwake()
        {
            // ignore
        }

        private void Start()
        {
            SetCursorIcon(JEMCursorIconName.Default);
        }

        private void Update()
        {
            if (Application.isEditor)
            {
                ListOfLookcers.Clear();
                foreach (var l in CursorLock)
                    if (l.Value)
                        ListOfLookcers.Add(l.Key);
                ListOfUnlockers.Clear();
                foreach (var l in CursorLock)
                    if (!l.Value)
                        ListOfUnlockers.Add(l.Key);
            }

            InternalUpdateCursorState();
        }

        private void InternalUpdateCursorState()
        {
            if (CursorImage != null)
            {
                if (CursorLocked)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false; //!(CursorImage.sprite == null);
                    CursorImage.enabled = false;//!(CursorImage.sprite != null);
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = CursorImage.sprite == null;
                    CursorImage.enabled = CursorImage.sprite != null;
                }

                if (UpdatePosition)
                    CursorImage.transform.position = Input.mousePosition;
            }
            else
            {
                if (CursorLocked)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }

        /// <summary>
        ///     Lock cursor under given name.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="state"></param>
        /// <param name="forceNewState">Force to update cursor in this frame.</param>
        public static void LockCursor(string source, bool state, bool forceNewState = false)
        {
            if (string.IsNullOrEmpty(source)) source = "default";

            if (CursorLock.ContainsKey(source))
                CursorLock[source] = state;
            else
                CursorLock.Add(source, state);

            CursorLocked = true;
            foreach (var i in CursorLock)
            {
                if (i.Value) continue;
                CursorLocked = false;
                break;
            }

            if (Instance == null || !forceNewState)
                return;

            Instance.InternalUpdateCursorState();
        }

        /// <summary>
        ///     Gets state of cursor lock.
        /// </summary>
        public static bool GetCursorLock(string source)
        {
            return CursorLock.ContainsKey(source) && CursorLock[source];
        }

        /// <summary>
        ///     Clears cursor lock.
        /// </summary>
        public static void ClearCursorLock()
        {
            CursorLock.Clear();
            CursorLocked = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        /// <summary>
        ///     Moves cursor to given position in 2d space.
        /// </summary>
        public static void SetFixedCursorPosition(Vector2 position)
        {
            SetCursorPos((int) position.x, (int) position.y);
        }

        /// <summary>
        ///     Gets fixed cursor position in 2d space.
        /// </summary>
        public static Vector2 GetFixedCursorPosition()
        {
            GetCursorPos(out var point);
            return point;
        }

        /// <summary>
        ///     Moves cursor to given position in world space.
        /// </summary>
        public static void SetCursorPosition(Vector3 position)
        {
            SetCursorPosition(Camera.main, position);
        }

        /// <summary>
        ///     Moves cursor to given position in world space.
        /// </summary>
        public static void SetCursorPosition(Camera camera, Vector3 position)
        {
            Vector2 inputCursor = Input.mousePosition;
            inputCursor.y = Screen.height - 1 - inputCursor.y;
            GetCursorPos(out var point);
            var renderRegionOffset = point - inputCursor;
            var newXPos = (int) (camera.WorldToScreenPoint(position).x + renderRegionOffset.x);
            var newYPos = (int) (Screen.height - camera.WorldToScreenPoint(position).y + renderRegionOffset.y);

            SetCursorPos(newXPos, newYPos);
        }

        /// <summary>
        ///     Sets current cursor.
        /// </summary>
        public static void SetCursorIcon(JEMCursorIconName name)
        {
            if (Instance == null)
                return;

            if (Instance.CursorImage == null)
            {
                return;
            }

            CurrentCursorIconName = name;
            switch (CurrentCursorIconName)
            {
                case JEMCursorIconName.Default:
                    Instance.CursorImage.sprite = Instance.CursorIconDefault;
                    break;
                case JEMCursorIconName.Move:
                    Instance.CursorImage.sprite = Instance.CursorIconMove;
                    break;
                case JEMCursorIconName.Resize:
                    Instance.CursorImage.sprite = Instance.CursorIconResize;
                    break;
                case JEMCursorIconName.Rotate:
                    Instance.CursorImage.sprite = Instance.CursorIconRotate;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (Instance.CursorImage.sprite != null) return;
            if (Instance.CursorIconDefault == null)
                return;
            JEMLogger.LogWarning($"System was unable to set cursor icon {name}. " +
                                 "Target sprite is not set.", JEMLogSource.JEM);
            Instance.CursorImage.sprite = Instance.CursorIconDefault;
            Instance.CursorImage.enabled = Instance.CursorImage.sprite != null;
        }

        [DllImport("user32.dll")]
        internal static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(out Point pos);

        /// <summary>
        ///     Point on screen.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
#pragma warning disable 1591
            public int X;
            public int Y;

            public static implicit operator Vector2(Point p)
            {
                return new Vector2(p.X, p.Y);
            }
#pragma warning restore 1591
        }


        private static Dictionary<string, bool> CursorLock { get; } = new Dictionary<string, bool>();

        /// <summary>
        ///     Cursor lock state.
        /// </summary>
        public static bool CursorLocked { get; private set; }

        /// <summary>
        ///     Defines whether the position of cursor should be updated.
        /// </summary>
        public static bool UpdatePosition { get; set; } = true;

        /// <summary>
        ///     Current interface cursor.
        /// </summary>
        public static JEMCursorIconName CurrentCursorIconName { get; private set; } = JEMCursorIconName.Default;
    }
}