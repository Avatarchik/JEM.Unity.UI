//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace JEM.Unity.UI.Systems.Tooltip
{
    /// <inheritdoc cref="MonoBehaviour" />
    /// <summary>
    ///     A base of trigger components of tooltip system.
    /// </summary>
    public abstract class JEMTooltipTrigger<TTriggerData> : MonoBehaviour
        where TTriggerData: ITooltipTriggerData
    {
        /// <summary>
        ///     Called by <see cref="JEMTooltipController{TTriggerData}"/> to resolve <see cref="TTriggerData"/> type based data.
        /// </summary>
        public abstract TTriggerData ResolveTriggerData();

        /// <summary>
        ///     Called to know if this trigger should trigger :D
        /// </summary>
        public abstract bool ShouldTrigger();

        /// <summary>
        ///     Call <see cref="JEMTooltipController{TTriggerData}.SetToolTip"/> using this trigger.
        /// </summary>
        public void Trigger()
        {
            if (!ShouldTrigger())
            {
                return;
            }

            Controller.SetToolTip(this);
        }

        /// <summary>
        ///     Reference to the controller for this trigger.
        /// </summary>
        public static JEMTooltipController<TTriggerData> Controller => JEMTooltipController<TTriggerData>.Instance;
    }
}
