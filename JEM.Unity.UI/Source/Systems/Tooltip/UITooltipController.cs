//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Unity.UI.Systems.Tooltip.Implementations;

namespace JEM.Unity.UI.Systems.Tooltip
{
    /// <summary>
    ///     Interface that implements data used by <see cref="UITextTooltip"/> based types to draw tooltip.
    /// </summary>
    public interface ITooltipTriggerData { }

    /// <inheritdoc />
    /// <summary>
    ///     A base controller class of Tooltip system.
    /// </summary>
    public abstract class UITooltipController<TTriggerData> : JEMSingletonBehaviour<UITooltipController<TTriggerData>>
        where TTriggerData : ITooltipTriggerData
    {
        /// <summary>
        ///     Reference to active tooltip trigger component.
        /// </summary>
        public UITooltipTrigger<TTriggerData> ActiveTrigger { get; private set; }

        /// <summary>
        ///     Disable active tooltip.
        /// </summary>
        public void Disable() => SetToolTip(null);

        /// <summary>
        ///     Sets the active tooltip.
        /// </summary>
        public void SetToolTip(UITooltipTrigger<TTriggerData> trigger)
        {
            ActiveTrigger = trigger;
            if (trigger == null)
            {
                OnTooltipActive(false);
            }
            else
            {
                OnTooltipActive(true);
                OnTooltipTrigger(trigger.ResolveTriggerData());
            }
        }

        protected abstract void OnTooltipActive(bool activeState);
        protected abstract void OnTooltipTrigger(TTriggerData data);
    }
}
