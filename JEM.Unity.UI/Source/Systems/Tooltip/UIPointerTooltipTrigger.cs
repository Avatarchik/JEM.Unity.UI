//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;
using UnityEngine.EventSystems;

namespace JEM.Unity.UI.Systems.Tooltip
{
    /// <inheritdoc cref="UITooltipTrigger{TTriggerData}" />
    /// <summary>
    ///     A tooltip component that implements <see cref="T:UnityEngine.EventSystems.IPointerEnterHandler" /> and <see cref="T:UnityEngine.EventSystems.IPointerExitHandler" /> to trigger active tooltip.
    /// </summary>
    public abstract class UIPointerTooltipTrigger<TTriggerData> : UITooltipTrigger<TTriggerData>,
        IPointerEnterHandler, IPointerExitHandler where TTriggerData : ITooltipTriggerData
    {
        [Header("Pointer Settings")]
        public bool TriggerUsingPointerEvents = true;

        /// <inheritdoc />
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (!TriggerUsingPointerEvents)
            {
                return;
            }

            Trigger();
        }

        /// <inheritdoc />
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (!TriggerUsingPointerEvents)
            {
                return;
            }

            Controller.Disable();
        }
    }
}
