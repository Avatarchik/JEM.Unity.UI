//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using JEM.Core.Text;
using UnityEngine;
using UnityEngine.UI;

namespace JEM.Unity.UI.Systems.Tooltip.Implementations
{
    /// <inheritdoc cref="ITooltipTriggerData" />
    [Serializable]
    public struct JEMTextTooltipData : ITooltipTriggerData
    {
        /// <summary>
        ///     Text content to drawn of this tooltip.
        ///     NOTE: If localeKey is not empty, this value will be always overwritten by JEMLocale.Resolve method.
        /// </summary>
        [TextArea]
        public string Text;

        /// <summary>
        ///     Locale group of this tooltip.
        /// </summary>
        [Space]
        public string LocaleGroup;

        /// <summary>
        ///     Locale key of the tooltip.
        /// </summary>
        public string LocaleKey;
    }

    /// <inheritdoc />
    /// <summary>
    ///     Simple tooltip implementation that draws text or locale text data from received <see cref="JEMTextTooltipData" />.
    /// </summary>
    public class UITextTooltip : UIPointerTooltip<JEMTextTooltipData>
    {
        [Header("Text Settings")]
        public Text TextReference;

        /// <inheritdoc />
        protected override void OnTooltipTrigger(JEMTextTooltipData data)
        {
            string targetText;
            if (!string.IsNullOrEmpty(data.LocaleKey))
            {
                targetText = JEMLocale.Resolve(data.LocaleGroup, data.LocaleKey.ToUpper());
                targetText = targetText.Replace(@"\t", "\t");
            }
            else
            {
                targetText = data.Text;
            }

            TextReference.text = targetText;
        }
    }
}
