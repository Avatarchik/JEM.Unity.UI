//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using JetBrains.Annotations;

namespace JEM.Unity.UI.Systems
{
    /// <summary>
    ///     A MenuItem like system for unity UI.
    ///     This static class just have references to <see cref="UIMenuItemManager"/> class for shorter syntax.
    ///     For more properties or methods that are not implemented here, see <see cref="UIMenuItemManager"/> or <see cref="UIMenuItemElement"/>.
    /// </summary>
    public static class UIMenuItem
    {
        /// <summary>
        ///     Register new item in to the menu.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void RegisterItem([NotNull] string path, [NotNull] Action onClick) => UIMenuItemManager.RegisterItem(path, onClick);
    }
}
