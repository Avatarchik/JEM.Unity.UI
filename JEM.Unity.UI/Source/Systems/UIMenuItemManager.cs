//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections.Generic;
using JEM.Unity.UI.Animation;
using JetBrains.Annotations;
using UnityEngine;

namespace JEM.Unity.UI.Systems
{
    /// <inheritdoc />
    /// <summary>
    ///     A manager of context menu for unity ui.
    /// </summary>
    [DefaultExecutionOrder(-10)]
    public class UIMenuItemManager : JEMSingletonBehaviour<UIMenuItemManager>
    {
        internal struct Cell
        {
            public string Name;
            public UIMenuItemElement Interface;

            public List<Cell> Children;

            public bool IsValid;

            public RectTransform CellContentDown => Interface.CellContentDown.GetComponent<RectTransform>();
            public RectTransform CellContentRight => Interface.CellContentRight.GetComponent<RectTransform>();

            /// <summary>
            ///     Gets a cell of <see cref="cellName"/>.
            /// </summary>
            /// <exception cref="ArgumentNullException"/>
            public Cell GetCell([NotNull] string cellName)
            {
                if (cellName == null) throw new ArgumentNullException(nameof(cellName));
                if (Children == null) return default;

                foreach (var c in Children)
                {
                    if (c.Name == cellName)
                        return c;
                }

                return default;
            }
        }

        [Header("Settings")]
        public RectTransform ItemsContent;
        public GameObject ItemPrefab;
        public UIFadeAnimation LockMask;

        /// <inheritdoc />
        protected override void OnAwake()
        {
            // ignore
        }

        /// <summary>
        ///     Disable the active item menu.
        /// </summary>
        public void DisableMenu()
        {
            foreach (var i in UIMenuItemElement.Items)
            {
                i.SetState(false);
            }

            UIMenuItemElement.SetRootDrawn(false);
        }

        /// <summary>
        ///     Sets 
        /// </summary>
        public void TagMouseLost() => WasMouseLost = true;
      
        /// <summary>
        ///     Register new item in to the menu.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void RegisterItem([NotNull] string path, [NotNull] Action onClick)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (onClick == null) throw new ArgumentNullException(nameof(onClick));
            if (Instance == null) throw new NullReferenceException("There is not active instance of JEMInterfaceMenuItemManager on the scene!");

            var cells = path.Split('/');
            if (cells.Length == 0)
            {
                Debug.LogWarning($"Failed to register menu item of path '{path}'. Unable to resolve any cells.");
                return;
            }

            if (cells.Length < 2)
            {
                Debug.LogError($"Failed to register menu item of path '{path}'. Root can't be a final item.");
                return;
            }

            var rootName = cells[0];
            var fullPath = rootName;
            var rootCell = GetCell(rootName);
            if (!rootCell.IsValid)
            {
                rootCell = CreateNewCell(rootName, Instance.ItemsContent, true, fullPath);
                RootCells.Add(rootCell);
            }

            var prevCell = rootCell;
            for (int index = 1; index < cells.Length; index++)
            {
                var isFinal = index +1 >= cells.Length;
                var cellName = cells[index];
                var cell = prevCell.GetCell(cellName);
                if (!cell.IsValid)
                {
                    fullPath += $"/{cellName}";
                    cell = CreateNewCell(cellName, index == 1 ? prevCell.CellContentDown : prevCell.CellContentRight, false, fullPath);
                    prevCell.Children.Add(cell);
                }
                else
                {
                    fullPath += $"/{cellName}";
                }

                prevCell = cell;

                if (isFinal)
                {
                    prevCell.Interface.OnClick = onClick;
                }
            }
        }

        /// <summary>
        ///     Creates new <see cref="Cell"/> struct with <see cref="UIMenuItemElement"/> instance.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        internal static Cell CreateNewCell([NotNull] string cellName, [NotNull] RectTransform root, bool isRoot, string fullPath)
        {
            if (cellName == null) throw new ArgumentNullException(nameof(cellName));
            if (root == null) throw new ArgumentNullException(nameof(root));
            var cellUI = CreateNewItem(root);
            cellUI.SetInterface(cellName, isRoot, fullPath);

            var rootCell = new Cell
            {
                Name = cellName,
                Interface = cellUI,

                Children = new List<Cell>(),
                IsValid = true
            };

            return rootCell;
        }

        /// <summary>
        ///     Creates new <see cref="UIMenuItemElement"/> instance.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        internal static UIMenuItemElement CreateNewItem([NotNull] RectTransform root)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));
            var obj = JEMObject.Instantiate(Instance.ItemPrefab, root);
            var item = obj.GetComponent<UIMenuItemElement>();
            if (item == null)
                throw new NullReferenceException(nameof(item));
            obj.SetActive(true);

            return item;
        }

        /// <summary>
        ///     Gets a cell by <paramref name="cellName"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        internal static Cell GetCell([NotNull] string cellName)
        {
            if (cellName == null) throw new ArgumentNullException(nameof(cellName));
            foreach (var c in RootCells)
            {
                if (c.IsValid && c.Name == cellName)
                    return c;
            }

            return default;
        }

        /// <summary>
        ///     List of all root cells.
        /// </summary>
        internal static List<Cell> RootCells { get; } = new List<Cell>();

        /// <summary>
        ///     Defines if mouse on active menu was lost.
        /// </summary>
        internal static bool WasMouseLost { get; set; }
    }
}
