//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using UnityEngine;
using UnityEngine.UI;

namespace JEM.Unity.UI.Control
{
    /// <inheritdoc />
    /// <summary>
    ///     Apply gradient to target image that's position is based on slider's value.
    /// </summary>
    [AddComponentMenu("JEM/UI/Control/Slider Gradient")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Slider))]
    [ExecuteInEditMode]
    public sealed class UISliderGradient : MonoBehaviour
    {
        /// <summary>
        ///     Target image.
        /// </summary>
        [Header("Slider Gradient")]
        public Image Target;

        /// <summary>
        ///     Gradient.
        /// </summary>
        public Gradient Gradient;

        /// <summary>
        ///     Speed of color interpolation.
        ///     If set to zero, interpolation will be ignored.
        /// </summary>
        public float Smooth = 5f;

        /// <summary>
        ///     Current slider.
        /// </summary>
        public Slider Slider { get; private set; }

        private void Awake()
        {
            Slider = GetComponent<Slider>();
        }

        private void Update()
        {
            if (Target == null)
                return;

            if (Math.Abs(Smooth) > float.Epsilon)
                Target.color = Color.Lerp(Target.color, Gradient.Evaluate(Slider.value), Time.deltaTime * Smooth);
            else
                Target.color = Gradient.Evaluate(Slider.value);
        }
    }
}