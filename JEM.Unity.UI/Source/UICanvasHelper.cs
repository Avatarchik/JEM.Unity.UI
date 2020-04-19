//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections.Generic;
using JEM.Unity.Attribute.Controls;
using JEM.Unity.Attribute.Style;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace JEM.Unity.UI
{
    /// <inheritdoc />
    /// <summary>
    ///     Controls a scale mode, scale factor and ui resolution of local Canvas.
    /// </summary>
    [AddComponentMenu("JEM/UI/Canvas Helper")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas)), RequireComponent(typeof(CanvasScaler))]
    public sealed class UICanvasHelper : MonoBehaviour
    {
        /// <summary>
        ///     The default canvas resolution.
        /// </summary>
        [JEMHeader("Canvas Settings")]
        public Vector2 DefaultResolution = new Vector2(1920, 1080);

        /// <summary>
        ///     Minimal possible resolution of the canvas.
        /// </summary>
        [JEMSpace]
        public Vector2 MinResolution = new Vector2(1280, 720);

        /// <summary>
        ///     Maximal possible resolution of the canvas.
        /// </summary>
        public Vector2 MaxResolution = new Vector2(3840, 2160);

        /// <summary>
        ///     Defines how canvas scale factor should be clamped.
        /// </summary>
        [JEMSpace]
        [JEMMinMaxRange(0.5f, 2.5f)]
        public Vector2 ScaleFactorClamp = new Vector2(0.8f, 1.5f);
        
        /// <summary>
        ///     Reference to the <see cref="Canvas"/> component.
        /// </summary>
        public Canvas Canvas { get; private set; }

        /// <summary>
        ///     Reference to the <see cref="CanvasScaler"/> component.
        /// </summary>
        public CanvasScaler Scaler { get; private set; }

        private bool _hasEventsRegistered;
        
        private void Awake() => AllControllers.Add(this);    
        private void Start()
        {
            // Collect the components.
            Canvas = GetComponent<Canvas>();
            Scaler = GetComponent<CanvasScaler>();

            // Try to register events.
            RegisterEvents();
        }

        private void OnDestroy()
        {
            // Remove.
            AllControllers.Remove(this);

            // Try to unregister events
            UnRegisterEvents();
        }
        
        private void RegisterEvents()
        {
            if (_hasEventsRegistered) return;
            _hasEventsRegistered = true;

            PerformScaleModeChange += OnScaleModeChanged;
            PerformScaleFactorChange += OnScaleFactorChanged;
            PerformResolutionChange += OnScaleResolutionChanged;
        }

        private void UnRegisterEvents()
        {
            if (!_hasEventsRegistered)  return;
            _hasEventsRegistered = false;

            PerformScaleModeChange -= OnScaleModeChanged;
            PerformScaleFactorChange -= OnScaleFactorChanged;
            PerformResolutionChange -= OnScaleResolutionChanged;
        }
        
        private void OnScaleModeChanged(UICanvasMode mode)
        {
            switch (mode)
            {
                case UICanvasMode.ScaleWithScreenSize:
                    Scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    break;
                default:
                    // Always apply constantPixelSize when mode may be unknown.
                    Scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                    break;
            }
        }

        private void OnScaleFactorChanged(float scaleFactor)
        {
            var factor = Mathf.Clamp(scaleFactor, ScaleFactorClamp.x, ScaleFactorClamp.y);
            Scaler.scaleFactor = factor;
        }

        private void OnScaleResolutionChanged(string resolution)
        {
            if (JEMScreen.TryParseResolution(resolution, out var width, out var height))
            {
                width = Convert.ToInt32(Mathf.Clamp(width, MinResolution.x, MaxResolution.x));
                height = Convert.ToInt32(Mathf.Clamp(height, MinResolution.y, MaxResolution.y));
                Scaler.referenceResolution = new Vector2(width, height);
            }
            else
            {
                Scaler.referenceResolution = DefaultResolution;
            }
        }

        public static Action<UICanvasMode> PerformScaleModeChange { get; set; }
        public static Action<float> PerformScaleFactorChange { get; set; }
        public static Action<string> PerformResolutionChange { get; set; }

        /// <summary>
        ///     List of all <see cref="UICanvasHelper"/> components in current scene.
        /// </summary>
        public static List<UICanvasHelper> AllControllers { get; } = new List<UICanvasHelper>();
    }
}
