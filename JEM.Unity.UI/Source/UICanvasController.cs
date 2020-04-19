//
// JEM For Unity
//
// Copyright (c) 2020 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JEM.Unity.UI
{
    /// <inheritdoc />
    /// <summary>
    ///     Canvas Controller component.
    ///     Controls a scale mode, scale factor and ui resolution of target Canvas.
    /// </summary>
    [AddComponentMenu("JEM/UI/Canvas Controller")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas)), RequireComponent(typeof(CanvasScaler))]
    public class UICanvasController : MonoBehaviour
    {
        /// <summary>
        ///     A default canvas resolution.
        /// </summary>
        [Header("Canvas Settings")]
        public Vector2 DefaultResolution = new Vector2(1920, 1080);

        /// <summary>
        ///     Minimal possible resolution.
        /// </summary>
        public Vector2 MinResolution = new Vector2(1280, 720);

        /// <summary>
        ///     Maximal possible resolution.
        /// </summary>
        public Vector2 MaxResolution = new Vector2(3840, 2160);

        /// <summary>
        ///     Reference to the <see cref="global::UnityEngine.Canvas"/> component.
        /// </summary>
        public Canvas Canvas { get; private set; }

        /// <summary>
        ///     Reference to the <see cref="CanvasScaler"/> component.
        /// </summary>
        public CanvasScaler Scaler { get; private set; }

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

        private bool _hasEventsRegistered;
        private void RegisterEvents()
        {
            if (_hasEventsRegistered)
            {
                return;
            }

//            if (OnUIScaleModeChange == null || OnUIScaleFactor == null || OnUIResolution == null)
//            {
//#if DEBUG
//                JEMLogger.LogWarning("You are trying to register events for JEMCanvasController " +
//                                     "but Change Actions Events has been not set.");
//#endif
//                return;
//            }

            _hasEventsRegistered = true;

            OnUIScaleModeChange += OnScaleModeChanged;
            OnUIScaleFactor += OnScaleFactorChanged;
            OnUIResolution += OnScaleResolutionChanged;
        }

        private void UnRegisterEvents()
        {
            if (!_hasEventsRegistered)
            {
                return;
            }

            _hasEventsRegistered = false;

            OnUIScaleModeChange -= OnScaleModeChanged;
            OnUIScaleFactor -= OnScaleFactorChanged;
            OnUIResolution -= OnScaleResolutionChanged;
        }

        #region EVENTS

        private void OnScaleModeChanged(UICanvasMode mode)
        {
            switch (mode)
            {
                case UICanvasMode.ScaleWithScreenSize:
                    Scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    break;
                default:
                    Scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                    break;
            }
        }

        private void OnScaleFactorChanged(float scaleFactor)
        {
            var factor = Mathf.Clamp(scaleFactor, 0.8f, 1.5f);
            Scaler.scaleFactor = factor;
        }

        private void OnScaleResolutionChanged(string resolution)
        {
            var i = resolution.Split('x');
            if (i.Length != 2)
            {
                // apply default
                Scaler.referenceResolution = DefaultResolution;
            }
            else
            {
                if (float.TryParse(i[0], out var x))
                {
                    if (float.TryParse(i[1], out var y))
                    {
                        x = Mathf.Clamp(x, MinResolution.x, MaxResolution.x);
                        y = Mathf.Clamp(y, MinResolution.y, MaxResolution.y);
                        Scaler.referenceResolution = new Vector2(x, y);
                    }
                    else
                    {
                        // Apply default.
                        Scaler.referenceResolution = DefaultResolution;
                    }
                }
                else
                {
                    // Apply default.
                    Scaler.referenceResolution = DefaultResolution;
                }
            }
        }

        #endregion
       
        /// <summary>
        ///     Registers events for all <see cref="UICanvasController"/> in <see cref="AllControllers"/> list.
        /// </summary>
        public static void RegisterEventsAll()
        {
            for (var index = 0; index < AllControllers.Count; index++)
            {
                var c = AllControllers[index];
                c.RegisterEvents();
            }
        }

        /// <summary>
        ///     A UIScaleMode Change event.
        /// </summary>
        /// <remarks>
        ///     Should be always called when scale mode of UI changes.
        /// </remarks>
        public static Action<UICanvasMode> OnUIScaleModeChange { get; set; }

        /// <summary>
        ///     A UIScaleFactor Change event.
        /// </summary>
        public static Action<float> OnUIScaleFactor { get; set; }

        /// <summary>
        ///     A UIResolution Change event.
        /// </summary>
        public static Action<string> OnUIResolution { get; set; }

        /// <summary>
        ///     List of all <see cref="UICanvasController"/> components in current scene.
        /// </summary>
        public static List<UICanvasController> AllControllers { get; } = new List<UICanvasController>();
    }
}
