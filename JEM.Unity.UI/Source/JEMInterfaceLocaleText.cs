//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JEM.Unity.UI
{
    /// <inheritdoc />
    /// <summary>
    ///     Simple UI component for any type of text that will apply locale string via defined key and group.
    /// </summary>
    /// <remarks>
    ///    Possible string formatting.
    /// </remarks>
    [AddComponentMenu("JEM/UI/JEM Locale Text"), DisallowMultipleComponent]
    public class JEMInterfaceLocaleText : JEMInterfaceLocaleElement
    {
        /// <summary>
        ///     Key of the locale.
        /// </summary>
        [Header("Settings")]
        public string Key = "UNKNOWN_KEY";

        /// <summary>
        ///     Group of the locale.
        /// </summary>
        public string Group = "SYSTEM";

        /// <summary>
        ///     String array that will be given as parameter to string.Format
        /// </summary>
        public string[] AutoFormat = new string[0];

        /// <summary>
        ///     String that will be added at the beginning of the string.
        /// </summary>
        public string AutoStart;

        /// <summary>
        ///     String that will be added at the end of the string.
        /// </summary>
        public string AutoEnd;

        /// <summary>
        ///     If true, the result string will be updated to upper case.
        /// </summary>
        public bool ToUpper;

        /// <summary>
        ///     Reference to the <see cref="Text"/> component.
        /// </summary>
        public Text Text { get; private set; }
        
        /// <summary>
        ///     Reference to the <see cref="TextMeshPro"/> component.
        /// </summary>
        public TextMeshPro TextPro { get; private set; }
        
        /// <summary>
        ///     Reference to the <see cref="TextMeshProUGUI"/> component.
        /// </summary>
        public TextMeshProUGUI TextProUGUI { get; private set; }

        /// <inheritdoc />
        public override void RefreshLocale()
        {
            // Construct.
            var product = string.Empty;
            if (JEMLocale.GetSelectedLocale() != null)
            {
                product = AutoStart;
                product += JEMLocale.Resolve(Group, Key, AutoFormat);
                product += AutoEnd;
            }

            if (ToUpper)
            {
                product = product.ToUpper();
            }

            // Apply.
            Apply(product);
        }

        private void Apply(string str)
        {
            if (Text == null)
                Text = GetComponent<Text>();

            if (Text == null)
            {
                if (TextProUGUI == null)
                    TextProUGUI = GetComponent<TextMeshProUGUI>();

                if (TextPro == null)
                    TextPro = GetComponent<TextMeshPro>();

                if (TextProUGUI != null)
                    TextProUGUI.text = str;

                if (TextPro != null)
                    TextPro.text = str;
            }
            else Text.text = str;
        }
    }
}
