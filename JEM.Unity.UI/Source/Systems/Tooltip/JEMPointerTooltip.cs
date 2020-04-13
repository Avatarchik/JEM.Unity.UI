//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Unity.UI.Animation;
using JEM.Unity.UI.Window;
using UnityEngine;

namespace JEM.Unity.UI.Systems.Tooltip
{
    /// <inheritdoc />
    /// <summary>
    ///     A tooltip controller that draws content under user's mouse and uses <see cref="JEMInterfaceWindow" /> to clamp transform in target workspace rect.
    /// </summary>
    public abstract class JEMPointerTooltip<TTriggerData> : JEMTooltipController<TTriggerData>
        where TTriggerData : ITooltipTriggerData
    {
        [Header("Window Settings")]
        public JEMInterfaceWindow TooltipWindow;
        public JEMInterfaceFadeAnimation TooltipPanel;

        [Header("Post-Processing")]
        public Vector2 Offset = new Vector2(0.5f, 0.5f);
        public Vector2 FixedPosition = new Vector2(0f, 0f);

        /// <inheritdoc />
        protected override void OnAwake() => TooltipPanel.SetActive(false);

        private void Update()
        {
            if (!TooltipPanel.IsActive) return;

            // Apply world position to window rect.
            var rectTransform = TooltipWindow.WindowTransform;
            rectTransform.position = (Vector2) Input.mousePosition;

            // Calculate and apply fixed offset to window.
            var offset = new Vector2(rectTransform.sizeDelta.x * Offset.x, rectTransform.sizeDelta.y * Offset.y);
            var nextPosition = rectTransform.anchoredPosition;

            // TEST
            // var xRatio = rectTransform.anchoredPosition.x / TooltipWindow.RootTransform.rect.width;
            var yRatio = rectTransform.anchoredPosition.y / TooltipWindow.RootTransform.rect.height;
            // var xSize = rectTransform.sizeDelta.x / TooltipWindow.RootTransform.rect.width;
            var ySize = rectTransform.sizeDelta.y / TooltipWindow.RootTransform.rect.height;

            if (ySize > yRatio)
            {
                nextPosition.y += rectTransform.sizeDelta.y;
                nextPosition -= FixedPosition;
            }
            else
            {
                nextPosition += FixedPosition;
            }

            // Debug.Log($"{xRatio}x{yRatio} vs {xSize}x{ySize}");

            nextPosition -= offset;

            // Apply rect position.
            rectTransform.anchoredPosition = nextPosition;

            // Clam window transform position to it's workspace.
            TooltipWindow.ClampWindowTransform();
        }

        /// <inheritdoc />
        protected override void OnTooltipActive(bool activeState) => TooltipPanel.SetActive(activeState);       
    }
}
