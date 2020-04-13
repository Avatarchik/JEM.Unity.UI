//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace JEM.Unity.UI.Systems.Tooltip.Implementations
{
    /// <inheritdoc />
    /// <summary>
    ///     A tooltip trigger of <see cref="JEMTextTooltip" /> controller.
    /// </summary>
    public class JEMTextTooltipTrigger : JEMPointerTooltipTrigger<JEMTextTooltipData>
    {
        [Header("Text Settings")]
        public JEMTextTooltipData TooltipData;

        private void Reset()
        {
            TooltipData = new JEMTextTooltipData
            {
                Text = "Unknown Text",
                LocaleGroup = "SYSTEM"
            };
        }

        /// <inheritdoc />
        public override bool ShouldTrigger() => !string.IsNullOrEmpty(TooltipData.Text) || !string.IsNullOrEmpty(TooltipData.LocaleKey);

        /// <inheritdoc />
        public override JEMTextTooltipData ResolveTriggerData() => TooltipData;
    }
}
