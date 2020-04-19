//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

// #define MENUITEM_DEBUG

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JEM.Unity.UI.Animation;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JEM.Unity.UI.Systems
{
    public class UIMenuItemElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("Settings")]
        public Text Text;
        public Image Image;
        public Color NormalColor;
        public Color HighlightColor;
        public UIFadeAnimation CellContentRight;
        public UIFadeAnimation CellContentDown;

        public string FullPath;
        public bool IsRoot;
        public Action OnClick;

        /// <summary>
        ///     Defines if content of this item is currently drawn to user.
        /// </summary>
        public bool IsEnabled { get; private set; } = true;

        /// <summary>
        ///     True if this item can be currently enabled by user mouse input.
        /// </summary>
        public bool CanEnable { get; private set; }

        /// <summary>
        ///     Updates the interface of this item.
        /// </summary>
        public void SetInterface(string cellName, bool isRoot, string fullPath)
        {
            Text.text = cellName;
            IsRoot = isRoot;
            FullPath = fullPath;
        }

        /// <summary>
        ///     Returns the name of the cell.
        /// </summary>
        public string GetCellName()
        {
            var call = FullPath.Split('/');
            return call[call.Length - 1];
        }

        private void Awake() => Items.Add(this);
        private void OnDestroy() => Items.Remove(this);
        private void OnEnable() => SetState(false);
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!CanEnableAny) return;
            if (IsDisabled || !CanEnable) return;

#if DEBUG && MENUITEM_DEBUG
            Debug.Log($"JEMInterfaceMenuItem.OnPointerEnter() itemName_{FullPath}", this);
#endif

            if (IsRoot)
            {
                SetHighlight(true);

                // Root cell can only be enabled by clicking on it.
                if (ShouldRootDrawn && !IsEnabled)
                {
                    foreach (var i in Items)
                    {
                        if (i.IsRoot && i.IsEnabled)
                        {
                            i.SetState(false);
                            i.DisableChildren();
                        }
                    }

                    SetState(true);
                }

                return;
            }


            // Try to rebuild the active path.
            if (IsEnabled && !HasChildrenDrawn() && !IsChildActive() || !IsEnabled)
            {
                RebuildMenu(FullPath);
            }

            ItemPath = FullPath;

            SetState(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
#if DEBUG && MENUITEM_DEBUG
            Debug.Log($"JEMInterfaceMenuItem.OnPointerExit() itemName_{FullPath}", this);
#endif
            if (!IsEnabled)
            {
                SetHighlight(false);
                return;
            }

            if (!IsChildActive() && !HasChildrenDrawn())
            {
                SetHighlight(false);
            }
            else if (ItemPath != FullPath && !HasChildrenDrawn())
            {
                RebuildMenu(FullPath);
            }

            //if (IsDisabled) return;
            //StartCoroutine(WaitForChildToEnable());
        }

        /*
        private IEnumerator WaitForChildToEnable()
        {
            yield return new WaitForSeconds(0.05f);
            if (IsDisabled) yield break;
            if (!IsEnabled)
                yield break;

            SetState(!IsChildActive());
        }
        */

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!CanEnable)
                return;

            if (IsRoot)
            {
                if (!IsEnabled)
                {
                    SetRootDrawn(true);
                    SetState(true);
                }
                else
                {
                    SetRootDrawn(false);
                    SetState(false);
                }

                return;
            }

            SetState(false);
            DisableParent();
            SetRootDrawn(false);
            JEMOperation.StartCoroutine(GlobalEnableTimeout());
            OnClick?.Invoke();
        }

        /// <summary>
        ///     Sets the active state of this item.
        /// </summary>
        internal void SetState(bool state)
        {
            if (IsEnabled == state)
            {
                return;
            }

#if DEBUG && MENUITEM_DEBUG
            Debug.Log($"JEMInterfaceMenuItem.SetState({state}) itemName_{FullPath}", this);
#endif
            IsEnabled = state;
            SetHighlight(state);

            var cellState = false;
            foreach (var unused in CellContentRight.transform)
            {
                cellState = true;
                break;
            }
            CellContentRight.SetActive(state && cellState);

            cellState = false;
            foreach (var unused in CellContentDown.transform)
            {
                cellState = true;
                break;
            }
            CellContentDown.SetActive(state && cellState);

            if (state) return;
            // Timeout next enable.
            JEMOperation.StartCoroutine(TimeoutForEnable());

            // Try to disable all children
            // DisableChildren();
        }

        private IEnumerator TimeoutForEnable()
        {
            CanEnable = false;
            yield return new WaitForSeconds(0.05f);
            CanEnable = true;
        }

        /// <summary>
        ///     Sets the state of highlight.
        /// </summary>
        internal void SetHighlight(bool state)
        {
#if DEBUG && MENUITEM_DEBUG
            Debug.Log($"JEMInterfaceMenuItem.SetHighlight({state}) itemName_{FullPath}", this);
#endif

            Image.color = state ? HighlightColor : NormalColor;
        }

        private bool HasChildrenDrawn()
        {
            var allChildren = transform.GetComponentsInChildren<UIMenuItemElement>(true);
            foreach (var c in allChildren)
            {
                if (c == this) continue;
                if (c.isActiveAndEnabled) return true;
            }

            return false;
        }

        /// <summary>
        ///     Checks if any of the children is active.
        /// </summary>
        private bool IsChildActive()
        {
            var allChildren = transform.GetComponentsInChildren<UIMenuItemElement>(true);
            foreach (var c in allChildren)
            {
                if (c == this) continue;
                if (c.IsEnabled) return true;
            }

            return false;
        }

        /// <summary>
        ///     Disables the active children of this item.
        /// </summary>
        private void DisableChildren()
        {
            var allChildren = transform.GetComponentsInChildren<UIMenuItemElement>(true);
            foreach (var c in allChildren)
            {
                if (c == this) continue;
                if (c.IsEnabled)
                {
                    JEMOperation.StartCoroutine(c.DisableTimeout());
                    c.SetState(false);
                }
            }
        }

        /// <summary>
        ///     Checks if any of the parents is active.
        /// </summary>
        private bool IsParentActive()
        {
            var allChildren = transform.GetComponentsInParent<UIMenuItemElement>(true);
            foreach (var c in allChildren)
            {
                if (c == this) continue;
                if (c.IsEnabled) return true;
            }

            return false;
        }

        /// <summary>
        ///     Disables the active parents of this item.
        /// </summary>
        private void DisableParent()
        {
            var allParent = transform.GetComponentsInParent<UIMenuItemElement>(true);
            foreach (var c in allParent)
            {
                if (c == this) continue;
                if (c.IsEnabled)
                {
                    JEMOperation.StartCoroutine(c.DisableTimeout());
                    c.SetState(false);
                }
            }
        }

        private bool IsDisabled { get; set; }
        private IEnumerator DisableTimeout()
        {
            IsDisabled = true;
            yield return new WaitForSeconds(0.15f);
            IsDisabled = false;
        }

        private static IEnumerator GlobalEnableTimeout()
        {
            CanEnableAny = false;
            yield return new WaitForSeconds(0.15f);
            CanEnableAny = true;
        }

        private static bool CanEnableAny { get; set; } = true;

        /// <summary>
        ///     Rebuilds the active menu to given path.
        /// </summary>
        internal static void RebuildMenu(string path)
        {
#if DEBUG && MENUITEM_DEBUG
            Debug.Log($"JEMInterfaceMenuItem.RebuildPath({path ?? "null!"})");
#endif
            if (path == null) return;

            // Collect all the cells.
            var cells = path.Split('/');
            var targetCell = GetCell(cells[0]);
            if (targetCell == null)
                return;

            // Activate target cells.
            var index = 0;
            var exclutedCells = new List<UIMenuItemElement>();
            while (targetCell != null)
            {
                if (!targetCell.IsEnabled)
                    targetCell.SetState(true);
                exclutedCells.Add(targetCell);

                index++;
                targetCell = index < cells.Length ? targetCell.GetLocalCell(cells[index]) : null;
            }

            // Disable old cells.
            foreach (var i in Items)
            {
                if (exclutedCells.Contains(i))
                    continue;

                if (i.IsEnabled)
                    i.SetState(false);
            }
        }

        /// <summary>
        ///     Sets the drawn state of root panel.
        ///     When set to true, all next root menus will be drawn on mouse enter.
        /// </summary>
        internal static void SetRootDrawn(bool activeState)
        {
            // Set the state.
            ShouldRootDrawn = activeState;
            // Set the state of lock mask so user will be unable to interact with the rest of UI while menu is active.
            UIMenuItemManager.Instance.LockMask.SetActive(activeState);
        }

        /// <summary>
        ///     Returns a local cell of given name
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        internal UIMenuItemElement GetLocalCell([NotNull] string cellName)
        {
            if (cellName == null) throw new ArgumentNullException(nameof(cellName));
            return GetComponentsInChildren<UIMenuItemElement>().FirstOrDefault(i => i.GetCellName() == cellName);
        }

        /// <summary>
        ///     Returns a cell of given name.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        internal static UIMenuItemElement GetCell([NotNull] string cellName)
        {
            if (cellName == null) throw new ArgumentNullException(nameof(cellName));
            return Items.FirstOrDefault(i => i.GetCellName() == cellName);
        }

        internal static string ItemPath { get; private set; }

        /// <summary>
        ///     Defines if any root of items menu is or can be drawn.
        /// </summary>
        internal static bool ShouldRootDrawn { get; private set; }

        /// <summary>
        ///     List of all menu items in scene.
        /// </summary>
        internal static List<UIMenuItemElement> Items { get; } = new List<UIMenuItemElement>();
    }
}
